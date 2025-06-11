namespace SkimSkript.Nodes.CollectionNodes
{
    public class CollectionException : Exception
    {
        private readonly string _collectionStrRep;

        public string CollectionStringRepresentation => _collectionStrRep;

        public CollectionException(string message, string collectionStrRep) : base(message) =>
            _collectionStrRep = collectionStrRep;
    }
}
