namespace SkimSkript.Nodes
{
    /// <summary>Class representing an if statement control structure potentially storing a child else
    /// if or else statement of the same class type.</summary>
    public class IfNode : ControlStructNode
    {
        private Node? _chainedStructure;
        private Node _condition;

        public Node Condition => _condition;

        public Node? ChainedStructure => _chainedStructure;

        public IfNode(Node condition, Node block, Node? chainedStructure) : base(block)
        {
            _condition = condition;
            _chainedStructure = chainedStructure;
        }

        public override string ToString() => $"if{base.ToString()}";
    }
}