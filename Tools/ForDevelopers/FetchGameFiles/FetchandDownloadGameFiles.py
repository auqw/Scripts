"""
AQ Game Fetcher -- GUI edition.

Fetches AQW game resources:
  - Client        (Game####.swf)   -- via gameversion API, falls back to
                                       exponential+binary search if the API
                                       is ever unavailable
  - Game Assets   (Assets_YYYYMMDD.swf, date-stamped -- you supply the date)
  - Game Version  (JSON metadata)
  - Servers       (JSON server list)

Everything runs on a background thread so the window never freezes, with
a shared log panel and per-resource Fetch buttons.
"""

import json
import os
import platform
import queue
import random
import subprocess
import threading
import time
import tkinter as tk
from tkinter import ttk, scrolledtext

import requests

# --------------------------------------------------------------------------
# Config
# --------------------------------------------------------------------------

GAMEFILES_BASE = "https://game.aq.com/game/gamefiles/{}"
CLIENT_URL_TMPL = "https://game.aq.com/game/gamefiles/Game{}.swf"
ASSETS_URL_TMPL = "https://game.aq.com/game/gamefiles/interface/Assets/Assets_{}.swf"
GAMEVERSION_URL = "https://game.aq.com/game/api/data/gameversion"
SERVERS_URL = "https://game.aq.com/game/api/data/servers"

START_NUMBER = 3097       # fallback seed for the manual-search path only
TIMEOUT = 10

MIN_DELAY = 0.25
MAX_DELAY = 0.75

MAX_RETRIES = 4
BACKOFF_BASE = 5

VERBOSE = False

SCRIPT_DIR = os.path.dirname(os.path.abspath(__file__))
STATE_FILE = os.path.join(SCRIPT_DIR, ".last_version")

HEADERS = {
    "User-Agent": (
        "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 "
        "(KHTML, like Gecko) Chrome/124.0.0.0 Safari/537.36"
    ),
    "Accept": "*/*",
    "Accept-Language": "en-US,en;q=0.9",
    "Connection": "keep-alive",
}

session = requests.Session()
session.headers.update(HEADERS)

# --------------------------------------------------------------------------
# Colors (dark theme)
# --------------------------------------------------------------------------

BG = "#1e1f26"
PANEL = "#262832"
ROW_BG = "#2c2f3a"
FG = "#e6e6e6"
ACCENT = "#7aa2f7"
GREEN = "#9ece6a"
YELLOW = "#e0af68"
RED = "#f7768e"
DIM = "#6b7089"

FONT_UI = ("Segoe UI", 10)
FONT_UI_BOLD = ("Segoe UI Semibold", 10)
FONT_MONO = ("Consolas", 10)
FONT_TITLE = ("Segoe UI Semibold", 13)


class Cancelled(Exception):
    pass


# --------------------------------------------------------------------------
# Worker logic -- everything reports through a queue, nothing prints directly
# --------------------------------------------------------------------------

class Fetcher:
    """Runs one resource fetch on a background thread."""

    def __init__(self, out_queue: queue.Queue):
        self.q = out_queue
        self.cancel_flag = threading.Event()
        self.checks_done = 0

    # -- messaging -----------------------------------------------------

    def emit(self, kind, **payload):
        self.q.put((kind, payload))

    def log(self, msg, level="info"):
        self.emit("log", msg=msg, level=level)

    def status(self, msg):
        self.emit("status", msg=msg)

    def phase(self, title):
        self.emit("phase", title=title)

    def progress(self, pct):
        self.emit("progress", pct=pct)

    # -- generic HTTP -----------------------------------------------------

    def get_json(self, url):
        resp = session.get(url, timeout=TIMEOUT)
        resp.raise_for_status()
        return resp.json()

    def download_file(self, url, dest_path, show_progress=True):
        start_time = time.time()
        with session.get(url, timeout=TIMEOUT, stream=True) as resp:
            resp.raise_for_status()
            total = int(resp.headers.get("Content-Length", 0))
            downloaded = 0
            with open(dest_path, "wb") as f:
                for chunk in resp.iter_content(chunk_size=65536):
                    if self.cancel_flag.is_set():
                        raise Cancelled()
                    if not chunk:
                        continue
                    f.write(chunk)
                    downloaded += len(chunk)
                    if total and show_progress:
                        pct = downloaded / total * 100
                        elapsed = max(time.time() - start_time, 1e-6)
                        speed = downloaded / elapsed
                        self.progress(pct)
                        self.status(
                            f"Downloading...  {downloaded/1024/1024:.2f}/"
                            f"{total/1024/1024:.2f} MB  ({speed/1024/1024:.2f} MB/s)"
                        )
        return time.time() - start_time

    # -- manual search fallback (only used if the API call fails) ----------

    def _looks_like_real_swf(self, resp):
        if resp.history and not resp.url.lower().endswith(".swf"):
            return False
        if "text/html" in resp.headers.get("Content-Type", "").lower():
            return False
        return True

    def _request_once(self, url):
        resp = session.head(url, timeout=TIMEOUT, allow_redirects=True)
        if resp.status_code in (405, 501):
            resp = session.get(url, timeout=TIMEOUT, stream=True)
            resp.close()
        return resp

    def exists(self, url):
        delay = BACKOFF_BASE
        for attempt in range(1, MAX_RETRIES + 1):
            if self.cancel_flag.is_set():
                raise Cancelled()
            try:
                resp = self._request_once(url)
            except requests.RequestException as e:
                if VERBOSE:
                    self.log(f"request failed ({attempt}/{MAX_RETRIES}): {e}", "warn")
            else:
                if resp.status_code == 429 or resp.status_code >= 500:
                    wait = float(resp.headers.get("Retry-After", delay))
                    time.sleep(wait)
                    delay *= 2
                    continue
                if resp.status_code != 200:
                    return False
                return self._looks_like_real_swf(resp)
            time.sleep(delay)
            delay *= 2
        return False

    def polite_pause(self):
        time.sleep(random.uniform(MIN_DELAY, MAX_DELAY))

    def load_last_version(self, default):
        try:
            with open(STATE_FILE, "r") as f:
                return int(f.read().strip())
        except (OSError, ValueError):
            return default

    def save_last_version(self, n):
        try:
            with open(STATE_FILE, "w") as f:
                f.write(str(n))
        except OSError as e:
            self.log(f"could not save last version: {e}", "warn")

    def _check(self, n):
        self.checks_done += 1
        found = self.exists(CLIENT_URL_TMPL.format(n))
        self.status(f"Checking Game{n}.swf...  ({self.checks_done} probes)")
        if found:
            self.log(f"Game{n}.swf found", "ok")
        self.polite_pause()
        return found

    def search_client_number(self):
        """Exponential + binary search fallback, only used if the API fails."""
        self.checks_done = 0
        start = self.load_last_version(START_NUMBER)
        self.phase("Fallback: searching for latest Game####.swf")

        if not self._check(start):
            if start != START_NUMBER:
                start = START_NUMBER
                if not self._check(start):
                    return None
            else:
                return None

        last_good = start
        step = 1
        while True:
            n = last_good + step
            if self._check(n):
                last_good = n
                step *= 2
            else:
                first_bad = n
                break

        lo, hi = last_good, first_bad
        while hi - lo > 1:
            mid = (lo + hi) // 2
            if self._check(mid):
                lo = mid
            else:
                hi = mid
        return lo

    # -- resource jobs -------------------------------------------------------

    def run_client(self):
        self.phase("Fetching client")
        try:
            self.status("Reading current version from gameversion API...")
            data = self.get_json(GAMEVERSION_URL)
            sfile = data["sFile"]
            self.log(f"gameversion API reports: {sfile} "
                     f"(build {data.get('sVersion', '?')})", "ok")
        except Exception as e:
            self.log(f"gameversion API failed ({e}), falling back to search", "warn")
            n = self.search_client_number()
            if n is None:
                self.log("Could not determine latest client version.", "error")
                self.emit("done", ok=False)
                return
            sfile = f"Game{n}.swf"

        url = GAMEFILES_BASE.format(sfile)
        dest = os.path.join(SCRIPT_DIR, sfile)
        self.status(f"Downloading {sfile}...")
        duration = self.download_file(url, dest)
        self.progress(100)
        self.log(f"Saved: {dest}", "ok")

        # remember the numeric version for the fallback search next time
        digits = "".join(c for c in sfile if c.isdigit())
        if digits:
            self.save_last_version(int(digits))

        self.emit("done", ok=True, dest=dest,
                   summary=f"{sfile} in {duration:.2f}s")

    def run_assets(self, date_str):
        self.phase("Fetching game assets")
        fname = f"Assets_{date_str}.swf"
        url = ASSETS_URL_TMPL.format(date_str)
        dest = os.path.join(SCRIPT_DIR, fname)
        self.status(f"Downloading {fname}...")
        try:
            duration = self.download_file(url, dest)
        except requests.RequestException as e:
            self.log(f"Download failed: {e}", "error")
            self.log("The assets filename is date-stamped and not exposed by "
                     "the version API -- double check the date is correct.",
                     "warn")
            self.emit("done", ok=False)
            return
        self.progress(100)
        self.log(f"Saved: {dest}", "ok")
        self.emit("done", ok=True, dest=dest,
                   summary=f"{fname} in {duration:.2f}s")

    def run_gameversion(self):
        self.phase("Fetching game version info")
        try:
            self.status("Requesting gameversion API...")
            data = self.get_json(GAMEVERSION_URL)
        except Exception as e:
            self.log(f"Request failed: {e}", "error")
            self.emit("done", ok=False)
            return

        for k, v in data.items():
            self.log(f"{k}: {v}", "info")

        dest = os.path.join(SCRIPT_DIR, "gameversion.json")
        with open(dest, "w") as f:
            json.dump(data, f, indent=2)
        self.log(f"Saved: {dest}", "ok")
        self.emit("done", ok=True, dest=dest, summary="gameversion.json saved")

    def run_servers(self):
        self.phase("Fetching server list")
        try:
            self.status("Requesting servers API...")
            data = self.get_json(SERVERS_URL)
        except Exception as e:
            self.log(f"Request failed: {e}", "error")
            self.emit("done", ok=False)
            return

        online = sum(1 for s in data if s.get("bOnline"))
        self.log(f"{len(data)} servers listed, {online} online", "ok")
        for s in sorted(data, key=lambda s: -s.get("iCount", 0)):
            self.log(f"  {s['sName']:<18} {s['iCount']:>4}/{s['iMax']:<4} "
                     f"({s['sIP']}:{s['iPort']})", "info")

        dest = os.path.join(SCRIPT_DIR, "servers.json")
        with open(dest, "w") as f:
            json.dump(data, f, indent=2)
        self.log(f"Saved: {dest}", "ok")
        self.emit("done", ok=True, dest=dest, summary="servers.json saved")


# --------------------------------------------------------------------------
# GUI
# --------------------------------------------------------------------------

class ResourceRow(tk.Frame):
    """One resource with a label, optional input, status text, and a Fetch button."""

    def __init__(self, master, title, subtitle, on_fetch, extra_widget=None):
        super().__init__(master, bg=ROW_BG)
        self.on_fetch = on_fetch

        pad = dict(padx=12, pady=10)

        text_frame = tk.Frame(self, bg=ROW_BG)
        text_frame.pack(side="left", fill="x", expand=True, **pad)

        tk.Label(text_frame, text=title, font=FONT_UI_BOLD,
                 bg=ROW_BG, fg=FG, anchor="w").pack(fill="x")
        tk.Label(text_frame, text=subtitle, font=("Segoe UI", 8),
                 bg=ROW_BG, fg=DIM, anchor="w").pack(fill="x")

        if extra_widget is not None:
            extra_widget(text_frame).pack(anchor="w", pady=(4, 0))

        self.status_var = tk.StringVar(value="")
        tk.Label(self, textvariable=self.status_var, font=("Segoe UI", 8),
                 bg=ROW_BG, fg=ACCENT, width=16, anchor="e").pack(
            side="left", padx=(0, 8))

        self.btn = tk.Button(
            self, text="Fetch", font=FONT_UI, command=self._fetch,
            bg=ACCENT, fg="#101116", activebackground="#5f86d6",
            relief="flat", padx=14, pady=6, cursor="hand2"
        )
        self.btn.pack(side="right", padx=12, pady=10)

    def _fetch(self):
        self.on_fetch(self)

    def set_busy(self, busy):
        self.btn.configure(state="disabled" if busy else "normal")

    def set_status(self, text):
        self.status_var.set(text)


class App(tk.Tk):
    def __init__(self):
        super().__init__()
        self.title("AQ Game Fetcher")
        self.geometry("620x640")
        self.minsize(520, 500)
        self.configure(bg=BG)

        self.worker = None
        self.fetcher = None
        self.msg_queue = queue.Queue()
        self.active_row = None
        self.last_dest_dir = SCRIPT_DIR

        self.assets_date_var = tk.StringVar(value="20250328")

        self._build_ui()
        self.after(80, self._poll_queue)

    # -- layout --------------------------------------------------------------

    def _build_ui(self):
        header = tk.Frame(self, bg=BG)
        header.pack(fill="x", padx=18, pady=(18, 6))
        tk.Label(header, text="AQ Game Fetcher", font=FONT_TITLE,
                  bg=BG, fg=FG).pack(side="left")
        tk.Label(header, text="game.aq.com resource downloader",
                  font=FONT_UI, bg=BG, fg=DIM).pack(side="left", padx=(10, 0))

        self.status_var = tk.StringVar(value="Ready.")
        tk.Label(self, textvariable=self.status_var, font=FONT_UI,
                  bg=BG, fg=ACCENT, anchor="w").pack(fill="x", padx=18)

        style = ttk.Style(self)
        style.theme_use("default")
        style.configure("Fetch.Horizontal.TProgressbar",
                         troughcolor=PANEL, background=ACCENT,
                         bordercolor=PANEL, lightcolor=ACCENT, darkcolor=ACCENT)
        self.progress_var = tk.DoubleVar(value=0)
        ttk.Progressbar(self, variable=self.progress_var, maximum=100,
                         style="Fetch.Horizontal.TProgressbar").pack(
            fill="x", padx=18, pady=(6, 12))

        # Resource rows
        rows_frame = tk.Frame(self, bg=BG)
        rows_frame.pack(fill="x", padx=18)

        def make_row(title, subtitle, handler, extra_widget=None):
            row = ResourceRow(rows_frame, title, subtitle, handler, extra_widget)
            row.pack(fill="x", pady=4)
            return row

        self.client_row = make_row(
            "Client", "Game####.swf -- via gameversion API (search fallback)",
            self.fetch_client
        )

        def assets_extra(parent):
            f = tk.Frame(parent, bg=ROW_BG)
            tk.Label(f, text="Date (YYYYMMDD):", font=("Segoe UI", 8),
                     bg=ROW_BG, fg=DIM).pack(side="left")
            entry = tk.Entry(f, textvariable=self.assets_date_var, width=10,
                              bg=PANEL, fg=FG, insertbackground=FG,
                              relief="flat")
            entry.pack(side="left", padx=(6, 0))
            return f

        self.assets_row = make_row(
            "Game Assets", "Assets_YYYYMMDD.swf -- date-stamped, set manually",
            self.fetch_assets, extra_widget=assets_extra
        )

        self.version_row = make_row(
            "Game Version", "JSON metadata (current file, title, build)",
            self.fetch_version
        )

        self.servers_row = make_row(
            "Servers", "JSON server list with live population",
            self.fetch_servers
        )

        self.rows = [self.client_row, self.assets_row,
                     self.version_row, self.servers_row]

        # Log panel
        log_frame = tk.Frame(self, bg=PANEL)
        log_frame.pack(fill="both", expand=True, padx=18, pady=(12, 0))

        self.log_box = scrolledtext.ScrolledText(
            log_frame, bg=PANEL, fg=FG, insertbackground=FG,
            font=FONT_MONO, wrap="word", borderwidth=0, highlightthickness=0,
            state="disabled"
        )
        self.log_box.pack(fill="both", expand=True, padx=1, pady=1)

        for tag, color in (("ok", GREEN), ("warn", YELLOW),
                            ("error", RED), ("info", DIM),
                            ("phase", ACCENT)):
            self.log_box.tag_configure(tag, foreground=color)

        # Bottom bar
        btn_frame = tk.Frame(self, bg=BG)
        btn_frame.pack(fill="x", padx=18, pady=18)

        self.cancel_btn = tk.Button(
            btn_frame, text="Cancel", font=FONT_UI, command=self.cancel_fetch,
            bg=RED, fg="#101116", activebackground="#c94f63",
            relief="flat", padx=16, pady=8, cursor="hand2", state="disabled"
        )
        self.cancel_btn.pack(side="left")

        self.open_btn = tk.Button(
            btn_frame, text="Open Folder", font=FONT_UI, command=self.open_folder,
            bg=PANEL, fg=FG, activebackground="#33364a",
            relief="flat", padx=16, pady=8, cursor="hand2"
        )
        self.open_btn.pack(side="left", padx=(10, 0))

        self.clear_btn = tk.Button(
            btn_frame, text="Clear Log", font=FONT_UI, command=self.clear_log,
            bg=PANEL, fg=FG, activebackground="#33364a",
            relief="flat", padx=16, pady=8, cursor="hand2"
        )
        self.clear_btn.pack(side="left", padx=(10, 0))

    # -- log helpers -----------------------------------------------------------

    def append_log(self, msg, tag="info"):
        self.log_box.configure(state="normal")
        prefix = {"ok": "[OK]   ", "warn": "[WARN] ", "error": "[FAIL] ",
                  "phase": "=== ", "info": "  "}.get(tag, "  ")
        suffix = " ===" if tag == "phase" else ""
        self.log_box.insert("end", f"{prefix}{msg}{suffix}\n", tag)
        self.log_box.see("end")
        self.log_box.configure(state="disabled")

    # -- job control -----------------------------------------------------

    def _start_job(self, row, target_name, *args):
        if self.worker and self.worker.is_alive():
            return
        self.active_row = row
        self.progress_var.set(0)
        self.status_var.set("Starting...")
        row.set_status("Working...")
        for r in self.rows:
            r.set_busy(True)
        self.cancel_btn.configure(state="normal")

        self.msg_queue = queue.Queue()
        self.fetcher = Fetcher(self.msg_queue)
        target = getattr(self.fetcher, target_name)
        self.worker = threading.Thread(target=target, args=args, daemon=True)
        self.worker.start()

    def fetch_client(self, row):
        self._start_job(row, "run_client")

    def fetch_assets(self, row):
        date_str = self.assets_date_var.get().strip()
        if not (len(date_str) == 8 and date_str.isdigit()):
            self.append_log("Assets date must be 8 digits, e.g. 20250328", "error")
            return
        self._start_job(row, "run_assets", date_str)

    def fetch_version(self, row):
        self._start_job(row, "run_gameversion")

    def fetch_servers(self, row):
        self._start_job(row, "run_servers")

    def cancel_fetch(self):
        if self.fetcher:
            self.fetcher.cancel_flag.set()
        self.status_var.set("Cancelling...")

    def open_folder(self):
        path = self.last_dest_dir
        system = platform.system()
        try:
            if system == "Windows":
                os.startfile(path)  # type: ignore[attr-defined]
            elif system == "Darwin":
                subprocess.run(["open", path], check=False)
            else:
                subprocess.run(["xdg-open", path], check=False)
        except Exception as e:
            self.append_log(f"could not open folder: {e}", "warn")

    def clear_log(self):
        self.log_box.configure(state="normal")
        self.log_box.delete("1.0", "end")
        self.log_box.configure(state="disabled")

    def _job_finished(self):
        for r in self.rows:
            r.set_busy(False)
        self.cancel_btn.configure(state="disabled")
        if self.active_row:
            self.active_row.set_status("")
        self.active_row = None

    # -- queue polling -----------------------------------------------------

    def _poll_queue(self):
        try:
            while True:
                kind, payload = self.msg_queue.get_nowait()
                if kind == "log":
                    self.append_log(payload["msg"], payload.get("level", "info"))
                elif kind == "status":
                    self.status_var.set(payload["msg"])
                    if self.active_row:
                        self.active_row.set_status(payload["msg"][:20])
                elif kind == "phase":
                    self.append_log(payload["title"], "phase")
                elif kind == "progress":
                    self.progress_var.set(payload["pct"])
                elif kind == "done":
                    if payload.get("ok"):
                        self.status_var.set(f"Done -- {payload.get('summary', '')}")
                        self.last_dest_dir = os.path.dirname(payload["dest"])
                    else:
                        self.status_var.set("Failed -- see log.")
                    self._job_finished()
        except queue.Empty:
            pass
        self.after(80, self._poll_queue)


if __name__ == "__main__":
    App().mainloop()