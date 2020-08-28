using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;

namespace Onyx.Binding.Symbols
{
    public abstract class DeclarationSymbol<TReturn, TArgument> : TypeSymbol, IAnnotatable
    {
        ImmutableArray<BoundAnnotation> IAnnotatable.Annotations => annotations.ToImmutableArray();

        private readonly List<BoundAnnotation> annotations;

        public DeclarationSymbol(string name) : base(name) => annotations = new List<BoundAnnotation>();

        public void Annotate(BoundAnnotation? annotation)
        {
            if (annotation == null)
                return;

            annotations.AddRange(annotation.Annotations);
            annotations.Add(annotation);
        }
        public void Annotate(IEnumerable<BoundAnnotation> annotations)
        {
            foreach (var annotation in annotations)
                Annotate(annotation);
        }
        internal string[] Annotations()
        {
            var array = new string[annotations.Count];

            for (int i = 0; i < annotations.Count; i++)
                array[i] = annotations[i].ToString();

            return array;
        }
        internal bool HasAnnotations() => annotations.Any();

        internal abstract TReturn New(ImmutableArray<TArgument> arguments, ImmutableArray<TypeSymbol> types);
    }
}
