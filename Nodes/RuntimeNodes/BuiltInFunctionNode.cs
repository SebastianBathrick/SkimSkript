using SkimSkript.Syntax;

namespace SkimSkript.Nodes.Runtime
{
    /// <summary>Abstract class representing a built-in function node with predefined behavior and parameters.</summary>
    public abstract class BuiltInFunctionNode : CallableNode
    {
        /// <summary>Base constructor for child class objects.</summary>
        /// <param name="functionType">Enum representing the type/ID for a given built-in-function. This allows for
        /// the associated function identifier to be retrieved from the <see cref="SyntaxSpec">.</param>
        public BuiltInFunctionNode(BuiltInFunctionID functionType) :
            base(SyntaxSpec.BuiltInFunctionIdentifiers[(byte)functionType], null) { }

        /// <summary>Returns an array containing a single instance of each built-in function node currently defined.</summary>
        public static BuiltInFunctionNode[] GetFunctionInstances() =>  new BuiltInFunctionNode[] { new PrintNode(), new ReadNode(), new ClearNode() } ;

        /// <summary>Calls, sends arguments, and executes the built-in function</summary>
        /// <param name="arguments">List containing argument data with the source of the parameter's
        /// value and whether or not the argument is marked as reference.</param>
        /// <returns>The interpreted return value of the built-in-function.</returns>
        public abstract Node? Call(List<(Node source, bool isRef)>? arguments);
    }
}
