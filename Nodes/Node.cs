using SkimSkript.Nodes.ValueNodes;

namespace SkimSkript.Nodes
{
    /// <summary>Abstract class meant to represent a single node within an abstract syntax tree (AST). 
    /// Every node within an AST will be derived from this class directly or indirectly.</summary>
    /// <remarks>Instances of child classes will be instantiated during the parsing stage and accessed during the
    /// semantic analysis and interpretation stages.</remarks>
    public abstract class Node
    {
        /// <summary>Explicitly defined in each child class to create a visualization of the AST for debugging.</summary>
        public abstract override string ToString();
    }
}
