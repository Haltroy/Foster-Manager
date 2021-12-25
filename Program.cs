/*

Copyright © 2021 - 2022 haltroy

Use of this source code is governed by a GNU General Public License version 3.0 that can be found in github.com/haltroy/Foster-Manager/blob/master/COPYING

*/

using Foster;
using Foster.Modules;
using System;
using System.Linq;

namespace Foster_Manager
{
    // NOTES TO PACKAGE MAINTAINERS:
    // 1- Before building, please change the "RuntimeIdentifier" in "Foster Manager.csproj" file (with Microsoft's RIDs) and also
    // change value of "Arch" below (with Foster's Architectures). Also, change value of "Dev" to your username.
    // Microsoft RIDs: win-x86 win-x64 win-arm win-arm64 linux-x64 linux-x86 linux-musl-x64 linux-arm linux-arm64 osx-x64
    // Foster Archs: see https://github.com/Foster/tree/master/Foster%20Examples/Archs.md

    // NOTES TO CUSTOMIZATION:
    // To add new packers or parsers, add them into the HookParsersAndPackers() coid below and then build it.
    internal class Program
    {
        private static class Versioning
        {
            public static string Version => "1.0.0.0"; // <-- DO NOT CHANGE THIS
            public static string Dev => "haltroy"; // <-- Change this
            public static string Arch => "noarch"; // <-- Change this
        }

        private static void HookParsersAndPackers()
        {
            // NOTE: Add your own parsers and packers below. You can use the examples below to add. It's not thst hsrd dude you probably better than me in C++ this should be a piece of cake to you.
            new FosterPackerGZip().Register();
            new FosterPackerDeflate().Register();
        }

        private static void Main(string[] args)
        {
            HookParsersAndPackers();
            bool verbose = args.Contains("--verbose") || args.Contains("-v");
            bool nologo = args.Contains("--no-logo") || args.Contains("-n");
            FosterSettings.Verbose = verbose;
            if (!nologo) { Console.WriteLine("Foster Manager  Copyright (C) " + DateTime.Now.Year + "  " + Versioning.Dev + Environment.NewLine + "This program comes with ABSOLUTELY NO WARRANTY; for details type `info warranty'." + Environment.NewLine + "This is free software, and you are welcome to redistribute it under certain conditions; type `info conditions' for details." + Environment.NewLine + "This software is protected with GNU General Public License version 3.0; type `info license' or 'info copyright' for details."); }
            if (verbose) { Console.WriteLine("Foster Initialize complete with " + FosterSettings.Parsers.Length + " parser(s) and " + FosterSettings.Packers.Length + " packer(s)."); }
            string htumanName = System.Diagnostics.Process.GetCurrentProcess().ProcessName;
            if (args.Length <= 0 || args.Contains("--help") || args.Contains("help") || args.Contains("-h") || args.Contains("?") || args.Contains("/?"))
            {
                Console.WriteLine("Foster Manager ver. " + Versioning.Version + " [" + Versioning.Dev + "]");
                Console.WriteLine("--------------------");
                Console.WriteLine("");
                Console.WriteLine("USAGE:");
                Console.WriteLine("      fosterman [--verbose|-v] [--no-logo|-n] [-help|-h|help|*|/?|info|pack|unpack|delta|update|create] [OPTIONS]");
                Console.WriteLine("OPTIONS:");
                Console.WriteLine("help --help -h /? ?                                         Shows this information.");
                Console.WriteLine("--verbose -v                                                Shows more information while doing something.");
                Console.WriteLine("--no-logo -n                                                Hides the copyright information on run.");
                Console.WriteLine("info [OPTIONS]                                              Shows information about this program.");
                if (verbose)
                {
                    Console.WriteLine("[OPTIONS] = Additional arguments that can be used.");
                    Console.WriteLine("     copyright = shows the copyright notice.");
                    Console.WriteLine("     warranty = Shows the warranty notice.");
                    Console.WriteLine("     conditions = Shows the conditions notice.");
                    Console.WriteLine("     license = Shows the GNU General Public License version 3.0.");
                }
                Console.WriteLine("pack [Folder Path] [OPTIONS]                                Packs a folder into a Foster compatible package file.");
                if (verbose)
                {
                    Console.WriteLine("[Folder Path] = Path of the folder that will  be packed.");
                    Console.WriteLine("[OPTIONS] = Additional arguments that can be used.");
                    Console.WriteLine("     -o [File Path] = Similar to \"-Output\" parameter.");
                    Console.WriteLine("     -Output [File Path] = Path of the package file that will be created.");
                    Console.WriteLine("     -a [Algorithm] = Similar to \"-Algorithm\" parameter.");
                    Console.WriteLine("     -Algorithm [Algorithm] = The compression algorithm that will be used to compress the package file. Valid options are:");
                    Console.WriteLine("          none (0) = No compression will be applied.");
                    Console.WriteLine("          gzip (1) = GZip compression will be applied.");
                    Console.WriteLine("          deflate (2) = Deflate will be applied.");
                }
                Console.WriteLine("unpack [File Path] [OPTIONS]                                Packs a folder into a Foster compatible package file." + (verbose ? "(Compression algorithm will be detected automatically.)" : ""));
                if (verbose)
                {
                    Console.WriteLine("[File Path] = Path of the file that will be unpacked.");
                    Console.WriteLine("[OPTIONS] = Additional arguments that can be used.");
                    Console.WriteLine("     [Folder Path] = Folder to unpack to.");
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
                    Console.WriteLine("          none (0) = No compression will be applied.");
                    Console.WriteLine("          gzip (1) = GZip compression will be applied.");
                    Console.WriteLine("          deflate (2) = Deflate will be applied.");
                }
                Console.WriteLine("adelta [File Path] [Folder Path]                            Applies a delta package." + (verbose ? "(Compression algorithm will be detected automatically.)" : ""));
                if (verbose)
                {
                    Console.WriteLine("[Folder Path] = Path of the folder to apply delta on.");
                    Console.WriteLine("[File Path] = Path of the delta package that will be applied.");
                }
                Console.WriteLine("update [Folder] [URI] [OPTIONS]                             Updates a folder." + (verbose ? "(Compression algorithm will be detected automatically.)" : ""));
                if (verbose)
                {
                    Console.WriteLine("[Folder] = Path to the folder of the Foster that will be updated.");
                    Console.WriteLine("[URI] = Address of the Foster file on web. Can be minimalized (see https://github.com/Haltroy/HTAlt/tree/master/SHORTCUTS.md)");
                    Console.WriteLine("[OPTIONS] = Additional arguments that can be used.");
                    Console.WriteLine("     -a [Arch] = Similar to \"-Arch\".");
                    Console.WriteLine("     -Arch [Arch] = Current computer architecture (see documentation: https://github.com/Haltroy/Foster/tree/master/Foster%20Examples/Archs.md).");
                    Console.WriteLine("     --version  [Current Version] = Current version number.");
                    Console.WriteLine("     --retry [Number] = The number of retries for downloading & applying packages.");
                    Console.WriteLine("     --skip-backup = Skips the backup creation. Can be used as a boolean with \"true\", \"1\" and/or \"yes\" as enabling and \"false\", \"0\" and/or \"no\" as disabling. Not required but might save some time and space.");
                    Console.WriteLine("     --erase = Erases the work folder. Can be used as a boolean with \"true\", \"1\" and/or \"yes\" as enabling and \"false\", \"0\" and/or \"no\" as disabling. Not required but might save some time and space.");
                    Console.WriteLine("     --skip-size = Skips the size detection. Can be used as a boolean with \"true\", \"1\" and/or \"yes\" as enabling and \"false\", \"0\" and/or \"no\" as disabling. Not required but might save some time and space.");
                    Console.WriteLine("     --skip-hahes = Skips the file verification. Can be used as a boolean with \"true\", \"1\" and/or \"yes\" as enabling and \"false\", \"0\" and/or \"no\" as disabling. Not required but might save some time and space.");
                    Console.WriteLine("     --skip-backup-errors = Skips the errors occurred while creating backup. Can be used as a boolean with \"true\", \"1\" and/or \"yes\" as enabling and \"false\", \"0\" and/or \"no\" as disabling. Not required but might save some time.");
                    Console.WriteLine("     -y = Says \"Yes\" to every question.");
                }
                Console.WriteLine("install [Folder] [URI] [OPTIONS]                             Installs Foster to a folder." + (verbose ? "(Compression algorithm will be detected automatically.)" : ""));
                if (verbose)
                {
                    Console.WriteLine("[Folder] = Path to the folder of the Foster that will be updated.");
                    Console.WriteLine("[URI] = Address of the Foster file on web. Can be minimalized (see https://github.com/Haltroy/HTAlt/tree/master/SHORTCUTS.md)");
                    Console.WriteLine("[OPTIONS] = Additional arguments that can be used.");
                    Console.WriteLine("     -a [Arch] = Similar to \"-Arch\".");
                    Console.WriteLine("     -Arch [Arch] = Current computer architecture (see documentation: https://github.com/Haltroy/Foster/tree/master/Foster%20Examples/Archs.md).");
                    Console.WriteLine("     --retry [Number] = The number of retries for downloading & applying packages.");
                    Console.WriteLine("     --skip-backup = Skips the backup creation. Can be used as a boolean with \"true\", \"1\" and/or \"yes\" as enabling and \"false\", \"0\" and/or \"no\" as disabling. Not required but might save some time and space.");
                    Console.WriteLine("     --skip-size = Skips the size detection. Can be used as a boolean with \"true\", \"1\" and/or \"yes\" as enabling and \"false\", \"0\" and/or \"no\" as disabling. Not required but might save some time and space.");
                    Console.WriteLine("     --erase = Erases the work folder. Can be used as a boolean with \"true\", \"1\" and/or \"yes\" as enabling and \"false\", \"0\" and/or \"no\" as disabling. Not required but might save some time and space.");
                    Console.WriteLine("     --skip-hahes = Skips the file verification. Can be used as a boolean with \"true\", \"1\" and/or \"yes\" as enabling and \"false\", \"0\" and/or \"no\" as disabling. Not required but might save some time and space.");
                    Console.WriteLine("     --skip-backup-errors = Skips the errors occurred while creating backup. Can be used as a boolean with \"true\", \"1\" and/or \"yes\" as enabling and \"false\", \"0\" and/or \"no\" as disabling. Not required but might save some time.");
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
                    Console.WriteLine("     -a [Algorithm] = Similar to \"-Algorithm\"");
                    Console.WriteLine("     -Algorithm [Algorithm] = The compression algorithm that will be used to compress the package files. Valid options are:");
                    Console.WriteLine("          none (0) = No compression will be applied.");
                    Console.WriteLine("          gzip (1) = GZip compression will be applied.");
                    Console.WriteLine("          deflate (2) = Deflate will be applied.");
                    Console.WriteLine("          deflate (2) = Deflate will be applied.");
                    Console.WriteLine("     -p [Parser] = Similar to \"-Parser\"");
                    Console.WriteLine("     -Parser [Parser] = The parser that will be used to create Foster file. Valid options are:");
                    Console.WriteLine("          xml (0) = XML Foster file.");
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
                        case "warranty":
                            Console.WriteLine("");
                            Console.WriteLine("Please see sections 15, 16 and additionally 17 in license." + (verbose ? (Environment.NewLine + "The entire license can be found by using \"info license\" argument.") : ""));
                            Console.WriteLine("");
                            break;

                        case "conditions":
                            Console.WriteLine("");
                            Console.WriteLine("Please see sections, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13 and 14 in license." + (verbose ? (Environment.NewLine + "The entire license can be found by using \"info license\" argument.") : ""));
                            Console.WriteLine("");
                            break;

                        case "copyright":
                            Console.WriteLine("");
                            Console.WriteLine("Command-line utility for managing Fosters." + Environment.NewLine +
"Copyright (C) " + DateTime.Now.Year + " " + Versioning.Dev + Environment.NewLine
+ Environment.NewLine +
"This program is free software: you can redistribute it and/or modify" + Environment.NewLine +
"it under the terms of the GNU General Public License as published by" + Environment.NewLine +
"the Free Software Foundation, either version 3 of the License, or" + Environment.NewLine +
"(at your option) any later version." + Environment.NewLine
+ Environment.NewLine +
"This program is distributed in the hope that it will be useful," + Environment.NewLine +
"but WITHOUT ANY WARRANTY; without even the implied warranty of" + Environment.NewLine +
"MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the" + Environment.NewLine +
"GNU General Public License for more details." + Environment.NewLine
+ Environment.NewLine +
"You should have received a copy of the GNU General Public License" + Environment.NewLine +
"along with this program.  If not, see <https://www.gnu.org/licenses/>.");
                            Console.WriteLine("");
                            break;

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
                    Console.WriteLine("Foster Version: " + Foster.FosterSettings.FosterVersion);
                    string parsers = string.Empty;
                    string packers = string.Empty;
                    for (int i = 0; i < Foster.FosterSettings.Parsers.Length; i++)
                    {
                        parsers += Foster.FosterSettings.Parsers[i].ParserName + " ";
                    }
                    for (int i = 0; i < Foster.FosterSettings.Packers.Length; i++)
                    {
                        packers += Foster.FosterSettings.Packers[i].PackerName + " ";
                    }
                    Console.WriteLine("Foster Parser(s): " + parsers);
                    Console.WriteLine("Foster Packer(s): " + packers);
                    Console.WriteLine("");
                }
                return;
            }
            if (args.Contains("pack"))
            {
                int singleArgLoc = Array.IndexOf(args, "pack");
                System.Diagnostics.Stopwatch sw = new System.Diagnostics.Stopwatch();
                sw.Start();
                try
                {
                    if (args.Length < singleArgLoc + 1) { Console.WriteLine(" [E] Not enough information."); return; }
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
                        packFile = System.IO.Path.Combine(pfInfo.Parent.FullName, System.IO.Path.DirectorySeparatorChar + pfInfo.Name + ".hup");
                    }
                    FosterPackerBase algorithm = null;
                    if (args.Contains("-a", StringComparer.InvariantCultureIgnoreCase))
                    {
                        int algorithmIndex = Array.IndexOf(args, "-a");
                        if (args.Length >= algorithmIndex + 1)
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
                    if (args.Contains("-Algorithm", StringComparer.InvariantCultureIgnoreCase) && algorithm is null)
                    {
                        int algorithmIndex = Array.IndexOf(args, "-Algorithm");
                        if (args.Length >= algorithmIndex + 1)
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
                    if (algorithm == null) { algorithm = FosterSettings.Packers[0]; }
                    if (verbose) { Console.WriteLine("Packing \"" + packFolder + " to \"" + packFile + "\" with \"" + algorithm.PackerName + "\" algorithm..."); }
                    Packer.CompressDirectory(packFolder, packFile, algorithm);
                    if (verbose) { Console.WriteLine("Package packed successfully."); }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error while packing, exception caught:" + ex.ToString());
                }
                sw.Stop();
                if (verbose) { Console.WriteLine(htumanName + " " + string.Join(' ', args) + " in " + sw.ElapsedMilliseconds + " ms."); }
                return;
            }
            if (args.Contains("unpack"))
            {
                int singleArgLoc = Array.IndexOf(args, "unpack");
                System.Diagnostics.Stopwatch sw = new System.Diagnostics.Stopwatch();
                sw.Start();
                try
                {
                    if (args.Length < singleArgLoc + 1) { Console.WriteLine(" [E] Not enough information."); return; }
                    string unpackFile = System.IO.Path.GetFullPath(args[singleArgLoc + 1]);
                    System.IO.FileInfo ufInfo = new System.IO.FileInfo(unpackFile);
                    string unpackFolder = (args.Length <= singleArgLoc + 2) ? ufInfo.DirectoryName + System.IO.Path.DirectorySeparatorChar + System.IO.Path.GetFileNameWithoutExtension(unpackFile) + System.IO.Path.DirectorySeparatorChar : System.IO.Path.GetFullPath(args[singleArgLoc + 2]);
                    if (verbose) { Console.WriteLine("Unpacking \"" + unpackFile + " to \"" + unpackFolder + "\"..."); }
                    var status = Packer.DecompressToDirectory(unpackFile, unpackFolder);
                    if (verbose) { Console.WriteLine(status ? "Package unpacked successfully." : "Unknown error on unpacking detected."); }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error while unpacking, exception caught:" + ex.ToString());
                }
                sw.Stop();
                if (verbose) { Console.WriteLine(htumanName + " " + string.Join(' ', args) + " in " + sw.ElapsedMilliseconds + " ms."); }
                return;
            }
            if (args.Contains("delta"))
            {
                int singleArgLoc = Array.IndexOf(args, "delta");
                System.Diagnostics.Stopwatch sw = new System.Diagnostics.Stopwatch();
                sw.Start();
                try
                {
                    if (args.Length < singleArgLoc + 2) { Console.WriteLine(" [E] Not enough information."); return; }
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
                        dFile = System.IO.Path.Combine(cfInfo.Parent.FullName, System.IO.Path.DirectorySeparatorChar + cfInfo.Name + ".dhup");
                    }
                    if (verbose) { Console.WriteLine("Creating delta..."); }
                    FosterPackerBase algorithm = FosterSettings.Packers[0];
                    if (args.Contains("-a"))
                    {
                        int algorithmIndex = Array.IndexOf(args, "-a");
                        if (args.Length >= algorithmIndex + 1)
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
                                case "3":
                                    algorithm = FosterSettings.Packers[3];
                                    break;

                                default:
                                    throw new Exception("Unknown packer type \"" + algName.ToLowerEnglish() + "\".");
                            }
                        }
                    }
                    if (args.Contains("-Algorithm"))
                    {
                        int algorithmIndex = Array.IndexOf(args, "-Algorithm");
                        if (args.Length >= algorithmIndex + 1)
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
                    Packer.CreateDelta(System.IO.Path.GetFullPath(bFolder), System.IO.Path.GetFullPath(cFolder), System.IO.Path.GetFullPath(dFile), algorithm);
                    if (verbose) { Console.WriteLine("Creating delta successful."); }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error while creating delta, exception caught:" + ex.ToString());
                }
                sw.Stop();
                if (verbose) { Console.WriteLine(htumanName + " " + string.Join(' ', args) + " in " + sw.ElapsedMilliseconds + " ms."); }
                return;
            }
            if (args.Contains("adelta"))
            {
                int singleArgLoc = Array.IndexOf(args, "adelta");
                System.Diagnostics.Stopwatch sw = new System.Diagnostics.Stopwatch();
                sw.Start();
                try
                {
                    if (args.Length < singleArgLoc + 3) { Console.WriteLine(" [E] Not enough information."); return; }
                    string cFolder = System.IO.Path.GetFullPath(args[singleArgLoc + 1]);
                    string dFile = System.IO.Path.GetFullPath(args[singleArgLoc + 2]);
                    Packer.ApplyDelta(System.IO.Path.GetFullPath(cFolder), new System.IO.FileStream(System.IO.Path.GetFullPath(dFile), System.IO.FileMode.Open, System.IO.FileAccess.Read, System.IO.FileShare.ReadWrite));
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error while applying delta, exception caught:" + ex.ToString());
                }
                sw.Stop();
                if (verbose) { Console.WriteLine(htumanName + " " + string.Join(' ', args) + " in " + sw.ElapsedMilliseconds + " ms."); }
                return;
            }
            if (args.Contains("create"))
            {
                int singleArgLoc = Array.IndexOf(args, "create");
                System.Diagnostics.Stopwatch sw = new System.Diagnostics.Stopwatch();
                sw.Start();
                try
                {
                    if (args.Length < singleArgLoc + 2) { Console.WriteLine(" [E] Not enough information."); return; }
                    string sFolder = System.IO.Path.GetFullPath(args[singleArgLoc + 1]);
                    string oFolder = System.IO.Path.GetFullPath(args[singleArgLoc + 2]);
                    string htuFile = string.Empty;
                    if (args.Contains("-s")) { htuFile = args[Array.IndexOf(args, "-s") + 1]; }
                    if (args.Contains("-Source")) { htuFile = args[Array.IndexOf(args, "-Source") + 1]; }
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
                    if (string.IsNullOrWhiteSpace(htuFile))
                    {
                        if (verbose) { Console.WriteLine("Searching for a Skeleton Foster file..."); }
                        string[] allFiles = System.IO.Directory.GetFiles(sFolder, "*", System.IO.SearchOption.AllDirectories);
                        for (int i = 0; i < allFiles.Length; i++)
                        {
                            if (Tools.ToLowerEnglish(allFiles[i]).EndsWith(".foster"))
                            {
                                htuFile = allFiles[i];
                                if (verbose) { Console.WriteLine("Skeleton Foster file found at  \"" + htuFile + "\"."); }
                                break;
                            }
                        }
                        if (string.IsNullOrWhiteSpace(htuFile))
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
                    Packer.CreatePackages(sFolder, oFolder, algorithm, parser, htuFile, skipEmptyDirs, skipHashes);
                    if (verbose) { Console.WriteLine("Creating packages successful."); }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error while creating, exception caught:" + ex.ToString());
                }
                sw.Stop();
                if (verbose) { Console.WriteLine(htumanName + " " + string.Join(' ', args) + " in " + sw.ElapsedMilliseconds + " ms."); }
                return;
            }
            if (args.Contains("update"))
            {
                int singleArgLoc = Array.IndexOf(args, "update");
                System.Diagnostics.Stopwatch sw = new System.Diagnostics.Stopwatch();
                sw.Start();
                try
                {
                    if (args.Length < singleArgLoc + 3) { Console.WriteLine(" [E] Not enough information."); return; }
                    string curFolder = System.IO.Path.GetFullPath(args[singleArgLoc + 1]);
                    string verFile = System.IO.Path.Combine(curFolder, System.IO.Path.DirectorySeparatorChar + ".htuman");
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
                    var htu = new Foster.Foster(new System.IO.FileInfo(curFolder).Name, uri, curFolder, cver, arch)
                    {
                        SkipFileSizeInfo = true
                    };
                    bool noconfirm = false;
                    if (args.Length >= singleArgLoc + 3)
                    {
                        if (args.Contains("--skip-backup-errors"))
                        {
                            htu.SkipBackupError = true;
                            int shLoc = Array.IndexOf(args, "--skip-backup-errors");
                            if ((args.Length - 1) > shLoc)
                            {
                                string v = Tools.ToLowerEnglish(args[shLoc + 1]);
                                htu.SkipBackupError = v == "true" || v == "1" || v == "yes";
                                htu.SkipBackupError = !(v == "false" || v == "0" || v == "no");
                            }
                        }
                        if (args.Contains("--version"))
                        {
                            int shLoc = Array.IndexOf(args, "--version");
                            if ((args.Length - 1) > shLoc)
                            {
                                string v = Tools.ToLowerEnglish(args[shLoc + 1]);
                                htu.CurrentVer = int.Parse(v);
                            }
                            else
                            {
                                throw new ArgumentNullException("Version ID");
                            }
                        }
                        if (args.Contains("--skip-backup"))
                        {
                            htu.SkipBackup = true;
                            int sedLoc = Array.IndexOf(args, "--skip--backup");
                            if ((args.Length - 1) > sedLoc)
                            {
                                string v = Tools.ToLowerEnglish(args[sedLoc + 1]);
                                htu.SkipBackup = v == "true" || v == "1" || v == "yes";
                                htu.SkipBackup = !(v == "false" || v == "0" || v == "no");
                            }
                        }
                        if (args.Contains("--erase"))
                        {
                            htu.SkipBackup = true;
                            int sedLoc = Array.IndexOf(args, "--erase");
                            if ((args.Length - 1) > sedLoc)
                            {
                                string v = Tools.ToLowerEnglish(args[sedLoc + 1]);
                                htu.EraseFolder = v == "true" || v == "1" || v == "yes";
                                htu.EraseFolder = !(v == "false" || v == "0" || v == "no");
                            }
                        }
                        if (args.Contains("--skip-hashes"))
                        {
                            htu.SkipHashes = true;
                            int sedLoc = Array.IndexOf(args, "--skip--hashes");
                            if ((args.Length - 1) > sedLoc)
                            {
                                string v = Tools.ToLowerEnglish(args[sedLoc + 1]);
                                htu.SkipHashes = v == "true" || v == "1" || v == "yes";
                                htu.SkipHashes = !(v == "false" || v == "0" || v == "no");
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
                                htu.RetryCount = int.Parse(Tools.ToLowerEnglish(args[shLoc + 1]));
                            }
                            else
                            {
                                throw new ArgumentNullException("Retry Count");
                            }
                        }
                    }
                    htu.OnLogEntry += new Foster.Foster.OnLogEntryDelegate((sender, e) =>
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
                                    Console.WriteLine(" [E] " + e.LogEntry);
                                    break;

                                case LogLevel.Critical:
                                    Console.WriteLine(" [C] " + e.LogEntry);
                                    break;
                            }
                        }
                    });
                    Foster_Download[] downloadList = htu.GetDownloads();
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
                            Console.WriteLine("Cancelled.");
                            return;
                        }
                    }
                    htu.Update(true);
                    if (htu.LatestException != null)
                    {
                        throw htu.LatestException;
                    }
                    else
                    {
                        Tools.WriteFile(verFile, "" + htu.CurrentVersion.ID, System.Text.Encoding.Unicode);
                        Console.WriteLine("Update finished with no errors.");
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error while updating, exception caught:" + ex.ToString());
                }
                sw.Stop();
                if (verbose) { Console.WriteLine(htumanName + " " + string.Join(' ', args) + " in " + sw.ElapsedMilliseconds + " ms."); }
                return;
            }
            if (args.Contains("install"))
            {
                int singleArgLoc = Array.IndexOf(args, "install");
                System.Diagnostics.Stopwatch sw = new System.Diagnostics.Stopwatch();
                sw.Start();
                try
                {
                    if (args.Length < singleArgLoc + 3) { Console.WriteLine(" [E] Not enough information."); return; }
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
                    var htu = new Foster.Foster(new System.IO.FileInfo(curFolder).Name, uri, curFolder, 0, arch)
                    {
                        SkipFileSizeInfo = true
                    };
                    bool noconfirm = false;
                    if (args.Length >= singleArgLoc + 3)
                    {
                        if (args.Contains("--skip-backup-errors"))
                        {
                            htu.SkipBackupError = true;
                            int shLoc = Array.IndexOf(args, "--skip-backup-errors");
                            if ((args.Length - 1) > shLoc)
                            {
                                string v = Tools.ToLowerEnglish(args[shLoc + 1]);
                                htu.SkipBackupError = v == "true" || v == "1" || v == "yes";
                                htu.SkipBackupError = !(v == "false" || v == "0" || v == "no");
                            }
                        }
                        if (args.Contains("--version"))
                        {
                            int shLoc = Array.IndexOf(args, "--version");
                            if ((args.Length - 1) > shLoc)
                            {
                                string v = Tools.ToLowerEnglish(args[shLoc + 1]);
                                htu.CurrentVer = int.Parse(v);
                            }
                            else
                            {
                                throw new ArgumentNullException("Version ID");
                            }
                        }
                        if (args.Contains("--skip-hashes"))
                        {
                            htu.SkipHashes = true;
                            int sedLoc = Array.IndexOf(args, "--skip--hashes");
                            if ((args.Length - 1) > sedLoc)
                            {
                                string v = Tools.ToLowerEnglish(args[sedLoc + 1]);
                                htu.SkipHashes = v == "true" || v == "1" || v == "yes";
                                htu.SkipHashes = !(v == "false" || v == "0" || v == "no");
                            }
                        }
                        if (args.Contains("--erase"))
                        {
                            htu.SkipBackup = true;
                            int sedLoc = Array.IndexOf(args, "--erase");
                            if ((args.Length - 1) > sedLoc)
                            {
                                string v = Tools.ToLowerEnglish(args[sedLoc + 1]);
                                htu.EraseFolder = v == "true" || v == "1" || v == "yes";
                                htu.EraseFolder = !(v == "false" || v == "0" || v == "no");
                            }
                        }
                        if (args.Contains("--skip-backup"))
                        {
                            htu.SkipBackup = true;
                            int sedLoc = Array.IndexOf(args, "--skip--backup");
                            if ((args.Length - 1) > sedLoc)
                            {
                                string v = Tools.ToLowerEnglish(args[sedLoc + 1]);
                                htu.SkipBackup = v == "true" || v == "1" || v == "yes";
                                htu.SkipBackup = !(v == "false" || v == "0" || v == "no");
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
                                htu.RetryCount = int.Parse(Tools.ToLowerEnglish(args[shLoc + 1]));
                            }
                            else
                            {
                                throw new ArgumentNullException("Retry Count");
                            }
                        }
                    }
                    htu.OnLogEntry += new Foster.Foster.OnLogEntryDelegate((sender, e) =>
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
                                    Console.WriteLine(" [E] " + e.LogEntry);
                                    break;

                                case LogLevel.Critical:
                                    Console.WriteLine(" [C] " + e.LogEntry);
                                    break;
                            }
                        }
                    });
                    htu.LoadFromUrl();
                    Foster_Download[] downloadList = htu.GetDownloads(null, htu.LatestVersion);
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
                            Console.WriteLine("Cancelled.");
                            return;
                        }
                    }
                    htu.Update(true);
                    if (htu.LatestException != null)
                    {
                        throw htu.LatestException;
                    }
                    else
                    {
                        Tools.WriteFile(verFile, "" + htu.CurrentVersion.ID, System.Text.Encoding.Unicode);
                        Console.WriteLine("Update finished with no errors.");
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error while installing, exception caught:" + ex.ToString());
                }
                sw.Stop();
                if (verbose) { Console.WriteLine(htumanName + " " + string.Join(' ', args) + " in " + sw.ElapsedMilliseconds + " ms."); }
                return;
            }
            Console.WriteLine(" [E] Unknown command, please use help command to see all supported commands.");
        }
    }
}