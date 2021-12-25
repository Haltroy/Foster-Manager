# Foster Manager (Foster Manager)

A command-line tool for managing Fosters.

## Versions

<!-- **Latest Stable** Version: **[`1.0.0.0`](https://github.com/Haltroy/Foster/releases/tag/1.0.0.0-preview1)** -->

**Latest Development** Version: **[`1.0.0.0-preview1`](https://github.com/Haltroy/Foster/releases/tag/1.0.0.0-preview1)**

## Features

- Creating packages from a folder.
- Updating a folder.
- Creating or applying delta packages.
- Creating or extracting packages.

## Requirements

Foster Manager can be installed by package managers or can be used in a folder without installing it.

- .NET Core 3.1 supported machine (see [this document](https://github.com/dotnet/core/blob/main/release-notes/3.1/3.1-supported-os.md))
  - NOTE: The Official Document for .NET Core 3.1 might not include every operating system (such as Arch Linux), in order to learn if you operating system supports it or not, search on the internet (for example, I learned that Arch Linux supports .NET Core 3.1 by just typing `dotnet arch` into Google.)
- .NET Core 3.1 or newer installed on machine (required only for framework-dependent packages)

## Usage

Foster Manager can only be used in command-line, you can use the command-line tool that already comes with your operating system (Terminal, PowerShell, CMD, Konsole, etc.). 

To install Foster Manager, follow [this instructions](https://github.com/Haltroy/Foster/blob/master/Foster%20Manager/INSTALL.md).

To see all available commands, run Foster Manager with `--help` option: `fosterman.exe --help`

To see more details, use `--verbose` or `-v` alongside with the other options: `fosterman -v --help`

This option also tells more information on while doing something: `fosterman -v pack ...`

Here's a list of all options:
 - `pack`: Packs a folder into a file.
 - `unpack`: Unpacks a file into a folder.
 - `delta`: Creates delta packages (changes between two folders) into a file.
 - `adelta`: Applies a delta package to a folder.
 - `create`: Batch-creates a Foster system.
 - `install`: Installs a Foster into a directory.
 - `update`: Updates a Foster that is already installed into a directory.

## Releases

| Branch/Version                                                               | Foster | Minimum .NET | Status | Release Time |
|----------------------------------------------------------------------|---------------|---------------------|----------|------------|
| [`master`](https://github.com/haltroy/htalt)              | b1.7.3 | .NET Core 3.1 | Main | - |
| `1.0.0.1` | 1.0 | .NET Core 3.1 | *Development*| 2022 |
| [`1.0.0.0`](https://github.com/Haltroy/Foster/releases/tag/1.0.0.0-preview1) | 1.0-preview1 | .NET Core 3.1 | Unsupported | ? December 2021 |
