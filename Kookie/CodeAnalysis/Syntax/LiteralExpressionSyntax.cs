using System.Collections.Generic;

namespace Kookie.CodeAnalysis.Syntax
{
    public sealed class LiteralExpressionSyntax : ExpressionSyntax
    {
        public SyntaxToken LiteralToken { get; }

        public LiteralExpressionSyntax(SyntaxToken literalToken)
        {
            LiteralToken = literalToken;
        }

        public override SyntaxKind Kind => SyntaxKind.LiteralExpression;
        public override IEnumerable<SyntaxNode> GetChildren()
        {
            yield return LiteralToken;
        }
    }
}