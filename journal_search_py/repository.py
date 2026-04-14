from __future__ import annotations

import sqlite3
from datetime import date
from pathlib import Path

from .model import JournalEntry, decode_text


class JournalRepository:
    def __init__(self, db_path: Path):
        self.db_path = db_path
        self.db_path.parent.mkdir(parents=True, exist_ok=True)
        self._conn = sqlite3.connect(self.db_path)
        self._conn.row_factory = sqlite3.Row
        self._conn.execute("PRAGMA foreign_keys = ON")
        self._conn.execute("PRAGMA journal_mode = WAL")
        self._conn.execute("PRAGMA synchronous = NORMAL")
        self.init_schema()

    def close(self) -> None:
        self._conn.close()

    def init_schema(self) -> None:
        self._conn.executescript(
            """
            CREATE TABLE IF NOT EXISTS journals (
                id INTEGER PRIMARY KEY AUTOINCREMENT,
                flagged INTEGER NOT NULL DEFAULT 0,
                journal_name TEXT NOT NULL DEFAULT '',
                publisher_name TEXT NOT NULL DEFAULT '',
                issns TEXT NOT NULL DEFAULT '',
                source TEXT NOT NULL DEFAULT '',
                ranking TEXT NOT NULL DEFAULT '',
                rating TEXT NOT NULL DEFAULT '',
                h_index TEXT NOT NULL DEFAULT '',
                country TEXT NOT NULL DEFAULT '',
                categories TEXT NOT NULL DEFAULT '',
                areas TEXT NOT NULL DEFAULT '',
                info_url TEXT NOT NULL DEFAULT '',
                submit_url TEXT NOT NULL DEFAULT '',
                submit_history TEXT NOT NULL DEFAULT '',
                submit_fee TEXT NOT NULL DEFAULT '',
                apc TEXT NOT NULL DEFAULT '',
                notes TEXT NOT NULL DEFAULT '',
                last_updated TEXT NOT NULL DEFAULT ''
            );
            CREATE INDEX IF NOT EXISTS idx_journals_name ON journals(journal_name);
            CREATE INDEX IF NOT EXISTS idx_journals_source ON journals(source);
            """
        )
        columns = {
            str(row["name"])
            for row in self._conn.execute("PRAGMA table_info(journals)").fetchall()
        }
        if "last_updated" not in columns:
            self._conn.execute(
                "ALTER TABLE journals ADD COLUMN last_updated TEXT NOT NULL DEFAULT ''"
            )
        self._conn.commit()

    def count(self) -> int:
        row = self._conn.execute("SELECT COUNT(*) AS n FROM journals").fetchone()
        return int(row["n"])

    def fetch_all(self) -> list[JournalEntry]:
        rows = self._conn.execute(
            """
            SELECT id, flagged, journal_name, publisher_name, issns, source,
                   ranking, rating, h_index, country, categories, areas,
                   info_url, submit_url, submit_history, submit_fee, apc, notes, last_updated
            FROM journals
            """
        ).fetchall()
        return [JournalEntry.from_row(r) for r in rows]

    def insert_one(self, entry: JournalEntry) -> int:
        cur = self._conn.execute(
            """
            INSERT INTO journals(
                flagged, journal_name, publisher_name, issns, source,
                ranking, rating, h_index, country, categories, areas,
                info_url, submit_url, submit_history, submit_fee, apc, notes, last_updated
            ) VALUES (?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?)
            """,
            (
                int(entry.flagged),
                decode_text(entry.journal_name),
                decode_text(entry.publisher_name),
                decode_text(entry.issns),
                decode_text(entry.source),
                decode_text(entry.ranking),
                decode_text(entry.rating),
                decode_text(entry.h_index),
                decode_text(entry.country),
                decode_text(entry.categories),
                decode_text(entry.areas),
                JournalEntry.normalize(entry.info_url),
                JournalEntry.normalize(entry.submit_url),
                JournalEntry.normalize(entry.submit_history),
                JournalEntry.normalize(entry.submit_fee),
                JournalEntry.normalize(entry.apc),
                JournalEntry.normalize(entry.notes),
                JournalEntry.normalize(entry.last_updated),
            ),
        )
        self._conn.commit()
        return int(cur.lastrowid)

    def bulk_insert(self, entries: list[JournalEntry]) -> None:
        values = [
            (
                int(e.flagged),
                decode_text(e.journal_name),
                decode_text(e.publisher_name),
                decode_text(e.issns),
                decode_text(e.source),
                decode_text(e.ranking),
                decode_text(e.rating),
                decode_text(e.h_index),
                decode_text(e.country),
                decode_text(e.categories),
                decode_text(e.areas),
                JournalEntry.normalize(e.info_url),
                JournalEntry.normalize(e.submit_url),
                JournalEntry.normalize(e.submit_history),
                JournalEntry.normalize(e.submit_fee),
                JournalEntry.normalize(e.apc),
                JournalEntry.normalize(e.notes),
                JournalEntry.normalize(e.last_updated),
            )
            for e in entries
        ]
        self._conn.executemany(
            """
            INSERT INTO journals(
                flagged, journal_name, publisher_name, issns, source,
                ranking, rating, h_index, country, categories, areas,
                info_url, submit_url, submit_history, submit_fee, apc, notes, last_updated
            ) VALUES (?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?)
            """,
            values,
        )
        self._conn.commit()

    def delete_by_source(self, source: str) -> int:
        cur = self._conn.execute(
            "DELETE FROM journals WHERE lower(source) = ?", (source.strip().lower(),)
        )
        self._conn.commit()
        return int(cur.rowcount)

    def replace_source_with_entries(self, source: str, entries: list[JournalEntry]) -> int:
        source_name = source.strip()
        today = date.today().isoformat()

        with self._conn:
            source_keys = {
                decode_text(source_name).strip().lower()
                for source_name in ([source_name] if source_name else [])
            }
            if not source_keys:
                source_keys = {
                    decode_text(entry.source).strip().lower()
                    for entry in entries
                    if decode_text(entry.source).strip()
                }

            existing_rows: list[sqlite3.Row] = []
            if source_keys:
                placeholders = ", ".join("?" for _ in source_keys)
                existing_rows = self._conn.execute(
                    f"""
                    SELECT id, journal_name, source
                    FROM journals
                    WHERE lower(source) IN ({placeholders})
                    """,
                    tuple(source_keys),
                ).fetchall()
            existing_by_name = {
                (
                    JournalEntry.normalize(str(row["source"] or "")).lower(),
                    JournalEntry.normalize(str(row["journal_name"] or "")).lower(),
                ): int(row["id"])
                for row in existing_rows
                if JournalEntry.normalize(str(row["journal_name"] or ""))
            }

            for entry in entries:
                effective_source = source_name or JournalEntry.normalize(entry.source)
                effective_source = decode_text(effective_source)
                source_key = JournalEntry.normalize(effective_source).lower()
                name_key = JournalEntry.normalize(entry.journal_name).lower()
                existing_id = existing_by_name.get((source_key, name_key))
                if existing_id is not None:
                    self._conn.execute(
                        """
                        UPDATE journals
                        SET journal_name = ?, publisher_name = ?, issns = ?, source = ?,
                            ranking = ?, rating = ?, h_index = ?, country = ?,
                            categories = ?, areas = ?, last_updated = ?
                        WHERE id = ?
                        """,
                        (
                            decode_text(entry.journal_name),
                            decode_text(entry.publisher_name),
                            decode_text(entry.issns),
                            effective_source,
                            decode_text(entry.ranking),
                            decode_text(entry.rating),
                            decode_text(entry.h_index),
                            decode_text(entry.country),
                            decode_text(entry.categories),
                            decode_text(entry.areas),
                            today,
                            existing_id,
                        ),
                    )
                    continue

                cur = self._conn.execute(
                    """
                    INSERT INTO journals(
                        flagged, journal_name, publisher_name, issns, source,
                        ranking, rating, h_index, country, categories, areas,
                        info_url, submit_url, submit_history, submit_fee, apc, notes, last_updated
                    ) VALUES (?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?)
                    """,
                    (
                        int(entry.flagged),
                        decode_text(entry.journal_name),
                        decode_text(entry.publisher_name),
                        decode_text(entry.issns),
                        effective_source,
                        decode_text(entry.ranking),
                        decode_text(entry.rating),
                        decode_text(entry.h_index),
                        decode_text(entry.country),
                        decode_text(entry.categories),
                        decode_text(entry.areas),
                        JournalEntry.normalize(entry.info_url),
                        JournalEntry.normalize(entry.submit_url),
                        JournalEntry.normalize(entry.submit_history),
                        JournalEntry.normalize(entry.submit_fee),
                        JournalEntry.normalize(entry.apc),
                        JournalEntry.normalize(entry.notes),
                        today,
                    ),
                )
                existing_by_name[(source_key, name_key)] = int(cur.lastrowid)
        return len(entries)

    def update_cell(self, row_id: int, field_name: str, value: str | bool) -> None:
        if field_name not in {
            "flagged",
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
            "info_url",
            "submit_url",
            "submit_history",
            "submit_fee",
            "apc",
            "notes",
            "last_updated",
        }:
            raise ValueError(f"Unexpected field: {field_name}")

        if field_name == "flagged":
            db_value = int(bool(value))
        elif field_name in {
            "info_url",
            "submit_url",
            "submit_history",
            "submit_fee",
            "apc",
            "notes",
            "last_updated",
        }:
            db_value = JournalEntry.normalize(str(value))
        else:
            db_value = decode_text(str(value))

        self._conn.execute(
            f"UPDATE journals SET {field_name} = ? WHERE id = ?",
            (db_value, row_id),
        )
        self._conn.commit()

    def title_case_flagged(self) -> int:
        rows = self._conn.execute(
            """
            SELECT id, journal_name, publisher_name, source, country, categories, areas
            FROM journals WHERE flagged = 1
            """
        ).fetchall()
        for row in rows:
            self._conn.execute(
                """
                UPDATE journals
                SET journal_name = ?, publisher_name = ?, source = ?, country = ?,
                    categories = ?, areas = ?
                WHERE id = ?
                """,
                (
                    str(row["journal_name"]).title(),
                    str(row["publisher_name"]).title(),
                    str(row["source"]).title(),
                    str(row["country"]).title(),
                    str(row["categories"]).title(),
                    str(row["areas"]).title(),
                    int(row["id"]),
                ),
            )
        self._conn.commit()
        return len(rows)

    def cleanup_ampersands(self) -> int:
        rows = self._conn.execute(
            "SELECT id, journal_name FROM journals WHERE journal_name LIKE '%&%' OR journal_name LIKE '%&amp;%'"
        ).fetchall()
        for row in rows:
            fixed = str(row["journal_name"]).replace("&amp;", "and").replace("&", "and")
            self._conn.execute(
                "UPDATE journals SET journal_name = ? WHERE id = ?",
                (fixed, int(row["id"])),
            )
        self._conn.commit()
        return len(rows)

    def clear_flags(self) -> int:
        cur = self._conn.execute("UPDATE journals SET flagged = 0 WHERE flagged = 1")
        self._conn.commit()
        return int(cur.rowcount)

    def find_note_matches(self) -> dict[str, dict[str, str]]:
        rows = self._conn.execute(
            """
            SELECT journal_name, info_url, submit_url, submit_history,
                   submit_fee, apc, notes, last_updated
            FROM journals
            """
        ).fetchall()
        index: dict[str, dict[str, str]] = {}
        for row in rows:
            key = str(row["journal_name"]).strip().lower()
            if not key:
                continue
            index[key] = {
                "info_url": str(row["info_url"] or ""),
                "submit_url": str(row["submit_url"] or ""),
                "submit_history": str(row["submit_history"] or ""),
                "submit_fee": str(row["submit_fee"] or ""),
                "apc": str(row["apc"] or ""),
                "notes": str(row["notes"] or ""),
                "last_updated": str(row["last_updated"] or ""),
            }
        return index
