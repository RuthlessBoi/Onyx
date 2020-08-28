using Onyx.Compiler;
using System.Collections.Immutable;

namespace Onyx
{
    public sealed class CompilationResult
    {
        public ImmutableArray<Diagnostic> Diagnostics { get; }
        public string Path { get; }
        public CompilerType Type { get; }
        public CompilerResult Result { get; }

        internal CompilationResult(ImmutableArray<Diagnostic> diagnostics, string path, CompilerType type, CompilerResult result)
        {
            Diagnostics = diagnostics;
            Path = path;
            Type = type;
            Result = result;
        }
    }
}
