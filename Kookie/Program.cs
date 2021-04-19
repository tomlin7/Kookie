using System;
using System.Collections.Generic;
using System.Linq;

namespace Kookie
{
    // 1 + 2 * 3
    // 
    // 
    // 
    //     +
    //    / \
    //   1   *
    //      / \
    //     2   3
    
    // 1 + 2 + 3
    //
    //
    //
    //       +
    //      / \
    //     +   3
    //    / \
    //   1   2
    
    internal static class Program
    {
        static void Main(string[] args)
        {
            while (true)
            {
                Console.Write("> ");
                var line = Console.ReadLine();
                if (string.IsNullOrWhiteSpace(line))
                {
                    return;
                }
                
                var syntaxTree = SyntaxTree.Parse(line);

                var color = Console.ForegroundColor;
                Console.ForegroundColor = ConsoleColor.DarkGray;
                PrettyPrint(syntaxTree.Root);
                Console.ForegroundColor = color;

                if (!syntaxTree.Diagnostics.Any())
                {
                    var e = new Evaluator(syntaxTree.Root);
                    var result = e.Evaluate();
                    Console.WriteLine(result);
                }
                else
                {
                    Console.ForegroundColor = ConsoleColor.DarkRed;
                    foreach (var diagnostic in syntaxTree.Diagnostics)
                    {
                        Console.WriteLine(diagnostic);
                    }

                    Console.ForegroundColor = color;
                }
            }
        }

        static void PrettyPrint(SyntaxNode node, string indent = "", bool isLast = true)
        {
            // ├──
            // │
            // └──
            
            var marker = isLast ? "└──" : "├──";
            
            Console.Write(indent);
            Console.Write(marker);
            Console.Write(node.Kind);

            if (node is SyntaxToken {Value: { }} token)
            {
                Console.Write(" ");
                Console.Write(token.Value);
            }
            
            Console.WriteLine();
            indent += isLast ? "    " : "│   ";

            var lastChild = node.GetChildren().LastOrDefault();

            foreach (var child in node.GetChildren())
            {
                PrettyPrint(child, indent, child == lastChild);
            }
        }
    }
    
    internal enum SyntaxKind
    {
        NumberToken,
        WhiteSpaceToken,
        PlusToken,
        MinusToken,
        StarToken,
        SlashToken,
        OpenParenthesisToken,
        CloseParenthesisToken,
        BadToken,
        EndOfFileToken,
        NumberExpression,
        BinaryExpression,
        ParenthesizedExpression
    }
    
    internal class SyntaxToken : SyntaxNode
    {
        public override SyntaxKind Kind { get; }
        public override IEnumerable<SyntaxNode> GetChildren()
        {
            return Enumerable.Empty<SyntaxNode>();
        }

        public int Position { get; }
        public string Text { get; }
        public object Value { get; }

        public SyntaxToken(SyntaxKind kind,int position, string text, object value)
        {
            Kind = kind;
            Position = position;
            Text = text;
            Value = value;
        }
    }

    internal class Lexer
    {
        private readonly string _text;
        private int _position;
        private List<string> _diagnostics = new List<string>();
        
        public Lexer(string text)
        {
            _text = text;
        }

        public IEnumerable<string> Diagnostics => _diagnostics;
        private char Current()
        {
            return _position >= _text.Length ? '\0' : _text[_position];
        }

        private void Next()
        {
            _position++;
        }

        public SyntaxToken NextToken()
        {
            // <numbers>
            // + - * / ( )
            // <whitespace>

            if (_position >= _text.Length)
            {
                return new SyntaxToken(SyntaxKind.EndOfFileToken, _position, "\0", null);
            }

            if (char.IsDigit(Current()))
            {
                var start = _position;

                while (char.IsDigit(Current()))
                {
                    Next();
                }

                var length = _position - start;
                var text = _text.Substring(start, length);
                if (!int.TryParse(text, out var value))
                {
                    _diagnostics.Add($"The number {_text} isn't a valid Int32.");
                }
                return new SyntaxToken(SyntaxKind.NumberToken, start, text, value);
            }
            
            if (char.IsWhiteSpace(Current()))
            {
                var start = _position;

                while (char.IsWhiteSpace(Current()))
                {
                    Next();
                }

                var length = _position - start;
                var text = _text.Substring(start, length);
                return new SyntaxToken(SyntaxKind.WhiteSpaceToken, start, text, null);
            }

            if (Current() == '+')
            {
                return new SyntaxToken(SyntaxKind.PlusToken, _position++, "+", null);
            }

            if (Current() == '-')
            {
                return new SyntaxToken(SyntaxKind.MinusToken, _position++, "-", null);
            }

            if (Current() == '*')
            {
                return new SyntaxToken(SyntaxKind.StarToken, _position++, "*", null);
            }

            if (Current() == '/')
            {
                return new SyntaxToken(SyntaxKind.SlashToken, _position++, "/", null);
            }

            if (Current() == '(')
            {
                return new SyntaxToken(SyntaxKind.OpenParenthesisToken, _position++, "(", null);
            }

            if (Current() == ')')
            {
                return new SyntaxToken(SyntaxKind.CloseParenthesisToken, _position++, ")", null);
            }
            
            _diagnostics.Add($"ERROR: bad character input: '{Current()}'");
            return new SyntaxToken(SyntaxKind.BadToken, _position++, _text.Substring(_position - 1, 1), null);
        }
    }

    internal abstract class SyntaxNode
    {
        public abstract SyntaxKind Kind { get; }

        public abstract IEnumerable<SyntaxNode> GetChildren();
    }

    abstract class ExpressionSyntax : SyntaxNode
    {
        
    }

    sealed class NumberExpressionSyntax : ExpressionSyntax
    {
        public SyntaxToken NumberToken { get; }

        public NumberExpressionSyntax(SyntaxToken numberToken)
        {
            NumberToken = numberToken;
        }

        public override SyntaxKind Kind => SyntaxKind.NumberExpression;
        public override IEnumerable<SyntaxNode> GetChildren()
        {
            yield return NumberToken;
        }
    }

    sealed class BinaryExpressionSyntax : ExpressionSyntax
    {
        public ExpressionSyntax Left { get; }
        public SyntaxToken OperatorToken { get; }
        public ExpressionSyntax Right { get; }

        public BinaryExpressionSyntax(ExpressionSyntax left, SyntaxToken operatorToken, ExpressionSyntax right)
        {
            Left = left;
            OperatorToken = operatorToken;
            Right = right;
        }

        public override SyntaxKind Kind => SyntaxKind.BinaryExpression;
        public override IEnumerable<SyntaxNode> GetChildren()
        {
            yield return Left;
            yield return OperatorToken;
            yield return Right;
        }
    }

    sealed class ParenthesizedExpressionSyntax : ExpressionSyntax
    {
        public SyntaxToken OpenParenthesisToken { get; }
        public ExpressionSyntax Expression { get; }
        public SyntaxToken CloseParenthesisToken { get; }

        public ParenthesizedExpressionSyntax(SyntaxToken openParenthesisToken, ExpressionSyntax expression, SyntaxToken closeParenthesisToken)
        {
            OpenParenthesisToken = openParenthesisToken;
            Expression = expression;
            CloseParenthesisToken = closeParenthesisToken;
        }

        public override SyntaxKind Kind => SyntaxKind.ParenthesizedExpression;
        public override IEnumerable<SyntaxNode> GetChildren()
        {
            yield return OpenParenthesisToken;
            yield return Expression;
            yield return CloseParenthesisToken;
        }
    }

    sealed class SyntaxTree
    {
        public IEnumerable<string> Diagnostics { get; }
        public ExpressionSyntax Root { get; }
        public SyntaxToken EndOfFileToken { get; }

        public SyntaxTree(IEnumerable<string> diagnostics, ExpressionSyntax root, SyntaxToken EndOfFileToken)
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
    
    internal class Parser
    {
        private readonly SyntaxToken[] _tokens;
        
        private List<string> _diagnostics = new();
        private int _position;
        
        public Parser(string text)
        {
            var tokens = new List<SyntaxToken>();
            var lexer = new Lexer(text);
            SyntaxToken token;
            do
            {
                token = lexer.NextToken();

                if (token.Kind != SyntaxKind.WhiteSpaceToken &&
                    token.Kind != SyntaxKind.BadToken)
                {
                    tokens.Add(token);
                }
            } while (token.Kind != SyntaxKind.EndOfFileToken);

            _tokens = tokens.ToArray();
            _diagnostics.AddRange(lexer.Diagnostics);
        }

        public IEnumerable<string> Diagnostics => _diagnostics;

        private SyntaxToken Peek(int offset)
        {
            var index = _position + offset;
            return index >= _tokens.Length ? _tokens[^1] : _tokens[index];
        }

        private SyntaxToken Current => Peek(0);

        private SyntaxToken NextToken()
        {
            var current = Current;
            _position++;
            return current;
        }
        
        private SyntaxToken Match(SyntaxKind kind)
        {
            if (Current.Kind == kind)
            {
                return NextToken();
            }
            
            _diagnostics.Add($"ERROR: Unexpected token <{Current.Kind}>, expected <{kind}>");
            return new SyntaxToken(kind, Current.Position, null, null);
        }

        private ExpressionSyntax ParseExpression()
        {
            return ParseTerm();
        }

        public SyntaxTree Parse()
        {
            var expression = ParseTerm();
            var endOfFileToken = Match(SyntaxKind.EndOfFileToken);
            return new SyntaxTree(_diagnostics, expression, endOfFileToken);
        }
        
        public ExpressionSyntax ParseTerm()
        {
            var left = ParseFactor();

            while (Current.Kind is SyntaxKind.PlusToken or SyntaxKind.MinusToken)
            {
                var operatorToken = NextToken();
                var right = ParseFactor();
                left = new BinaryExpressionSyntax(left, operatorToken, right);
            }

            return left;
        }

        public ExpressionSyntax ParseFactor()
        {
            var left = ParsePrimaryExpression();

            while (Current.Kind is SyntaxKind.StarToken or SyntaxKind.SlashToken)
            {
                var operatorToken = NextToken();
                var right = ParsePrimaryExpression();
                left = new BinaryExpressionSyntax(left, operatorToken, right);
            }

            return left;
        }

        private ExpressionSyntax ParsePrimaryExpression()
        {
            if (Current.Kind == SyntaxKind.OpenParenthesisToken)
            {
                var left = NextToken();
                var expression = ParseExpression();
                var right = Match(SyntaxKind.CloseParenthesisToken);
                return new ParenthesizedExpressionSyntax(left, expression, right);
            }
            
            var numberToken = Match(SyntaxKind.NumberToken);
            return new NumberExpressionSyntax(numberToken);
        }
    }

    internal class Evaluator
    {
        private readonly ExpressionSyntax _root;

        public Evaluator(ExpressionSyntax root)
        {
            _root = root;
        }

        public int Evaluate()
        {
            return EvaluateExpression(_root);
        }

        private int EvaluateExpression(ExpressionSyntax node)
        {
            // BinaryExpression
            // NumberExpression

            if (node is NumberExpressionSyntax numberExpressionSyntax)
            {
                return (int) numberExpressionSyntax.NumberToken.Value;
            }

            if (node is BinaryExpressionSyntax binaryExpressionSyntax)
            {
                var left = EvaluateExpression(binaryExpressionSyntax.Left);
                var right = EvaluateExpression(binaryExpressionSyntax.Right);

                if (binaryExpressionSyntax.OperatorToken.Kind == SyntaxKind.PlusToken)
                {
                    return left + right;
                }

                if (binaryExpressionSyntax.OperatorToken.Kind == SyntaxKind.MinusToken)
                {
                    return left - right;
                }

                if (binaryExpressionSyntax.OperatorToken.Kind == SyntaxKind.StarToken)
                {
                    return left * right;
                }

                if (binaryExpressionSyntax.OperatorToken.Kind == SyntaxKind.SlashToken)
                {
                    return left / right;
                }

                throw new Exception($"Unexpected binary operator {binaryExpressionSyntax.OperatorToken.Kind}");
            }

            if (node is ParenthesizedExpressionSyntax parenthesizedExpressionSyntax)
            {
                return EvaluateExpression(parenthesizedExpressionSyntax.Expression);
            }
            
            throw new Exception($"Unexpected node {node.Kind}");
        }
    }
}