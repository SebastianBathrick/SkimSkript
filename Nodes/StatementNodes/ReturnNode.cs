namespace SkimSkript.Nodes
{
    /// <summary>Class representing a return statement where a function ceases to execute and returns control to the caller.</summary>
    /// <remarks>Optionally an expression can be stored to represent a return value.</remarks>
    public class ReturnNode : StatementNode
    {
        private Node? _expression;

        public bool IsExpression => _expression != null;

        public Node? Expression => _expression;

        public ReturnNode(Node? expression)
        {
            _expression = expression;
        }

        public override string ToString() =>
            $"return {_expression?.ToString()}";
    }
}
