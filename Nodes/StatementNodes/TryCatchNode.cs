namespace SkimSkript.Nodes.StatementNodes;

internal class TryCatchNode : StatementNode
{
    private readonly Node _tryBlock;
    private readonly Node _catchBlock;
    private readonly Node? _catchMessage;

    public Node TryBlock => _tryBlock;

    public Node CatchBlock => _catchBlock;

    public Node? CatchMessage => _catchMessage;

    public TryCatchNode(Node tryBlock, Node catchBlock, Node? catchMessage)
    {
        _tryBlock = tryBlock;
        _catchBlock = catchBlock;
        _catchMessage = catchMessage;
    }

    public override string ToString() => $"TryCatchNode(TryBlock: {_tryBlock}, CatchBlock: {_catchBlock}, CatchMessage: {_catchMessage})";
}
