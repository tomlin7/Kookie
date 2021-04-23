using System;
using System.Collections.Generic;
using Kookie.CodeAnalysis.Syntax;

namespace Kookie.CodeAnalysis.Binding
{
    internal sealed class Binder
    {
        private readonly DiagnosticBag _diagnostics = new();

        public DiagnosticBag Diagnostics => _diagnostics;
        
        public BoundExpression BindExpression(ExpressionSyntax syntax)
        {
            return syntax.Kind switch
            {
                SyntaxKind.ParenthesizedExpression => BindParenthesizedExpression(
                    (ParenthesizedExpressionSyntax) syntax),
                SyntaxKind.LiteralExpression => BindLiteralExpression((LiteralExpressionSyntax) syntax),
                SyntaxKind.NameExpression => BindNameExpression((NameExpressionSyntax) syntax),
                SyntaxKind.AssignmentExpression => BindAssignmentExpression((AssignmentExpressionSyntax) syntax),
                SyntaxKind.UnaryExpression => BindUnaryExpression((UnaryExpressionSyntax) syntax),
                SyntaxKind.BinaryExpression => BindBinaryExpression((BinaryExpressionSyntax) syntax),
               _ => throw new Exception($"Unexpected syntax {syntax.Kind}")
            };
        }
        
        private BoundExpression BindParenthesizedExpression(ParenthesizedExpressionSyntax syntax)
        {
            return BindExpression(syntax.Expression);
        }

        private BoundExpression BindLiteralExpression(LiteralExpressionSyntax syntax)
        {
            var value = syntax.Value ?? 0;
            return new BoundLiteralExpression(value);
        }
        
        private BoundExpression BindNameExpression(NameExpressionSyntax syntax)
        {
            throw new NotImplementedException();
        }
        
        private BoundExpression BindAssignmentExpression(AssignmentExpressionSyntax syntax)
        {
            throw new NotImplementedException();
        }

        private BoundExpression BindUnaryExpression(UnaryExpressionSyntax syntax)
        {
            var boundOperand = BindExpression(syntax.Operand);
            var boundOperator = BoundUnaryOperator.Bind(syntax.OperatorToken.Kind, boundOperand.Type);
            
            if (boundOperator == null)
            {
                _diagnostics.ReportUndefinedUnaryOperator(syntax.OperatorToken.Span, syntax.OperatorToken.Text, boundOperand.Type);
                return boundOperand;
            }
            return new BoundUnaryExpression(boundOperator, boundOperand);
        }

        private BoundExpression BindBinaryExpression(BinaryExpressionSyntax syntax)
        {
            var boundLeft = BindExpression(syntax.Left);
            var boundRight = BindExpression(syntax.Right);
            var boundOperator = BoundBinaryOperator.Bind(syntax.OperatorToken.Kind, boundLeft.Type, boundRight.Type);
            
            if (boundOperator == null)
            {
                _diagnostics.ReportUndefinedBinaryOperator(syntax.OperatorToken.Span, syntax.OperatorToken.Text, boundLeft.Type, boundRight.Type);
                return boundLeft;
            }
            
            return new BoundBinaryExpression(boundLeft, boundOperator, boundRight);
        }
    }
}