import xml.etree.ElementTree as ET
import tempfile
import json
import tkinter as tk
from tkinter import filedialog, messagebox
import os
import subprocess

# Hide main Tk window
root = tk.Tk()
root.withdraw()

# Pre-file selection instructions
messagebox.showinfo(
    "Instructions",
    "Please navigate to your Skua.Manager folder and select 'user.config'.\n\n"
    "Default folder will be opened automatically."
)

# Default Skua.Manager folder
default_dir = os.path.expandvars(r"%localappdata%\Skua.Manager")

# Open file dialog
old_config_path = filedialog.askopenfilename(
    initialdir=default_dir,
    title="Select your old Skua user.config",
    filetypes=[("CONFIG files", "*.config")]
)

if not old_config_path:
    print("No file selected. Exiting.")
    exit()

# Parse XML
try:
    tree = ET.parse(old_config_path)
    root_xml = tree.getroot()
except ET.ParseError:
    print(f"Failed to parse XML in {old_config_path}")
    exit()

# Extract accounts (keep original {=} format)
accounts = []
for s in root_xml.findall(".//ArrayOfString/string"):
    if s.text and "{=}" in s.text:
        accounts.append(s.text)

if not accounts:
    print("No valid accounts found in the selected file.")
    exit()

# Write "ManagedAccounts": [...] to temp file for preview
with tempfile.NamedTemporaryFile("w", delete=False, suffix=".txt") as tmp:
    tmp.write('"ManagedAccounts": ')
    tmp.write(json.dumps(accounts, indent=2))
    temp_path = tmp.name

# Open temp file for preview
subprocess.Popen(["notepad.exe", temp_path])

# Post-conversion instructions
messagebox.showinfo(
    "Next Steps",
    "Accounts have been exported and previewed.\n\n"
    "The script will now attempt to update your skua.settings.json automatically."
)

# Path to skua.settings.json
skua_settings_path = os.path.expandvars(r"%appdata%\Skua\skua.settings.json")

if not os.path.exists(skua_settings_path):
    messagebox.showwarning(
        "File Not Found",
        "Could not find skua.settings.json in %appdata%\\Skua.\n"
        "Please locate it manually to paste the accounts."
    )
else:
    # Read existing JSON
    with open(skua_settings_path, "r", encoding="utf-8") as f:
        try:
            data = json.load(f)
        except json.JSONDecodeError:
            messagebox.showerror(
                "JSON Error",
                "Failed to parse skua.settings.json. Manual paste required."
            )
            exit()

    # Update ManagedAccounts in the manager section
    if "manager" in data:
        data["manager"]["ManagedAccounts"] = accounts
    else:
        data["ManagedAccounts"] = accounts
    
    # Remove any root-level ManagedAccounts if it exists (shouldn't, but just in case)
    if "ManagedAccounts" in data and "manager" in data:
        del data["ManagedAccounts"]

    # Write back updated JSON
    with open(skua_settings_path, "w", encoding="utf-8") as f:
        json.dump(data, f, indent=2)

    messagebox.showinfo(
        "Done",
        "ManagedAccounts have been successfully updated in skua.settings.json."
    )