namespace SkimSkript.Nodes
{
    /// <summary>
    /// Exception thrown by <see cref="Interpretation.Interpreter"/> and meant to be caught by
    /// <see cref="SkimSkriptCore"/>.
    /// </summary>
    internal class AssertionException : Exception
    {
        public AssertionException(string message) : base(message) { }
    }
}
