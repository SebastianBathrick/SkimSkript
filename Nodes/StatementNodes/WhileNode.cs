namespace SkimSkript.Nodes.StatementNodes;

/// <summary>Class representing a while loop control structure.</summary>
internal class WhileNode : ConditionStructNode
{
    public WhileNode(Node condition, Node block, int endLexemeIndex) : base(condition, block, endLexemeIndex) { }

    public override string ToString() => $"while{base.ToString()}";
}
