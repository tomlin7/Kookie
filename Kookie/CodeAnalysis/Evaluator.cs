using System;

namespace Kookie.CodeAnalysis
{
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