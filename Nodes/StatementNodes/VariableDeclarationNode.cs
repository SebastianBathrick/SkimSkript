namespace SkimSkript.Nodes
{
    /// <summary>Class representing the declaration of a statically typed variable.</summary>
    public class VariableDeclarationNode : StatementNode
    {
        private string _identifier;   
        private Node _assignedExpression;
        private Node _valueNodeType; // TODO: Store the type of value node as opposed to an instance.

        /// <summary>Represents identifier of the variable.</summary>
        public string Identifier => _identifier;

        /// <summary>Whether the variable has a value assigned/intialized.</summary>
        /// <remarks>This is primarily used during the parameter declaration where this can return false.</remarks>
        public bool IsAssignment => _assignedExpression != null;

        /// <summary>Expression that will be assigned to the variable potentially to later be coerced 
        /// depending on the data type of the variable.</summary>
        public Node AssignedExpression => _assignedExpression; // TODO: Change to just the type as opposed to an instance.

        /// <summary>Instance of a node storing the variable's data type for identification.</summary>
        public Node ValueNodeType => _valueNodeType;

        /// <param name="identifier">Represents identifier of the variable.</param>
        /// <param name="assignedExpression">Expression that will be assigned to the variable potentially to later be coerced 
        /// depending on the data type of the variable.</param>
        public VariableDeclarationNode(string identifier, Node assignedExpression)
        {
            _assignedExpression = assignedExpression;
            _valueNodeType = assignedExpression;
            _identifier = identifier;
        }

        /// <param name="identifier">Represents identifier of the variable.</param>
        /// <param name="assignedExpression">Expression that will be assigned to the variable potentially to later
        /// be coerced depending on the data type of the variable.</param>
        /// <param name="valueNodeType">Instance of a node storing the variable's data type for identification.</param>
        public VariableDeclarationNode(string identifier, Node assignment, Node valueNodeType)
        {
            _assignedExpression = assignment;
            _valueNodeType = valueNodeType;
            _identifier = identifier;
        }

        public override string ToString() =>
            _assignedExpression == null ? $"{Identifier}" : $"{Identifier} = {_assignedExpression.ToString()}";
    }
}
