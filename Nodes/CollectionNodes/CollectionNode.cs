namespace SkimSkript.Nodes.CollectionNodes
{
    internal abstract class CollectionNode<T> : Node
    {
        private readonly Type _elementDataType;

        /// <summary> Type used to identify and retreive specific elements in the collection. </summary>
        public Type ElementIdType => typeof(T);

        /// <summary> Type of nodes that can be stored by this collection. </summary>
        public Type ElementDataType => _elementDataType;

        public CollectionNode(Type elementDataType) => _elementDataType = elementDataType;

        /// <summary> Returns the number of elements currently in the collection. </summary>
        public abstract int GetCount();

        /// <summary> Adds new element to be cached in the collection. </summary>
        /// <remarks>Assumes the type is checked if it matches the elements' data type externally.</remarks>
        public abstract void AddElement(Node element);

        /// <summary> Returns the element associated with the provided ID. </summary>
        public abstract Node GetElement(T elementId);

        /// <summary> Removes element with ID from the collection. </summary>
        public abstract void RemoveElement(T elementId);

        /// <summary> Whether the collection contains an element with the given ID. </summary>
        public abstract bool ContainsElementId(T elementId);
    }
}
