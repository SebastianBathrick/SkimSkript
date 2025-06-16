namespace SkimSkript.Nodes
{
    internal class ElseIfNode : IfNode
    {
        public ElseIfNode(Node condition, Node body, Node? chainedStructure)
            : base(condition, body, chainedStructure) { }

        public override string ToString() => $"else if {base.ToString()}";
    }
}
