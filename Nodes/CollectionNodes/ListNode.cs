using System.Text;

namespace SkimSkript.Nodes.CollectionNodes
{
    internal class ListNode : CollectionNode<int> // Specify the type argument for CollectionNode
    {
        private List<Node>? _elements = null!;

        private List<Node> Elements => _elements ??= new List<Node>();

        public ListNode(Type elementDataType) : base(elementDataType) { }

        public override int GetCount() => Elements.Count;

        public override void AddElement(Node element) => Elements.Add(element);

        public override Node GetElement(int elementId) => Elements[elementId];

        public override void RemoveElement(int elementId) => Elements.RemoveAt(elementId);

        public override bool ContainsElementId(int elementId) => elementId >= 0 && elementId < Elements.Count;

        public override string ToString()
        {
            var sb = new StringBuilder("[");

            for (int i = 0; i < Elements.Count; i++)
            {
                sb.Append(Elements[i].ToString());
                if (i < Elements.Count - 1)
                    sb.Append(", ");
            }

            sb.Append("]");
            return sb.ToString();
        }
    }
}
