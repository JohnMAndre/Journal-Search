from __future__ import annotations

from pathlib import Path
import tkinter as tk

from journal_search_py.ui.main_window import MainWindow


def main() -> None:
    root = MainWindow(Path(__file__).resolve().parent)
    icon_path = Path(__file__).resolve().parent.parent / "Journal Search" / "preview.ico"
    if icon_path.exists():
        try:
            root.iconbitmap(icon_path)
        except tk.TclError:
            # Ignore icon failures on environments that do not support .ico files.
            pass
    root.mainloop()


if __name__ == "__main__":
    main()
