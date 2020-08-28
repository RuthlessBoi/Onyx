using System;
using System.Collections.Generic;
using System.Text;

namespace Onyx.Compiler.CIL
{
    public sealed class CILCompiler : ICompiler
    {
        public CompilerResult Compile()
        {
            return CompilerResult.Success;
        }
    }
}
