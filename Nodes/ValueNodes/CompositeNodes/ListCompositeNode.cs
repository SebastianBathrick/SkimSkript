namespace SkimSkript.Nodes.ValueNodes.CompositeNodes
{
    internal class ListCompositeNode<T> : CompositeNode<T> where T : ValueNode
    {
        protected List<T> List => ((List<T>)CompositeValues);

        public T this[int key] => List[key];

        public ListCompositeNode() : base(new List<T>(), new(){ }) { }

        public void SetAtIndex(int index, T value) => List[index] = value;

        public override void AppendValue(T value) => List.Add(value);

        #region Inherited Methods
        public override bool ToBool()
        {
            throw new NotImplementedException();
        }

        public override float ToFloat()
        {
            throw new NotImplementedException();
        }

        public override int ToInt()
        {
            throw new NotImplementedException();
        }

        public override string ToString()
        {
            throw new NotImplementedException();
        }
        #endregion
    }
}
