# -----------------------------------------------------------------------------
# Instructions:
# 1. Set the 'root_dir' variable below to the root path of your scripts folder.
#    Example: root_dir = r'C:\Path\To\Your\Scripts'
# 2. Make sure CaseStorage.cs exists at Tools\ForDevelopers\CaseStorage.cs
#    relative to your root_dir.
# 3. Save this script.
# 4. Run the script using Python:
#       - Open a terminal/command prompt.
#       - Navigate to the folder containing this script.
#       - Run: python cases.py
# 5. The script will update CaseStorage.cs by adding any new cases found.
# -----------------------------------------------------------------------------
import os
import re
import time
from concurrent.futures import ThreadPoolExecutor, as_completed

# Automatically resolve Skua Scripts directory like in C#
appdata_dir = os.getenv("APPDATA")  # Typically C:\Users\<User>\AppData\Roaming
SkuaDIR = os.path.join(appdata_dir, "Skua")
SkuaScriptsDIR = os.path.join(SkuaDIR, "Scripts")

# Set root_dir to the Skua Scripts directory
root_dir = SkuaScriptsDIR
case_storage_path = os.path.join(root_dir, r"Tools\ForDevelopers\CaseStorage.cs")

# Pre-compile regexes for speed
case_name_re = re.compile(r'{\s*"([^"]+)"\s*,')
case_label_re = re.compile(r'\s*case\s+"([^"]+)"\s*:')

# 1. Load existing cases from CaseStorage.cs with file existence check
if not os.path.exists(case_storage_path):
    print(f"Error: CaseStorage.cs not found at:\n{case_storage_path}")
    exit(1)
else:
    with open(case_storage_path, encoding="utf-8") as f:
        case_storage_content = f.read()

existing_cases = set(case_name_re.findall(case_storage_content))

# 2. Find all *merge*.cs files
merge_files = []
for dirpath, _, filenames in os.walk(root_dir):
    for fn in filenames:
        if fn.lower().endswith(".cs") and "merge" in fn.lower():
            merge_files.append(os.path.join(dirpath, fn))

print(f"Found {len(merge_files)} merge script(s):")
for f in merge_files:
    print(f" - {f}")

# 3. Extract all case blocks from merge scripts
def extract_case_blocks(content):
    lines = content.splitlines()
    n = len(lines)
    i = 0
    case_blocks = []
    while i < n:
        # Find start of a case group
        case_labels = []
        while i < n:
            m = case_label_re.match(lines[i])
            if m:
                case_labels.append(m.group(1))
                i += 1
            else:
                break
        if case_labels:
            block_lines = []
            brace_depth = 0
            while i < n:
                line = lines[i]
                # Check for new case/default only if not inside a nested block
                if brace_depth == 0 and (
                    case_label_re.match(line)
                    or re.match(r"\s*default\s*:", line)
                    or re.match(r"\s*#endregion", line)
                ):
                    break
                # Track braces
                brace_depth += line.count("{")
                brace_depth -= line.count("}")
                block_lines.append(line)
                i += 1
                # Yield CPU briefly every 50 lines to reduce CPU hogging
                if i % 50 == 0:
                    time.sleep(0.01)
                if brace_depth < 0:
                    break
            # Remove trailing empty lines
            while block_lines and not block_lines[-1].strip():
                block_lines.pop()
            # Remove trailing '}' lines
            while block_lines and block_lines[-1].strip() == "}":
                block_lines.pop()
            case_blocks.append((case_labels, block_lines))
        else:
            i += 1
    return case_blocks

# Helper to process each file in parallel
def process_file(file_path):
    with open(file_path, encoding="utf-8") as f:
        content = f.read()
    cases_found = []
    blocks = extract_case_blocks(content)
    for case_labels, block_lines in blocks:
        cases_found.extend(case_labels)
    return file_path, cases_found, blocks

new_cases = {}

max_workers = min(4, os.cpu_count() or 4)  # Adjust max concurrency for your environment

with ThreadPoolExecutor(max_workers=max_workers) as executor:
    futures = {executor.submit(process_file, f): f for f in merge_files}
    for future in as_completed(futures):
        file = futures[future]
        try:
            file_path, cases_in_file, blocks = future.result()
            added_before = len(new_cases)
            for case_labels, block_lines in blocks:
                for case_name in case_labels:
                    if case_name not in existing_cases and case_name not in new_cases:
                        block = f'case "{case_name}":\n' + "\n".join(block_lines)
                        block_escaped = block.replace('"', '""')
                        new_cases[case_name] = block_escaped
            added_after = len(new_cases)
            print(
                f"File '{file_path}': Found {len(cases_in_file)} case(s), Added {added_after - added_before} new case(s) so far."
            )
        except Exception as e:
            print(f"Error processing {file}: {e}")

if not new_cases:
    print("No new cases found.")
    exit(0)

print(f"Total new cases to insert: {len(new_cases)}")

# 4. Prepare new cases in CaseStorage.cs dictionary format
formatted_cases = []
for name, block in new_cases.items():
    formatted = f'{{\n    "{name}",\n    @"\n{block}\n    "\n}},\n'
    formatted_cases.append(formatted)

# 5. Insert new cases before the closing '};'
insert_point = case_storage_content.rfind("};")
if insert_point == -1:
    print("Could not find end of dictionary in CaseStorage.cs!")
    exit(1)

new_content = (
    case_storage_content[:insert_point]
    + "".join(formatted_cases)
    + case_storage_content[insert_point:]
)

# 6. Write back to CaseStorage.cs (NO BACKUP)
with open(case_storage_path, "w", encoding="utf-8") as f:
    f.write(new_content)

print(f"Inserted {len(new_cases)} new cases into CaseStorage.cs.")
print("Done.")
