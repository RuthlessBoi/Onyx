using System.Collections.Generic;
using System.Collections.Immutable;

namespace Onyx.Binding
{
    public interface IAnnotatable
    {
        public ImmutableArray<BoundAnnotation> Annotations { get; }

        public void Annotate(BoundAnnotation? annotation);
        public void Annotate(IEnumerable<BoundAnnotation> annotations);
    }
}
