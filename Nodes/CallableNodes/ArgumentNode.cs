namespace SkimSkript.Nodes.CallableNodes
{
    internal class ArgumentNode : Node
    {
        private readonly bool _isRef;
        private readonly Node _value;

        public bool IsReference => _isRef;

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
