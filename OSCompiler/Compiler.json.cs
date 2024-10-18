using System;
using System.Collections.Generic;
using System.Data.SqlTypes;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OSCompiler
{
    public class CompilerJson
    {
        #region Compiler.json

        public class CompilerPaths
        {
            public string? GCC { get; set; }
            public string? GPP { get; set; }
            //public string? CC { get; set; }
            public string? AR { get; set; }
            public string? AS { get; set; }
            public string? FBC { get; set; }
            public string? QEMU { get; set; }
            public string? NASM { get; set; }
        }

        public class CompilerFlags
        {
            public string? CFLAGS { get; set; }
            public string? LDFLAGS { get; set; }
        }

        public class Directories
        {
            //Primary
            public string? SRC_DIR { get; set; }
            public string? BUILD_DIR { get; set; }

            //Secondary
            //public string? BUILD_DIR_CPP { get; set; }
            //public string? BUILD_DIR_HEADERS { get; set; }
            //public string? BUILD_DIR_ASM { get; set; }
            //public string? BUILD_DIR_S { get; set; }
        }

        public class ConfigData
        {
            public CompilerPaths? CompilerPaths { get; set; }
            public CompilerFlags? CompilerFlags { get; set; }
            public Directories? Directories { get; set; }
        }

        #endregion
    }
}
