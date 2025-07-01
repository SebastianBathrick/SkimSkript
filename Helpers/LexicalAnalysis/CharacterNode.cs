using SkimSkript.Tokens;

namespace SkimSkript.LexicalAnalysis.Helpers
{
    /// <summary> Trie node for storing reserved keywords, supporting alphabetic characters 
    /// and whitespace, with an associated <see cref="TokenType"/>. </summary>
    public class CharacterNode
    {
        private const int MAX_CHILDREN = 27; //26 for alphabetic chars and 1 extra for whitespace.
        private CharacterNode[] _children = new CharacterNode[MAX_CHILDREN];
        private TokenType _tokenType = TokenType.Identifier;

        /// <summary> <see cref="TokenType"/> stored in node. </summary>
        /// <remarks> If the node represents the final char in reserved word(s) then this will be associated with
        /// that keyword. Otherwise, the value will be <see cref="TokenType.Identifier"/>. </remarks>
        public TokenType TokenType => _tokenType;

        /// <summary> Assigns presumably a <see cref="TokenType"/> associated with reserved word(s). </summary>
        public void AssignTokenType(TokenType tokenType) => _tokenType = tokenType;

        /// <summary> Adds child associated with the given lowercase alpha character so long as there isn't already one. </summary>
        public CharacterNode AddChild(char childChar) => (_children[childChar - 'a'] ??= new CharacterNode());

        /// <summary> Adds a child associated with a space in between reserved words that form a phrase. </summary>
        /// <remarks> Will be ignored if a child representing a space is already present. </remarks>
        public CharacterNode AddSpaceChild() => (_children[MAX_CHILDREN - 1] ??= new CharacterNode());

        /// <summary> Gets child associated with a given lowercase/uppercase alpha character so long as there is one. </summary>
        public CharacterNode? GetChild(char childChar) => _children[childChar < 'a' ? childChar - 'A' : childChar - 'a'];

        /// <summary> Gets child associated with a space so long as there is one. </summary>
        public CharacterNode? GetSpaceChild() => _children[MAX_CHILDREN - 1];
    }
}
