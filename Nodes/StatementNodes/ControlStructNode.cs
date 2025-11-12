namespace SkimSkript.Nodes.StatementNodes;

/// <summary>Abstract class representing a control structure with a condition of execution for a stored block.</summary>
internal abstract class ControlStructNode : StatementNode
{
    private readonly Node _block;

    public Node Block => _block;

    public ControlStructNode(Node block, int endLexemeIndex) => _block = block;

    public override string ToString() => $"\n{_block}";
}
