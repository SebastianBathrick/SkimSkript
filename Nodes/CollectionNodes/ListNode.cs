using SkimSkript.Nodes.Composites;

namespace SkimSkript.Nodes.CollectionNodes;

internal class ListNode : CollectionNode
{
    private readonly List<Node> _elements;

    public ListNode(List<Node> elements) =>
        _elements = elements;

    public void Add(Node node) =>
        _elements.Add(node);

    public Node Get(int index) =>
        _elements[index];

    public int Count =>
        _elements.Count;

    public override string ToString() => throw new NotImplementedException();
}
