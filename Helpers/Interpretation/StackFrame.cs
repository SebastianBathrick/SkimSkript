using SkimSkript.Nodes.Runtime;
using SkimSkript.ErrorHandling;
using SkimSkript.Nodes;

namespace SkimSkript.Interpretation.Helpers
{
    using VariableMetadata = (int level, string identifier);

    /// <summary> Container for a top-level scope or function's scope. </summary>
    public class StackFrame
    {
        #region Data Members & Properties
        private const int INIT_CAP = 10; //Initial dictionary cap
       
        private int _currBlockLevel = 0;
        private Dictionary<VariableMetadata, VariableNode>? _frameDict;

        private Dictionary<VariableMetadata, VariableNode> FrameDictionary => _frameDict ??= new(INIT_CAP);
        #endregion

        #region Scope Management
        public void EnterScope() => _currBlockLevel++;

        public void ExitScope()
        {
            if (_frameDict == null)
                return;

            foreach (var item in _frameDict)
                if (item.Key.level == _currBlockLevel)
                    _frameDict.Remove(item.Key);

            _currBlockLevel--;
        }
        #endregion

        #region Variable Getter Methods
        public Node? GetVariablePointer(string identifier)
        {
            var variable = GetVariable(identifier);
            return variable?.Value;
        }

        public Node? GetVariableValueCopy(string identifier)
        {
            var variable = GetVariable(identifier);
            if (variable?.Value is ValueNode pointer)
            {
                return pointer.Copy();
            }

            return null;
        }

        public Type? GetVariableDataType(string identifier)
        {
            var variable = GetVariable(identifier);
            return variable?.DataType;
        }

        /// <summary> Search from the lowest to highest depths to get variable associated with identifier </summary>
        /// <exception cref="UnknownIdentifierError"> Given identifier was not found in stack frame.</exception>
        private VariableNode? GetVariable(string identifier)
        {
            // TODO: Refactor to increase efficiency
            for (int i = _currBlockLevel; i >= 0; i--)
            {
                if (FrameDictionary.TryGetValue((i, identifier), out VariableNode? var))
                    return var;
            }

            return null;
        }
        #endregion


        #region Variable Manipulation Methods
        /// <summary> Adds a variable to the most deeply nested scope in the stack frame. </summary>
        public void AddVariable(string identifier, Node value, Type dataType) =>
            FrameDictionary.Add((_currBlockLevel, identifier), new VariableNode(value, dataType));

        /// <summary> Updates assignNode of a variable in the current scope. </summary>
        /// <remarks>Maintains the same assignNode node pointer.</remarks>
        public void AssignValueToVariable(string identifier, Node assignNode)
        {
            var variable = GetVariable(identifier);

            if (variable == null)
                throw new RuntimeException("Unknown identifier {Identifier}", identifier);

            if (assignNode is not ValueNode valueNode)
                throw new InvalidDataException($"Cannot assign {assignNode.GetType().Name} to variable {identifier}");

            var pointer = (ValueNode)variable.Value;

            pointer.AssignValue(valueNode);
        }
        #endregion

    }
}
