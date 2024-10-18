using DiscUtils.Fat;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OSCompiler
{
    class Program
    {
        private static CompilerJson.ConfigData? configData;

        //dotnet YourCSharpFile.dll path/to/source_file.cpp path/to/another_file.cpp
        //xorriso -as mkisofs -o output.iso input.img
        static void Main(string[] args)
        {
            //args = new string[] { @"C:\Users\djlw7\Desktop\OSKitCPP\OS\src\kernel.cpp" };
            args = [@"C:\Users\djlw7\Desktop\OSKitCPP\OS\src\kernel.cpp"];
            var builder = new OSBuilder(configData);

            bool shouldClean = false;

            //OSBuilder.CompileNasm();

            if (args.Length > 0)
            {
                //builder.Clean() would be put before CompileOS if clean always was the case

                if (shouldClean)
                    builder.Clean();

                builder.CompileOS(args);
                builder.RunOS();
                // builder.Clean(); // Uncomment if you want to clean the build directory after execution
            }
            else
            {
                Console.WriteLine("Please specify C/C++ files to compile.");
            }

            Console.ReadKey();
        }
    }
}