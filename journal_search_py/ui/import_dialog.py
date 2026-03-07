from __future__ import annotations

import tkinter as tk
from tkinter import messagebox, ttk

from ..constants import APP_NAME, IMPORT_COLUMNS
from ..filtering import normalize_name_for_matching
from ..model import JournalEntry, decode_text


INSTRUCTIONS = (
    "Open the data in a spreadsheet, then copy the columns in the order you see here "
    "(leaving blank columns is acceptable), and paste the data from the menu above. "
    "Please do not include the header row when copying from the spreadsheet."
)

IMPORT_PREVIEW_COLUMNS = [
    ("journal_name", "Journal name", 150),
    ("publisher_name", "Publisher", 120),
    ("issns", "ISSNs", 120),
    ("source", "Source", 120),
    ("ranking", "Ranking", 90),
    ("rating", "Rating", 90),
    ("h_index", "H-Index", 90),
    ("country", "Country", 120),
    ("categories", "Categories", 220),
    ("areas", "Areas", 180),
    ("notes", "Notes", 220),
]


class ImportDialog(tk.Toplevel):
    def __init__(self, parent: tk.Misc, note_index: dict[str, dict[str, str]]):
        super().__init__(parent)
        self.title("Import Data")
        self.geometry("1280x430")
        self.transient(parent)
        self.grab_set()

        self.note_index = note_index
        self.result: tuple[list[JournalEntry], str] | None = None
        self.entries: list[JournalEntry] = []

        self._build_ui()

    def _build_ui(self) -> None:
        self.columnconfigure(0, weight=1)
        self.rowconfigure(2, weight=1)

        menubar = tk.Menu(self)

        file_menu = tk.Menu(menubar, tearoff=0)
        file_menu.add_command(label="Exit", command=self.destroy)
        menubar.add_cascade(label="File", menu=file_menu)

        edit_menu = tk.Menu(menubar, tearoff=0)
        edit_menu.add_command(label="Copy columns", command=self.copy_columns)
        edit_menu.add_command(label="Paste", command=self.paste_from_clipboard)
        menubar.add_cascade(label="Edit", menu=edit_menu)

        self.config(menu=menubar)

        instructions = ttk.Label(self, text=INSTRUCTIONS, wraplength=1240, justify="left")
        instructions.grid(row=0, column=0, sticky="ew", padx=12, pady=(8, 8))

        columns = [k for k, _, _ in IMPORT_PREVIEW_COLUMNS]
        self.tree = ttk.Treeview(self, columns=columns, show="headings")
        for key, label, width in IMPORT_PREVIEW_COLUMNS:
            self.tree.heading(key, text=label)
            self.tree.column(key, width=width, anchor="w", stretch=True)

        y_scroll = ttk.Scrollbar(self, orient="vertical", command=self.tree.yview)
        x_scroll = ttk.Scrollbar(self, orient="horizontal", command=self.tree.xview)
        self.tree.configure(yscrollcommand=y_scroll.set, xscrollcommand=x_scroll.set)

        self.tree.grid(row=2, column=0, sticky="nsew", padx=(12, 0), pady=(0, 8))
        y_scroll.grid(row=2, column=1, sticky="ns", pady=(0, 8))
        x_scroll.grid(row=3, column=0, sticky="ew", padx=(12, 0))

        bottom = ttk.Frame(self)
        bottom.grid(row=4, column=0, columnspan=2, sticky="ew", padx=12, pady=8)
        bottom.columnconfigure(1, weight=1)

        ttk.Label(bottom, text="Source name:").grid(row=0, column=0, sticky="w")
        self.source_var = tk.StringVar()
        self.source_entry = tk.Entry(bottom, textvariable=self.source_var, bg="#ffff66")
        self.source_entry.grid(row=0, column=1, sticky="ew", padx=8)
        ttk.Label(
            bottom,
            text="(all existing data from this source will be deleted before importing this data)",
        ).grid(row=0, column=2, sticky="w")

        self.import_btn = ttk.Button(bottom, text="Import", command=self.run_import, state="disabled")
        self.import_btn.grid(row=0, column=3, padx=(12, 0))

    def copy_columns(self) -> None:
        columns = "\t".join([label for _key, label, _ in IMPORT_PREVIEW_COLUMNS])
        self.clipboard_clear()
        self.clipboard_append(columns)

    def _assign_existing_notes(self, entry: JournalEntry) -> None:
        item = self.note_index.get(normalize_name_for_matching(entry.journal_name))
        if not item:
            return
        entry.info_url = item.get("info_url", "")
        entry.submit_url = item.get("submit_url", "")
        entry.submit_history = item.get("submit_history", "")
        entry.submit_fee = item.get("submit_fee", "")
        entry.apc = item.get("apc", "")
        entry.notes = item.get("notes", "")

    def paste_from_clipboard(self) -> None:
        try:
            raw = self.clipboard_get()
        except tk.TclError:
            messagebox.showinfo(APP_NAME, "The clipboard is empty.")
            return

        if not raw or "\t" not in raw:
            messagebox.showinfo(
                APP_NAME,
                "The clipboard data does not contain tabs. It must be spreadsheet data.",
            )
            return

        self.entries.clear()
        self.tree.delete(*self.tree.get_children())

        rows = [r for r in raw.splitlines() if r.strip()]
        total = len(rows)
        for index, row in enumerate(rows, start=1):
            values = row.split("\t")
            entry = JournalEntry()
            for position, field_name in enumerate(IMPORT_COLUMNS):
                if position >= len(values):
                    break
                setattr(entry, field_name, decode_text(values[position]))

            if entry.journal_name.lower() == "journal name":
                continue

            self._assign_existing_notes(entry)
            self.entries.append(entry)
            self.title(f"Import Data (pasting {index:,} of {total:,})")

        for entry in self.entries:
            self.tree.insert(
                "",
                "end",
                values=(
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
                    entry.notes,
                ),
            )

        if self.entries:
            self.source_var.set(self.entries[0].source)
            self.import_btn.configure(state="normal")

        self.title(f"Import - {len(self.entries):,}")

    def run_import(self) -> None:
        if not self.entries:
            return
        self.result = (self.entries, self.source_var.get().strip())
        self.destroy()
