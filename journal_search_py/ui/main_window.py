from __future__ import annotations

import shutil
import tempfile
import tkinter as tk
from pathlib import Path
from tkinter import messagebox, ttk
import webbrowser

from ..constants import APP_NAME, COLUMN_DEFS, DB_FILE, FILTER_COLUMNS
from ..filtering import apply_filter, filter_exact_journal_name
from ..migrator import discover_best_legacy_xml, load_legacy_xml
from ..model import JournalEntry
from ..repository import JournalRepository
from .import_dialog import ImportDialog


class MainWindow(tk.Tk):
    FLAG_MARK = "\u2713"

    def __init__(self, app_dir: Path):
        super().__init__()
        self.app_dir = app_dir
        self.data_dir = app_dir / "data"
        self.db_path = self.data_dir / DB_FILE
        self.repo = JournalRepository(self.db_path)
        self._repo_closed = False

        self._exit_without_saving = False
        self._db_backup: Path | None = None
        self._has_unsaved_changes = False

        self.entries: list[JournalEntry] = []
        self.visible_entries: list[JournalEntry] = []
        self.entries_by_id: dict[int, JournalEntry] = {}
        self.sort_state: dict[str, bool] = {}
        self.current_edit: tk.Entry | None = None
        self.last_clicked_column: str | None = None

        self.title(APP_NAME)
        self.geometry("1260x700")

        self._build_ui()
        self._bind_shortcuts()
        self._bootstrap_data()

        self.protocol("WM_DELETE_WINDOW", self.exit_with_save)

    def _create_db_backup(self) -> Path | None:
        if not self.db_path.exists():
            return None
        backup_path = Path(tempfile.gettempdir()) / "journal_search_python_backup.db"
        shutil.copy2(self.db_path, backup_path)
        return backup_path

    def _ensure_db_backup(self) -> None:
        if self._db_backup is None:
            self._db_backup = self._create_db_backup()

    def _restore_db_backup(self) -> None:
        if not self._db_backup or not self._db_backup.exists():
            return
        if not self._repo_closed:
            self.repo.close()
            self._repo_closed = True
        shutil.copy2(self._db_backup, self.db_path)

    def _build_ui(self) -> None:
        self.columnconfigure(0, weight=1)
        self.rowconfigure(2, weight=1)

        self._build_menu()
        self._build_filter_bar()
        self._build_table()
        self._build_status_bar()

    def _build_menu(self) -> None:
        menubar = tk.Menu(self)

        file_menu = tk.Menu(menubar, tearoff=0)
        file_menu.add_command(label="Exit (saving changes)", command=self.exit_with_save)
        file_menu.add_command(label="Exit without saving", command=self.exit_without_save)
        menubar.add_cascade(label="File", menu=file_menu)

        data_menu = tk.Menu(menubar, tearoff=0)
        data_menu.add_command(label="Import / update database...", command=self.open_import)
        data_menu.add_command(label="Add one entry", accelerator="Ctrl+N", command=self.add_one_entry)
        data_menu.add_command(label="Clear filter", accelerator="Ctrl+Q", command=self.clear_filter)
        data_menu.add_separator()
        data_menu.add_command(label="Data cleanup", command=self.data_cleanup)
        menubar.add_cascade(label="Data", menu=data_menu)

        view_menu = tk.Menu(menubar, tearoff=0)
        view_menu.add_command(label="Autosize columns", accelerator="Ctrl+E", command=self.autosize_columns)
        view_menu.add_command(label="Mini-size columns", accelerator="Ctrl+W", command=self.minisize_columns)
        view_menu.add_command(label="This journal", command=self.filter_this_journal)
        menubar.add_cascade(label="View", menu=view_menu)

        action_menu = tk.Menu(menubar, tearoff=0)
        action_menu.add_command(label="Launch Info URL", command=lambda: self.launch_url("info_url"))
        action_menu.add_command(label="Launch Submit URL", command=lambda: self.launch_url("submit_url"))
        action_menu.add_command(label="Change title case (flagged)", command=self.change_title_case_flagged)
        action_menu.add_command(label="Clear flags", command=self.clear_flags)
        menubar.add_cascade(label="Action", menu=action_menu)

        self.config(menu=menubar)

    def _build_filter_bar(self) -> None:
        frm = ttk.Frame(self)
        frm.grid(row=0, column=0, sticky="ew", padx=8, pady=6)
        frm.columnconfigure(1, weight=1)

        ttk.Label(frm, text="Filter:").grid(row=0, column=0, sticky="w")

        self.filter_var = tk.StringVar()
        self.filter_entry = ttk.Entry(frm, textvariable=self.filter_var)
        self.filter_entry.grid(row=0, column=1, sticky="ew", padx=(6, 8))
        self.filter_entry.bind("<Return>", self._on_filter_enter)
        self.filter_entry.bind("<KP_Enter>", self._on_filter_enter)
        self.filter_entry.bind("<Control-Return>", self._on_filter_ctrl_enter)
        self.filter_entry.bind("<Control-KP_Enter>", self._on_filter_ctrl_enter)

        self.match_var = tk.StringVar(value="Match all")
        self.match_combo = ttk.Combobox(
            frm,
            state="readonly",
            width=12,
            values=["Match all", "Match any"],
            textvariable=self.match_var,
        )
        self.match_combo.grid(row=0, column=2, padx=(0, 8))

        ttk.Label(frm, text="Column:").grid(row=0, column=3)
        self.filter_column_var = tk.StringVar(value=FILTER_COLUMNS[0][1])
        self.filter_column_combo = ttk.Combobox(
            frm,
            state="readonly",
            width=16,
            values=[label for _, label in FILTER_COLUMNS],
            textvariable=self.filter_column_var,
        )
        self.filter_column_combo.grid(row=0, column=4, padx=(6, 8))

        self.filter_button = ttk.Button(frm, text=">>", width=4, command=self.apply_filter)
        self.filter_button.grid(row=0, column=5)

        self.filtered_count_var = tk.StringVar(value="")
        ttk.Label(frm, textvariable=self.filtered_count_var).grid(row=0, column=6, padx=(10, 0), sticky="w")

    def _build_table(self) -> None:
        table_frame = ttk.Frame(self)
        table_frame.grid(row=2, column=0, sticky="nsew")
        table_frame.columnconfigure(0, weight=1)
        table_frame.rowconfigure(0, weight=1)

        column_keys = [key for key, _, _ in COLUMN_DEFS]
        self.tree = ttk.Treeview(table_frame, columns=column_keys, show="headings", selectmode="browse")
        self.tree.tag_configure("pred_row", background="#ffd6e7")
        self.tree.tag_configure("unranked_row", background="#fff59d")
        centered_cols = {"issns", "ranking", "rating", "h_index"}
        for key, label, width in COLUMN_DEFS:
            self.tree.heading(key, text=label, command=lambda c=key: self.sort_by_column(c))
            anchor = "center" if key in centered_cols else "w"
            self.tree.column(key, width=width, minwidth=40, stretch=True, anchor=anchor)

        y_scroll = ttk.Scrollbar(table_frame, orient="vertical", command=self.tree.yview)
        x_scroll = ttk.Scrollbar(table_frame, orient="horizontal", command=self.tree.xview)
        self.tree.configure(yscrollcommand=y_scroll.set, xscrollcommand=x_scroll.set)

        self.tree.grid(row=0, column=0, sticky="nsew")
        y_scroll.grid(row=0, column=1, sticky="ns")
        x_scroll.grid(row=1, column=0, sticky="ew")

        self.tree.bind("<Control-c>", self.copy_current_cell)
        self.tree.bind("<Double-1>", self.on_double_click)
        self.tree.bind("<Button-1>", self.on_single_click, add=True)

    def _build_status_bar(self) -> None:
        status = ttk.Frame(self)
        status.grid(row=3, column=0, sticky="ew")
        status.columnconfigure(0, weight=1)

        self.status_var = tk.StringVar(value="")
        ttk.Label(status, textvariable=self.status_var).grid(row=0, column=0, sticky="w", padx=8, pady=4)

        self.progress = ttk.Progressbar(status, orient="horizontal", mode="determinate", length=400)
        self.progress.grid(row=0, column=1, sticky="e", padx=8, pady=4)
        self.progress.grid_remove()

    def _bind_shortcuts(self) -> None:
        self.bind_all("<Control-n>", lambda _e: self.add_one_entry())
        self.bind_all("<Control-q>", lambda _e: self.clear_filter())
        self.bind_all("<Control-m>", lambda _e: self.match_selected_journal_name())
        self.bind_all("<Control-e>", lambda _e: self.autosize_columns())
        self.bind_all("<Control-w>", lambda _e: self.minisize_columns())

    def _bootstrap_data(self) -> None:
        if self.repo.count() == 0:
            self.status_var.set("Migrating legacy XML data...")
            self.update_idletasks()
            discovered = discover_best_legacy_xml(self.app_dir.parent)
            if discovered:
                legacy_main, legacy_extra = discovered
                seeded = load_legacy_xml(legacy_main, legacy_extra)
                if seeded:
                    self.repo.bulk_insert(seeded)

        self.reload_data()

    def reload_data(self) -> None:
        self.entries = self.repo.fetch_all()
        self.entries_by_id = {entry.id: entry for entry in self.entries if entry.id is not None}
        self.visible_entries = list(self.entries)
        self.refresh_table(self.visible_entries, with_progress=len(self.visible_entries) >= 10000)
        self.filtered_count_var.set("")
        self.title(f"{APP_NAME} - {len(self.entries):,}")

    def refresh_table(self, entries: list[JournalEntry], with_progress: bool = False) -> None:
        self.tree.delete(*self.tree.get_children())

        if with_progress and entries:
            self.progress.configure(maximum=len(entries), value=0)
            self.progress.grid()

        insert_row = self.tree.insert
        progress_interval = 2000

        for index, entry in enumerate(entries, start=1):
            insert_row(
                "",
                "end",
                iid=str(entry.id),
                values=self._entry_values(entry),
                tags=self._row_tags(entry),
            )
            if with_progress and index % progress_interval == 0:
                self.progress.configure(value=index)
                self.status_var.set(f"Loading journal {index:,} of {len(entries):,}")
                self.update_idletasks()

        if with_progress and entries:
            self.progress.configure(value=len(entries))

        self.status_var.set("")
        self.progress.grid_remove()

    def _entry_values(self, entry: JournalEntry) -> tuple[str, ...]:
        return (
            self.FLAG_MARK if entry.flagged else "",
            entry.journal_name,
            entry.publisher_name,
            entry.issns,
            entry.source,
            entry.ranking,
            entry.rating,
            entry.h_index,
            entry.country,
            entry.categories,
            entry.areas,
            entry.info_url,
            entry.submit_url,
            entry.submit_history,
            entry.apc,
            entry.notes,
            entry.last_updated,
        )

    def _row_tags(self, entry: JournalEntry) -> tuple[str, ...]:
        ranking = entry.ranking.strip().lower()
        if ranking == "pred":
            return ("pred_row",)
        if ranking in {"unranked", "ur", "nr"}:
            return ("unranked_row",)
        return ()

    def _selected_entry(self) -> JournalEntry | None:
        selected = self.tree.selection()
        if not selected:
            return None
        row_id = int(selected[0])
        return self.entries_by_id.get(row_id)

    def _column_key_by_display(self, display_name: str) -> str:
        for key, label in FILTER_COLUMNS:
            if label == display_name:
                return key
        return "all"

    def apply_filter(self) -> None:
        try:
            query = self.filter_var.get()
            column_key = self._column_key_by_display(self.filter_column_var.get())
            match_all = self.match_var.get() == "Match all"
            filtered = apply_filter(self.entries, query, column_key, match_all)
            self.visible_entries = filtered
            self.refresh_table(filtered)
            self.filtered_count_var.set(f"({len(filtered):,} filtered)")
        except Exception as ex:
            messagebox.showerror(APP_NAME, f"There was an error filtering: {ex}")

    def _on_filter_enter(self, _event: tk.Event) -> None:
        self.apply_filter()

    def _on_filter_ctrl_enter(self, _event: tk.Event) -> None:
        text = self.filter_var.get().replace('"', "")
        self.filter_var.set(f'"{text}"')
        self.apply_filter()
        return "break"

    def clear_filter(self) -> None:
        if self.filter_var.get():
            self.filter_var.set("")
            self.apply_filter()
        self.filter_entry.focus_set()

    def match_selected_journal_name(self) -> None:
        try:
            filtered = filter_exact_journal_name(self.entries, self.filter_var.get())
            self.visible_entries = filtered
            self.refresh_table(filtered)
            self.filtered_count_var.set(f"({len(filtered):,} filtered)")
        except Exception as ex:
            messagebox.showerror(APP_NAME, f"There was an error filtering: {ex}")

    def filter_this_journal(self) -> None:
        entry = self._selected_entry()
        if not entry:
            messagebox.showinfo(APP_NAME, "Select a row first.")
            return

        self.filter_var.set(entry.journal_name)
        self.filter_column_var.set("Journal name")
        filtered = filter_exact_journal_name(self.entries, entry.journal_name)
        self.visible_entries = filtered
        self.refresh_table(filtered)
        self.filtered_count_var.set(f"({len(filtered):,} filtered)")

    def sort_by_column(self, col_key: str) -> None:
        reverse = self.sort_state.get(col_key, False)
        if col_key == "flagged":
            self.visible_entries.sort(key=lambda x: int(bool(x.flagged)), reverse=reverse)
        else:
            self.visible_entries.sort(key=lambda x: str(getattr(x, col_key, "")).casefold(), reverse=reverse)
        self.sort_state[col_key] = not reverse
        self.reorder_table(self.visible_entries)

    def reorder_table(self, entries: list[JournalEntry]) -> None:
        iids = [str(entry.id) for entry in entries if entry.id is not None]
        if not iids:
            return

        try:
            # Fast path: reorder all top-level children in one Tcl call.
            self.tree.set_children("", *iids)
        except tk.TclError:
            # Fallback for edge cases where a row might no longer exist.
            move_row = self.tree.move
            for index, iid in enumerate(iids):
                try:
                    move_row(iid, "", index)
                except tk.TclError:
                    continue

    def copy_current_cell(self, _event: tk.Event | None = None) -> None:
        selected = self.tree.selection()
        if not selected or not self.last_clicked_column:
            return
        values = self.tree.item(selected[0], "values")
        cols = [key for key, _, _ in COLUMN_DEFS]
        try:
            idx = cols.index(self.last_clicked_column)
        except ValueError:
            return
        if idx < len(values):
            self.clipboard_clear()
            self.clipboard_append(str(values[idx]))

    def on_single_click(self, event: tk.Event) -> None:
        region = self.tree.identify("region", event.x, event.y)
        if region != "cell":
            return
        col = self.tree.identify_column(event.x)
        if col.startswith("#"):
            idx = int(col[1:]) - 1
            cols = [key for key, _, _ in COLUMN_DEFS]
            if 0 <= idx < len(cols):
                self.last_clicked_column = cols[idx]

    def on_double_click(self, event: tk.Event) -> None:
        if self.current_edit is not None:
            self.current_edit.destroy()
            self.current_edit = None

        row_iid = self.tree.identify_row(event.y)
        col_id = self.tree.identify_column(event.x)
        if not row_iid or not col_id:
            return

        col_index = int(col_id[1:]) - 1
        cols = [key for key, _, _ in COLUMN_DEFS]
        if col_index < 0 or col_index >= len(cols):
            return
        col_key = cols[col_index]

        entry = self._entry_by_iid(row_iid)
        if not entry:
            return

        if col_key == "flagged":
            self._ensure_db_backup()
            new_value = not entry.flagged
            entry.flagged = new_value
            self.repo.update_cell(entry.id or 0, "flagged", new_value)
            self.tree.set(row_iid, col_key, self.FLAG_MARK if new_value else "")
            self._has_unsaved_changes = True
            return

        bbox = self.tree.bbox(row_iid, col_id)
        if not bbox:
            return
        x, y, width, height = bbox

        current_value = str(getattr(entry, col_key, ""))
        editor = tk.Entry(self.tree)
        editor.insert(0, current_value)
        editor.place(x=x, y=y, width=width, height=height)
        editor.focus_set()

        def commit_edit(_evt: tk.Event | None = None) -> None:
            new_text = editor.get()
            self._ensure_db_backup()
            setattr(entry, col_key, new_text)
            self.repo.update_cell(entry.id or 0, col_key, new_text)
            self.tree.set(row_iid, col_key, new_text)
            self.tree.item(row_iid, tags=self._row_tags(entry))
            editor.destroy()
            self.current_edit = None
            self._has_unsaved_changes = True

        def cancel_edit(_evt: tk.Event | None = None) -> None:
            editor.destroy()
            self.current_edit = None

        editor.bind("<Return>", commit_edit)
        editor.bind("<KP_Enter>", commit_edit)
        editor.bind("<Escape>", cancel_edit)
        editor.bind("<FocusOut>", commit_edit)
        self.current_edit = editor

    def _entry_by_iid(self, iid: str) -> JournalEntry | None:
        return self.entries_by_id.get(int(iid))

    def open_import(self) -> None:
        dlg = ImportDialog(self, self.repo.find_note_matches())
        self.wait_window(dlg)
        if not dlg.result:
            return

        entries, source_to_delete = dlg.result
        self._ensure_db_backup()
        self.repo.replace_source_with_entries(source_to_delete, entries)
        self._has_unsaved_changes = True
        self.reload_data()

    def add_one_entry(self) -> None:
        self._ensure_db_backup()
        obj = JournalEntry(source="ManualEntry", journal_name="New journal")
        new_id = self.repo.insert_one(obj)
        obj.id = new_id
        self._has_unsaved_changes = True
        self.reload_data()
        self.filter_var.set("ManualEntry")
        self.filter_column_var.set(FILTER_COLUMNS[0][1])
        self.apply_filter()

    def minisize_columns(self) -> None:
        widths = [40, 250, 220, 120, 120, 60, 60, 60, 110, 450, 250, 180, 180, 180, 100, 250, 120]
        for idx, (key, _, _) in enumerate(COLUMN_DEFS):
            self.tree.column(key, width=widths[idx])

    def autosize_columns(self) -> None:
        sample = self.visible_entries[:250]
        for key, label, _default in COLUMN_DEFS:
            max_len = len(label)
            for row in sample:
                value = "1" if key == "flagged" and row.flagged else str(getattr(row, key, ""))
                if len(value) > max_len:
                    max_len = len(value)
            self.tree.column(key, width=max(50, min(900, int(max_len * 7.4))))

    def launch_url(self, field_name: str) -> None:
        entry = self._selected_entry()
        if not entry:
            messagebox.showinfo(APP_NAME, "Select a row first.")
            return
        url = str(getattr(entry, field_name, "")).strip()
        if not url:
            messagebox.showinfo(APP_NAME, "No URL in this field.")
            return
        webbrowser.open(url)

    def change_title_case_flagged(self) -> None:
        self._ensure_db_backup()
        count = self.repo.title_case_flagged()
        self._has_unsaved_changes = True
        self.reload_data()
        messagebox.showinfo(APP_NAME, f"Flagged entries updated: {count:,}")

    def clear_flags(self) -> None:
        self._ensure_db_backup()
        count = self.repo.clear_flags()
        for entry in self.entries:
            entry.flagged = False
        for entry in self.visible_entries:
            entry.flagged = False
            if entry.id is not None:
                self.tree.set(str(entry.id), "flagged", "")
        self._has_unsaved_changes = True
        messagebox.showinfo(APP_NAME, f"Flags cleared: {count:,}")

    def data_cleanup(self) -> None:
        confirmed = messagebox.askyesno(
            APP_NAME,
            "This will replace all '&' and '&amp;' with 'and' in Journal name. Continue?",
        )
        if not confirmed:
            return
        self._ensure_db_backup()
        count = self.repo.cleanup_ampersands()
        self._has_unsaved_changes = True
        self.reload_data()
        messagebox.showinfo(APP_NAME, f"Entries updated: {count:,}")

    def exit_without_save(self) -> None:
        self._exit_without_saving = True
        self.destroy()

    def exit_with_save(self) -> None:
        self._exit_without_saving = False
        self.destroy()

    def destroy(self) -> None:
        if self._exit_without_saving and self._has_unsaved_changes:
            self._restore_db_backup()
        if not self._repo_closed:
            self.repo.close()
            self._repo_closed = True
        super().destroy()
