using Onyx.Text;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;

namespace Onyx.Syntax
{
    public sealed class SyntaxToken : SyntaxNode
    {
        public override SyntaxType Type { get; }
        public int Position { get; }
        public string Text { get; }
        public object? Value { get; }
        public override TextSpan Span => new TextSpan(Position, Text.Length);
        public override TextSpan FullSpan
        {
            get
            {
                var start = LeadingTrivia.Length == 0
                                ? Span.Start
                                : LeadingTrivia.First().Span.Start;

                var end = TrailingTrivia.Length == 0
                                ? Span.End
                                : TrailingTrivia.Last().Span.End;

                return TextSpan.FromBounds(start, end);
            }
        }
        public bool IsMissing { get; }

        public ImmutableArray<SyntaxTrivia> LeadingTrivia { get; }
        public ImmutableArray<SyntaxTrivia> TrailingTrivia { get; }

        internal SyntaxToken(SyntaxTree syntaxTree, SyntaxType type, int position, string? text, object? value, ImmutableArray<SyntaxTrivia> leadingTrivia, ImmutableArray<SyntaxTrivia> trailingTrivia) : base(syntaxTree)
        {
            Type = type;
            Position = position;
            Text = text ?? string.Empty;
            IsMissing = text == null;
            Value = value;
            LeadingTrivia = leadingTrivia;
            TrailingTrivia = trailingTrivia;
        }

        public override IEnumerable<SyntaxNode> GetChildren() => Array.Empty<SyntaxNode>();
    }
}
