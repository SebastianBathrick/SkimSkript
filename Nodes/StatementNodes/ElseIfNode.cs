namespace SkimSkript.Nodes
{
    internal class ElseIfNode : IfNode
    {
        public ElseIfNode(Node condition, Node body, Node? chainedStructure, int endLexemeIndex)
            : base(condition, body, chainedStructure, endLexemeIndex) { }

        public override string ToString() => $"else if {base.ToString()}";
    }
}
