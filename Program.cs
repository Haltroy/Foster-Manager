/*

Copyright © 2021 - 2022 haltroy

Use of this source code is governed by a MIT License that can be found in github.com/haltroy/Foster-Manager/blob/master/COPYING

*/

using LibFoster;
using LibFoster.Modules;
using System;
using System.Linq;
using System.Reflection;

namespace Foster_Manager
{
    // NOTES TO PACKAGE MAINTAINERS:
    // 1- Before building, please change the "RuntimeIdentifier" in "Foster Manager.csproj" file (with Microsoft's RIDs) and also
    // change value of "Arch" below (with Foster's Architectures). Also, change value of "Dev" to your username or the distribution name.
    // Microsoft RIDs: win-x86 win-x64 win-arm win-arm64 linux-x64 linux-x86 linux-musl-x64 linux-arm linux-arm64 osx-x64
    // Foster Archs: see https://github.com/Foster/tree/master/Foster%20Examples/Archs.md

    // NOTES TO CUSTOMIZATION:
    // To add new packers/parsers/encryptions, add them into the HookParsersAndPackers() void below and then build it. Or you can add their DLL files to HookLoc and GlobalHookLoc folders.
    internal class Program
    {
        private static class Versioning
        {
            public static string Version => "2.0.0.0"; // <-- DO NOT CHANGE THIS
            public static string Dev => "haltroy"; // <-- Change this
            public static string Arch => "noarch"; // <-- Change this
        }

        private static string HookLoc => System.IO.Path.Combine(System.Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "fosterman" + System.IO.Path.DirectorySeparatorChar + "hooks");
        private static string GlobalHookLoc => System.IO.Path.Combine(System.Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData), "fosterman" + System.IO.Path.DirectorySeparatorChar + "hooks");

        private static void HookParsersAndPackers(bool skipFolder = false, bool verbose = false)
        {
            // NOTE: Add your own parsers, encryptions and packers below. You can use the examples below to add.
            // It's not that hard dude you probably better than me in C++ this should be a piece of cake to you.
            new FosterPackerGZip().Register();
            new FosterPackerDeflate().Register();
            new FosterXmlParser().Register();
            new FosterEncryptionAes().Register();
            // This code below auto-registers any parser/encryption/packer in HookLoc and GlobalHookLoc folder. They only have to be a DLL file and must include a class with Register
            // void that registers the said extension to FosterSettings.
            if (!skipFolder)
            {
                try
                {
                    if (!System.IO.Directory.Exists(HookLoc))
                    {
                        System.IO.Directory.CreateDirectory(HookLoc);
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Foster Manager cannot create the hooks folder for this user. Please create this path to fix this error:");
                    Console.WriteLine(HookLoc);
                    if (verbose)
                    {
                        Console.WriteLine("Detailed error information: " + ex.ToString());
                    }
                }
                try
                {
                    if (!System.IO.Directory.Exists(GlobalHookLoc))
                    {
                        System.IO.Directory.CreateDirectory(GlobalHookLoc);
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Foster Manager cannot create the system-wide hooks folder. Please create this path to fix this error:");
                    Console.WriteLine(GlobalHookLoc);
                    if (verbose)
                    {
                        Console.WriteLine("Detailed error information: " + ex.ToString());
                    }
                }
                var hooks = System.IO.Directory.GetFiles(HookLoc, "*.dll", System.IO.SearchOption.TopDirectoryOnly).Concat(System.IO.Directory.GetFiles(GlobalHookLoc, "*.dll", System.IO.SearchOption.TopDirectoryOnly)).ToArray();
                for (int i = 0; i < hooks.Length; i++)
                {
                    var dll = Assembly.LoadFile(hooks[i]);
                    Type[] exportedTypes = dll.GetExportedTypes();
                    for (int i1 = 0; i1 < exportedTypes.Length; i1++)
                    {
                        Type type = exportedTypes[i1];
                        try
                        {
                            var c = Activator.CreateInstance(type);
                            type.InvokeMember("Register", BindingFlags.InvokeMethod, null, c, null);
                        }
                        catch (Exception) { continue; } // ignored.
                    }
                }
            }
        }
        private static void Main(string[] args)
        {
            bool skipFolder = args.Contains("--skip-addons") || args.Contains("-a");
            FosterSettings.VerbosityLevel = args.Contains("-v") ? FosterSettings.Verbosity.OnlyErrors : FosterSettings.Verbosity.None;
            FosterSettings.VerbosityLevel = args.Contains("-vv") ? FosterSettings.Verbosity.Progress : FosterSettings.VerbosityLevel;
            FosterSettings.VerbosityLevel = args.Contains("-vvv") ? FosterSettings.Verbosity.Detailed : FosterSettings.VerbosityLevel;
            FosterSettings.VerbosityLevel = args.Contains("-vvvv") ? FosterSettings.Verbosity.Everything : FosterSettings.VerbosityLevel;
            bool verbose = FosterSettings.VerbosityLevel != FosterSettings.Verbosity.None;
            HookParsersAndPackers(skipFolder, verbose);
            bool nologo = args.Contains("--no-logo") || args.Contains("-n");

            if (!nologo) { Console.WriteLine("Foster Manager | Copyright (C) " + DateTime.Now.Year + " " + Versioning.Dev + Environment.NewLine + "This program comes with ABSOLUTELY NO WARRANTY; for details type `info license'." + Environment.NewLine + "This software is protected with MIT License; type `info license' or 'info copyright' for details."); }
            if (verbose) { Console.WriteLine("Foster Initialize complete with " + FosterSettings.Parsers.Length + " parser(s) and " + FosterSettings.Packers.Length + " packer(s)."); }
            string fostermanName = System.Diagnostics.Process.GetCurrentProcess().ProcessName;
            if (args.Length <= 0 || args.Contains("--help") || args.Contains("help") || args.Contains("-h") || args.Contains("?") || args.Contains("/?"))
            {
                Console.WriteLine("Foster Manager ver. " + Versioning.Version + " [" + Versioning.Dev + "]");
                Console.WriteLine("--------------------");
                Console.WriteLine("");
                Console.WriteLine("USAGE:");
                Console.WriteLine("      fosterman [--verbose|-v] [--no-logo|-n] [-help|-h|help|*|/?|info|clean|query|pack|unpack|delta|update|create|convert|change-encrypt|change-packer|create-info] [OPTIONS]");
                Console.WriteLine("OPTIONS:");
                Console.WriteLine("help --help -h /? ?                                         Shows this information.");
                Console.WriteLine("--skip-addons -a                                            Skips add-ons.");
                Console.WriteLine("-v                                                          Shows more information while doing something. To add more verbosity, add more 'v'. (Max. 4)");
                if (verbose)
                {
                    Console.WriteLine("v: Only verbose errors from LibFoster and more information from Foster Manager.");
                    Console.WriteLine("vv: Only verbose errors and progress from LibFoster and more information from Foster Manager.");
                    Console.WriteLine("vvv: Detailed output from LibFoster and more information from Foster Manager.");
                    Console.WriteLine("vvvv: Everything from LibFoster and more information from Foster Manager.");
                }

                Console.WriteLine("--no-logo -n                                                Hides the copyright information on run.");
                Console.WriteLine("info [OPTIONS]                                              Shows information about this program.");
                if (verbose)
                {
                    Console.WriteLine("[OPTIONS] = Additional arguments that can be used.");
                    Console.WriteLine("     license = Shows the MIT License.");
                    Console.WriteLine("     copyright = Similar to \"license\".");
                }
                Console.WriteLine("clean                                                      Cleans the Foster temporary folder.");
                Console.WriteLine("query [Path]                                               Gets information about a file.");
                if (verbose)
                {
                    Console.WriteLine("[Path] = Location of the file on Web or local drive(s).");
                }
                Console.WriteLine("convert [Input File] [Output File] [Format]                Converts a Foster information file into a specific format.");
                if (verbose)
                {
                    Console.WriteLine("[Input File] = Path to the original file.");
                    Console.WriteLine("[Output File] = Path to the edited file.");
                    Console.WriteLine("[Format] = Format to convert. Supported formats are:");
                    for (int i = 0; i < FosterSettings.Parsers.Length; i++)
                    {
                        Console.WriteLine("          " + FosterSettings.Parsers[i].ParserName + "(" + i + ") ");
                    }
                }
                Console.WriteLine("create-info [Path] [Format]                                Creates an empty Foster Information file with no encryption and compression.");
                if (verbose)
                {
                    Console.WriteLine("[Path] = Path to the file.");
                    Console.WriteLine("[Format] = Format to create. Supported formats are:");
                    for (int i = 0; i < FosterSettings.Parsers.Length; i++)
                    {
                        Console.WriteLine("          " + FosterSettings.Parsers[i].ParserName + "(" + i + ") ");
                    }
                }
                Console.WriteLine("change-encrypt [Path] [Format] [Options]                   Changes encryption of a file.");
                if (verbose)
                {
                    Console.WriteLine("[Input File] = Path to the file.");
                    Console.WriteLine("[Format] = Format to change. Supported formats are:");
                    for (int i = 0; i < FosterSettings.Encryptions.Length; i++)
                    {
                        Console.WriteLine("          " + FosterSettings.Encryptions[i].EncryptionName + "(" + i + ") ");
                    }
                    Console.WriteLine("[OPTIONS] = Additional arguments that can be used.");
                    Console.WriteLine("     -o [Output File] = Path to the output file.");
                    Console.WriteLine("     -p [Password] = Password of file.");
                    Console.WriteLine("     -q [Password] = New password of file.");
                    Console.WriteLine("     -c [Argument] = Similar to -EncryptArgs");
                    Console.WriteLine("     -EncryptArgs [Argument] = Argument to pass to encryption method.");
                }
                Console.WriteLine("change-packer [Path] [Format] [Options]                    Changes compression of a file.");
                if (verbose)
                {
                    Console.WriteLine("[Input File] = Path to the file.");
                    Console.WriteLine("[Format] = Format to change. Supported formats are:");
                    for (int i = 0; i < FosterSettings.Packers.Length; i++)
                    {
                        Console.WriteLine("          " + FosterSettings.Packers[i].PackerName + "(" + i + ") ");
                    }
                    Console.WriteLine("[OPTIONS] = Additional arguments that can be used.");
                    Console.WriteLine("     -o [Output File] = Path to the output file.");
                    Console.WriteLine("     -p [Password] = Password of file.");
                    Console.WriteLine("     -q [Password] = New password of file.");
                }
                Console.WriteLine("pack [Folder Path] [OPTIONS]                                Packs a folder into a Foster compatible package file.");
                if (verbose)
                {
                    Console.WriteLine("[Folder Path] = Path of the folder that will  be packed.");
                    Console.WriteLine("[OPTIONS] = Additional arguments that can be used.");
                    Console.WriteLine("     -o [File Path] = Similar to \"-Output\" parameter.");
                    Console.WriteLine("     -Output [File Path] = Path of the package file that will be created.");
                    Console.WriteLine("     -p [Password] = Similar to \"-Password\" parameter.");
                    Console.WriteLine("     -Password [Password] = Password of file.");
                    Console.WriteLine("     -a [Algorithm] = Similar to \"-Algorithm\" parameter.");
                    Console.WriteLine("     -Algorithm [Algorithm] = The compression algorithm that will be used to compress the package file. Valid options are:");
                    for (int i = 0; i < FosterSettings.Packers.Length; i++)
                    {
                        Console.WriteLine("          " + FosterSettings.Packers[i].PackerName + "(" + i + ") ");
                    }
                    Console.WriteLine("     -e [Encryption] = Similar to \"-Encryption\"");
                    Console.WriteLine("     -Encryption [Encryption] = The encryption algorithm that will be used to create files. Valid options are:");
                    for (int i = 0; i < FosterSettings.Encryptions.Length; i++)
                    {
                        Console.WriteLine("          " + FosterSettings.Encryptions[i].EncryptionName + "(" + i + ") ");
                    }
                    Console.WriteLine("     -c [Argument] = Similar to -EncryptArgs");
                    Console.WriteLine("     -EncryptArgs [Argument] = Argument to pass to encryption method.");
                }
                Console.WriteLine("unpack [File Path] [OPTIONS]                                Packs a folder into a Foster compatible package file." + (verbose ? "(Compression algorithm will be detected automatically.)" : ""));
                if (verbose)
                {
                    Console.WriteLine("[File Path] = Path of the file that will be unpacked.");
                    Console.WriteLine("[OPTIONS] = Additional arguments that can be used.");
                    Console.WriteLine("     [Folder Path] = Folder to unpack to.");
                    Console.WriteLine("     -p [Password] = Similar to \"-Password\" parameter.");
                    Console.WriteLine("     -Password [Password] = Password of file.");
                }
                Console.WriteLine("delta [Folder Path] [Based Folder Path] [OPTIONS]          Creates a delta package based on a version." + (verbose ? "(Compression algorithm will be detected automatically.)" : ""));
                if (verbose)
                {
                    Console.WriteLine("[Folder Path] = Path of the delta version's folder.");
                    Console.WriteLine("[Based Folder Path] = Path of the folder which is the delta version is based of.");
                    Console.WriteLine("[OPTIONS] = Additional arguments that can be used.");
                    Console.WriteLine("     -o [File Path] = Path of the delta package that will be created.");
                    Console.WriteLine("     -Output [File Path] = Path of the delta package that will be created.");
                    Console.WriteLine("     -a [Algorithm] = The compression algorithm that will be used to compress the package file. Valid options are:");
                    Console.WriteLine("     -Algorithm [Algorithm] = The compression algorithm that will be used to compress the package file. Valid options are:");
                    for (int i = 0; i < FosterSettings.Packers.Length; i++)
                    {
                        Console.WriteLine("          " + FosterSettings.Packers[i].PackerName + "(" + i + ") ");
                    }
                    Console.WriteLine("     -e [Encryption] = Similar to \"-Encryption\"");
                    Console.WriteLine("     -Encryption [Encryption] = The encryption algorithm that will be used to create files. Valid options are:");
                    for (int i = 0; i < FosterSettings.Encryptions.Length; i++)
                    {
                        Console.WriteLine("          " + FosterSettings.Encryptions[i].EncryptionName + "(" + i + ") ");
                    }
                    Console.WriteLine("     -c [Argument] = Similar to -EncryptArgs");
                    Console.WriteLine("     -EncryptArgs [Argument] = Argument to pass to encryption method.");
                }
                Console.WriteLine("adelta [File Path] [Folder Path] [OPTIONS]                  Applies a delta package." + (verbose ? "(Compression algorithm will be detected automatically.)" : ""));
                if (verbose)
                {
                    Console.WriteLine("[Folder Path] = Path of the folder to apply delta on.");
                    Console.WriteLine("[File Path] = Path of the delta package that will be applied.");
                    Console.WriteLine("[OPTIONS] = Additional arguments that can be used.");
                    Console.WriteLine("     -p [Password] = Similar to \"-Password\" parameter.");
                    Console.WriteLine("     -Password [Password] = Password of file.");
                }
                Console.WriteLine("update [Folder] [URI] [OPTIONS]                             Updates a folder." + (verbose ? "(Compression algorithm will be detected automatically.)" : ""));
                if (verbose)
                {
                    Console.WriteLine("[Folder] = Path to the folder of the Foster that will be updated.");
                    Console.WriteLine("[URI] = Address of the Foster file on web.");
                    Console.WriteLine("[OPTIONS] = Additional arguments that can be used.");
                    Console.WriteLine("     -a [Arch] = Similar to \"-Arch\".");
                    Console.WriteLine("     -Arch [Arch] = Current computer architecture (see documentation: https://github.com/Haltroy/Foster/tree/master/Foster%20Examples/Archs.md).");
                    Console.WriteLine("     --version  [Current Version] = Current version number.");
                    Console.WriteLine("     --retry [Number] = The number of retries for downloading & applying packages.");
                    Console.WriteLine("     --skip-backup = Skips the backup creation. Can be used as a boolean with \"true\", \"1\" and/or \"yes\" as enabling and \"false\", \"0\" and/or \"no\" as disabling. Not required but might save some time and space.");
                    Console.WriteLine("     --erase = Erases the work folder. Can be used as a boolean with \"true\", \"1\" and/or \"yes\" as enabling and \"false\", \"0\" and/or \"no\" as disabling. Not required but might save some time and space.");
                    Console.WriteLine("     --skip-size = Skips the size detection. Can be used as a boolean with \"true\", \"1\" and/or \"yes\" as enabling and \"false\", \"0\" and/or \"no\" as disabling. Not required but might save some time and space.");
                    Console.WriteLine("     --skip-hashes = Skips the file verification. Can be used as a boolean with \"true\", \"1\" and/or \"yes\" as enabling and \"false\", \"0\" and/or \"no\" as disabling. Not required but might save some time and space.");
                    Console.WriteLine("     --skip-backup-errors = Skips the errors occurred while creating backup. Can be used as a boolean with \"true\", \"1\" and/or \"yes\" as enabling and \"false\", \"0\" and/or \"no\" as disabling. Not required but might save some time.");
                    Console.WriteLine("     -p [Password] = Similar to \"-Password\" parameter.");
                    Console.WriteLine("     -Password [Password] = Password of file.");
                    Console.WriteLine("     -y = Says \"Yes\" to every question.");
                }
                Console.WriteLine("install [Folder] [URI] [OPTIONS]                             Installs Foster to a folder." + (verbose ? "(Compression algorithm will be detected automatically.)" : ""));
                if (verbose)
                {
                    Console.WriteLine("[Folder] = Path to the folder of the Foster that will be updated.");
                    Console.WriteLine("[URI] = Address of the Foster file on web.");
                    Console.WriteLine("[OPTIONS] = Additional arguments that can be used.");
                    Console.WriteLine("     -a [Arch] = Similar to \"-Arch\".");
                    Console.WriteLine("     -Arch [Arch] = Current computer architecture (see documentation: https://github.com/Haltroy/Foster/tree/master/Foster%20Examples/Archs.md).");
                    Console.WriteLine("     --retry [Number] = The number of retries for downloading & applying packages.");
                    Console.WriteLine("     --skip-backup = Skips the backup creation. Can be used as a boolean with \"true\", \"1\" and/or \"yes\" as enabling and \"false\", \"0\" and/or \"no\" as disabling. Not required but might save some time and space.");
                    Console.WriteLine("     --skip-size = Skips the size detection. Can be used as a boolean with \"true\", \"1\" and/or \"yes\" as enabling and \"false\", \"0\" and/or \"no\" as disabling. Not required but might save some time and space.");
                    Console.WriteLine("     --erase = Erases the work folder. Can be used as a boolean with \"true\", \"1\" and/or \"yes\" as enabling and \"false\", \"0\" and/or \"no\" as disabling. Not required but might save some time and space.");
                    Console.WriteLine("     --skip-hashes = Skips the file verification. Can be used as a boolean with \"true\", \"1\" and/or \"yes\" as enabling and \"false\", \"0\" and/or \"no\" as disabling. Not required but might save some time and space.");
                    Console.WriteLine("     --skip-backup-errors = Skips the errors occurred while creating backup. Can be used as a boolean with \"true\", \"1\" and/or \"yes\" as enabling and \"false\", \"0\" and/or \"no\" as disabling. Not required but might save some time.");
                    Console.WriteLine("     -p [Password] = Similar to \"-Password\" parameter.");
                    Console.WriteLine("     -Password [Password] = Password of file.");
                    Console.WriteLine("     -y = Says \"Yes\" to every question.");
                }
                Console.WriteLine("create [Source Folder] [Output Folder] [OPTIONS]            Creates Foster packages.");
                if (verbose)
                {
                    Console.WriteLine("[Source Folder] = Path of the folder to batch create Foster packages.");
                    Console.WriteLine("[Output Folder] = Path of the folder where created packages will be located.");
                    Console.WriteLine("[OPTIONS] = Additional arguments that can be used.");
                    Console.WriteLine("     -s [Skeleton Foster file] = Similar to \"-Source\"");
                    Console.WriteLine("     -Source [Skeleton Foster file] = The name of the Skeleton Foster file, optional.");
                    Console.WriteLine("     --skip-empty-dirs = Skips the empty directories. Can be used as a boolean with \"true\", \"1\" and/or \"yes\" as enabling and \"false\", \"0\" and/or \"no\" as disabling. Not required but might save some time and space.");
                    Console.WriteLine("     --skip-hashes = Skips the hash-checking part. Can be used as a boolean with \"true\", \"1\" and/or \"yes\" as enabling and \"false\", \"0\" and/or \"no\" as disabling. Not required but might save some time.");
                    Console.WriteLine("     -pass [Password] = Similar to \"-Password\" parameter.");
                    Console.WriteLine("     -Password [Password] = Password of file.");
                    Console.WriteLine("     -a [Algorithm] = Similar to \"-Algorithm\"");
                    Console.WriteLine("     -Algorithm [Algorithm] = The compression algorithm that will be used to compress the package files. Valid options are:");
                    for (int i = 0; i < FosterSettings.Packers.Length; i++)
                    {
                        Console.WriteLine("          " + FosterSettings.Packers[i].PackerName + "(" + i + ") ");
                    }
                    Console.WriteLine("     -p [Parser] = Similar to \"-Parser\"");
                    Console.WriteLine("     -Parser [Parser] = The parser that will be used to create Foster file. Valid options are:");
                    for (int i = 0; i < FosterSettings.Parsers.Length; i++)
                    {
                        Console.WriteLine("          " + FosterSettings.Parsers[i].ParserName + "(" + i + ") ");
                    }
                    Console.WriteLine("     -e [Encryption] = Similar to \"-Encryption\"");
                    Console.WriteLine("     -Encryption [Encryption] = The encryption algorithm that will be used to create files. Valid options are:");
                    for (int i = 0; i < FosterSettings.Encryptions.Length; i++)
                    {
                        Console.WriteLine("          " + FosterSettings.Encryptions[i].EncryptionName + "(" + i + ") ");
                    }
                    Console.WriteLine("     -c [Argument] = Similar to -EncryptArgs");
                    Console.WriteLine("     -EncryptArgs [Argument] = Argument to pass to encryption method.");
                }
                return;
            }
            if (args.Contains("info"))
            {
                int singleArgLoc = Array.IndexOf(args, "info");
                if (args.Length - 1 > singleArgLoc)
                {
                    string infoArg = args[singleArgLoc + 1];
                    switch (infoArg.ToLowerEnglish())
                    {
                        default:
                        case "copyright":
                        case "license":
                            Console.WriteLine("");
                            Console.WriteLine(Properties.Resources.license);
                            Console.WriteLine("");
                            break;
                    }
                }
                else
                {
                    Console.WriteLine("");
                    Console.WriteLine("Dev: " + Versioning.Dev);
                    Console.WriteLine("Version: " + Versioning.Version);
                    Console.WriteLine("Arch: " + Versioning.Arch);
                    Console.WriteLine("Foster Version: " + FosterSettings.FosterVersion);
                    Console.WriteLine("Hooks (User) Folder: " + HookLoc);
                    Console.WriteLine("Hooks (System-wide) Folder: " + GlobalHookLoc);
                    string parsers = string.Empty;
                    string packers = string.Empty;
                    for (int i = 0; i < FosterSettings.Parsers.Length; i++)
                    {
                        parsers += FosterSettings.Parsers[i].ParserName + " ";
                    }
                    for (int i = 0; i < FosterSettings.Packers.Length; i++)
                    {
                        packers += FosterSettings.Packers[i].PackerName + " ";
                    }
                    Console.WriteLine("Foster Parser(s): [" + FosterSettings.Parsers.Length + "]" + parsers);
                    Console.WriteLine("Foster Packer(s): [" + FosterSettings.Packers.Length + "]" + packers);
                    Console.WriteLine("");
                }
                return;
            }
            if (args.Contains("convert"))
            {
                int singleArgLoc = Array.IndexOf(args, "convert");
                System.Diagnostics.Stopwatch sw = new System.Diagnostics.Stopwatch();
                sw.Start();
                try
                {
                    if (args.Length < singleArgLoc + 3) { Console.Error.WriteLine(" [E] Not enough information."); return; }
                    string inputFile = System.IO.Path.GetFullPath(args[singleArgLoc + 1]);
                    string outputFile = System.IO.Path.GetFullPath(args[singleArgLoc + 2]);
                    string algName = args[singleArgLoc + 3].ToLowerEnglish();
                    var parseDone = int.TryParse(algName, out int algNo);
                    FosterParserBase parser = null;
                    for (int i = 0; i < FosterSettings.Parsers.Length; i++)
                    {
                        if ((parseDone && algNo == i) || (FosterSettings.Parsers[i].ParserName == algName))
                        {
                            parser = FosterSettings.Parsers[i];
                            break;
                        }
                    }
                    if (parser == null)
                    {
                        throw new Exception("Couldn't find a parser with name \"" + algName + "\".");
                    }
                    byte[] password = null;
                    if (args.Contains("-p", StringComparer.InvariantCultureIgnoreCase) && password is null)
                    {
                        int algorithmIndex = Array.IndexOf(args, "-p");
                        if (args.Length >= algorithmIndex + 1)
                        {
                            string pass = args[algorithmIndex + 1].ToLowerEnglish();
                            password = System.Text.Encoding.Unicode.GetBytes(algName);
                        }
                    }
                    if (args.Contains("-Password", StringComparer.InvariantCultureIgnoreCase) && password is null)
                    {
                        int algorithmIndex = Array.IndexOf(args, "-Password");
                        if (args.Length >= algorithmIndex + 1)
                        {
                            string pass = args[algorithmIndex + 1].ToLowerEnglish();
                            password = System.Text.Encoding.Unicode.GetBytes(algName);
                        }
                    }
                    HeaderContext header = new HeaderContext(new Foster(), null, parser, null, null);
                    using (var fStr = new System.IO.FileStream(inputFile, System.IO.FileMode.OpenOrCreate, System.IO.FileAccess.ReadWrite, System.IO.FileShare.ReadWrite))
                    {
                        var _header = Foster.GetHeader(fStr);
                        header.Encryption = _header.Encryption;
                        header.Packer = _header.Packer;
                        var reqPassword = _header.Encryption.RequiresPassword;

                        if (reqPassword)
                        {
                            Console.WriteLine("Please enter the password:");
                            var pwd = new System.Security.SecureString();
                            while (true)
                            {
                                ConsoleKeyInfo i = Console.ReadKey(true);
                                if (i.Key == ConsoleKey.Enter)
                                {
                                    break;
                                }
                                else if (i.Key == ConsoleKey.Backspace)
                                {
                                    if (pwd.Length > 0)
                                    {
                                        pwd.RemoveAt(pwd.Length - 1);
                                        Console.Write("\b \b");
                                    }
                                }
                                else if (i.KeyChar != '\u0000') // KeyChar == '\u0000' if the key pressed does not correspond to a printable character, e.g. F1, Pause-Break, etc
                                {
                                    pwd.AppendChar(i.KeyChar);
                                    Console.Write("*");
                                }
                            }
                            password = System.Text.Encoding.Unicode.GetBytes(pwd.ToString());
                        }
                        fStr.Position = 0;
                        Foster.Read(header.Foster, fStr, password);
                    }
                    using (var fStr = new System.IO.FileStream(outputFile, System.IO.FileMode.OpenOrCreate, System.IO.FileAccess.ReadWrite, System.IO.FileShare.ReadWrite))
                    {
                        Foster.Write(header, fStr);
                    }
                    if (verbose) { Console.WriteLine("Converted successfully."); }
                }
                catch (Exception ex)
                {
                    Console.Error.WriteLine("Error while converting, exception caught:" + ex.ToString());
                }
                sw.Stop();
                if (verbose) { Console.WriteLine(fostermanName + " " + string.Join(' ', args) + " in " + sw.ElapsedMilliseconds + " ms."); }
                return;
            }
            if (args.Contains("create-info"))
            {
                int singleArgLoc = Array.IndexOf(args, "create-info");
                System.Diagnostics.Stopwatch sw = new System.Diagnostics.Stopwatch();
                sw.Start();
                try
                {
                    if (args.Length < singleArgLoc + 2) { Console.Error.WriteLine(" [E] Not enough information."); return; }
                    string filePath = System.IO.Path.GetFullPath(args[singleArgLoc + 1]);
                    string algName = args[singleArgLoc + 2].ToLowerEnglish();
                    var parseDone = int.TryParse(algName, out int algNo);
                    FosterParserBase parser = null;
                    for (int i = 0; i < FosterSettings.Parsers.Length; i++)
                    {
                        if ((parseDone && algNo == i) || (FosterSettings.Parsers[i].ParserName == algName))
                        {
                            parser = FosterSettings.Parsers[i];
                            break;
                        }
                    }
                    if (parser == null)
                    {
                        throw new Exception("Couldn't find a parser with name \"" + algName + "\".");
                    }
                    HeaderContext header = new HeaderContext(new Foster(), null, parser, null, null);
                    using (var fStr = new System.IO.FileStream(filePath, System.IO.FileMode.OpenOrCreate, System.IO.FileAccess.ReadWrite, System.IO.FileShare.ReadWrite))
                    {
                        Foster.Write(header, fStr);
                    }
                    if (verbose) { Console.WriteLine("Created successfully."); }
                }
                catch (Exception ex)
                {
                    Console.Error.WriteLine("Error while packing, exception caught:" + ex.ToString());
                }
                sw.Stop();
                if (verbose) { Console.WriteLine(fostermanName + " " + string.Join(' ', args) + " in " + sw.ElapsedMilliseconds + " ms."); }
                return;
            }
            if (args.Contains("change-encrypt"))
            {
                int singleArgLoc = Array.IndexOf(args, "change-encrypt");
                System.Diagnostics.Stopwatch sw = new System.Diagnostics.Stopwatch();
                sw.Start();
                try
                {
                    if (args.Length < singleArgLoc + 2) { Console.Error.WriteLine(" [E] Not enough information."); return; }
                    string packFolder = System.IO.Path.GetFullPath(args[singleArgLoc + 1]);
                    FosterEncryptionBase encryption = null;
                    string algName = args[singleArgLoc + 2].ToLowerEnglish();
                    var parseDone = int.TryParse(algName, out int algNo);
                    for (int i = 0; i < FosterSettings.Encryptions.Length; i++)
                    {
                        if ((parseDone && algNo == i) || (FosterSettings.Encryptions[i].EncryptionName == algName))
                        {
                            encryption = FosterSettings.Encryptions[i];
                            break;
                        }
                    }
                    System.IO.DirectoryInfo pfInfo = new System.IO.DirectoryInfo(packFolder);
                    string packFile = null;
                    if (args.Contains("-o"))
                    {
                        int outputIndex = Array.IndexOf(args, "-o");
                        packFile = System.IO.Path.GetFullPath(args[outputIndex + 1]);
                    }
                    if (args.Contains("-Output") && string.IsNullOrWhiteSpace(packFile))
                    {
                        int outputIndex = Array.IndexOf(args, "-Output");
                        packFile = System.IO.Path.GetFullPath(args[outputIndex + 1]);
                    }
                    if (string.IsNullOrWhiteSpace(packFile))
                    {
                        packFile = System.IO.Path.Combine(pfInfo.Parent.FullName, pfInfo.Name + ".fp");
                    }

                    byte[] password = null;
                    if (args.Contains("-p", StringComparer.InvariantCultureIgnoreCase) && password is null)
                    {
                        int algorithmIndex = Array.IndexOf(args, "-p");
                        if (args.Length >= algorithmIndex + 1)
                        {
                            password = System.Text.Encoding.Unicode.GetBytes(args[algorithmIndex + 1]);
                        }
                    }
                    byte[] qassword = null;
                    if (args.Contains("-q", StringComparer.InvariantCultureIgnoreCase) && qassword is null)
                    {
                        int algorithmIndex = Array.IndexOf(args, "-q");
                        if (args.Length >= algorithmIndex + 1)
                        {
                            qassword = System.Text.Encoding.Unicode.GetBytes(args[algorithmIndex + 1]);
                        }
                    }
                    string encryptArgs = string.Empty;
                    if (args.Contains("-c", StringComparer.InvariantCultureIgnoreCase) && string.IsNullOrWhiteSpace(encryptArgs))
                    {
                        int algorithmIndex = Array.IndexOf(args, "-c");
                        if (args.Length >= algorithmIndex + 1)
                        {
                            encryptArgs = args[algorithmIndex + 1];
                        }
                    }
                    if (args.Contains("-EncryptArgs", StringComparer.InvariantCultureIgnoreCase) && string.IsNullOrWhiteSpace(encryptArgs))
                    {
                        int algorithmIndex = Array.IndexOf(args, "-EncryptArgs");
                        if (args.Length >= algorithmIndex + 1)
                        {
                            encryptArgs = args[algorithmIndex + 1];
                        }
                    }
                    if (encryption == null) { encryption = FosterSettings.Encryptions[0]; }
                    if (encryption.RequiresPassword && qassword.Length <= 0)
                    {
                        string pwd = Tools.GenerateRandomText(10);
                        Console.WriteLine("Encryption enabled but password not filled, using \"" + pwd + "\" as password.");
                        qassword = System.Text.Encoding.Unicode.GetBytes(pwd);
                    }
                    if (verbose) { Console.WriteLine("Changing encryption method of file \"" + packFolder + " to \"" + encryption.EncryptionName + "\"..."); }
                    using (var fStr = new System.IO.FileStream(packFile, System.IO.FileMode.OpenOrCreate, System.IO.FileAccess.ReadWrite, System.IO.FileShare.ReadWrite))
                    {
                        var header = Foster.GetHeader(fStr);

                        if (header.Encryption.RequiresPassword)
                        {
                            Console.WriteLine("Please enter the password:");
                            var pwd = new System.Security.SecureString();
                            while (true)
                            {
                                ConsoleKeyInfo i = Console.ReadKey(true);
                                if (i.Key == ConsoleKey.Enter)
                                {
                                    break;
                                }
                                else if (i.Key == ConsoleKey.Backspace)
                                {
                                    if (pwd.Length > 0)
                                    {
                                        pwd.RemoveAt(pwd.Length - 1);
                                        Console.Write("\b \b");
                                    }
                                }
                                else if (i.KeyChar != '\u0000') // KeyChar == '\u0000' if the key pressed does not correspond to a printable character, e.g. F1, Pause-Break, etc
                                {
                                    pwd.AppendChar(i.KeyChar);
                                    Console.Write("*");
                                }
                            }
                            password = System.Text.Encoding.Unicode.GetBytes(pwd.ToString());
                        }
                        fStr.Position = 0;
                        string tempFolder = System.IO.Path.Combine(System.IO.Path.GetTempPath(), "fosterman" + System.IO.Path.DirectorySeparatorChar + "temp_" + Tools.GenerateRandomText());
                        if (System.IO.Directory.Exists(tempFolder))
                        {
                            System.IO.Directory.Delete(tempFolder, true);
                        }
                        System.IO.Directory.CreateDirectory(tempFolder);
                        Foster.Read(tempFolder, fStr, password);
                        var newheader = new HeaderContext(tempFolder, packer: header.Packer, encryption: encryption, encryption.GenerateArguments(encryptArgs)) { Password = qassword };
                        Foster.Write(newheader, fStr);
                    }
                    if (verbose) { Console.WriteLine("Changed encryption successfully."); }
                }
                catch (Exception ex)
                {
                    Console.Error.WriteLine("Error while changing encryption, exception caught:" + ex.ToString());
                }
                sw.Stop();
                if (verbose) { Console.WriteLine(fostermanName + " " + string.Join(' ', args) + " in " + sw.ElapsedMilliseconds + " ms."); }
                return;
            }
            if (args.Contains("change-packer"))
            {
                int singleArgLoc = Array.IndexOf(args, "change-packer");
                System.Diagnostics.Stopwatch sw = new System.Diagnostics.Stopwatch();
                sw.Start();
                try
                {
                    if (args.Length < singleArgLoc + 2) { Console.Error.WriteLine(" [E] Not enough information."); return; }
                    string packFolder = System.IO.Path.GetFullPath(args[singleArgLoc + 1]);
                    FosterPackerBase packer = null;
                    string algName = args[singleArgLoc + 2].ToLowerEnglish();
                    var parseDone = int.TryParse(algName, out int algNo);
                    for (int i = 0; i < FosterSettings.Encryptions.Length; i++)
                    {
                        if ((parseDone && algNo == i) || (FosterSettings.Packers[i].PackerName == algName))
                        {
                            packer = FosterSettings.Packers[i];
                            break;
                        }
                    }
                    System.IO.DirectoryInfo pfInfo = new System.IO.DirectoryInfo(packFolder);
                    string packFile = null;
                    if (args.Contains("-o"))
                    {
                        int outputIndex = Array.IndexOf(args, "-o");
                        packFile = System.IO.Path.GetFullPath(args[outputIndex + 1]);
                    }
                    if (args.Contains("-Output") && string.IsNullOrWhiteSpace(packFile))
                    {
                        int outputIndex = Array.IndexOf(args, "-Output");
                        packFile = System.IO.Path.GetFullPath(args[outputIndex + 1]);
                    }
                    if (string.IsNullOrWhiteSpace(packFile))
                    {
                        packFile = System.IO.Path.Combine(pfInfo.Parent.FullName, pfInfo.Name + ".fp");
                    }

                    byte[] password = null;
                    if (args.Contains("-p", StringComparer.InvariantCultureIgnoreCase) && password is null)
                    {
                        int algorithmIndex = Array.IndexOf(args, "-p");
                        if (args.Length >= algorithmIndex + 1)
                        {
                            password = System.Text.Encoding.Unicode.GetBytes(args[algorithmIndex + 1]);
                        }
                    }
                    byte[] qassword = null;
                    if (args.Contains("-q", StringComparer.InvariantCultureIgnoreCase) && qassword is null)
                    {
                        int algorithmIndex = Array.IndexOf(args, "-q");
                        if (args.Length >= algorithmIndex + 1)
                        {
                            qassword = System.Text.Encoding.Unicode.GetBytes(args[algorithmIndex + 1]);
                        }
                    }
                    if (packer == null) { packer = FosterSettings.Packers[0]; }
                    if (verbose) { Console.WriteLine("Changing compression method of file \"" + packFolder + " to \"" + packer.PackerName + "\"..."); }
                    using (var fStr = new System.IO.FileStream(packFile, System.IO.FileMode.OpenOrCreate, System.IO.FileAccess.ReadWrite, System.IO.FileShare.ReadWrite))
                    {
                        var header = Foster.GetHeader(fStr);

                        if (header.Encryption.RequiresPassword)
                        {
                            Console.WriteLine("Please enter the password:");
                            var pwd = new System.Security.SecureString();
                            while (true)
                            {
                                ConsoleKeyInfo i = Console.ReadKey(true);
                                if (i.Key == ConsoleKey.Enter)
                                {
                                    break;
                                }
                                else if (i.Key == ConsoleKey.Backspace)
                                {
                                    if (pwd.Length > 0)
                                    {
                                        pwd.RemoveAt(pwd.Length - 1);
                                        Console.Write("\b \b");
                                    }
                                }
                                else if (i.KeyChar != '\u0000') // KeyChar == '\u0000' if the key pressed does not correspond to a printable character, e.g. F1, Pause-Break, etc
                                {
                                    pwd.AppendChar(i.KeyChar);
                                    Console.Write("*");
                                }
                            }
                            password = System.Text.Encoding.Unicode.GetBytes(pwd.ToString());
                            Console.WriteLine("Encryption enabled but password not filled, using the same password.");
                            qassword = password;
                        }
                        fStr.Position = 0;
                        string tempFolder = System.IO.Path.Combine(System.IO.Path.GetTempPath(), "fosterman" + System.IO.Path.DirectorySeparatorChar + "temp_" + Tools.GenerateRandomText());
                        if (System.IO.Directory.Exists(tempFolder))
                        {
                            System.IO.Directory.Delete(tempFolder, true);
                        }
                        System.IO.Directory.CreateDirectory(tempFolder);
                        Foster.Read(tempFolder, fStr, password);
                        var newheader = new HeaderContext(tempFolder, packer: packer, encryption: header.Encryption, header.EncryptionArgs) { Password = qassword };
                        Foster.Write(newheader, fStr);
                    }
                    if (verbose) { Console.WriteLine("Changed encryption successfully."); }
                }
                catch (Exception ex)
                {
                    Console.Error.WriteLine("Error while changing encryption, exception caught:" + ex.ToString());
                }
                sw.Stop();
                if (verbose) { Console.WriteLine(fostermanName + " " + string.Join(' ', args) + " in " + sw.ElapsedMilliseconds + " ms."); }
                return;
            }
            if (args.Contains("clean"))
            {
                System.Diagnostics.Stopwatch sw = new System.Diagnostics.Stopwatch();
                sw.Start();
                try
                {
                    var foster = new Foster();
                    if (verbose)
                    {
                        Console.WriteLine("Cleaning temporary folder \"" + foster.TempFolder + "\"...");
                    }
                    long size = Tools.GetDirSize(foster.TempFolder);
                    if (Tools.HasWriteAccess(foster.TempFolder))
                    {
                        System.IO.Directory.Delete(foster.TempFolder, true);
                        if (verbose)
                        {
                            Console.WriteLine("Cleaned temporary folder.");
                        }
                    }
                    else
                    {
                        Console.Error.WriteLine("Cannot clean temporary folder, no permission.");
                    }
                }
                catch (Exception ex)
                {
                    Console.Error.WriteLine("Error while cleaning, exception caught:" + ex.ToString());
                }
                sw.Stop();
                if (verbose) { Console.WriteLine(fostermanName + " " + string.Join(' ', args) + " in " + sw.ElapsedMilliseconds + " ms."); }
                return;
            }
            if (args.Contains("pack"))
            {
                int singleArgLoc = Array.IndexOf(args, "pack");
                System.Diagnostics.Stopwatch sw = new System.Diagnostics.Stopwatch();
                sw.Start();
                try
                {
                    if (args.Length < singleArgLoc + 1) { Console.Error.WriteLine(" [E] Not enough information."); return; }
                    string packFolder = System.IO.Path.GetFullPath(args[singleArgLoc + 1]);
                    System.IO.DirectoryInfo pfInfo = new System.IO.DirectoryInfo(packFolder);
                    string packFile = null;
                    if (args.Contains("-o"))
                    {
                        int outputIndex = Array.IndexOf(args, "-o");
                        packFile = System.IO.Path.GetFullPath(args[outputIndex + 1]);
                    }
                    if (args.Contains("-Output") && string.IsNullOrWhiteSpace(packFile))
                    {
                        int outputIndex = Array.IndexOf(args, "-o");
                        packFile = System.IO.Path.GetFullPath(args[outputIndex + 1]);
                    }
                    if (string.IsNullOrWhiteSpace(packFile))
                    {
                        packFile = System.IO.Path.Combine(pfInfo.Parent.FullName, pfInfo.Name + ".fp");
                    }
                    FosterPackerBase algorithm = null;
                    if (args.Contains("-a", StringComparer.InvariantCultureIgnoreCase))
                    {
                        int algorithmIndex = Array.IndexOf(args, "-a");
                        if (args.Length >= algorithmIndex + 1)
                        {
                            string algName = args[algorithmIndex + 1].ToLowerEnglish();
                            var parseDone = int.TryParse(algName, out int algNo);
                            for (int i = 0; i < FosterSettings.Packers.Length; i++)
                            {
                                if ((parseDone && algNo == i) || (FosterSettings.Packers[i].PackerName == algName))
                                {
                                    algorithm = FosterSettings.Packers[i];
                                    break;
                                }
                            }
                            if (algorithm == null)
                            {
                                if (verbose) { Console.Error.WriteLine("Couldn't find a packer with name \"" + algName + "\". Using the default packer."); }
                                algorithm = FosterSettings.Packers[0];
                            }
                        }
                    }
                    if (args.Contains("-Algorithm", StringComparer.InvariantCultureIgnoreCase) && algorithm is null)
                    {
                        int algorithmIndex = Array.IndexOf(args, "-Algorithm");
                        if (args.Length >= algorithmIndex + 1)
                        {
                            string algName = args[algorithmIndex + 1].ToLowerEnglish();
                            var parseDone = int.TryParse(algName, out int algNo);
                            for (int i = 0; i < FosterSettings.Packers.Length; i++)
                            {
                                if ((parseDone && 0 == i) || (FosterSettings.Packers[i].PackerName == algName))
                                {
                                    algorithm = FosterSettings.Packers[i];
                                    break;
                                }
                            }
                            if (algorithm == null)
                            {
                                if (verbose) { Console.Error.WriteLine("Couldn't find a packer with name \"" + algName + "\". Using the default packer."); }
                                algorithm = FosterSettings.Packers[0];
                            }
                        }
                    }
                    FosterEncryptionBase encryption = null;
                    if (args.Contains("-e", StringComparer.InvariantCultureIgnoreCase) && encryption is null)
                    {
                        int algorithmIndex = Array.IndexOf(args, "-e");
                        if (args.Length >= algorithmIndex + 1)
                        {
                            string algName = args[algorithmIndex + 1].ToLowerEnglish();
                            var parseDone = int.TryParse(algName, out int algNo);
                            for (int i = 0; i < FosterSettings.Encryptions.Length; i++)
                            {
                                if ((parseDone && algNo == i) || (FosterSettings.Encryptions[i].EncryptionName == algName))
                                {
                                    encryption = FosterSettings.Encryptions[i];
                                    break;
                                }
                            }
                            if (encryption == null)
                            {
                                if (verbose) { Console.Error.WriteLine("Couldn't find an encryption with name \"" + algName + "\". Using the default encryption."); }
                                encryption = FosterSettings.Encryptions[0];
                            }
                        }
                    }
                    if (args.Contains("-Encryption", StringComparer.InvariantCultureIgnoreCase) && encryption is null)
                    {
                        int algorithmIndex = Array.IndexOf(args, "-Encryption");
                        if (args.Length >= algorithmIndex + 1)
                        {
                            string algName = args[algorithmIndex + 1].ToLowerEnglish();
                            var parseDone = int.TryParse(algName, out int algNo);
                            for (int i = 0; i < FosterSettings.Encryptions.Length; i++)
                            {
                                if ((parseDone && algNo == i) || (FosterSettings.Encryptions[i].EncryptionName == algName))
                                {
                                    encryption = FosterSettings.Encryptions[i];
                                    break;
                                }
                            }
                            if (encryption == null)
                            {
                                if (verbose) { Console.Error.WriteLine("Couldn't find an encryption with name \"" + algName + "\". Using the default encryption."); }
                                encryption = FosterSettings.Encryptions[0];
                            }
                        }
                    }
                    byte[] password = null;
                    if (args.Contains("-p", StringComparer.InvariantCultureIgnoreCase) && password is null)
                    {
                        int algorithmIndex = Array.IndexOf(args, "-p");
                        if (args.Length >= algorithmIndex + 1)
                        {
                            string algName = args[algorithmIndex + 1].ToLowerEnglish();
                            password = System.Text.Encoding.Unicode.GetBytes(algName);
                        }
                    }
                    if (args.Contains("-Password", StringComparer.InvariantCultureIgnoreCase) && password is null)
                    {
                        int algorithmIndex = Array.IndexOf(args, "-Password");
                        if (args.Length >= algorithmIndex + 1)
                        {
                            string algName = args[algorithmIndex + 1].ToLowerEnglish();
                            password = System.Text.Encoding.Unicode.GetBytes(algName);
                        }
                    }
                    string encryptArgs = string.Empty;
                    if (args.Contains("-c", StringComparer.InvariantCultureIgnoreCase) && string.IsNullOrWhiteSpace(encryptArgs))
                    {
                        int algorithmIndex = Array.IndexOf(args, "-c");
                        if (args.Length >= algorithmIndex + 1)
                        {
                            encryptArgs = args[algorithmIndex + 1];
                        }
                    }
                    if (args.Contains("-EncryptArgs", StringComparer.InvariantCultureIgnoreCase) && string.IsNullOrWhiteSpace(encryptArgs))
                    {
                        int algorithmIndex = Array.IndexOf(args, "-EncryptArgs");
                        if (args.Length >= algorithmIndex + 1)
                        {
                            encryptArgs = args[algorithmIndex + 1];
                        }
                    }
                    if (algorithm == null) { algorithm = FosterSettings.Packers[0]; }
                    if (encryption == null) { encryption = FosterSettings.Encryptions[0]; }
                    if (encryption.RequiresPassword && password.Length <= 0)
                    {
                        string pwd = Tools.GenerateRandomText(10);
                        Console.WriteLine("Encryption enabled but password not filled, using \"" + pwd + "\" as password.");
                        password = System.Text.Encoding.Unicode.GetBytes(pwd);
                    }
                    if (verbose) { Console.WriteLine("Packing \"" + packFolder + " to \"" + packFile + "\" with \"" + algorithm.PackerName + "\" algorithm..."); }
                    HeaderContext header = new HeaderContext(packFolder, algorithm.PackerName, encryption.EncryptionName, encryption.GenerateArguments(encryptArgs)) { Password = password };
                    using (var fStr = new System.IO.FileStream(packFile, System.IO.FileMode.OpenOrCreate, System.IO.FileAccess.ReadWrite, System.IO.FileShare.ReadWrite))
                    {
                        Foster.Write(header, fStr);
                    }
                    if (verbose) { Console.WriteLine("Packaged successfully."); }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error while packing, exception caught:" + ex.ToString());
                }
                sw.Stop();
                if (verbose) { Console.Error.WriteLine(fostermanName + " " + string.Join(' ', args) + " in " + sw.ElapsedMilliseconds + " ms."); }
                return;
            }
            if (args.Contains("unpack"))
            {
                int singleArgLoc = Array.IndexOf(args, "unpack");
                System.Diagnostics.Stopwatch sw = new System.Diagnostics.Stopwatch();
                sw.Start();
                try
                {
                    if (args.Length < singleArgLoc + 1) { Console.Error.WriteLine(" [E] Not enough information."); return; }
                    string unpackFile = System.IO.Path.GetFullPath(args[singleArgLoc + 1]);
                    System.IO.FileInfo ufInfo = new System.IO.FileInfo(unpackFile);
                    string unpackFolder = (args.Length <= singleArgLoc + 2) ? ufInfo.DirectoryName + System.IO.Path.DirectorySeparatorChar + System.IO.Path.GetFileNameWithoutExtension(unpackFile) + System.IO.Path.DirectorySeparatorChar : System.IO.Path.GetFullPath(args[singleArgLoc + 2]);
                    byte[] password = null;
                    if (args.Contains("-p", StringComparer.InvariantCultureIgnoreCase))
                    {
                        int algorithmIndex = Array.IndexOf(args, "-p");
                        if (args.Length >= algorithmIndex + 1)
                        {
                            string algName = args[algorithmIndex + 1].ToLowerEnglish();
                            password = System.Text.Encoding.Unicode.GetBytes(algName);
                        }
                    }
                    if (args.Contains("-Password", StringComparer.InvariantCultureIgnoreCase))
                    {
                        int algorithmIndex = Array.IndexOf(args, "-Password");
                        if (args.Length >= algorithmIndex + 1)
                        {
                            string algName = args[algorithmIndex + 1].ToLowerEnglish();
                            password = System.Text.Encoding.Unicode.GetBytes(algName);
                        }
                    }
                    if (verbose) { Console.WriteLine("Unpacking \"" + unpackFile + " to \"" + unpackFolder + "\"..."); }
                    using (var fStr = new System.IO.FileStream(unpackFile, System.IO.FileMode.Open, System.IO.FileAccess.Read, System.IO.FileShare.ReadWrite))
                    {
                        var reqPassword = Foster.GetHeader(fStr).Encryption.RequiresPassword;

                        if (reqPassword)
                        {
                            Console.WriteLine("Please enter the password:");
                            var pwd = new System.Security.SecureString();
                            while (true)
                            {
                                ConsoleKeyInfo i = Console.ReadKey(true);
                                if (i.Key == ConsoleKey.Enter)
                                {
                                    break;
                                }
                                else if (i.Key == ConsoleKey.Backspace)
                                {
                                    if (pwd.Length > 0)
                                    {
                                        pwd.RemoveAt(pwd.Length - 1);
                                        Console.Write("\b \b");
                                    }
                                }
                                else if (i.KeyChar != '\u0000') // KeyChar == '\u0000' if the key pressed does not correspond to a printable character, e.g. F1, Pause-Break, etc
                                {
                                    pwd.AppendChar(i.KeyChar);
                                    Console.Write("*");
                                }
                            }
                            password = System.Text.Encoding.Unicode.GetBytes(pwd.ToString());
                        }
                        fStr.Position = 0;
                        Foster.Read(unpackFolder, fStr, password);
                    }
                    if (verbose) { Console.WriteLine("Package unpacked successfully."); }
                }
                catch (Exception ex)
                {
                    Console.Error.WriteLine("Error while unpacking, exception caught:" + ex.ToString());
                }
                sw.Stop();
                if (verbose) { Console.WriteLine(fostermanName + " " + string.Join(' ', args) + " in " + sw.ElapsedMilliseconds + " ms."); }
                return;
            }
            if (args.Contains("delta"))
            {
                int singleArgLoc = Array.IndexOf(args, "delta");
                System.Diagnostics.Stopwatch sw = new System.Diagnostics.Stopwatch();
                sw.Start();
                try
                {
                    if (args.Length < singleArgLoc + 2) { Console.Error.WriteLine(" [E] Not enough information."); return; }
                    string cFolder = System.IO.Path.GetFullPath(args[singleArgLoc + 1]);
                    string bFolder = System.IO.Path.GetFullPath(args[singleArgLoc + 2]);
                    System.IO.DirectoryInfo cfInfo = new System.IO.DirectoryInfo(cFolder);
                    string dFile = null;
                    if (args.Contains("-o"))
                    {
                        int outputIndex = Array.IndexOf(args, "-o");
                        dFile = System.IO.Path.GetFullPath(args[outputIndex + 1]);
                    }
                    if (args.Contains("-Output"))
                    {
                        int outputIndex = Array.IndexOf(args, "-o");
                        dFile = System.IO.Path.GetFullPath(args[outputIndex + 1]);
                    }
                    if (string.IsNullOrWhiteSpace(dFile))
                    {
                        dFile = System.IO.Path.Combine(cfInfo.Parent.FullName, System.IO.Path.DirectorySeparatorChar + cfInfo.Name + ".dfp");
                    }
                    if (verbose) { Console.WriteLine("Creating delta..."); }
                    FosterPackerBase algorithm = FosterSettings.Packers[0];
                    if (args.Contains("-a"))
                    {
                        int algorithmIndex = Array.IndexOf(args, "-a");
                        if (args.Length >= algorithmIndex + 1)
                        {
                            string algName = args[algorithmIndex + 1].ToLowerEnglish();
                            var parseDone = int.TryParse(algName, out int algNo);
                            for (int i = 0; i < FosterSettings.Packers.Length; i++)
                            {
                                if ((parseDone && algNo == i) || (FosterSettings.Packers[i].PackerName == algName))
                                {
                                    algorithm = FosterSettings.Packers[i];
                                    break;
                                }
                            }
                            if (algorithm == null)
                            {
                                if (verbose) { Console.Error.WriteLine("Couldn't find a packer with name \"" + algName + "\". Using the default packer."); }
                                algorithm = FosterSettings.Packers[0];
                            }
                        }
                    }
                    if (args.Contains("-Algorithm"))
                    {
                        int algorithmIndex = Array.IndexOf(args, "-Algorithm");
                        if (args.Length >= algorithmIndex + 1)
                        {
                            string algName = args[algorithmIndex + 1].ToLowerEnglish();
                            var parseDone = int.TryParse(algName, out int algNo);
                            for (int i = 0; i < FosterSettings.Packers.Length; i++)
                            {
                                if ((parseDone && algNo == i) || (FosterSettings.Packers[i].PackerName == algName))
                                {
                                    algorithm = FosterSettings.Packers[i];
                                    break;
                                }
                            }
                            if (algorithm == null)
                            {
                                if (verbose) { Console.Error.WriteLine("Couldn't find a packer with name \"" + algName + "\". Using the default packer."); }
                                algorithm = FosterSettings.Packers[0];
                            }
                        }
                    }
                    FosterEncryptionBase encryption = null;
                    if (args.Contains("-e", StringComparer.InvariantCultureIgnoreCase) && encryption is null)
                    {
                        int algorithmIndex = Array.IndexOf(args, "-e");
                        if (args.Length >= algorithmIndex + 1)
                        {
                            string algName = args[algorithmIndex + 1].ToLowerEnglish();
                            var parseDone = int.TryParse(algName, out int algNo);
                            for (int i = 0; i < FosterSettings.Encryptions.Length; i++)
                            {
                                if ((parseDone && algNo == i) || (FosterSettings.Encryptions[i].EncryptionName == algName))
                                {
                                    encryption = FosterSettings.Encryptions[i];
                                    break;
                                }
                            }
                            if (encryption == null)
                            {
                                if (verbose) { Console.Error.WriteLine("Couldn't find an encryption with name \"" + algName + "\". Using the default encryption."); }
                                encryption = FosterSettings.Encryptions[0];
                            }
                        }
                    }
                    if (args.Contains("-Encryption", StringComparer.InvariantCultureIgnoreCase) && encryption is null)
                    {
                        int algorithmIndex = Array.IndexOf(args, "-Encryption");
                        if (args.Length >= algorithmIndex + 1)
                        {
                            string algName = args[algorithmIndex + 1].ToLowerEnglish();
                            var parseDone = int.TryParse(algName, out int algNo);
                            for (int i = 0; i < FosterSettings.Encryptions.Length; i++)
                            {
                                if ((parseDone && algNo == i) || (FosterSettings.Encryptions[i].EncryptionName == algName))
                                {
                                    encryption = FosterSettings.Encryptions[i];
                                    break;
                                }
                            }
                            if (encryption == null)
                            {
                                if (verbose) { Console.Error.WriteLine("Couldn't find an encryption with name \"" + algName + "\". Using the default encryption."); }
                                encryption = FosterSettings.Encryptions[0];
                            }
                        }
                    }
                    byte[] password = null;
                    if (args.Contains("-p", StringComparer.InvariantCultureIgnoreCase) && password is null)
                    {
                        int algorithmIndex = Array.IndexOf(args, "-p");
                        if (args.Length >= algorithmIndex + 1)
                        {
                            string algName = args[algorithmIndex + 1].ToLowerEnglish();
                            password = System.Text.Encoding.Unicode.GetBytes(algName);
                        }
                    }
                    if (args.Contains("-Password", StringComparer.InvariantCultureIgnoreCase) && password is null)
                    {
                        int algorithmIndex = Array.IndexOf(args, "-Password");
                        if (args.Length >= algorithmIndex + 1)
                        {
                            string algName = args[algorithmIndex + 1].ToLowerEnglish();
                            password = System.Text.Encoding.Unicode.GetBytes(algName);
                        }
                    }
                    string encryptArgs = string.Empty;
                    if (args.Contains("-c", StringComparer.InvariantCultureIgnoreCase) && string.IsNullOrWhiteSpace(encryptArgs))
                    {
                        int algorithmIndex = Array.IndexOf(args, "-c");
                        if (args.Length >= algorithmIndex + 1)
                        {
                            encryptArgs = args[algorithmIndex + 1];
                        }
                    }
                    if (args.Contains("-EncryptArgs", StringComparer.InvariantCultureIgnoreCase) && string.IsNullOrWhiteSpace(encryptArgs))
                    {
                        int algorithmIndex = Array.IndexOf(args, "-EncryptArgs");
                        if (args.Length >= algorithmIndex + 1)
                        {
                            encryptArgs = args[algorithmIndex + 1];
                        }
                    }
                    if (algorithm == null) { algorithm = FosterSettings.Packers[0]; }
                    if (encryption == null) { encryption = FosterSettings.Encryptions[0]; }
                    if (encryption.RequiresPassword && password.Length <= 0)
                    {
                        string pwd = Tools.GenerateRandomText(10);
                        Console.WriteLine("Encryption enabled but password not filled, using \"" + pwd + "\" as password.");
                        password = System.Text.Encoding.Unicode.GetBytes(pwd);
                    }
                    HeaderContext header = new HeaderContext(System.IO.Path.GetFullPath(bFolder), System.IO.Path.GetFullPath(cFolder), algorithm, parser: null, encryption, encryption.GenerateArguments(encryptArgs)) { Password = password };
                    using (var fStr = new System.IO.FileStream("", System.IO.FileMode.OpenOrCreate, System.IO.FileAccess.ReadWrite, System.IO.FileShare.ReadWrite))
                    {
                        Foster.Write(header, fStr);
                    }
                    if (verbose) { Console.WriteLine("Creating delta successful."); }
                }
                catch (Exception ex)
                {
                    Console.Error.WriteLine("Error while creating delta, exception caught:" + ex.ToString());
                }
                sw.Stop();
                if (verbose) { Console.WriteLine(fostermanName + " " + string.Join(' ', args) + " in " + sw.ElapsedMilliseconds + " ms."); }
                return;
            }
            if (args.Contains("adelta"))
            {
                int singleArgLoc = Array.IndexOf(args, "adelta");
                System.Diagnostics.Stopwatch sw = new System.Diagnostics.Stopwatch();
                sw.Start();
                try
                {
                    if (args.Length < singleArgLoc + 3) { Console.Error.WriteLine(" [E] Not enough information."); return; }
                    string cFolder = System.IO.Path.GetFullPath(args[singleArgLoc + 1]);
                    string dFile = System.IO.Path.GetFullPath(args[singleArgLoc + 2]);
                    byte[] password = null;
                    if (args.Contains("-p", StringComparer.InvariantCultureIgnoreCase))
                    {
                        int algorithmIndex = Array.IndexOf(args, "-p");
                        if (args.Length >= algorithmIndex + 1)
                        {
                            string algName = args[algorithmIndex + 1].ToLowerEnglish();
                            password = System.Text.Encoding.Unicode.GetBytes(algName);
                        }
                    }
                    if (args.Contains("-Password", StringComparer.InvariantCultureIgnoreCase))
                    {
                        int algorithmIndex = Array.IndexOf(args, "-Password");
                        if (args.Length >= algorithmIndex + 1)
                        {
                            string algName = args[algorithmIndex + 1].ToLowerEnglish();
                            password = System.Text.Encoding.Unicode.GetBytes(algName);
                        }
                    }
                    using var fStr = new System.IO.FileStream(dFile, System.IO.FileMode.Open, System.IO.FileAccess.Read, System.IO.FileShare.ReadWrite);
                    var reqPassword = Foster.GetHeader(fStr).Encryption.RequiresPassword;

                    if (reqPassword)
                    {
                        Console.WriteLine("Please enter the password:");
                        var pwd = new System.Security.SecureString();
                        while (true)
                        {
                            ConsoleKeyInfo i = Console.ReadKey(true);
                            if (i.Key == ConsoleKey.Enter)
                            {
                                break;
                            }
                            else if (i.Key == ConsoleKey.Backspace)
                            {
                                if (pwd.Length > 0)
                                {
                                    pwd.RemoveAt(pwd.Length - 1);
                                    Console.Write("\b \b");
                                }
                            }
                            else if (i.KeyChar != '\u0000') // KeyChar == '\u0000' if the key pressed does not correspond to a printable character, e.g. F1, Pause-Break, etc
                            {
                                pwd.AppendChar(i.KeyChar);
                                Console.Write("*");
                            }
                        }
                        password = System.Text.Encoding.Unicode.GetBytes(pwd.ToString());
                    }
                    fStr.Position = 0;
                    Foster.Read(cFolder, fStr, password);
                }
                catch (Exception ex)
                {
                    Console.Error.WriteLine("Error while applying delta, exception caught:" + ex.ToString());
                }
                sw.Stop();
                if (verbose) { Console.WriteLine(fostermanName + " " + string.Join(' ', args) + " in " + sw.ElapsedMilliseconds + " ms."); }
                return;
            }
            if (args.Contains("create"))
            {
                int singleArgLoc = Array.IndexOf(args, "create");
                System.Diagnostics.Stopwatch sw = new System.Diagnostics.Stopwatch();
                sw.Start();
                try
                {
                    if (args.Length < singleArgLoc + 2) { Console.Error.WriteLine(" [E] Not enough information."); return; }
                    string sFolder = System.IO.Path.GetFullPath(args[singleArgLoc + 1]);
                    string oFolder = System.IO.Path.GetFullPath(args[singleArgLoc + 2]);
                    string fosterFile = string.Empty;
                    if (args.Contains("-s")) { fosterFile = args[Array.IndexOf(args, "-s") + 1]; }
                    if (args.Contains("-Source")) { fosterFile = args[Array.IndexOf(args, "-Source") + 1]; }
                    bool skipEmptyDirs = false;
                    bool skipHashes = false;
                    int detectSED = 0;
                    if (args.Length >= singleArgLoc + 3)
                    {
                        if (args.Contains("--skip-empty-dirs"))
                        {
                            detectSED++;
                            skipEmptyDirs = true;
                            int sedLoc = Array.IndexOf(args, "--skip--empty-dirs");
                            if ((args.Length - 1) > sedLoc)
                            {
                                detectSED++;
                                string v = Tools.ToLowerEnglish(args[sedLoc + 1]);
                                skipEmptyDirs = v == "true" || v == "1" || v == "yes";
                                skipEmptyDirs = !(v == "false" || v == "0" || v == "no");
                            }
                        }
                        if (args.Contains("--skip-hashes"))
                        {
                            skipHashes = true;
                            detectSED++;
                            int shLoc = Array.IndexOf(args, "--skip-hashes");
                            if ((args.Length - 1) > shLoc)
                            {
                                detectSED++;
                                string v = Tools.ToLowerEnglish(args[shLoc + 1]);
                                skipHashes = v == "true" || v == "1" || v == "yes";
                                skipHashes = !(v == "false" || v == "0" || v == "no");
                            }
                        }
                    }
                    if (string.IsNullOrWhiteSpace(fosterFile))
                    {
                        if (verbose) { Console.WriteLine("Searching for a Skeleton Foster file..."); }
                        string[] allFiles = System.IO.Directory.GetFiles(sFolder, "*", System.IO.SearchOption.AllDirectories);
                        for (int i = 0; i < allFiles.Length; i++)
                        {
                            if (Tools.ToLowerEnglish(allFiles[i]).EndsWith(".foster"))
                            {
                                fosterFile = allFiles[i];
                                if (verbose) { Console.WriteLine("Skeleton Foster file found at  \"" + fosterFile + "\"."); }
                                break;
                            }
                        }
                        if (string.IsNullOrWhiteSpace(fosterFile))
                        {
                            throw new Exception("No Skeleton Foster (\".foster\") files are found in directory \"" + sFolder + "\".");
                        }
                    }
                    if (verbose) { Console.WriteLine("Creating packages..."); }
                    FosterPackerBase algorithm = FosterSettings.Packers[0];
                    if (args.Contains("-a"))
                    {
                        int algorithmIndex = Array.IndexOf(args, "-a");
                        if (args.Length < algorithmIndex + 1)
                        {
                            string algName = args[algorithmIndex + 1];
                            switch (algName.ToLowerEnglish())
                            {
                                case "none":
                                case "0":
                                    algorithm = FosterSettings.Packers[0];
                                    break;

                                case "gzip":
                                case "1":
                                    algorithm = FosterSettings.Packers[1];
                                    break;

                                case "deflate":
                                case "2":
                                    algorithm = FosterSettings.Packers[2];
                                    break;

                                default:
                                    throw new Exception("Unknown packer type \"" + algName.ToLowerEnglish() + "\".");
                            }
                        }
                    }
                    if (args.Contains("-Algorithm"))
                    {
                        int algorithmIndex = Array.IndexOf(args, "-Algorithm");
                        if (args.Length < algorithmIndex + 1)
                        {
                            string algName = args[algorithmIndex + 1];
                            switch (algName.ToLowerEnglish())
                            {
                                case "none":
                                case "0":
                                    algorithm = FosterSettings.Packers[0];
                                    break;

                                case "gzip":
                                case "1":
                                    algorithm = FosterSettings.Packers[1];
                                    break;

                                case "deflate":
                                case "2":
                                    algorithm = FosterSettings.Packers[2];
                                    break;

                                default:
                                    throw new Exception("Unknown packer type \"" + algName.ToLowerEnglish() + "\".");
                            }
                        }
                    }
                    FosterParserBase parser = FosterSettings.Parsers[0];
                    if (args.Contains("-p"))
                    {
                        int algorithmIndex = Array.IndexOf(args, "-a");
                        if (args.Length < algorithmIndex + 1)
                        {
                            string algName = args[algorithmIndex + 1];
                            switch (algName.ToLowerEnglish())
                            {
                                case "xml":
                                case "0":
                                    parser = FosterSettings.Parsers[1];
                                    break;

                                default:
                                    throw new Exception("Unknown parser type \"" + algName.ToLowerEnglish() + "\".");
                            }
                        }
                    }
                    if (args.Contains("-Parser"))
                    {
                        int algorithmIndex = Array.IndexOf(args, "-Parser");
                        if (args.Length < algorithmIndex + 1)
                        {
                            string algName = args[algorithmIndex + 1];
                            switch (algName.ToLowerEnglish())
                            {
                                case "xml":
                                case "0":
                                    parser = FosterSettings.Parsers[1];
                                    break;

                                default:
                                    throw new Exception("Unknown parser type \"" + algName.ToLowerEnglish() + "\".");
                            }
                        }
                    }
                    FosterEncryptionBase encryption = null;
                    if (args.Contains("-e", StringComparer.InvariantCultureIgnoreCase) && encryption is null)
                    {
                        int algorithmIndex = Array.IndexOf(args, "-e");
                        if (args.Length >= algorithmIndex + 1)
                        {
                            string algName = args[algorithmIndex + 1].ToLowerEnglish();
                            var parseDone = int.TryParse(algName, out int algNo);
                            for (int i = 0; i < FosterSettings.Encryptions.Length; i++)
                            {
                                if ((parseDone && algNo == i) || (FosterSettings.Encryptions[i].EncryptionName == algName))
                                {
                                    encryption = FosterSettings.Encryptions[i];
                                    break;
                                }
                            }
                            if (encryption == null)
                            {
                                if (verbose) { Console.Error.WriteLine("Couldn't find an encryption with name \"" + algName + "\". Using the default encryption."); }
                                encryption = FosterSettings.Encryptions[0];
                            }
                        }
                    }
                    if (args.Contains("-Encryption", StringComparer.InvariantCultureIgnoreCase) && encryption is null)
                    {
                        int algorithmIndex = Array.IndexOf(args, "-Encryption");
                        if (args.Length >= algorithmIndex + 1)
                        {
                            string algName = args[algorithmIndex + 1].ToLowerEnglish();
                            var parseDone = int.TryParse(algName, out int algNo);
                            for (int i = 0; i < FosterSettings.Encryptions.Length; i++)
                            {
                                if ((parseDone && algNo == i) || (FosterSettings.Encryptions[i].EncryptionName == algName))
                                {
                                    encryption = FosterSettings.Encryptions[i];
                                    break;
                                }
                            }
                            if (encryption == null)
                            {
                                if (verbose) { Console.Error.WriteLine("Couldn't find an encryption with name \"" + algName + "\". Using the default encryption."); }
                                encryption = FosterSettings.Encryptions[0];
                            }
                        }
                    }
                    byte[] password = null;
                    if (args.Contains("-pass", StringComparer.InvariantCultureIgnoreCase) && password is null)
                    {
                        int algorithmIndex = Array.IndexOf(args, "-p");
                        if (args.Length >= algorithmIndex + 1)
                        {
                            string algName = args[algorithmIndex + 1].ToLowerEnglish();
                            password = System.Text.Encoding.Unicode.GetBytes(algName);
                        }
                    }
                    if (args.Contains("-Password", StringComparer.InvariantCultureIgnoreCase) && password is null)
                    {
                        int algorithmIndex = Array.IndexOf(args, "-Password");
                        if (args.Length >= algorithmIndex + 1)
                        {
                            string algName = args[algorithmIndex + 1].ToLowerEnglish();
                            password = System.Text.Encoding.Unicode.GetBytes(algName);
                        }
                    }
                    string encryptArgs = string.Empty;
                    if (args.Contains("-c", StringComparer.InvariantCultureIgnoreCase) && string.IsNullOrWhiteSpace(encryptArgs))
                    {
                        int algorithmIndex = Array.IndexOf(args, "-c");
                        if (args.Length >= algorithmIndex + 1)
                        {
                            encryptArgs = args[algorithmIndex + 1];
                        }
                    }
                    if (args.Contains("-EncryptArgs", StringComparer.InvariantCultureIgnoreCase) && string.IsNullOrWhiteSpace(encryptArgs))
                    {
                        int algorithmIndex = Array.IndexOf(args, "-EncryptArgs");
                        if (args.Length >= algorithmIndex + 1)
                        {
                            encryptArgs = args[algorithmIndex + 1];
                        }
                    }
                    if (algorithm == null) { algorithm = FosterSettings.Packers[0]; }
                    if (encryption == null) { encryption = FosterSettings.Encryptions[0]; }
                    if (encryption.RequiresPassword && password.Length <= 0)
                    {
                        string pwd = Tools.GenerateRandomText(10);
                        Console.WriteLine("Encryption enabled but password not filled, using \"" + pwd + "\" as password.");
                        password = System.Text.Encoding.Unicode.GetBytes(pwd);
                    }
                    HeaderContext header = new HeaderContext() { Encryption = encryption, Packer = algorithm, Parser = parser, EncryptionArgs = encryption.GenerateArguments(encryptArgs), Password = password };
                    Foster.CreatePackages(sFolder, oFolder, header, fosterFile, skipEmptyDirs, skipHashes);
                    if (verbose) { Console.WriteLine("Creating packages successful."); }
                }
                catch (Exception ex)
                {
                    Console.Error.WriteLine("Error while creating, exception caught:" + ex.ToString());
                }
                sw.Stop();
                if (verbose) { Console.WriteLine(fostermanName + " " + string.Join(' ', args) + " in " + sw.ElapsedMilliseconds + " ms."); }
                return;
            }
            if (args.Contains("update"))
            {
                int singleArgLoc = Array.IndexOf(args, "update");
                System.Diagnostics.Stopwatch sw = new System.Diagnostics.Stopwatch();
                sw.Start();
                try
                {
                    if (args.Length < singleArgLoc + 3) { Console.Error.WriteLine(" [E] Not enough information."); return; }
                    string curFolder = System.IO.Path.GetFullPath(args[singleArgLoc + 1]);
                    string verFile = System.IO.Path.Combine(curFolder, System.IO.Path.DirectorySeparatorChar + ".fosterman");
                    string uri = args[singleArgLoc + 2];
                    int cver = System.IO.File.Exists(verFile) ? int.Parse(Tools.ReadFile(verFile, System.Text.Encoding.Unicode)) : 0;
                    string arch = "noarch";
                    if (args.Contains("-a"))
                    {
                        arch = args[Array.IndexOf(args, "-a") + 1];
                    }
                    if (args.Contains("-Arch"))
                    {
                        arch = args[Array.IndexOf(args, "-Arch") + 1];
                    }
                    if (string.IsNullOrWhiteSpace(arch)) { arch = "noarch"; }
                    var foster = new Foster(new System.IO.FileInfo(curFolder).Name, uri, curFolder, cver, arch)
                    {
                        SkipFileSizeInfo = true
                    };
                    bool noconfirm = false;
                    if (args.Length >= singleArgLoc + 3)
                    {
                        if (args.Contains("--skip-backup-errors"))
                        {
                            foster.SkipBackupError = true;
                            int shLoc = Array.IndexOf(args, "--skip-backup-errors");
                            if ((args.Length - 1) > shLoc)
                            {
                                string v = Tools.ToLowerEnglish(args[shLoc + 1]);
                                foster.SkipBackupError = v == "true" || v == "1" || v == "yes";
                                foster.SkipBackupError = !(v == "false" || v == "0" || v == "no");
                            }
                        }
                        if (args.Contains("--version"))
                        {
                            int shLoc = Array.IndexOf(args, "--version");
                            if ((args.Length - 1) > shLoc)
                            {
                                string v = Tools.ToLowerEnglish(args[shLoc + 1]);
                                foster.CurrentVer = int.Parse(v);
                            }
                            else
                            {
                                throw new ArgumentNullException("Version ID");
                            }
                        }
                        if (args.Contains("--skip-backup"))
                        {
                            foster.SkipBackup = true;
                            int sedLoc = Array.IndexOf(args, "--skip--backup");
                            if ((args.Length - 1) > sedLoc)
                            {
                                string v = Tools.ToLowerEnglish(args[sedLoc + 1]);
                                foster.SkipBackup = v == "true" || v == "1" || v == "yes";
                                foster.SkipBackup = !(v == "false" || v == "0" || v == "no");
                            }
                        }
                        if (args.Contains("--erase"))
                        {
                            foster.SkipBackup = true;
                            int sedLoc = Array.IndexOf(args, "--erase");
                            if ((args.Length - 1) > sedLoc)
                            {
                                string v = Tools.ToLowerEnglish(args[sedLoc + 1]);
                                foster.EraseFolder = v == "true" || v == "1" || v == "yes";
                                foster.EraseFolder = !(v == "false" || v == "0" || v == "no");
                            }
                        }
                        if (args.Contains("--skip-hashes"))
                        {
                            foster.SkipHashes = true;
                            int sedLoc = Array.IndexOf(args, "--skip--hashes");
                            if ((args.Length - 1) > sedLoc)
                            {
                                string v = Tools.ToLowerEnglish(args[sedLoc + 1]);
                                foster.SkipHashes = v == "true" || v == "1" || v == "yes";
                                foster.SkipHashes = !(v == "false" || v == "0" || v == "no");
                            }
                        }
                        if (args.Contains("-y"))
                        {
                            noconfirm = true;
                        }
                        if (args.Contains("--retry"))
                        {
                            int shLoc = Array.IndexOf(args, "--retry");
                            if ((args.Length - 1) > shLoc)
                            {
                                foster.RetryCount = int.Parse(Tools.ToLowerEnglish(args[shLoc + 1]));
                            }
                            else
                            {
                                throw new ArgumentNullException("Retry Count");
                            }
                        }
                    }
                    byte[] password = null;
                    if (args.Contains("-p", StringComparer.InvariantCultureIgnoreCase) && password is null)
                    {
                        int algorithmIndex = Array.IndexOf(args, "-p");
                        if (args.Length >= algorithmIndex + 1)
                        {
                            string algName = args[algorithmIndex + 1].ToLowerEnglish();
                            password = System.Text.Encoding.Unicode.GetBytes(algName);
                        }
                    }
                    if (args.Contains("-Password", StringComparer.InvariantCultureIgnoreCase) && password is null)
                    {
                        int algorithmIndex = Array.IndexOf(args, "-Password");
                        if (args.Length >= algorithmIndex + 1)
                        {
                            string algName = args[algorithmIndex + 1].ToLowerEnglish();
                            password = System.Text.Encoding.Unicode.GetBytes(algName);
                        }
                    }
                    foster.Password = password;
                    foster.OnLogEntry += new Foster.OnLogEntryDelegate((sender, e) =>
                    {
                        if (verbose)
                        {
                            switch (e.Level)
                            {
                                case LogLevel.Hidden:
                                    break;

                                default:
                                case LogLevel.None:
                                    Console.WriteLine(e.LogEntry);
                                    break;

                                case LogLevel.Info:
                                    Console.WriteLine(" [I] " + e.LogEntry);
                                    break;

                                case LogLevel.Warning:
                                    Console.WriteLine(" [W] " + e.LogEntry);
                                    break;

                                case LogLevel.Error:
                                    Console.Error.WriteLine(" [E]" + e.LogEntry);
                                    break;

                                case LogLevel.Critical:
                                    Console.WriteLine(" [C] " + e.LogEntry);
                                    break;
                            }
                        }
                    });
                    Foster_Download[] downloadList = foster.GetDownloads();
                    long totalDownload = 0;
                    long totalDisk = 0;
                    string downloadSize = "Package(s) to download: " + Environment.NewLine;
                    for (int i = 0; i < downloadList.Length; i++)
                    {
                        totalDownload += downloadList[i].FileDownloadSize;
                        totalDisk += downloadList[i].FileDiskSize;
                        downloadSize += downloadList[i].Name + " (" + Tools.ByteToReadable(downloadList[i].FileDownloadSize) + ") ";
                    }
                    downloadSize += Environment.NewLine + "Total Download Size: " + Tools.ByteToReadable(totalDownload) + Environment.NewLine + "Total change on disk: " + (totalDisk < 0 ? ("-" + Tools.ByteToReadable(Math.Abs(totalDisk))) : ("+" + Tools.ByteToReadable(totalDisk))) + Environment.NewLine;
                    Console.WriteLine(downloadSize);
                    Console.Write("Proceed [y/N] ?");
                    if (!noconfirm)
                    {
                        if (Tools.ToLowerEnglish(Console.ReadLine()) != "y")
                        {
                            Console.Error.WriteLine("Canceled.");
                            return;
                        }
                    }
                    foster.Update(true);
                    if (foster.LatestException != null)
                    {
                        throw foster.LatestException;
                    }
                    else
                    {
                        Tools.WriteFile(verFile, "" + foster.CurrentVersion.ID, System.Text.Encoding.Unicode);
                        Console.WriteLine("Update finished with no errors.");
                    }
                }
                catch (Exception ex)
                {
                    Console.Error.WriteLine("Error while updating, exception caught:" + ex.ToString());
                }
                sw.Stop();
                if (verbose) { Console.WriteLine(fostermanName + " " + string.Join(' ', args) + " in " + sw.ElapsedMilliseconds + " ms."); }
                return;
            }
            if (args.Contains("install"))
            {
                int singleArgLoc = Array.IndexOf(args, "install");
                System.Diagnostics.Stopwatch sw = new System.Diagnostics.Stopwatch();
                sw.Start();
                try
                {
                    if (args.Length < singleArgLoc + 3) { Console.Error.WriteLine(" [E] Not enough information."); return; }
                    string curFolder = System.IO.Path.GetFullPath(args[singleArgLoc + 1]);
                    string verFile = System.IO.Path.Combine(curFolder, System.IO.Path.DirectorySeparatorChar + ".fosterman");
                    string uri = args[singleArgLoc + 2];
                    string arch = "noarch";
                    if (args.Contains("-a"))
                    {
                        arch = args[Array.IndexOf(args, "-a") + 1];
                    }
                    if (args.Contains("-Arch"))
                    {
                        arch = args[Array.IndexOf(args, "-Arch") + 1];
                    }
                    if (string.IsNullOrWhiteSpace(arch)) { arch = "noarch"; }
                    var foster = new Foster(new System.IO.FileInfo(curFolder).Name, uri, curFolder, 0, arch)
                    {
                        SkipFileSizeInfo = true
                    };
                    bool noconfirm = false;
                    if (args.Length >= singleArgLoc + 3)
                    {
                        if (args.Contains("--skip-backup-errors"))
                        {
                            foster.SkipBackupError = true;
                            int shLoc = Array.IndexOf(args, "--skip-backup-errors");
                            if ((args.Length - 1) > shLoc)
                            {
                                string v = Tools.ToLowerEnglish(args[shLoc + 1]);
                                foster.SkipBackupError = v == "true" || v == "1" || v == "yes";
                                foster.SkipBackupError = !(v == "false" || v == "0" || v == "no");
                            }
                        }
                        if (args.Contains("--version"))
                        {
                            int shLoc = Array.IndexOf(args, "--version");
                            if ((args.Length - 1) > shLoc)
                            {
                                string v = Tools.ToLowerEnglish(args[shLoc + 1]);
                                foster.CurrentVer = int.Parse(v);
                            }
                            else
                            {
                                throw new ArgumentNullException("Version ID");
                            }
                        }
                        if (args.Contains("--skip-hashes"))
                        {
                            foster.SkipHashes = true;
                            int sedLoc = Array.IndexOf(args, "--skip--hashes");
                            if ((args.Length - 1) > sedLoc)
                            {
                                string v = Tools.ToLowerEnglish(args[sedLoc + 1]);
                                foster.SkipHashes = v == "true" || v == "1" || v == "yes";
                                foster.SkipHashes = !(v == "false" || v == "0" || v == "no");
                            }
                        }
                        if (args.Contains("--erase"))
                        {
                            foster.SkipBackup = true;
                            int sedLoc = Array.IndexOf(args, "--erase");
                            if ((args.Length - 1) > sedLoc)
                            {
                                string v = Tools.ToLowerEnglish(args[sedLoc + 1]);
                                foster.EraseFolder = v == "true" || v == "1" || v == "yes";
                                foster.EraseFolder = !(v == "false" || v == "0" || v == "no");
                            }
                        }
                        if (args.Contains("--skip-backup"))
                        {
                            foster.SkipBackup = true;
                            int sedLoc = Array.IndexOf(args, "--skip--backup");
                            if ((args.Length - 1) > sedLoc)
                            {
                                string v = Tools.ToLowerEnglish(args[sedLoc + 1]);
                                foster.SkipBackup = v == "true" || v == "1" || v == "yes";
                                foster.SkipBackup = !(v == "false" || v == "0" || v == "no");
                            }
                        }
                        if (args.Contains("-y"))
                        {
                            noconfirm = true;
                        }
                        if (args.Contains("--retry"))
                        {
                            int shLoc = Array.IndexOf(args, "--retry");
                            if ((args.Length - 1) > shLoc)
                            {
                                foster.RetryCount = int.Parse(Tools.ToLowerEnglish(args[shLoc + 1]));
                            }
                            else
                            {
                                throw new ArgumentNullException("Retry Count");
                            }
                        }
                    }
                    byte[] password = null;
                    if (args.Contains("-p", StringComparer.InvariantCultureIgnoreCase) && password is null)
                    {
                        int algorithmIndex = Array.IndexOf(args, "-p");
                        if (args.Length >= algorithmIndex + 1)
                        {
                            string algName = args[algorithmIndex + 1].ToLowerEnglish();
                            password = System.Text.Encoding.Unicode.GetBytes(algName);
                        }
                    }
                    if (args.Contains("-Password", StringComparer.InvariantCultureIgnoreCase) && password is null)
                    {
                        int algorithmIndex = Array.IndexOf(args, "-Password");
                        if (args.Length >= algorithmIndex + 1)
                        {
                            string algName = args[algorithmIndex + 1].ToLowerEnglish();
                            password = System.Text.Encoding.Unicode.GetBytes(algName);
                        }
                    }
                    foster.Password = password;
                    foster.OnLogEntry += new Foster.OnLogEntryDelegate((sender, e) =>
                    {
                        if (verbose)
                        {
                            switch (e.Level)
                            {
                                case LogLevel.Hidden:
                                    break;

                                default:
                                case LogLevel.None:
                                    Console.WriteLine(e.LogEntry);
                                    break;

                                case LogLevel.Info:
                                    Console.WriteLine(" [I] " + e.LogEntry);
                                    break;

                                case LogLevel.Warning:
                                    Console.WriteLine(" [W] " + e.LogEntry);
                                    break;

                                case LogLevel.Error:
                                    Console.Error.WriteLine(" [E]" + e.LogEntry);
                                    break;

                                case LogLevel.Critical:
                                    Console.WriteLine(" [C] " + e.LogEntry);
                                    break;
                            }
                        }
                    });
                    foster.LoadFromUrl();
                    Foster_Download[] downloadList = foster.GetDownloads(null, foster.LatestVersion);
                    long totalDownload = 0;
                    long totalDisk = 0;
                    string downloadSize = "Package(s) to download: " + Environment.NewLine;
                    for (int i = 0; i < downloadList.Length; i++)
                    {
                        totalDownload += downloadList[i].FileDownloadSize;
                        totalDisk += downloadList[i].FileDiskSize;
                        downloadSize += downloadList[i].Name + " (" + Tools.ByteToReadable(downloadList[i].FileDownloadSize) + ") ";
                    }
                    downloadSize += Environment.NewLine + "Total Download Size: " + Tools.ByteToReadable(totalDownload) + Environment.NewLine + "Total change on disk: " + (totalDisk < 0 ? ("-" + Tools.ByteToReadable(Math.Abs(totalDisk))) : ("+" + Tools.ByteToReadable(totalDisk))) + Environment.NewLine;
                    Console.WriteLine(downloadSize);
                    Console.Write("Proceed [y/N] ?");
                    if (!noconfirm)
                    {
                        if (Tools.ToLowerEnglish(Console.ReadLine()) != "y")
                        {
                            Console.Error.WriteLine("Canceled.");
                            return;
                        }
                    }
                    foster.Update(true);
                    if (foster.LatestException != null)
                    {
                        throw foster.LatestException;
                    }
                    else
                    {
                        Tools.WriteFile(verFile, "" + foster.CurrentVersion.ID, System.Text.Encoding.Unicode);
                        Console.WriteLine("Update finished with no errors.");
                    }
                }
                catch (Exception ex)
                {
                    Console.Error.WriteLine("Error while installing, exception caught:" + ex.ToString());
                }
                sw.Stop();
                if (verbose) { Console.WriteLine(fostermanName + " " + string.Join(' ', args) + " in " + sw.ElapsedMilliseconds + " ms."); }
                return;
            }
            if (args.Contains("query"))
            {
                int singleArgLoc = Array.IndexOf(args, "query");
                System.Diagnostics.Stopwatch sw = new System.Diagnostics.Stopwatch();
                sw.Start();
                try
                {
                    if (args.Length < singleArgLoc + 1) { Console.Error.WriteLine(" [E] Not enough information."); return; }
                    string uri = args[singleArgLoc + 1];
                    var foster = new Foster()
                    {
                        SkipFileSizeInfo = true,
                        URL = uri,
                    };
                    byte[] password = null;
                    if (args.Contains("-p", StringComparer.InvariantCultureIgnoreCase) && password is null)
                    {
                        int algorithmIndex = Array.IndexOf(args, "-p");
                        if (args.Length >= algorithmIndex + 1)
                        {
                            string algName = args[algorithmIndex + 1].ToLowerEnglish();
                            password = System.Text.Encoding.Unicode.GetBytes(algName);
                        }
                    }
                    if (args.Contains("-Password", StringComparer.InvariantCultureIgnoreCase) && password is null)
                    {
                        int algorithmIndex = Array.IndexOf(args, "-Password");
                        if (args.Length >= algorithmIndex + 1)
                        {
                            string algName = args[algorithmIndex + 1].ToLowerEnglish();
                            password = System.Text.Encoding.Unicode.GetBytes(algName);
                        }
                    }
                    foster.Password = password;
                    foster.OnLogEntry += new Foster.OnLogEntryDelegate((sender, e) =>
                    {
                        if (verbose)
                        {
                            switch (e.Level)
                            {
                                case LogLevel.Hidden:
                                    break;

                                default:
                                case LogLevel.None:
                                    Console.WriteLine(e.LogEntry);
                                    break;

                                case LogLevel.Info:
                                    Console.WriteLine(" [I] " + e.LogEntry);
                                    break;

                                case LogLevel.Warning:
                                    Console.WriteLine(" [W] " + e.LogEntry);
                                    break;

                                case LogLevel.Error:
                                    Console.Error.WriteLine(" [E]" + e.LogEntry);
                                    break;

                                case LogLevel.Critical:
                                    Console.WriteLine(" [C] " + e.LogEntry);
                                    break;
                            }
                        }
                    });

                    foster.LoadFromUrl();

                    Console.WriteLine("Foster Name: " + foster.Name);
                    Console.WriteLine("Latest Version: " + foster.LatestVer);
                    Console.WriteLine("Dependencies: [" + (foster.Dependencies != null && foster.Dependencies.Count > 0 ? foster.Dependencies.Count : 0) + "]");
                    if (verbose)
                    {
                        for (int i = 0; i < foster.Dependencies.Count; i++)
                        {
                            Console.Write(foster.Dependencies[i].Name);
                        }
                        Console.WriteLine();
                    }

                    Console.WriteLine("Versions: [" + foster.Versions.Count + "]");
                    if (verbose)
                    {
                        for (int i = 0; i <= foster.Versions.Count; i++)
                        {
                            var version = foster.Versions[i];
                            Console.WriteLine("#");
                            Console.WriteLine("# Version ID: " + version.ID);
                            Console.WriteLine("# Version Name: " + version.Name);
                            Console.WriteLine("# based on Version: " + (version.BasedVersion != null ? (version.BasedVersion is int ınt ? "" + ınt : "" + (version.BasedVersion as Foster_Version).ID) : "<none>"));
                            Console.WriteLine("# Version Flags: " + String.Join(';', version.Flags));
                            Console.WriteLine("# Version Long-Term Support: " + (version.LTS ? ((version.IsLTSRevoked ? "Revoked in " : "Will revoke in ") + version.LTSRevokeDate) : "<false>"));
                            Console.WriteLine("# Version Dependencies: [" + version.Dependencies + "]");
                            for (int ı = 0; ı < version.Dependencies.Count; ı++)
                            {
                                var dep = version.Dependencies[ı];
                                Console.WriteLine("## Dependency Name:" + dep.Dependency.Name);
                                Console.WriteLine("## Dependency Version:" + dep.RequiredVerID);
                            }
                            Console.WriteLine("# Version Architectures: [" + version.Archs + "]");
                            for (int ı = 0; ı < version.Archs.Count; ı++)
                            {
                                var arch = version.Archs[ı];
                                Console.WriteLine("##");
                                Console.WriteLine("## Architecture Arch:" + arch.Arch);
                                Console.WriteLine("## Architecture Disk Size:" + arch.DiskSize.ByteToReadable() + " [ " + arch.DiskSize + " ]");
                                Console.WriteLine("## Architecture Download Size:" + arch.DownloadSize.ByteToReadable() + " [ " + arch.DownloadSize + " ]");
                                Console.WriteLine("## Architecture URL(s):" + Environment.NewLine + "###" + Environment.NewLine + string.Join(Environment.NewLine + "###", arch.Url) + Environment.NewLine + "###");
                                Console.WriteLine("## Architecture Hashes: [" + arch.Hashes.Count + "]");
                                for (int _i = 0; _i < arch.Hashes.Count; _i++)
                                {
                                    var hash = arch.Hashes[_i];
                                    Console.WriteLine("###");
                                    Console.WriteLine("### Algorithm: " + hash.AlgorithmShortName);
                                    Console.WriteLine("### Hash: " + hash.Hash);
                                    Console.WriteLine("###");
                                }
                                Console.WriteLine("##");
                            }
                            Console.WriteLine("#");
                        }
                    }
                    if (foster.LatestException != null)
                    {
                        throw foster.LatestException;
                    }
                }
                catch (Exception ex)
                {
                    Console.Error.WriteLine("Error while getting information, exception caught:" + ex.ToString());
                }
                sw.Stop();
                if (verbose) { Console.WriteLine(fostermanName + " " + string.Join(' ', args) + " in " + sw.ElapsedMilliseconds + " ms."); }
                return;
            }
            Console.Error.WriteLine(" [E] Unknown command, please use help command to see all supported commands.");
        }
    }
}