from __future__ import annotations

from pathlib import Path
import re
import xml.etree.ElementTree as ET

from .constants import DISCONTINUED_IDENTIFIER
from .model import JournalEntry, decode_text


def _match_name(name: str) -> str:
    return name.strip().lower().replace(DISCONTINUED_IDENTIFIER, "")


_CHAR_REF_PATTERN = re.compile(r"&#(x[0-9A-Fa-f]+|[0-9]+);")


def _is_valid_xml_codepoint(codepoint: int) -> bool:
    return (
        codepoint in (0x9, 0xA, 0xD)
        or 0x20 <= codepoint <= 0xD7FF
        or 0xE000 <= codepoint <= 0xFFFD
        or 0x10000 <= codepoint <= 0x10FFFF
    )


def _sanitize_xml_text(text: str) -> str:
    def _replace_char_ref(match: re.Match[str]) -> str:
        raw = match.group(1)
        code = int(raw[1:], 16) if raw.lower().startswith("x") else int(raw, 10)
        return match.group(0) if _is_valid_xml_codepoint(code) else ""

    text = _CHAR_REF_PATTERN.sub(_replace_char_ref, text)
    return "".join(ch for ch in text if _is_valid_xml_codepoint(ord(ch)))


def _load_root(xml_path: Path) -> ET.Element:
    raw = xml_path.read_text(encoding="utf-8", errors="replace")
    try:
        return ET.fromstring(raw)
    except ET.ParseError:
        # Some source files contain invalid XML char refs (for example &#x1C;).
        # Sanitize and retry so migration can continue.
        cleaned = _sanitize_xml_text(raw)
        return ET.fromstring(cleaned)


def _count_journal_nodes(xml_path: Path) -> int:
    count = 0
    with xml_path.open("r", encoding="utf-8", errors="ignore") as handle:
        for line in handle:
            if "<Journal " in line:
                count += 1
    return count


def discover_best_legacy_xml(repo_root: Path) -> tuple[Path, Path | None] | None:
    candidates = [
        repo_root / "Journal Search" / "JournalSearchData.xml",
        repo_root / "Journal Search" / "bin" / "Debug" / "JournalSearchData.xml",
        repo_root / "Journal Search" / "bin" / "Debug" / "biggerData" / "JournalSearchData.xml",
        repo_root / "Journal Search" / "bin" / "Debug" / "holding" / "JournalSearchData.xml",
    ]

    best_main: Path | None = None
    best_count = -1
    for main in candidates:
        if not main.exists():
            continue
        count = _count_journal_nodes(main)
        if count > best_count:
            best_main = main
            best_count = count

    if not best_main or best_count <= 0:
        return None

    extra = best_main.with_name("JournalSearchDataExtra.xml")
    return best_main, (extra if extra.exists() else None)


def load_legacy_xml(main_xml: Path, extra_xml: Path | None) -> list[JournalEntry]:
    if not main_xml.exists():
        return []

    root = _load_root(main_xml)
    entries: list[JournalEntry] = []
    for node in root.findall("Journal"):
        entries.append(
            JournalEntry(
                journal_name=decode_text(node.get("JournalName", "")),
                publisher_name=decode_text(node.get("PublisherName", "")),
                issns=decode_text(node.get("ISSNs", "")),
                source=decode_text(node.get("Source", "")),
                ranking=decode_text(node.get("Ranking", "")),
                rating=decode_text(node.get("Rating", "")),
                h_index=decode_text(node.get("HIndex", "")),
                country=decode_text(node.get("Country", "")),
                categories=decode_text(node.get("Categories", "")),
                areas=decode_text(node.get("Areas", "")),
            )
        )

    if extra_xml and extra_xml.exists():
        extra_root = _load_root(extra_xml)
        notes_map: dict[str, dict[str, str]] = {}
        for node in extra_root.findall("JournalNote"):
            journal_name = node.get("JournalName", "")
            if not journal_name:
                continue
            notes_map[_match_name(journal_name)] = {
                "info_url": (node.findtext("InfoURL") or "").strip(),
                "submit_url": (node.findtext("SubmitURL") or "").strip(),
                "submit_history": (node.findtext("SubmitHistory") or "").strip(),
                "submit_fee": (node.findtext("SubmitFee") or "").strip(),
                "apc": (node.findtext("APC") or "").strip(),
                "notes": (node.findtext("Notes") or "").strip(),
            }

        for entry in entries:
            data = notes_map.get(_match_name(entry.journal_name))
            if not data:
                continue
            entry.info_url = data["info_url"]
            entry.submit_url = data["submit_url"]
            entry.submit_history = data["submit_history"]
            entry.submit_fee = data["submit_fee"]
            entry.apc = data["apc"]
            entry.notes = data["notes"]

    return entries
