using Onyx.Syntax;
using System.IO;

namespace Onyx.Binding
{
    internal abstract class BoundNode
    {
        public abstract BoundNodeType Type { get; }
        public SyntaxNode Syntax { get; }

        protected BoundNode(SyntaxNode syntax) => Syntax = syntax;

        public override string ToString()
        {
            using (var writer = new StringWriter())
            {
                //this.WriteTo(writer);
                return writer.ToString();
            }
        }
    }
}
