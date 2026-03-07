# Journal Search (Python)

This is a Python desktop rewrite of the original VB.NET **Journal Search** app.

## What It Preserves
- Main grid with near-identical columns and top filter bar.
- Menus and shortcuts:
  - `File`: exit with save, exit without saving.
  - `Data`: import/update, add one entry, clear filter, match selected journal name, data cleanup.
  - `View`: autosize columns, mini-size columns.
  - `Action`: launch URLs, title-case flagged rows.
- Spreadsheet clipboard import workflow with source-based replacement.
- Filtering behavior (all/column, match all/match any, quoted phrase with `Ctrl+Enter`).
- Sort by clicking column headers.

## Storage Design
- Uses SQLite (`python_app/data/journal_search.db`) instead of XML.
- On first run (when DB is empty), migrates from legacy files if present:
  - `../Journal Search/JournalSearchData.xml`
  - `../Journal Search/JournalSearchDataExtra.xml`
- Notes and user fields are stored in the same table for simpler and faster updates.

## Run
```bash
cd python_app
python3 app.py
```

## Notes
- The original .NET project is not modified.
- `Exit without saving` restores a session backup of the SQLite DB from app startup.
