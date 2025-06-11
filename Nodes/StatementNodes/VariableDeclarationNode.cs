namespace SkimSkript.Nodes
{
    /// <summary>Class representing the declaration of a statically typed variable.</summary>
    public class VariableDeclarationNode : StatementNode
    {
        private string _identifier;   
        private Node _assignedExpression;
        private Type _dataType;

        /// <summary>Represents identifier of the variable.</summary>
        public string Identifier => _identifier;

        /// <summary>Whether the variable has a value assigned/intialized.</summary>
        /// <remarks>This is primarily used during the parameter declaration where this can return false.</remarks>
        public bool IsAssignment => _assignedExpression != null;

        /// <summary>Expression that will be assigned to the variable potentially to later be coerced 
        /// depending on the data type of the variable.</summary>
        public Node AssignedExpression => _assignedExpression; // TODO: Change to just the type as opposed to an instance.

        /// <summary> Data type of variable. </summary>
        public Type DataType => _dataType;

        /// <param name="identifier">Represents identifier of the variable.</param>
        /// <param name="dataType">Data type of the variable.</param>
        /// <param name="assignedExpression">Expression that will be assigned to the variable potentially to later be coerced 
        /// depending on the data type of the variable.</param>
        public VariableDeclarationNode(string identifier, Type dataType, Node assignedExpression)
        {
            _assignedExpression = assignedExpression;
            _dataType = dataType;
            _identifier = identifier;
        }

        public override string ToString() =>
            _assignedExpression == null ? $"{Identifier}" : $"{Identifier} = {_assignedExpression.ToString()}";
    }
}
