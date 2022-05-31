% FOSTERMAN(1) fosterman 2.0.0.0
% haltroy
% July 2022

# NAME

fosterman - A Command-line tool for managing Fosters.

# SYNOPSIS

**fosterman** _OPTIONS_

# DESCRIPTION

**Foster Manager** is a command-line tool for creating,
converting, changing, querying, installing and/or updating
Foster files or folders.
**Foster** is a package management library made in _.NET_.
However, with this command-line tool anyone can use Foster's
features without needing a _C#_ code.
Additional features (ex. packers, parsers, encryptions) can be installed into `[USER FOLDER]` (for per user) or `[SYSTEM WIDE]` (for system-wide) folders.

## Types of Foster Packages

Foster packages can be three types; an information file, a package file or a delta file.
An Information file stores the information about versions of projects. Foster
checks this file to install or update a project. Information files can be written
in many languages such as **Fostrian** or **XML** or **JSON**.
A package file contains the files to be installed into the drive. File informations are
not compressed but the data inside each file might be using algorithms such
as **GZip**, **Deflate** etc.
A delta package allows developers to send updates to users/customers by sending
the differences from the previous version. This makes download process and updates
faster by making the package smaller.

## Creating an Information File

To create an information file using **fosterman**, one must determine which
parser should be used. Foster Manager comes with these parsers: Fostrian (bin), XML (xml).

First, create an empty file using `fosterman create-info [File Path] [Parser Name]` command.

Example: `fosterman create-info MyFosterFile.fp xml`

Then open the file you just created with the text editor of your choice.
Then add the information to the end of the file. **DO NOT DELETE THE GARBAGE TEXT ON TOP**.

## Converting an Information File

If the parsers are also installed and registered, Foster can parse their information. However, some
apps might not register these parsers. Foster only comes with **Fostrian** (Bİnary Parser) as default.
Also, Fostrian is smaller than the other formats which makes it more smaller.

Foster detects the format from the header so you don't have to worry about that part.

To convert an information file, use `fosterman convert [File Name] [Output Name] [Format]` command.

For example, to convert from XML to Fostrian (Binary): `fosterman convert MyXMLInfo.fp MyFostrianInfo.fp bin`

# OPTIONS

**help --help -help /?**
: Prints the help page and exits.

**--skip-addons -a**
: Skips loading additional libraries (parsers, packers, encryptions etc.).

**-v -vv -vvv -vvvv**
: Shows more information while doing something. Each "v" determines the verbosity level.

**--no-logo -n**
: Skips the logo.

**info [Additional: license|copyright]**
: Shows information. Additional parameters show other informations such as the license and copyright information.

**clean**
: Cleans the Foster temporary folder.

**query [Path]**
: Gets information about a Foster information package.

**convert [Input Path] [Output Path] [Format]**
: Converts an information file to another format and saves it into the output path.

**create-info [Path] [Format]**
: Creates an information file with determined format and saves it into the path.

**change-encrypt [Path] [Format] [Additional: -o|-p|-q|-c -EncryptArgs]**
: Changes encryption of a file. If determined after _-o_ argument, can save the resulted file into another file. If determined after _-p_ option, Foster Manager will not ask for a password and use that password instead. If determined after _-q_ option, changes the password of the file. If determined after _-c_ or _-EncryptArgs_, sets the encryption algorithm's arguments.

**change-packer [Path] [Format] [Additional: -o|-p|-q]**
: Changes the compression algorithm of a file. If determined after _-o_ argument, can save the resulted file into another file. If determined after _-p_ option, Foster Manager will not ask for a password and use that password instead. If determined after _-q_ option, changes the password of the file.

**pack [Folder Path] [Additional: -o -Output|-p -Password|-a -Algorithm|-e -Encryption|-c -EncryptArgs]**
: Packs a folder into a Foster file. If determined after _-o_ or _-Output_ argument, can save the resulted file into another file. If determined after _-p_ or _-Password_ option, Foster Manager will not ask for a password and use that password instead. If determined after _-a_ or _-Algortihm_ argument, sets which compression algorithm is going to be used. If determined after _-e_ or _-Encryption_ argument, sets which encryption algorithm is going to be used. If determined after _-c_ or _-EncryptArgs_, sets the encryption algorithm's arguments.

**unpack [File Path] [Additional: Folder Path] [Additional: -p -Password]**
: Unpacks a file into a folder.Additionally, one can change which folder to extract on by adding the folder path after the file path. If determined after _-p_ option, Foster Manager will not ask for a password and use that password instead.

**delta [Folder Path] [Based Folder Path] [Additional: -o -Output|-p -Password|-a -Algorithm|-e -Encryption|-c -EncryptArgs]**
: Compares a folder with another folder and notes the changes into a file. If determined after _-o_ or _-Output_ argument, can save the resulted file into another file. If determined after _-p_ or _-Password_ option, Foster Manager will not ask for a password and use that password instead. If determined after _-a_ or _-Algortihm_ argument, sets which compression algorithm is going to be used. If determined after _-e_ or _-Encryption_ argument, sets which encryption algorithm is going to be used. If determined after _-c_ or _-EncryptArgs_, sets the encryption algorithm's arguments.

**adelta [File Path] [Folder Path] [Additional: -p -Password]**
: Applies the changes noted in a file into a folder. If determined after _-p_ option, Foster Manager will not ask for a password and use that password instead.

**create [Source Folder] [Output Folder] [Additional: -s -Source|--skip-empty-dirs|--skip-hashes|-pass -Password|-p -Parser|-a -Algorithm|-e -Encryption|-c -EncryptArgs]**
: Bulk-creates all packages and a Foster file from a folder that contains a skeleton Foster file and packages themselves. ıf determined after _-s_ or _-Source_, can load the required information from that specific skeleton Foster file instead. If _--skip-empty-dirs_ is present, skips empty directories, this might result some delta packages that depend on them to use the create flag instead of change possibility. If _--skip-hashes_ is present, skips adding verification hashes on file. If determined after _-pass_ or _-Password_ option, Foster Manager will not ask for a password and use that password instead. If determined after _-a_ or _-Algortihm_ argument, sets which compression algorithm is going to be used. If determined after _-e_ or _-Encryption_ argument, sets which encryption algorithm is going to be used. If determined after _-p_ or _-Parser_ argument, sets which parser is going to be used. If determined after _-c_ or _-EncryptArgs_, sets the encryption algorithm's arguments.

**install [Folder] [URI] [Additional: -a -Arch|--retry|--skip-backup|--skip-size|--erase|--skip-hashes|--skip-backup-errors|-y|-p -Password]**
: Installs a Foster file into a directory. If determined after _-a_ or _-Arch_, install packages specific to a machine architecture. If determined after _--retry_, tells how many attemps should be done. If _--skip-backup_ is present, skips backing up the folder. If _--skip-size_ is present, skips disk size checks. If _--skip-hashes_ is present, skips verification step on downloaded files. If _--erase_ is present, erases the folder. If _--skip-backup-erros_ is present, skips errors made while backing up the folder. If determined after _-p_ or _-Password_ option, Foster Manager will not ask for a password and use that password instead. If _-y_ is present, skips the confirmation.

**update [Folder] [URI] [Additional: -a -Arch|--retry|--skip-backup|--skip-size|--erase|--skip-hashes|--skip-backup-errors|-y|-p -Password]**
: Updates a Foster installed directory from a Foster file. If determined after _-a_ or _-Arch_, install packages specific to a machine architecture. If determined after _--retry_, tells how many attemps should be done. If _--skip-backup_ is present, skips backing up the folder. If _--skip-size_ is present, skips disk size checks. If _--skip-hashes_ is present, skips verification step on downloaded files. If _--erase_ is present, erases the folder. If _--skip-backup-erros_ is present, skips errors made while backing up the folder. If determined after _-p_ or _-Password_ option, Foster Manager will not ask for a password and use that password instead. If _-y_ is present, skips the confirmation.

# EXAMPLES

**fosterman create-info MyFosterInfo.fp xml**
: Creates a new information file with XML parser.

**fosterman convert MyXmlInfo.fp MyFostrianInfo.fp bin**
: Converts an information file to Fostrian.

**fosterman pack ./MyProject -o ./MyProject.fp -a gzip**
: Creates a package from folder with GZip.

**fosterman unpack ./MyProject.fp**
: Unzips a package from file to a folder.

**fosterman delta ./MySecondVersion ./MyFirstVersion -o MySecondVersion.fp**
: Creates a delta file from a folder.

**fosterman adelta ./MyDeltaPackage.fp ./MyProjectFolder -p MyPassword123**
: Applies delta from a file to folder with a predefined password.

**fosterman create ./MyVersions ./MyPackages**
: Bulk-creates packages from a folder.

**fosterman install ./MyProject https://mywebsite.com/MyFosterInfo.fp**
: Install a package.

**fosterman update ./MyProject https://mywebsite.com/MyFosterInfo.fp -y**
: Updates a folder to latest without asking anything.

**fosterman query https://mywebsite.com/MyFosterInfo.fp**
: Gets information about a package.

# EXIT VALUES

**0**
: The only value Foster Manager will return. If there's an error, Foster Manager will type the entire .NET Exception into screen and quit.

# BUGS

If found any, please report it to <https://github.com/haltroy/Foster-Manager/issues>.

# COPYRIGHT

Copyright (C) 2021-2022 haltroy. Code is protected with an MIT license.

<https://github.com/haltroy/Foster-Manager/blob/main/LICENSE>
