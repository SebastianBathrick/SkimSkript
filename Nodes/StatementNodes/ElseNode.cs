namespace SkimSkript.Nodes.StatementNodes;

internal class ElseNode : ControlStructNode
{
    public ElseNode(Node block, int endLexemeIndex) : base(block, endLexemeIndex) { }
}
