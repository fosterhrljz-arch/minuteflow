"""Audit the CPU speech runtime's PE imports against packaged files and Windows DLLs."""
import pathlib
import struct

ROOT = pathlib.Path(__file__).resolve().parents[1] / 'artifacts' / 'MinuteFlow'
SYSTEM = {'kernel32.dll', 'advapi32.dll', 'ntdll.dll', 'user32.dll', 'ole32.dll',
          'oleaut32.dll', 'shell32.dll', 'ws2_32.dll', 'bcrypt.dll', 'version.dll'}


def imports(path):
    blob = path.read_bytes()
    pe = struct.unpack_from('<I', blob, 0x3c)[0]
    count = struct.unpack_from('<H', blob, pe + 6)[0]
    optional_size = struct.unpack_from('<H', blob, pe + 20)[0]
    optional = pe + 24
    is64 = struct.unpack_from('<H', blob, optional)[0] == 0x20b
    directory = optional + (112 if is64 else 96)
    import_rva = struct.unpack_from('<I', blob, directory + 8)[0]
    sections = optional + optional_size

    def offset(rva):
        for i in range(count):
            virtual_size, virtual_address, raw_size, raw_pointer = struct.unpack_from(
                '<IIII', blob, sections + i * 40 + 8)
            if virtual_address <= rva < virtual_address + max(virtual_size, raw_size):
                return raw_pointer + rva - virtual_address
        return rva

    if not import_rva:
        return []
    cursor = offset(import_rva)
    names = []
    while any(struct.unpack_from('<IIIII', blob, cursor)):
        name = offset(struct.unpack_from('<I', blob, cursor + 12)[0])
        names.append(blob[name:blob.index(b'\0', name)].decode().lower())
        cursor += 20
    return names


available = {p.name.lower(): p for p in ROOT.glob('*.dll')}
available.update({p.name.lower(): p for p in (ROOT / 'runtimes' / 'win-x64').glob('*.dll')})
queue = list((ROOT / 'runtimes' / 'win-x64').glob('*.dll'))
checked = set()
while queue:
    path = queue.pop()
    if path.name.lower() in checked:
        continue
    checked.add(path.name.lower())
    for name in imports(path):
        if name in SYSTEM or name.startswith(('api-ms-win-', 'ext-ms-win-')):
            continue
        if name not in available:
            raise SystemExit(f'MISSING DEPENDENCY: {path.name} -> {name}')
        queue.append(available[name])
print(f'PASS: {len(checked)} native speech/runtime DLLs; all non-system imports are bundled.')
