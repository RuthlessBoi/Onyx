using Onyx.Binding.Symbols;

namespace Onyx.Binding
{
    public sealed class BoundTemplate : BoundInstance<TemplateSymbol>
    {
        internal BoundTemplate(TemplateSymbol modelType) : base(modelType) { }
    }
}
