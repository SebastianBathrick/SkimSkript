namespace SkimSkript.Nodes
{
    /// <summary>Abstract class representing a node containing data associated with a subroutine. That data
    /// includes the subroutine's identifierNode, parameters it accepts, a return type, and whether or not it's variadic.</summary>
    public abstract class CallableNode : Node
    {
        private readonly Node _identifierNode;
        private readonly Node[]? _parameters;
        private readonly Type? _returnTypeNode;
        private readonly bool _isVariadic = false;

        #region Properties
        /// <summary> Node containing unique identifier for object. </summary>
        public Node IdentfierNode => _identifierNode;

        /// <summary> Nodes containing parameter declaration info if not null. </summary>
        public Node[]? Parameters => _parameters;

        /// <summary> Defined return type if not null. </summary>
        public Type? ReturnType => _returnTypeNode;

        /// <summary> Whether it's no return type was defined. </summary>
        public bool IsVoid => _returnTypeNode == null;

        /// <summary> Can except zero or more arguments in a single call. </summary>
        public bool IsVariadic => _isVariadic;

        /// <summary> If callable requires one or more parameters or is variadic. </summary>
        public bool IsParameters => _parameters != null || _isVariadic;

        /// <summary> Number of arguments that need to be sent to call this object. </summary>>
        public int RequiredArgCount =>  IsParameters ? _parameters!.Length : 0;
        
        #endregion

        public CallableNode(Node identifierNode, Node[]? parameters, Type? returnType=null, bool isVariadic=false)
        {
            _identifierNode = identifierNode;
            _parameters = parameters;
            _returnTypeNode = returnType;
            _isVariadic = isVariadic;
        }

        public override string ToString() => $"{_identifierNode} CallableNode";
    }
}
 