// TODO: Split ConditionExpressionNode into two seperate classes (ComparisonExpression & LogicalExpression)

namespace SkimSkript.Nodes
{
    /// <summary>Class representing a conditional expression.</summary>
    /// <remarks>Depending on which constructor is called, an instance can either represent a 
    /// comparison expression or a logical expression.</remarks>
    public class ConditionExpressionNode : ExpressionNode 
    {
        private LogicalOperator _logicalOp;
        private ComparisonOperator _comparisonOp;
        private bool _isComparison = true;

        /// <summary>Boolean indicating whether the stored operator and the expression itself are logical.</summary>
        /// <remarks>If false, the operator/expression are considered as a comparison.</remarks>
        public bool IsLogical => !_isComparison;

        /// <summary>Enum value representing a stored comparison operator.</summary>
        public ComparisonOperator ComparisonOperator => _comparisonOp;

        /// <summary>Enum value representing a stored logical operator.</summary>
        public LogicalOperator LogicalOperator => _logicalOp;

        /// <summary>Constructor meant to instantiate comparison expressions.</summary>
        /// <param name="comparisonOp">Enum value representing the comparison operator used in the expression.</param>
        /// <param name="leftOperand">Node representing the left operand. This can be any factor, term, or
        /// child of <see cref="ExpressionNode"/>.</param>
        /// <param name="rightOperand">Node representing the right operand. This can be any factor, term, or
        /// child of <see cref="ExpressionNode"/>.</param>
        public ConditionExpressionNode(ComparisonOperator comparisonOp, Node leftOperand, Node rightOperand) : base(leftOperand, rightOperand) =>
            _comparisonOp = comparisonOp;

        /// <summary>Constructor meant to instantiate logical expressions.</summary>
        /// <param name="logicalOp">Enum value representing the logical operator used in the expression.</param>
        /// <param name="leftOperand">Node representing the left operand. Can be any factor, term, or
        /// <see cref="ExpressionNode"/> child.</param>
        /// <param name="rightOperand">Node representing the right operand. Can be any factor, term, or
        /// <see cref="ExpressionNode"/> child.</param>
        public ConditionExpressionNode(LogicalOperator logicalOp, Node leftOperand, Node rightOperand) : base(leftOperand, rightOperand)
        {
            _logicalOp = logicalOp;
            _isComparison = false;
        }

        public override string ToString() =>
            _isComparison ? $"{LeftOperand} {_comparisonOp} {RightOperand}" : $"{LeftOperand} {_logicalOp} {RightOperand}";
    }

}