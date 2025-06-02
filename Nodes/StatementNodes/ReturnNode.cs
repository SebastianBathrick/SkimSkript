namespace SkimSkript.Nodes
{
    /// <summary>Class representing a return statement where a function ceases to execute and returns control to the caller.</summary>
    /// <remarks>Optionally an expression can be stored to represent a return value.</remarks>
    public class ReturnNode : StatementNode
    {
        private Node? _expression;

        /// <summary>Expression meant to represent the value being returned to the caller.</summary>
        public Node? Expression => _expression;

        /// <summary>Whether an expression is present after the return reserved word(s).</summary>
        public bool IsExpression => _expression != null;

        /// <param name="expression">Expression meant to represent the value being returned to the caller.</param>
        public ReturnNode(Node? expression)
        {
            _expression = expression;
        }

        public override string ToString() =>
            $"return {_expression.ToString()}";
    }
}
