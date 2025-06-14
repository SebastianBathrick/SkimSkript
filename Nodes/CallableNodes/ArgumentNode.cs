namespace SkimSkript.Nodes.CallableNodes
{
    internal class ArgumentNode : Node
    {
        private readonly bool _isRef;
        private readonly Node _value;

        /// <summary> Whether argument was labeled as pass-by-reference in the function call. </summary>
        public bool IsReference => _isRef;

        /// <summary> Pass-by-value expression or pass-by-reference IdentifierNode associated with variable. </summary>
        public Node Value => _value;

        public ArgumentNode(bool isRef, Node value)
        {
            _isRef = isRef;
            _value = value;
        }

        public override string ToString() =>
            _isRef ? "ref " : string.Empty + _value.ToString(); 
    }
}
