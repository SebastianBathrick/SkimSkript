namespace SkimSkript.Nodes.StatementNodes;

/// <summary>Represents a call to a user-defined or built-in function.</summary>
internal class FunctionCallNode : StatementNode
{
    private readonly Node _identifierNode;
    private readonly Node[]? _arguments;

    /// <summary>The identifier of the function being called.</summary>
    public Node IdentifierNode => _identifierNode;

    public Node[]? Arguments => _arguments;

    public FunctionCallNode(Node identifierNode, Node[]? arguments)
    {
        _identifierNode = identifierNode;
        _arguments = arguments;
    }

    public override string ToString() => $"{_identifierNode}()";
}
