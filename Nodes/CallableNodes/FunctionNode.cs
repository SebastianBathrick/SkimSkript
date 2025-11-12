namespace SkimSkript.Nodes.CallableNodes;

/// <summary>Class representing a user-defined function node with an identifier, parameters, return type, 
/// and a code block.</summary>
public class FunctionNode : CallableNode
{
    private readonly Node _block;

    public Node Block => _block;

    public FunctionNode(Node identifierNode, Type? returnType, Node[]? parameters, Node block) : base(identifierNode, parameters, returnType) =>
        _block = block;
}
