using SkimSkript.Nodes.ValueNodes;

namespace SkimSkript.Core.Helpers.Interpreter
{
    internal enum BlockExitType { StatementsExhausted, ReturnStatement}

    internal class BlockExitData
    {
        private readonly BlockExitType _exitType;
        private readonly ValueNode? _returnData = null;

        public bool IsReturnData => _returnData != null;

        public ValueNode ReturnData => _returnData!;

        public BlockExitType ExitType => _exitType;

        public BlockExitData(BlockExitType exitType, ValueNode? returnData)
        {
            _exitType = exitType;
            _returnData = returnData;
        }

        public BlockExitData(BlockExitType exitType) => _exitType = exitType;
    }
}
