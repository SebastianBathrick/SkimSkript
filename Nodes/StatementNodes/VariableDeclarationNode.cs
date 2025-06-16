namespace SkimSkript.Nodes
{
    /// <summary>Class representing the declaration of a statically typed variable.</summary>
    public class VariableDeclarationNode : StatementNode
    {
        private Node _identifierNode;   
        private Node _assignedExpression;
        private Type _dataType;

        /// <summary>Represents identifier of the variable.</summary>
        public Node IdentifierNode => _identifierNode;

        /// <summary>Whether the variable has a value assigned/intialized.</summary>
        /// <remarks>This is primarily used during the parameter declaration where this can return false.</remarks>
        public bool IsAssignment => _assignedExpression != null;

        /// <summary>Expression that will be assigned to the variable potentially to later be coerced 
        /// depending on the data type of the variable.</summary>
        public Node AssignedExpression => _assignedExpression; // TODO: Change to just the type as opposed to an instance.

        /// <summary> Data type of variable. </summary>
        public Type DataType => _dataType;



        public VariableDeclarationNode(Node identifierNode, Type dataType, Node assignedExpression)
        {
            _assignedExpression = assignedExpression;
            _dataType = dataType;
            _identifierNode = identifierNode;
        }

        public override string ToString() =>
            _assignedExpression == null ? $"{IdentifierNode}" : $"{IdentifierNode} = {_assignedExpression.ToString()}";
    }
}
