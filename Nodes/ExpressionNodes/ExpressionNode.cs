namespace SkimSkript.Nodes
{
    /// <summary> Represents an expression with an operator, left and right operands used to model operations. </summary>
    public abstract class ExpressionNode<T> : Node where T : Enum
    {
        private Node _leftOperand;
        private Node _rightOperand;
        private T _operator;

        /// <summary> Enum value representing the type of operator being used in this expression. </summary>
        public T Operator => _operator;
        
        /// <summary> Node representing left operand in the form of an expression of any type, term, or factor. </summary>
        public Node LeftOperand => _leftOperand;

        /// <summary> Node representing right operand in the form of an expression of any type, term, or factor. </summary>
        public Node RightOperand => _rightOperand;

        public ExpressionNode(T operatorType, Node leftOperand, Node rightOperand)
        {
            _operator = operatorType;
            _leftOperand = leftOperand;
            _rightOperand = rightOperand;
        }

        public override string ToString()
        {
            // TODO: Make better expression to strings
            return $"{LeftOperand} {_operator} {RightOperand}";
        }
    }
}

