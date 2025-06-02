using SkimSkript.TokenManagement.Tokens;
using SkimSkript.TokenManagement;
using SkimSkript.Syntax;
using System.Text;

namespace SkimSkript.ErrorHandling
{
    public class ErrorHandler
    {
        public ErrorHandler(Exception exception)
        {
            switch (exception)
            {
                case LexicalError lexError: HandleLexicalError(lexError); break;
                case SyntaxError syntaxError: HandleSyntaxError(syntaxError); break;
                case UnknownIdentifierError identifierError: HandleUnknownIdentifierError(identifierError); break;
                default: HandleOtherExceptions(exception); break;
            }
        }

        private void HandleLexicalError(LexicalError error) => Console.WriteLine(error.ToString());

        private void HandleSyntaxError(SyntaxError error)
        {
            TokenContainer tokenContainer = error.TokenContainer;

            if (error.IsExpectedTokenType)
            {
                Console.Write($"{error}Expected {GetTokenTypeErrorMessageRep(error.ExpectedTokenType)}, ");
                Console.WriteLine($"but instead found {GetTokenTypeErrorMessageRep(error.TokenContainer.PeekType())} token.");
            }
            else
                Console.WriteLine(error);

            if (error.Position == ErrorTokenPosition.Forward)
                tokenContainer.MovePointerTowardsRear();
            else if (error.Position == ErrorTokenPosition.Backward)
                tokenContainer.MovePointerTowardsFront();

            PrintLineOfCode(tokenContainer);              
        }

        private void HandleUnknownIdentifierError(UnknownIdentifierError identfierError) =>
            Console.WriteLine(identfierError.ToString());

        private void PrintLineOfCode(TokenContainer tokenContainer)
        {
            List<Token> tokenList = tokenContainer.GetTokensOnLine(out int errorTokenIndex);

            Console.Write($"Line {tokenContainer.CurrentLineNumber}: ");

            for(int i = 0; i < tokenList.Count; i++)
            {
                if (i != errorTokenIndex)
                    Console.ForegroundColor = ConsoleColor.White;
                else
                    Console.ForegroundColor = ConsoleColor.Red;

                if (TryGetPredefinedLexeme(tokenList[i].Type, out string preDefLexeme))
                    Console.Write($"{preDefLexeme} ");
                else if(tokenList[i].Type == TokenType.String)
                    Console.Write($"\"{tokenList[i].Lexeme}\" ");
                else
                    Console.Write($"{tokenList[i].Lexeme} ");
            }

            Console.WriteLine();
            Console.ForegroundColor = ConsoleColor.White;
        }           

        private string GetTokenTypeErrorMessageRep(TokenType tokenType)
        {
            string errorRep;

            if (TryGetPredefinedLexeme(tokenType, out string lexeme))
                errorRep = lexeme;
            else
            {
                errorRep = tokenType switch
                {
                    TokenType.Identifier => "an identifier", 
                    TokenType.Integer => "an integer literal",
                    TokenType.String => "a string literal",
                    TokenType.Float => "a floating point literal",
                    _ => "an undefined type"
                };
            }

            return errorRep;
        }

        /// <summary>Attempts to get string associated with a <see cref="TokenType"/>.
        /// Will succeed and return true so long as the <see cref="TokenType"/> is not
        /// associated with a literal or identifier.</summary>
        private bool TryGetPredefinedLexeme(TokenType tokenType, out string preDefLexeme)
        {
            if(tokenType == TokenType.Identifier || tokenType == TokenType.String 
                || tokenType == TokenType.Float || tokenType == TokenType.Integer)
            {
                preDefLexeme = string.Empty;
                return false;
            }


            if (SyntaxSpec.IsOperatorTokenType(tokenType))
                preDefLexeme = SyntaxSpec.GetOperatorLexeme(tokenType);
            else if (SyntaxSpec.IsDelimeterType(tokenType))
                preDefLexeme = SyntaxSpec.GetDelimeterLexeme(tokenType);
            else
                preDefLexeme = SyntaxSpec.GetReservedWordLexeme(tokenType);

            return true;
        }

        private void HandleOtherExceptions(Exception exception) =>
            Console.WriteLine(exception.Message);
    }
}
