using SkimSkript.Nodes.Runtime;
using SkimSkript.ErrorHandling;

namespace SkimSkript.Interpretation.Helpers
{
    /// <summary> Container for a top-level scope or function's scope. </summary>
    public class StackFrame
    {
        private const int INIT_CAP = 10; //Initial dictionary cap

        private Dictionary<(int level, string identifier), VariableNode> _frameDict;
        private int _currBlockLevel = 0;

        /// <summary> Enter a block scope within the same frame and travel downwards one level. </summary>
        public void EnterScope() => _currBlockLevel++;

        /// <summary> Removes the current block level and goes back to the previous. </summary>
        public void ExitScope()
        {
            if (_frameDict == null)
                return;

            foreach (var item in _frameDict)
                if (item.Key.level == _currBlockLevel)
                    _frameDict.Remove(item.Key);

            _currBlockLevel--;
        }

        /// <summary> Adds a variable to the most deeply nested scope in the stack frame. </summary>
        public VariableNode AddVariable(string identifier, VariableNode variable)
        {
            _frameDict ??= GetNewFrameDict();
            _frameDict.Add((_currBlockLevel, identifier), variable);
            return variable;
        }

        /// <summary> Search from the lowest to highest depths to get variable associated with identifier </summary>
        public VariableNode? GetVariable(string identifier)
        {
            _frameDict ??= GetNewFrameDict();

            for (int i = _currBlockLevel; i >= 0; i--)
                if(_frameDict.TryGetValue((i, identifier), out VariableNode? var))
                    return var;

            throw new UnknownIdentifierError(identifier);
        }

        /// <summary> Returns a new frame dictionary. Intended to be called when one is not already present. </summary>
        private Dictionary<(int, string), VariableNode> GetNewFrameDict() =>
            new Dictionary<(int, string), VariableNode>(INIT_CAP);
    }
}
