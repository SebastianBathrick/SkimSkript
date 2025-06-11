using System.Text;

namespace SkimSkript.Nodes.ValueNodes
{
    internal class ListValueNode : ValueNode
    {
        List<Node> _elements = new();

        public override void AssignValue(ValueNode value)
        {
            throw new NotImplementedException();
        }

        public override ValueNode Copy()
        {
            throw new NotImplementedException();
        }

        public override bool ToBool() => _elements.Count != 0;

        public override float ToFloat() => throw new NotImplementedException();

        public override int ToInt()
        {
            throw new NotImplementedException();
        }

        public override string ToString()
        {
            var sb = new StringBuilder("[");
            for(int i = 0; i < _elements.Count; i++)
            {
                sb.Append(_elements[i].ToString());
                if (i < _elements.Count - 1)
                {
                    sb.Append(", ");
                }
            }

            sb.Append(']');
            return sb.ToString();
        }
    }
}
