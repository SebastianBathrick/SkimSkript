using SkimSkript.ErrorHandling;
using SkimSkript.Nodes;
using SkimSkript.Nodes.Runtime;

namespace SkimSkript.Interpretation.Helpers
{
    /// <summary> Module that changes scope and retrieves in-scope values/references based on associated identifiers runtime. </summary>
    public class ScopeManager
    {
        private StackFrame _topLevelFrame;
        private Stack<StackFrame> _functionCallStackFrames;
        private bool _isTopLevel;

        /// <summary> Topmost frame stored in the stack. Meaning, the most deeply nested scope. </summary>
        private StackFrame CurrentStackFrame => (_isTopLevel ? _topLevelFrame : _functionCallStackFrames.Peek());

        /// <summary> Instantiates the object and sets up and enters the top-level scope. </summary>
        public ScopeManager()
        {
            _isTopLevel = true;
            _functionCallStackFrames = new Stack<StackFrame>();
            _topLevelFrame = new StackFrame();
        }

        #region Variable Methods
        // TODO: Check using semantic analysis in order to enforce the type when referencing a
        // variable w/ a parameter.

        /// <summary> Adds a <see cref="VariableNode"/> associated with an iddentifier to the current scope. </summary>
        public void AddVariable(string identifier, Node value, Type dataType) =>
            CurrentStackFrame.AddVariable(identifier, value, dataType);

        public void AssignValueToVariable(string identifier, Node value)
        {
            if (CurrentStackFrame.GetVariablePointer(identifier) is not null)
                CurrentStackFrame.AssignValueToVariable(identifier, value);
            else if (_topLevelFrame.GetVariablePointer(identifier) is not null)
                _topLevelFrame.AssignValueToVariable(identifier, value);
            else
                throw UnknownIdentifierError(identifier);
        }

        public Node GetVariableValueCopy(string identifier)
        {
            var valueCopy = CurrentStackFrame.GetVariableValueCopy(identifier);
            if (valueCopy != null) return valueCopy;

            valueCopy = _topLevelFrame.GetVariableValueCopy(identifier);
            if (valueCopy != null) return valueCopy;

            throw UnknownIdentifierError(identifier);
        }

        public Type GetVariableDataType(string identifier)
        {
            var type = CurrentStackFrame.GetVariableDataType(identifier);
            if (type != null) return type;

            type = _topLevelFrame.GetVariableDataType(identifier);
            if (type != null) return type;

            throw UnknownIdentifierError(identifier);
        }

        public Node GetVariablePointer(string identifier)
        {
            var pointer = CurrentStackFrame.GetVariablePointer(identifier);
            if (pointer != null) return pointer;

            pointer = _topLevelFrame.GetVariablePointer(identifier);
            if (pointer != null) return pointer;

            throw UnknownIdentifierError(identifier);
        }
        #endregion

        /// <summary> Exits the current function scope and switches to the next on the stack. </summary>
        public void EnterScope() => CurrentStackFrame.EnterScope();

        /// <summary> Enters a new function scope. Meaning, what will at that time be the most deeply nested scope. </summary>
        public void ExitScope() => CurrentStackFrame.ExitScope();

        /// <summary> Enters a new function scope one level deeper that has access to all previous scopes. </summary>
        public void EnterFunctionScope()
        {
            _isTopLevel = false;
            _functionCallStackFrames.Push(new StackFrame());
        }

        /// <summary> Goes to the previous function scope level and removes the current level. </summary>
        public void ExitFunctionScope()
        {
            _functionCallStackFrames.Pop();
            _isTopLevel = _functionCallStackFrames.Count == 0;
        }

        private RuntimeException UnknownIdentifierError(string identifier) =>
            new RuntimeException("Unknown identifier {Identifier}", null, identifier);
    }
}
