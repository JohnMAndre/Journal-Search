from __future__ import annotations

from dataclasses import dataclass
from typing import Mapping


@dataclass(slots=True)
class JournalEntry:
    id: int | None = None
    flagged: bool = False
    journal_name: str = ""
    publisher_name: str = ""
    issns: str = ""
    source: str = ""
    ranking: str = ""
    rating: str = ""
    h_index: str = ""
    country: str = ""
    categories: str = ""
    areas: str = ""
    info_url: str = ""
    submit_url: str = ""
    submit_history: str = ""
    submit_fee: str = ""
    apc: str = ""
    notes: str = ""

    @staticmethod
    def normalize(text: str | None) -> str:
        return (text or "").strip()

    @classmethod
    def from_row(cls, row: Mapping[str, object]) -> "JournalEntry":
        return cls(
            id=int(row["id"]) if row["id"] is not None else None,
            flagged=bool(row["flagged"]),
            journal_name=cls.normalize(str(row["journal_name"] or "")),
            publisher_name=cls.normalize(str(row["publisher_name"] or "")),
            issns=cls.normalize(str(row["issns"] or "")),
            source=cls.normalize(str(row["source"] or "")),
            ranking=cls.normalize(str(row["ranking"] or "")),
            rating=cls.normalize(str(row["rating"] or "")),
            h_index=cls.normalize(str(row["h_index"] or "")),
            country=cls.normalize(str(row["country"] or "")),
            categories=cls.normalize(str(row["categories"] or "")),
            areas=cls.normalize(str(row["areas"] or "")),
            info_url=cls.normalize(str(row["info_url"] or "")),
            submit_url=cls.normalize(str(row["submit_url"] or "")),
            submit_history=cls.normalize(str(row["submit_history"] or "")),
            submit_fee=cls.normalize(str(row["submit_fee"] or "")),
            apc=cls.normalize(str(row["apc"] or "")),
            notes=cls.normalize(str(row["notes"] or "")),
        )


def decode_text(value: str | None) -> str:
    """Mirror legacy normalization that replaces '&' with 'and'."""
    return (value or "").strip().replace("&", "and")
