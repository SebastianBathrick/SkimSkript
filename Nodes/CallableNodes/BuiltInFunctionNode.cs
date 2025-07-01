using SkimSkript.Syntax;

namespace SkimSkript.Nodes
{
    /// <summary>Abstract class representing a built-in function node with predefined behavior and parameters.</summary>
    public abstract class BuiltInFunctionNode : CallableNode
    {
        /// <summary>Base constructor for child class objects.</summary>
        /// <param _name="builtInFunctionId">Enum representing the type/ID for a given built-in-function. This allows for
        /// the associated function identifier to be retrieved from the <see cref="SyntaxSpec">.</param>
        public BuiltInFunctionNode(
            BuiltInFunctionID builtInFunctionId, 
            Type? returnType = null, 
            Node[]? parameters = null, bool isVariadic = false) 
            : base(
                  new IdentifierNode(SyntaxSpec.BuiltInFunctionIdentifiers[(byte)builtInFunctionId]), 
                  parameters, returnType, isVariadic) { } // TODO: Replace hardcoded identifier node

        public static BuiltInFunctionNode[] GetFunctionInstances() =>  new BuiltInFunctionNode[] { new PrintNode(), new ReadNode(), new ClearNode() } ;

        public abstract Node? Call(Node[]? arguments);

        protected ValueNode[] ConvertArgumentsToValue(Node[] arguments)
        {
            var valueNodes = new ValueNode[arguments.Length];
            for(int i = 0; i < arguments.Length; i++)
                valueNodes[i] = (ValueNode)arguments[i];   
            return valueNodes;
        }
    }
}
