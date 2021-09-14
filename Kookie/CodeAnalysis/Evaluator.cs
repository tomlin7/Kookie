using System;
using System.Collections.Generic;
using Kookie.CodeAnalysis.Binding;

namespace Kookie.CodeAnalysis
{
    internal sealed class Evaluator
    {
        private readonly BoundExpression _root;
        private readonly Dictionary<VariableSymbol, object> _variables;

        public Evaluator(BoundExpression root, Dictionary<VariableSymbol, object> variables)
        {
            _root = root;
            _variables = variables;
        }

        public object Evaluate()
        {
            return EvaluateExpression(_root);
        }

        private object EvaluateExpression(BoundExpression node)
        {
            return node.Kind switch
            {
                BoundNodeKind.LiteralExpression => EvaluateLiteralExpression((BoundLiteralExpression) node),
                BoundNodeKind.VariableExpression => EvaluateVariableExpression((BoundVariableExpression) node),
                BoundNodeKind.AssignmentExpression => EvaluateAssignmentExpression((BoundAssignmentExpression) node),
                BoundNodeKind.UnaryExpression => EvaluateUnaryExpression((BoundUnaryExpression) node),
                BoundNodeKind.BinaryExpression => EvaluateBinaryExpression((BoundBinaryExpression) node),
                _ => throw new Exception($"Unexpected node {node.Kind}")
            };
        }
        
        private object EvaluateLiteralExpression(BoundLiteralExpression boundLiteralExpression)
        {
            return boundLiteralExpression.Value;
        }
        
        private object EvaluateVariableExpression(BoundVariableExpression boundVariableExpression)
        {
            return _variables[boundVariableExpression.Variable];
        }
        
        private object EvaluateAssignmentExpression(BoundAssignmentExpression boundAssignmentExpression)
        {
            var value = EvaluateExpression(boundAssignmentExpression.Expression);
            _variables[boundAssignmentExpression.Variable] = value;
            return value;
        }
        
        private object EvaluateUnaryExpression(BoundUnaryExpression boundUnaryExpression)
        {
            var operand = EvaluateExpression(boundUnaryExpression.Operand);

            return boundUnaryExpression.Op.Kind switch
            {
                BoundUnaryOperatorKind.Identity => (int) operand,
                BoundUnaryOperatorKind.Negation => -(int) operand,
                BoundUnaryOperatorKind.LogicalNegation => !(bool) operand,
                _ => throw new Exception($"Unexpected unary operator {boundUnaryExpression.Op}")
            };
        }

        private object EvaluateBinaryExpression(BoundBinaryExpression boundBinaryExpression)
        {
            var left = EvaluateExpression(boundBinaryExpression.Left);
            var right = EvaluateExpression(boundBinaryExpression.Right);

            return boundBinaryExpression.Op.Kind switch
            {
                BoundBinaryOperatorKind.Addition => (int) left + (int) right,
                BoundBinaryOperatorKind.Subtraction => (int) left - (int) right,
                BoundBinaryOperatorKind.Multiplication => (int) left * (int) right,
                BoundBinaryOperatorKind.Division => (int) left / (int) right,
                BoundBinaryOperatorKind.LogicalAnd => (bool) left && (bool) right,
                BoundBinaryOperatorKind.LogicalOr => (bool) left || (bool) right,
                BoundBinaryOperatorKind.Equals => Equals(left, right),
                BoundBinaryOperatorKind.NotEquals => !Equals(left, right),
                _ => throw new Exception($"Unexpected binary operator {boundBinaryExpression.Op}")
            };
        }
    }
}