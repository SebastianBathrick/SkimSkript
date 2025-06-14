namespace SkimSkript.ErrorHandling
{
    /// <summary> Abstract class that inherits from <see cref="Exception"/> and will serve as the
    /// parent of any error specific to the language. </summary>
    public abstract class LanguageError : Exception
    {
        public LanguageError(string message) : base(message) { }

        /// <summary> Returns the entire error message and line of code associated. </summary>
        public override string ToString() => Message;
    }
}
