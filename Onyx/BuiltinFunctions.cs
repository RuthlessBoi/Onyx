using Onyx.Binding.Nodes.Expressions;
using Onyx.Binding.Symbols;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Reflection;

namespace Onyx.Binding
{
    internal static class BuiltinFunctions
    {
        public static readonly FunctionSymbol Print = new FunctionSymbol("print", ImmutableArray.Create(new ParameterSymbol("text", TypeSymbol.Any, 0)), TypeSymbol.Void);
        public static readonly FunctionSymbol PrintL = new FunctionSymbol("printl", ImmutableArray.Create(new ParameterSymbol("text", TypeSymbol.Any, 0)), TypeSymbol.Void);
        public static readonly FunctionSymbol Read = new FunctionSymbol("read", ImmutableArray<ParameterSymbol>.Empty, TypeSymbol.String);
        public static readonly FunctionSymbol Rnd = new FunctionSymbol("rnd", ImmutableArray.Create(new ParameterSymbol("max", TypeSymbol.Int, 0)), TypeSymbol.Int);
        public static readonly FunctionSymbol GetAnnotations = new FunctionSymbol("getAnnotations", ImmutableArray.Create(new ParameterSymbol("type", TypeSymbol.InternalType, 0)), TypeSymbol.String);

        internal static readonly Dictionary<FunctionSymbol, Func<BoundCallExpression, object?>> Functions = new Dictionary<FunctionSymbol, Func<BoundCallExpression, object?>>()
        {
            { Print, PrintFunc },
            { PrintL, PrintLineFunc },
            { Read, ReadLineFunc },
            { Rnd, RandomFunc },
            { GetAnnotations, GetAnnotationsFunc },
        };

        private static Evaluator Evaluator;
        private static Random? random;

        internal static void Initialize(Evaluator evaluator) => Evaluator = evaluator;

        internal static bool HasFunction(FunctionSymbol symbol) => symbol != null && Functions.ContainsKey(symbol);
        internal static object? ExecuteFunction(BoundCallExpression node) => Functions[node.Function].Invoke(node);
        internal static IEnumerable<FunctionSymbol> GetAll() => typeof(BuiltinFunctions)
                                       .GetFields(BindingFlags.Public | BindingFlags.Static)
                                       .Where(f => f.FieldType == typeof(FunctionSymbol))
                                       .Select(f => (FunctionSymbol)f.GetValue(null)!);

        private static object? PrintFunc(BoundCallExpression node)
        {
            var value = Evaluator.EvaluateExpression(node.Arguments[0]);

            if (value is BoundArray ba)
            {
                Console.Write($"[{string.Join(", ", ba.GetArray())}]");
            }
            else
                Console.Write(value ?? "undefined");

            return null;
        }
        private static object? PrintLineFunc(BoundCallExpression node)
        {
            var value = Evaluator.EvaluateExpression(node.Arguments[0]);

            if (value is BoundArray ba)
                Console.WriteLine($"[{string.Join(", ", ba.GetArray())}]");
            else
                Console.WriteLine(value ?? "undefined");

            return null;
        }
        private static object? ReadLineFunc(BoundCallExpression node) => Console.ReadLine();
        private static object? RandomFunc(BoundCallExpression node)
        {
            var max = (int)Evaluator.EvaluateExpression(node.Arguments[0])!;

            if (random == null)
                random = new Random();

            return random.Next(max);
        }
        private static object? GetAnnotationsFunc(BoundCallExpression node)
        {
            var type = (TypeContainer)Evaluator.EvaluateExpression(node.Arguments[0]);

            if (type.Type is TemplateSymbol template && template.HasAnnotations())
            {
                var annotations = template.Annotations();
                var array = new BoundArray(TypeSymbol.String.Array, annotations.Length);

                var index = 0;

                foreach (var annotation in annotations)
                    array.Assign(index++, new OnyxValue(annotation, TypeSymbol.String));

                return array;
            }

            return TypeSymbol.String.Array.Empty;
        }
    }
}
