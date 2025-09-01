namespace SkimSkript.Nodes
{
    /// <summary>Stores a single identifier derived from a alphabetic/alphanumeric lexeme to be accessed 
    /// using <see cref="Lexeme"/> and evaluated runtime.</summary>
    public class IdentifierNode : Node
    {
        private string _lexeme;

        public string Lexeme => _lexeme;

        public IdentifierNode(string lexeme) => _lexeme = lexeme;

        public override string ToString() => _lexeme;
    }
}
