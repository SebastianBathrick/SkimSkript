using SkimSkript.Syntax;

namespace SkimSkript.Nodes
{
    /// <summary>Class representing a built-in-function node for clearing the console buffer.</summary>
    public class ClearNode : BuiltInFunctionNode
    {
        public ClearNode() : base(BuiltInFunctionID.Clear, null) { }

        /// <summary>Called to clear the console buffer.</summary>
        public override Node? Call(Node[]? arguments = null)
        {
            Console.Clear();
            return null;
        }
    }
}
