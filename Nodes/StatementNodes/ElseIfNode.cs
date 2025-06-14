namespace SkimSkript.Nodes.StatementNodes
{
    internal class ElseIfNode : ControlStructNode
    {
        private Node _condition;
        private Node? _chainedStructure;

        public Node Condition => _condition;

        public Node? ChainedStructure => _chainedStructure;

        public ElseIfNode(Node condition, Node block, Node? chainedStructure) : base(block)
        {
            _condition = condition;
            _chainedStructure = chainedStructure;
        }

        public override string ToString() => $"else if {base.ToString()}";
    }
}
