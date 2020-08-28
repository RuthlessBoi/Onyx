using Onyx.Binding.Symbols;
using Onyx.Syntax;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.IO;
using System.Linq;

namespace Onyx.REPL
{
    internal sealed class Program
    {
        internal static bool Running { get; private set; } = true;

        internal static void Main(string[] args)
        {
            Console.WriteLine("Enter file name:");

            while (Running)
            {
                Console.Write("> ");
                var line = Console.ReadLine();

                if (line == "!stop")
                {
                    Console.WriteLine("Ending REPL...");
                    Running = false;
                }
                else
                    Process(line);
            }
        }

        internal static void Process(string name)
        {
            if (File.Exists(name))
            {
                var compilation = Compilation.Create(LoadWithStd(name).ToArray());
                var result = compilation.Evaluate(new Dictionary<VariableSymbol, OnyxValue>());

                if (result.Diagnostics.Any())
                {
                    foreach (var diagnostic in result.Diagnostics)
                    {
                        Console.WriteLine($"{diagnostic.Location.Span} {(diagnostic.IsWarning ? "Warning" : "Error")}: {diagnostic.Message}");
                    }
                }
                else
                {
                    Console.WriteLine();
                    Console.WriteLine("successful evaluation...");
                }
            }
            else
                Console.WriteLine($"The file '{name}' does not exist.");
        }

        internal static IEnumerable<SyntaxTree> LoadWithStd(params string[] names)
        {
            var syntaxTrees = new List<SyntaxTree>();
            
            foreach (var name in names)
                syntaxTrees.Add(SyntaxTree.Load(name));

            syntaxTrees.AddRange(LoadStandardLibrary());

            return syntaxTrees;
        }
        internal static IEnumerable<SyntaxTree> LoadStandardLibrary()
        {
            var syntaxTrees = new List<SyntaxTree>();

            foreach (var file in Directory.GetFiles("std"))
                syntaxTrees.Add(SyntaxTree.Load(file));

            return syntaxTrees;
        }
    }
}
