namespace Kookie.CodeAnalysis.Syntax
{
    public enum SyntaxKind
    {
        // Tokens
        BadToken,
        EndOfFileToken,
        WhiteSpaceToken,
        NumberToken,
        PlusToken,
        MinusToken,
        StarToken,
        SlashToken,
        BangToken,
        AmpersandAmpersandToken,
        PipePipeToken,
        OpenParenthesisToken,
        CloseParenthesisToken,
        IdentifierToken,
        
        // Keywords
        FalseKeyword,
        TrueKeyword,
        
        // Expressions
        LiteralExpression,
        UnaryExpression,
        BinaryExpression,
        ParenthesizedExpression,
    }
}