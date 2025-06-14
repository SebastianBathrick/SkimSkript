using SkimSkript.Syntax;
using SkimSkript.TokenManagement;

namespace SkimSkript.Nodes
{
    /// <summary>Abstract class representing an expression with left and right operands, 
    /// used to model operations.</summary>
    public abstract class ExpressionNode<T> : Node where T : Enum
    {
        private Node _leftOperand;
        private Node _rightOperand;
        private T _operator;

        public T Operator => _operator;
        

        /// <summary> Node representing left operand in the form of an expression of
        /// any type, term, or factor. </summary>
        /// <remarks>The node type can be dependent on the child class. More info
        /// can be found in the child class constructor documentation.</remarks>
        public Node LeftOperand => _leftOperand;

        /// <summary> Node representing right operand in the form of an expression of
        /// any type, term, or factor. </summary>
        /// <remarks>The node type can be dependent on the child class. More info
        /// can be found in the child class constructor documentation.</remarks>
        public Node RightOperand => _rightOperand;

        /// <summary>Base constructor for child classes lacking an operator.</summary>
        /// <param name="leftOperand">Node representing left operand in the form of an expression of
        /// any inherited type, term, or factor.</param>
        /// <param name="rightOperand">Node representing right operand in the form of an expression of
        /// any inherited type, term, or factor.</param>
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

