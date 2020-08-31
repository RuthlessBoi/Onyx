namespace Onyx.Syntax.Nodes.Statements
{
    public abstract class BlockSyntax : StatementSyntax
    {
        public override SyntaxType Type => SyntaxType.BlockStatement;

        internal BlockSyntax(SyntaxTree syntaxTree) : base(syntaxTree) { }
    }
}
