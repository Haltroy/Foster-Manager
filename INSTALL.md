# Installation

Foster Manager can be used without installing it, but you can still install it into your machine.

To use Foster Mananager without installing, open a command prompt/terminal emulator and navigate to the folder which hosts the program.

In *nix systems (such as GNU/Linux distribution such as Debian, Fedora, openSUSE, Arch Linux, Gentoo, Void Linux and all other distibutions), you have to make the file executable with this command: `sudo chmod +x ./fosterman` if it's not already.

## Windows
First, determine which architecture you should download.
Hit `Ctrl` and `R` at the same time on your keyboard and type `shell:::{bb06c0e4-d293-4f75-8a90-cb05b6477eee}` and hit `Enter`. You can find your processor type on `System > System type:` field.
Foster Manager can be installed into these processor types: `x86 x64 arm arm64`

Then, download the installer that is designed for your processor in [Releases page](https://github.com/haltroy/foster/releases/). Names of file looks like this: `fosterman-installer-[Processor type].exe`

You can verify the package by right-clicking on the folder who hosts the installer file while holding `Shift` key and selecting `Open Powershell`. Then you can compare the outputs of the this command with checksum of installer file, which can be found in same release notes. The Powershell command: `Get-FileHash [Installer file name] -Algorithm [Algorthm Name]` 

And finally, start the installer by opening it. If an antivirus blocks the process, it is most likely to be a false alarm. If that happens or the installer contains malicious code, please let me know by [opening an issue about it](https://github.com/haltroy/foster/issues/new).

## Linux & macOS
To learn what is your processor architecture is, open a terminal editor and type this: `$ uname -a`. 

The output looks similar to this: `Linux HALTROY-WIN10 5.10.16.3-microsoft-standard-WSL2 #1 SMP Fri Apr 2 22:23:49 UTC 2021 `**`x86_64`**` GNU/Linux` 
The translation of each processor are:
 - `x86`, `arm` and `arm64` stay same.
 - `x86_64` or `x64` is `amd64`.


Some file managers such as KDE's Dolphin includes file verification. To do this, open up properties and navigate to `Checksums` tab. Then, copy any of the file checksum you want and wait until it verifies.

Download the tarball which is designed to your architecture in [Releases page](https://github.com/haltroy/foster/releases/). Names of file looks like this: `fosterman-[Processor type].tar...`

Verify the files with this command: `echo "[Checksum] *[File name]" | shasum -a 256 --check` or `echo "[Checksum] *[File name]" | md5sum -c` then extract it. Copy the file to `/usr/local/bin` folder or add the directory that hosts the Foster Manager into your path variable.

Now, you can execute Foster Manager from your favorite terminal emulator.