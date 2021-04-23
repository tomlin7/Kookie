namespace Kookie.CodeAnalysis.Syntax
{
    internal static class SyntaxFacts
    {
        // -1 * 3
        //
        //     -
        //     |
        //     *
        //    / \
        //   1  2
        //
        //     -
        //     |
        //     *
        //    / \
        //   -  2
        //   |
        //   1
        public static int GetUnaryOperatorPrecedence(this SyntaxKind kind)
        {
            switch (kind)
            {
                case SyntaxKind.PlusToken:
                case SyntaxKind.MinusToken:
                case SyntaxKind.BangToken:
                    return 6;
                
                default:
                    return 0;

            }
        }
        
        public static int GetBinaryOperatorPrecedence(this SyntaxKind kind)
        {
            switch (kind)
            {
                case SyntaxKind.StarToken:
                case SyntaxKind.SlashToken:
                    return 5;
                
                case SyntaxKind.PlusToken:
                case SyntaxKind.MinusToken:
                    return 4;
                
                case SyntaxKind.EqualsEqualsToken:
                case SyntaxKind.BangEqualsToken:
                    return 3;
                
                case SyntaxKind.AmpersandAmpersandToken:
                    return 2;
                
                case SyntaxKind.PipePipeToken:
                    return 1;
                
                default:
                    return 0;

            }
        }

        public static SyntaxKind GetKeyWordKind(string text)
        {
            return text switch
            {
                "true" => SyntaxKind.TrueKeyword,
                "false" => SyntaxKind.FalseKeyword,
                _ => SyntaxKind.IdentifierToken
            };
        }
    }
}