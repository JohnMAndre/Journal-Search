from __future__ import annotations

APP_NAME = "Journal Search"
DISCONTINUED_IDENTIFIER = " (discontinued)"

DB_FILE = "journal_search.db"

COLUMN_DEFS = [
    ("flagged", "Flag", 50),
    ("journal_name", "Journal name", 220),
    ("publisher_name", "Publisher", 180),
    ("issns", "ISSNs", 120),
    ("source", "Source", 140),
    ("ranking", "Ranking", 90),
    ("rating", "Rating", 90),
    ("h_index", "H-Index", 80),
    ("country", "Country", 120),
    ("categories", "Categories", 280),
    ("areas", "Areas", 220),
    ("info_url", "InfoURL", 180),
    ("submit_url", "SubmitURL", 180),
    ("submit_history", "SubmitHistory", 180),
    ("apc", "APC", 100),
    ("notes", "Notes", 260),
    ("last_updated", "Last updated", 120),
]

FILTER_COLUMNS = [
    ("all", "(all columns)"),
    ("journal_name", "Journal name"),
    ("publisher_name", "Publisher"),
    ("issns", "ISSN1"),
    ("source", "Source"),
    ("ranking", "Ranking"),
    ("rating", "Rating"),
    ("h_index", "H-Index"),
    ("country", "Country"),
    ("categories", "Categories"),
    ("areas", "Areas"),
    ("notes", "Notes"),
    ("last_updated", "Last updated"),
]

SEARCHABLE_COLUMNS = [
    "journal_name",
    "publisher_name",
    "issns",
    "source",
    "ranking",
    "rating",
    "h_index",
    "country",
    "categories",
    "areas",
    "notes",
    "last_updated",
]

IMPORT_COLUMNS = [
    "journal_name",
    "publisher_name",
    "issns",
    "source",
    "ranking",
    "rating",
    "h_index",
    "country",
    "categories",
    "areas",
]
