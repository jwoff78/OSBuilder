using System.ComponentModel;
using Newtonsoft.Json;
using System.Diagnostics;
using static OSCompiler.CompilerJson;
using System.Runtime.CompilerServices; //this is probably the easier way

namespace OSCompiler
{
    public class OSBuilder
    {
        //TODO: use Tools/get-tools read line by line and download all the files, unzip them, then call them with config as part of a (setup) function
        string compilerJsonFilePath = "JSON/Compiler.json";
        string compilerJsonData = "";
        Dictionary<string, string> compilerJsonContent;

        bool isDebug = false;

        //private readonly string GCC = string.Empty;
        private readonly string GPP = string.Empty;
        //private readonly string CC = string.Empty;
        private readonly string AS = string.Empty;
        private readonly string FBC = string.Empty;
        private readonly string QEMU = string.Empty;
        private readonly string NASM = string.Empty;

        private readonly string CFLAGS = string.Empty;
        private readonly string LDFLAGS = string.Empty;

        private readonly string SRC_DIR = string.Empty;
        private readonly string BUILD_DIR = string.Empty;

        //*/

        //TODO: json directory map
        static string? BUILD_DIR_CPP;
        static string? BUILD_DIR_HEADERS;
        static string? BUILD_DIR_ASM;
        static string? BUILD_DIR_SFILE;

        //static ConfigData? configData;

        public OSBuilder(ConfigData? configData)
        {
            // Read JSON file contents
            compilerJsonData = File.ReadAllText(compilerJsonFilePath);

            // Deserialize JSON into ConfigData object
            configData = JsonConvert.DeserializeObject<ConfigData>(compilerJsonData);

            #region config data null check

            // Helper method to perform null checks
            void EnsureNotNull(object? value, string name)
            {
                if (value is null)
                    throw new NullReferenceException($"{name} is null!");
            }

            // Setup
            EnsureNotNull(configData, nameof(configData));
            EnsureNotNull(configData.CompilerPaths, nameof(configData.CompilerPaths));
            EnsureNotNull(configData.CompilerFlags, nameof(configData.CompilerFlags));
            EnsureNotNull(configData.Directories, nameof(configData.Directories));

            // Compiler paths checks
            EnsureNotNull(configData.CompilerPaths.GPP, $"{nameof(configData.CompilerPaths)}.{nameof(configData.CompilerPaths.GPP)}");
            EnsureNotNull(configData.CompilerPaths.FBC, $"{nameof(configData.CompilerPaths)}.{nameof(configData.CompilerPaths.FBC)}");
            EnsureNotNull(configData.CompilerPaths.AS, $"{nameof(configData.CompilerPaths)}.{nameof(configData.CompilerPaths.AS)}");
            EnsureNotNull(configData.CompilerPaths.QEMU, $"{nameof(configData.CompilerPaths)}.{nameof(configData.CompilerPaths.QEMU)}");
            EnsureNotNull(configData.CompilerPaths.NASM, $"{nameof(configData.CompilerPaths)}.{nameof(configData.CompilerPaths.NASM)}");

            // Compiler flags checks
            EnsureNotNull(configData.CompilerFlags.CFLAGS, $"{nameof(configData.CompilerFlags)}.{nameof(configData.CompilerFlags.CFLAGS)}");
            EnsureNotNull(configData.CompilerFlags.LDFLAGS, $"{nameof(configData.CompilerFlags)}.{nameof(configData.CompilerFlags.LDFLAGS)}");

            // Directory checks
            EnsureNotNull(configData.Directories.SRC_DIR, $"{nameof(configData.Directories)}.{nameof(configData.Directories.SRC_DIR)}");
            EnsureNotNull(configData.Directories.BUILD_DIR, $"{nameof(configData.Directories)}.{nameof(configData.Directories.BUILD_DIR)}");

            #endregion

            // Create a dictionary to store key-value pairs
            compilerJsonContent = new Dictionary<string, string>
            {
                // Add compiler paths to the dictionary
                { "GPP", configData.CompilerPaths.GPP },
                { "AS", configData.CompilerPaths.AS },
                { "FBC", configData.CompilerPaths.FBC },
                { "QEMU", configData.CompilerPaths.QEMU },
                { "NASM", configData.CompilerPaths.NASM },

                // Add compiler flags to the dictionary
                { "CFLAGS", configData.CompilerFlags.CFLAGS },
                { "LDFLAGS", configData.CompilerFlags.LDFLAGS },

                // Add directories to the dictionary
                { "SRC_DIR", configData.Directories.SRC_DIR },
                { "BUILD_DIR", configData.Directories.BUILD_DIR }
            };

            if (isDebug)
            {
                Console.WriteLine(StaticUtil.GenerateLineWithText(string.Empty));

                // Now you can use the dictionary with keys as variables and values as their corresponding data
                foreach (var kvp in compilerJsonContent)
                {
                    Console.WriteLine($"{kvp.Key}: {kvp.Value}");
                    // Use kvp.Key as variable name and kvp.Value as its corresponding value in your code
                }
                Console.WriteLine(StaticUtil.GenerateLineWithText(string.Empty));
            }
        }

        private List<string> cppFiles = new List<string>();
        private List<string> headerFiles = new List<string>();
        private List<string> asmFiles = new List<string>();
        private List<string> sFiles = new List<string>();

        private List<string> freeBasicFiles = new List<string>();

        //TODO: finish this
        public List<string> AssembleCompilerObjects(List<string> objects, List<string> files)
        {
            for (int i = 0; i < objects.Count; i++)
            {
                //string arg = asmFileBootObjects[i];
                string fileTok = (i < files.Count) ? files[i] : string.Empty;
            }
            return [];
        }

        /*
         * for (int i = 0; i < asmFileBootObjects.Count; i++)
            {
                string arg = asmFileBootObjects[i];
                string asmFile = (i < asmFiles.Count) ? asmFiles[i] : string.Empty;

                if (!string.IsNullOrEmpty(asmFile))
                {
                    string tmp = $"-f elf32 {Path.Combine(compilerJsonContent[nameof(SRC_DIR)], asmFile)} -o {arg}";
                    asmFileBootArgs.Add(tmp);
                }
                else
                {
                    // Handle the case where asmFiles doesn't have a corresponding file for each boot object
                    // You can choose to skip, log an error, or handle it according to your requirements.
                    Console.WriteLine($"No corresponding asm file for boot object: {arg}");
                }
            }
         *
         * 
         */

        public void CompileOS(string[] cFiles)
        {
            //Emit function name for debugging
            if (isDebug)
                Console.WriteLine($"{nameof(CompileOS)}");

            Directory.CreateDirectory(compilerJsonContent[nameof(BUILD_DIR)]);

            foreach (var file in cFiles)
            {
                if (file.EndsWith(".cpp"))
                {
                    cppFiles.Add(file); //kernel.cpp
                    var directory = Path.GetDirectoryName(file);

                    if (directory is null)
                        throw new NullReferenceException($"{file} is not a directory");

                    var headerDir = Path.Combine(directory, "headers"); //replace with BUILD_DIR_HEADERS
                    var cppDir = Path.Combine(directory, "cpp"); //replace with BUILD_DIR_CPP
                    var asmDir = Path.Combine(directory, "asm"); //replace with BUILD_DIR_ASM
                    var sFileDir = Path.Combine(directory, "sFiles"); //replace with BUILD_DIR_SFILE

                    var freeBasicDir = Path.Combine(directory, "freebasic"); //replace with BUILD_DIR_SFILE

                    //TODO: optional file structure init
                    bool optionalAddins = false;
                    //Directory.EnumerateFiles(headerDir, "*. ", SearchOption.AllDirectories);

                    //TODO: add suport for .hpp
                    //.h
                    if (Directory.Exists(headerDir))
                    {
                        if (optionalAddins)
                            cppFiles.AddRange(Directory.GetFiles(cppDir, "*.cpp"));
                        headerFiles.AddRange(Directory.GetFiles(headerDir, "*.h"));
                    }

                    //TODO: add support for .c
                    //.cpp
                    if (Directory.Exists(cppDir))
                    {
                        if (optionalAddins)
                            headerFiles.AddRange(Directory.GetFiles(headerDir, "*.h"));
                        cppFiles.AddRange(Directory.GetFiles(cppDir, "*.cpp"));
                    }

                    //if (Directory.Exists(freeBasicDir))

                    //.asm
                    if (Directory.Exists(asmDir))
                    {
                        if ((Directory.GetFiles(asmDir, "*.asm").Length > 0))
                            asmFiles.AddRange(Directory.GetFiles(asmDir, "*.asm"));
                    }

                    //.s
                    if (Directory.Exists(sFileDir))
                    {
                        if ((Directory.GetFiles(sFileDir, "*.s").Length > 0))
                            sFiles.AddRange(Directory.GetFiles(sFileDir, "*.s"));
                    }

                    //.bas
                    if (Directory.Exists(freeBasicDir))
                    {
                        if ((Directory.GetFiles(freeBasicDir, "*.bas").Length > 0))
                            freeBasicFiles.AddRange(Directory.GetFiles(freeBasicDir, "*.bas"));
                    }
                    

                    //TODO: add support for freebasic
                    //then maybe support Rust & Ada
                    //then maybe pascal and fortran


                }
            }
            //TODO: Later: check to see if what we are compiling actually contains anything
            //TODO: Later: Change these objects into a dictionary because likely you'll want to have more .s/.S files

            var fileCollections = new List<(IEnumerable<string> Files, string Label)>
            {
                (sFiles, "sFiles"),
                (asmFiles, "asmFiles"),
                (cppFiles, "cppFiles"),
                (freeBasicFiles, "freeBasicFiles")
            };

            foreach (var (Files, Label) in fileCollections)
            {
                foreach (string file in Files)
                {
                    StaticUtil.PrintColoredText($"{Label} -> {Path.GetFileName(file)}");
                }

                // Optionally print a separator line between file groups if not at the end of the list
                if (Label != fileCollections.Last().Label) //If it's not the last index
                {
                    Console.WriteLine(StaticUtil.GenerateLineWithText(Label.ToUpper() + " FILES"));
                }
            }

            /*
            Console.WriteLine(StaticUtil.GenerateLineWithText("S FILES"));

            foreach (string file in sFiles)
            {
                StaticUtil.PrintColoredText($"sFiles -> {Path.GetFileName(file)}");
            }

            Console.WriteLine(StaticUtil.GenerateLineWithText("ASM FILES"));

            foreach (string file in asmFiles)
            {
                StaticUtil.PrintColoredText($"asmFiles -> {Path.GetFileName(file)}");
            }

            Console.WriteLine(StaticUtil.GenerateLineWithText("CPP FILES"));

            foreach (string file in cppFiles)
            {
                StaticUtil.PrintColoredText($"cppFiles -> {Path.GetFileName(file)}");
            }

            Console.WriteLine(StaticUtil.GenerateLineWithText("BAS FILES"));

            foreach (string file in freeBasicFiles)
            {
                StaticUtil.PrintColoredText($"freeBasicFiles -> {Path.GetFileName(file)}");
            }

            Console.WriteLine(StaticUtil.GenerateLineWithText(string.Empty));

            */

            // Define paths for the output object files and associate them with file extensions
            var fileCollections = new Dictionary<string, List<string>>
                    {
                        {".asm", new List<string>()},
                        {".s", new List<string>()},
                        {".cpp", new List<string>()},
                        {".bas", new List<string>()}
                    };

            // Dictionary to map extensions to file lists
            var filesByExtension = new Dictionary<string, IEnumerable<string>>
                    {
                        {".asm", asmFiles},
                        {".s", sFiles},
                        {".cpp", cppFiles},
                        {".bas", freeBasicFiles}
                    };

            // Process each file type
            foreach (var kvp in filesByExtension)
            {
                foreach (string fkvpv in kvp.Value) //file kvp value
                {
                    string trimFn = Path.GetFileName(fkvpv).Replace(kvp.Key, string.Empty);
                    fileCollections[kvp.Key].Add(Path.Combine(compilerJsonContent[nameof(BUILD_DIR)], $"{trimFn}.o"));
                }
            }

            // Now fileCollections contains all the processed file paths organized by type

            /*
            // Define paths for the output object files
            List<string> asmFileBootObjects = new List<string>();
            List<string> sFileBootObjects = new List<string>();
            List<string> cppFileBootObjects = new List<string>();
            List<string> freeBasicFileBootObjects = new List<string>();

            //ASM FILES

            foreach (string file in asmFiles)
            {
                string trimFn = Path.GetFileName(file).Replace(".asm", string.Empty);
                asmFileBootObjects.Add(Path.Combine(compilerJsonContent[nameof(BUILD_DIR)], $"{trimFn}.o"));

            }

            //S FILES

            foreach (string file in sFiles)
            {
                string trimFn = Path.GetFileName(file).Replace(".s", string.Empty);
                sFileBootObjects.Add(Path.Combine(compilerJsonContent[nameof(BUILD_DIR)], $"{trimFn}.o"));
            }

            //CPP FILES

            foreach (string file in cppFiles)
            {
                string trimFn = Path.GetFileName(file).Replace(".cpp", string.Empty);
                cppFileBootObjects.Add(Path.Combine(compilerJsonContent[nameof(BUILD_DIR)], $"{trimFn}.o"));
            }

            //BAS FILES

            foreach (string file in freeBasicFiles)
            {
                string trimFn = Path.GetFileName(file).Replace(".bas", string.Empty);
                freeBasicFileBootObjects.Add(Path.Combine(compilerJsonContent[nameof(BUILD_DIR)], $"{trimFn}.o"));
            }

            */

            List<string> sFileBootArgs = new List<string>();
            List<string> asmFileBootArgs = new List<string>();
            List<string> cppFileBootArgs = new List<string>();
            List<string> freeBasicFileBootArgs = new List<string>();

            //ASM FILES

            for (int i = 0; i < asmFileBootObjects.Count; i++)
            {
                string arg = asmFileBootObjects[i];
                string asmFile = (i < asmFiles.Count) ? asmFiles[i] : string.Empty;

                if (!string.IsNullOrEmpty(asmFile))
                {
                    string tmp = $"-f elf32 {Path.Combine(compilerJsonContent[nameof(SRC_DIR)], asmFile)} -o {arg}";
                    asmFileBootArgs.Add(tmp);
                }
                else
                {
                    // Handle the case where asmFiles doesn't have a corresponding file for each boot object
                    // You can choose to skip, log an error, or handle it according to your requirements.
                    Console.WriteLine($"No corresponding asm file for boot object: {arg}");
                }
            }

            //S FILES

            for (int i = 0; i < sFileBootObjects.Count; i++)
            {
                string arg = sFileBootObjects[i];
                string sFile = (i < sFiles.Count) ? sFiles[i] : string.Empty;

                if (!string.IsNullOrEmpty(sFile))
                {
                    string tmp = $"--32 -c {Path.Combine(compilerJsonContent[nameof(SRC_DIR)], sFile)} -o {arg}";
                    sFileBootArgs.Add(tmp);
                }
                else
                {
                    // Handle the case where asmFiles doesn't have a corresponding file for each boot object
                    // You can choose to skip, log an error, or handle it according to your requirements.
                    Console.WriteLine($"No corresponding s file for boot object: {arg}");
                }
            }

            //CPP FILES

            for (int i = 0; i < cppFileBootObjects.Count; i++)
            {
                string arg = cppFileBootObjects[i];
                string cppFile = (i < cppFiles.Count) ? cppFiles[i] : string.Empty;

                if (!string.IsNullOrEmpty(cppFile))
                {
                    string tmp = $"-c {Path.Combine(compilerJsonContent[nameof(SRC_DIR)], cppFile)} -o {arg} {compilerJsonContent[nameof(CFLAGS)]}";
                    cppFileBootArgs.Add(tmp);
                }
                else
                {
                    // Handle the case where asmFiles doesn't have a corresponding file for each boot object
                    // You can choose to skip, log an error, or handle it according to your requirements.
                    Console.WriteLine($"No corresponding cpp file for boot object: {arg}");
                }
            }

            //BAS FILES

            for (int i = 0; i < freeBasicFileBootObjects.Count; i++)
            {
                string arg = freeBasicFileBootObjects[i];
                string basFile = (i < freeBasicFiles.Count) ? freeBasicFiles[i] : string.Empty;

                if (!string.IsNullOrEmpty(basFile))
                {
                    string tmp = $"-c {Path.Combine(compilerJsonContent[nameof(SRC_DIR)], basFile)} -o {arg}";
                    freeBasicFileBootArgs.Add(tmp);
                }
                else
                {
                    // Handle the case where asmFiles doesn't have a corresponding file for each boot object
                    // You can choose to skip, log an error, or handle it according to your requirements.
                    Console.WriteLine($"No corresponding bas file for boot object: {arg}");
                }
            }

            Console.WriteLine();

            //ASM FILE
            foreach (string arg in asmFileBootArgs)
            {
                StaticUtil.PrintColoredText($"asmFileBootArgs -> {arg} \n");
            }

            //S FILE
            foreach (string arg in sFileBootArgs)
            {
                StaticUtil.PrintColoredText($"sFileBootArgs -> {arg}\n");
            }

            //CPP FILE
            foreach (string arg in cppFileBootArgs)
            {
                StaticUtil.PrintColoredText($"cppFileBootArgs -> {arg}\n");
            }

            //BAS FILE
            foreach (string arg in freeBasicFileBootArgs)
            {
                StaticUtil.PrintColoredText($"freeBasicFileBootArgs -> {arg}\n");
            }

            Console.WriteLine(StaticUtil.GenerateLineWithText(string.Empty));

            // Define arguments and process information for compiling the kernel file using the C++ compiler (CC)
            List<ProcessStartInfo> asmProcessInfo = new List<ProcessStartInfo>();
            List<ProcessStartInfo> sProcessInfo = new List<ProcessStartInfo>();

            ConsoleColor gReset = Console.ForegroundColor;
            Console.ForegroundColor = ConsoleColor.Cyan;

            //ASM FILE

            foreach (string process in asmFileBootArgs)
            {
                if (asmFileBootArgs.Count > 1)
                    StaticUtil.SafeWriteLine($"PROCESS ASM: {process}\n");
                else
                    StaticUtil.SafeWriteLine($"PROCESS ASM: {process}");

                CompileNasm(process);
            }

            //S FILE

            foreach (string process in sFileBootArgs)
            {
                if (sFileBootArgs.Count > 1)
                    StaticUtil.SafeWriteLine($"PROCESS S: {process}\n");
                else
                    StaticUtil.SafeWriteLine($"PROCESS S: {process}");

                CompileSFile(process);

            }

            //CPP FILE

            foreach (string process in cppFileBootArgs)
            {
                if (cppFileBootArgs.Count > 1)
                    StaticUtil.SafeWriteLine($"PROCESS CPP: {process}\n");
                else
                    StaticUtil.SafeWriteLine($"PROCESS CPP: {process}");

                CompileCppFile(process);

            }

            //BAS FILE

            foreach (string process in freeBasicFileBootArgs)
            {
                if (freeBasicFileBootArgs.Count > 1)
                    StaticUtil.SafeWriteLine($"PROCESS BAS: {process}\n");
                else
                    StaticUtil.SafeWriteLine($"PROCESS BAS: {process}");

                CompileBasFile(process);

            }
            Console.ForegroundColor = gReset;

            Console.WriteLine(StaticUtil.GenerateLineWithText(string.Empty));

            string asmBootObjects = StaticUtil.ConcatenateWithoutExtraSpace(asmFileBootObjects.ToArray());
            string sBootObjects = StaticUtil.ConcatenateWithoutExtraSpace(sFileBootObjects.ToArray());
            string cppExtraObjects = StaticUtil.ConcatenateWithoutExtraSpace(cppFileBootObjects.ToArray());
            string basExtraObjects = StaticUtil.ConcatenateWithoutExtraSpace(freeBasicFileBootObjects.ToArray());
            string bootS = Path.Combine(compilerJsonContent[nameof(SRC_DIR)], "boot.s");
            string bootArgs = $"-c {Path.Combine(compilerJsonContent[nameof(SRC_DIR)], bootS)} -o boot.o";
            //Console.WriteLine("bootArgs -> " + bootArgs);
            //$"-c {Path.Combine(compilerJsonContent[nameof(SRC_DIR)], sFile)} -o {arg}";
            //CompileSFile(bootArgs);
            // Define arguments and process information for linking the boot and kernel object files into the final binary                                            //boot.o
            var linkArgs = $"-T {Path.Combine(compilerJsonContent[nameof(SRC_DIR)], "linker.ld")} -o {Path.Combine(compilerJsonContent[nameof(BUILD_DIR)], "myos.bin")} {sBootObjects} {asmBootObjects} {cppExtraObjects} {basExtraObjects} {compilerJsonContent[nameof(LDFLAGS)]}";

            if (true) //ifDebug
            {
                ConsoleColor reset = Console.ForegroundColor;
                Console.ForegroundColor = ConsoleColor.Yellow;
                StaticUtil.SafeWriteLine(linkArgs);
                Console.ForegroundColor = reset;
                Console.WriteLine(StaticUtil.GenerateLineWithText(string.Empty));
            }

            var linkProcessInfo = new ProcessStartInfo(compilerJsonContent[nameof(GPP)], linkArgs);

            // Set UseShellExecute to false to run the linking process directly without shell execution
            linkProcessInfo.UseShellExecute = false;

            // Start the linking process to create the final binary file and wait for its completion
            Process? compile = Process.Start(linkProcessInfo);
            compile?.WaitForExit();

            //if (compile is { HasExited: true })
            // Check if the process is complete and successful
            if (compile != null && compile.HasExited)
            {
                if (compile.ExitCode == 0)
                {
                    ConsoleColor reset = Console.ForegroundColor;
                    Console.ForegroundColor = ConsoleColor.Magenta;
                    Console.WriteLine("Compiled `myos.bin` successfully!");
                    Console.ForegroundColor = reset;
                }
                else
                {
                    StaticUtil.PrintColoredText($"Error: Compilation failed with exit code {compile.ExitCode}", firstPartColor: ConsoleColor.Red, seperator: ":");
                }
            }
            else
            {
                StaticUtil.PrintColoredText("Error: Compilation process did not complete.", firstPartColor: ConsoleColor.Red, seperator: ":");
            }
        }

        public void RunOS()
        {
            //Emit function name for debugging
            if (isDebug)
                Console.WriteLine($"{nameof(RunOS)}");

            var myos = Path.Combine(compilerJsonContent[nameof(BUILD_DIR)], "myos.bin");

            if (File.Exists(myos))
            {
                // Define arguments and process information
                var runProcessInfo = new ProcessStartInfo(compilerJsonContent[nameof(QEMU)], $"-kernel {myos}");

                // Set UseShellExecute to false to run the linking process directly without shell execution
                runProcessInfo.UseShellExecute = false;

                Process.Start(runProcessInfo);
            }
            else
            {
                Console.WriteLine("OS binary not found. Compile the OS first.");
            }
        }

        private void CompileNasm(string args)
        {
            // Define arguments and process information
            var runProcessInfo = new ProcessStartInfo(compilerJsonContent[nameof(NASM)], args)
            {
                // Set UseShellExecute to false to run the linking process directly without shell execution
                UseShellExecute = false
            };

            Process.Start(runProcessInfo)?.WaitForExit();
        }

        private void CompileSFile(string args)
        {
            // Define arguments and process information
            var runProcessInfo = new ProcessStartInfo(compilerJsonContent[nameof(AS)], args)
            {
                // Set UseShellExecute to false to run the linking process directly without shell execution
                UseShellExecute = false
            };

            Process.Start(runProcessInfo)?.WaitForExit();
        }

        private void CompileCppFile(string args)
        {
            // Define arguments and process information
            var runProcessInfo = new ProcessStartInfo(compilerJsonContent[nameof(GPP)], $"{args}")
            {
                // Set UseShellExecute to false to run the linking process directly without shell execution
                UseShellExecute = false
            };

            Process.Start(runProcessInfo)?.WaitForExit();
        }

        private void CompileBasFile(string args)
        {
            // Define arguments and process information
            var runProcessInfo = new ProcessStartInfo(compilerJsonContent[nameof(FBC)], args)
            {
                // Set UseShellExecute to false to run the linking process directly without shell execution
                UseShellExecute = false
            };

            Process.Start(runProcessInfo)?.WaitForExit();
        }

        public void Clean()
        {
            //Emit function name for debugging
            if (isDebug)
                Console.WriteLine($"{nameof(Clean)}");

            Directory.Delete(compilerJsonContent[nameof(BUILD_DIR)], true);
        }
    }
}