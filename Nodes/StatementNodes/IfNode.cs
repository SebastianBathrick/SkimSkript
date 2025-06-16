using SkimSkript.Nodes.StatementNodes;

namespace SkimSkript.Nodes
{
    /// <summary>Class representing an if statement control structure potentially storing a child else
    /// if or else statement of the same class type.</summary>
    internal class IfNode : ConditionStructNode
    {
        private Node? _chainedStructure;

        public Node? ChainedStructure => _chainedStructure;

        public IfNode(Node condition, Node block, Node? chainedStructure) : base(condition, block) =>
            _chainedStructure = chainedStructure;

        public override string ToString() => $"if{base.ToString()}";
    }
}