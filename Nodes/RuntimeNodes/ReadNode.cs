using SkimSkript.Syntax;
using SkimSkript.Nodes.ValueNodes;

namespace SkimSkript.Nodes.Runtime
{
    /// <summary>Class representing a built-in function node for reading user input and returning it as a string.</summary>
    public class ReadNode : BuiltInFunctionNode
    {
        public ReadNode() : base(BuiltInFunctionID.Read) =>
            _returnTypeNode = new StringValueNode(String.Empty);

        /// <summary>Called to retrieve keyboard input via the console window.</summary>
        /// <returns>Node containing the string value of the keyboard input entered.</returns>
        public override Node? Call(List<(Node source, bool isRef)>? arguments)
        {
            string? inputValue = Console.ReadLine();
            inputValue ??= string.Empty;
            return new StringValueNode(inputValue);
        }
    }
}
