using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;

namespace Onyx.Binding
{
    public sealed class BoundAnnotation : IAnnotatable
    {
        public string Name { get; }
        public ImmutableArray<object> Values { get; }
        public ImmutableArray<BoundAnnotation> Annotations => annotations.ToImmutableArray();
        public IAnnotatable Child { get; }

        private readonly List<BoundAnnotation> annotations;

        internal BoundAnnotation(string name, ImmutableArray<object> values)
        {
            Name = name;
            Values = values;

            annotations = new List<BoundAnnotation>();
        }

        public override bool Equals(object obj) => obj is BoundAnnotation annotation && annotation.ToString() == ToString();
        public override int GetHashCode() => ToString().GetHashCode();
        public override string ToString()
        {
            var builder = new StringBuilder();

            builder.Append(Name);

            if (Values.Any())
                builder.Append("(").Append(string.Join(", ", Values)).Append(")");

            return builder.ToString();
        }

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
    }
}
