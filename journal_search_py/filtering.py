from __future__ import annotations

from .constants import DISCONTINUED_IDENTIFIER, SEARCHABLE_COLUMNS
from .model import JournalEntry


def _match_token(value: str, token: str) -> bool:
    return token in value.lower()


def _entry_matches_token(entry: JournalEntry, token: str, column_key: str) -> bool:
    if column_key == "all":
        return any(_match_token(str(getattr(entry, c)), token) for c in SEARCHABLE_COLUMNS)
    return _match_token(str(getattr(entry, column_key, "")), token)


def _tokens_from_query(query: str) -> tuple[list[str], bool]:
    query = query.strip().lower()
    if not query:
        return [], False

    if " " in query and not query.startswith('"'):
        tokens = [t for t in query.split(" ") if t]
        return tokens, False

    phrase = query.replace('"', "")
    return [phrase], True


def apply_filter(
    entries: list[JournalEntry],
    query: str,
    column_key: str,
    match_all: bool,
) -> list[JournalEntry]:
    normalized_query = query.replace("&", "and")
    tokens, _is_phrase = _tokens_from_query(normalized_query)
    if not tokens:
        return list(entries)

    filtered: list[JournalEntry] = []
    for entry in entries:
        decisions = [_entry_matches_token(entry, token, column_key) for token in tokens]
        if (all(decisions) if match_all else any(decisions)):
            filtered.append(entry)
    return filtered


def filter_exact_journal_name(entries: list[JournalEntry], query: str) -> list[JournalEntry]:
    normalized = query.strip().lower().replace('"', "")
    return [e for e in entries if e.journal_name.lower() == normalized]


def normalize_name_for_matching(name: str) -> str:
    return name.strip().lower().replace(DISCONTINUED_IDENTIFIER, "")
