namespace SkimSkript.Nodes.ValueNodes
{
    internal abstract class CompositeNode<T> : ValueNode where T : class
    {
        #region Variables
        private IEnumerable<T> _compositeValues;
        private readonly Dictionary<string, Node> _callableMap;
        #endregion

        #region Properties
        public Type GetCompositeValueType() =>  typeof(T);

        public int ValueCount => _compositeValues.Count();

        protected IEnumerable<T> CompositeValues => _compositeValues;
        #endregion

        public override void AssignValue(ValueNode value) =>
            throw new Exception($"Attempted to copy data members from {value.GetType()}" +
                $" to reference type {GetType()}");

        public CompositeNode(
            IEnumerable<T> compositeValues,
            Dictionary<string, Node> callableMap)
        {
            _compositeValues = compositeValues;
            _callableMap = callableMap;
        }

        public Node GetCallable(string identifier) =>
            _callableMap.TryGetValue(identifier, out var callable) ?
            callable : throw new ArgumentException($"Unknown callable {identifier} in composite instance.");

        public abstract void AppendValue(T value);

        public override ValueNode Copy() =>
            throw new Exception($"Attempted to copy reference type {GetType()}");

    }
}
