import re
import json 

DATE_PATTERN = re.compile(r"^\d{1,2}/\d{1,2}/\d{4} \d{1,2}:\d{2}:\d{2} [AP]M$")

def dictConverter(block):
    entry = {
        "timestamp": None,
        "mesh_count": None,
        "meshes": []   # list of mesh dicts
    }

    first_line = block[0]
    mesh_count_match = re.search(r"(\d+) meshes", first_line)
    if mesh_count_match:
        entry["mesh_count"] = int(mesh_count_match.group(1))

    entry["timestamp"] = block[1]
    current_mesh = None

    for line in block[2:]:
        line = line.strip()

        if line.startswith("Mesh"):
            parts = line.split(":")
            name = parts[1].strip()
            current_mesh = {
                "name": name,
                "vertices": None,
                "faces": None
            }
            entry["meshes"].append(current_mesh)

        elif line.startswith("Vertices"):
            vertices = int(line.split(":")[1].strip())
            current_mesh["vertices"] = vertices
        elif line.startswith("Faces"):
            faces = int(line.split(":")[1].strip())
            current_mesh["faces"] = faces

    return entry

def fileReader(filename, target_date, json_path_out=None):
    blocks = []
    current_block = []

    try:
        infile = open(filename, 'r')
        lines = infile.readlines()
        infile.close()
    except Exception as e:
        print(f"An error occured when handling {filename}: {e}")
        return []

    for line in lines:
        line = line.rstrip("\n")

        if line.startswith("Scene loaded with"):
            if current_block:
                blocks.append(current_block)
            current_block = [line]

        elif DATE_PATTERN.match(line):
            # empty conditional checks if the list has items 
            if current_block:
                current_block.append(line)

        else:
            if current_block:
                current_block.append(line)

    if current_block:
        blocks.append(current_block)

    
    for block in blocks:
        if len(block) > 1 and block[1].startswith(target_date):
            print('='*40)
            for line in block:
                print(line)

    entries = [dictConverter(block) for block in blocks]
    entries = [e for e in entries if e["timestamp"].startswith(target_date)]

    if json_path_out:
        try:
            outfile = open(json_path_out, 'w')
            json.dump(entries, outfile, indent=4)
            print(f"JSON written to: {json_path_out}")
            outfile.close()
        except Exception as e:
            print(F"An error occured when writing to JSON: {json_path_out}")
            
    return entries 

# date = input("Please enter target date(MM/DD/YYYY): ")
date = "12/4/2025"
entries = fileReader('debug_output.txt', date, json_path_out="debug_output.json")
print(entries)

# test_block = [
#     "Scene loaded with 2 meshes",
#     "12/2/2025 1:26:01 PM",
#     "Mesh 0: Cube.001",
#     "  Vertices: 2411",
#     "  Faces: 972",
#     "Mesh 1: Cube.001",
#     "  Vertices: 2411",
#     "  Faces: 972"
# ]

# result = dictConverter(test_block)
# print(result)