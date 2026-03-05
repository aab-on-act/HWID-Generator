# AAB HWID Generator

A command-line tool written in C# designed to generate a unique Hardware Identifier (HWID) based on several system components.

## Overview

The `HwidGenerator` reads identifying information from the following hardware components using WMI (Windows Management Instrumentation):
- Motherboard Serial Number
- CPU Processor ID
- Disk Drive Serial Number
- BIOS UUID
- Network Adapter MAC Address

The information from each component is then hashed using SHA-256 and concatenated with a delimiter to produce a secure, consistent final `AABINFO` string.

## Requirements

- **OS:** Windows (Due to extensive reliance on `Win32` WMI classes)
- **Runtime:** .NET 9.0 (as specified in `HwidGenerator.csproj`)

## How It Works

1. Queries WMI classes (`Win32_BaseBoard`, `Win32_Processor`, `Win32_DiskDrive`, `Win32_ComputerSystemProduct`) and `NetworkInterface.GetAllNetworkInterfaces()` for hardware serials and IDs.
2. If any query fails or returns an empty value, it defaults to `"UNKNOWN"` or `"NOMAC"`.
3. Hashes the values individually using `SHA256` (taking the first 20 characters of the lowercased hex string).
4. Concatenates these hashes using the delimiter `zczc`.
5. Copies the final generated string to the user's clipboard and shows a confirmation message box.

## Building the Project

### Using .NET CLI

1. Ensure you have the .NET 9.0 SDK installed.
2. Clone this repository.
3. Run the following command in the directory containing `HwidGenerator.csproj`:

```bash
dotnet build --configuration Release
```

The resulting executable will be found in the `bin/Release/net9.0-windows/` directory.

### Autobuild

This repository includes a GitHub Actions workflow that automatically builds the project upon new pushes or Pull Requests to the `main`/`master` branch. You can download the pre-compiled executable directly from the **Actions** tab.

## Note
This tool requires the `System.Management` package, which is essential for interacting with WMI. The project uses `<UseWindowsForms>true</UseWindowsForms>` solely for copying data to the clipboard and displaying message boxes.
