using System.Collections.Generic;
using System.Linq;

namespace Kookie.CodeAnalysis.Syntax
{
    public sealed class SyntaxTree
    {
        public IEnumerable<Diagnostic> Diagnostics { get; }
        public ExpressionSyntax Root { get; }
        public SyntaxToken EndOfFileToken { get; }

        public SyntaxTree(IEnumerable<Diagnostic> diagnostics, ExpressionSyntax root, SyntaxToken EndOfFileToken)
        {
            Diagnostics = diagnostics.ToArray();
            Root = root;
            this.EndOfFileToken = EndOfFileToken;
        }

        public static SyntaxTree Parse(string text)
        {
            var parser = new Parser(text);
            return parser.Parse();
        }
    }
}