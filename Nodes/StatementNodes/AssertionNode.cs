namespace SkimSkript.Nodes.StatementNodes;

internal class AssertionNode : StatementNode
{
    private readonly Node _condition;

    public Node Condition => _condition;

    public AssertionNode(Node conditionExpression) =>
        _condition = conditionExpression;

    public override string ToString() =>
        $"{_condition}";
}
