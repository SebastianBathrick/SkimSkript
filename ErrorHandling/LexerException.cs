namespace SkimSkript.ErrorHandling
{
    internal class LexerException : SkimSkriptException
    {
        public override string Message => "Error occured on line {Line}" + base.Message;

        public LexerException(string message, int lineIndex)
            : base(message, lineIndex) { }
    }
}
