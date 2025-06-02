using SkimSkript.Syntax;

namespace SkimSkript.Nodes.Runtime
{
    /// <summary>Class representing a built-in-function node for clearing the console buffer.</summary>
    public class ClearNode : BuiltInFunctionNode
    {
        public ClearNode() : base(BuiltInFunctionID.Clear) => _returnTypeNode = null;

        /// <summary>Called to clear the console buffer.</summary>
        public override Node? Call(List<(Node source, bool isRef)>? arguments = null)
        {            
            Console.Clear();
            return null;
        }
    }
}
