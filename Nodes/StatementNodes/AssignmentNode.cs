namespace SkimSkript.Nodes
{
    /// <summary>Class representing assignment statement in which a variable being referenced 
    /// gets assigned an expression.</summary>
    public class AssignmentNode : StatementNode
    {
        private Node _identifierNode;
        private Node _assignedExpression;

        /// <summary>Node representing a factor, term, or expression to be assinged to a variable.</summary>
        public Node AssignedExpression => _assignedExpression;

        /// <summary>Node containing a string representing a variable identifier.</summary>
        public Node IdentifierNode => _identifierNode;

        /// <param _name="identifierNode">Node containing a string representing the identifier of a variable.</param>
        /// <param _name="assignedExpression">Node representing a factor, term, or expression to be assinged to a variable.</param>
        public AssignmentNode(Node identifierNode, Node assignedExpression)
        {
            _identifierNode = identifierNode;
            _assignedExpression = assignedExpression;
        }

        public override string ToString() => $"{_identifierNode} = {_assignedExpression}";
    }
}
