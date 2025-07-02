using SkimSkript.Nodes;

namespace SkimSkript.Interpretation.Helpers
{
    internal enum BlockExitType { StatementsExhausted, ReturnStatement }

    /// <summary> Contains info about how a block exited and any data being transmitted out of the block. </summary>
    internal class BlockExitData
    {
        private readonly BlockExitType _exitType;
        private readonly Node? _returnData = null;


        public Node? ReturnData => _returnData;

        public BlockExitType ExitType => _exitType;

        public BlockExitData(BlockExitType exitType, Node? returnData)
        {
            _exitType = exitType;
            _returnData = returnData;
        }

        public BlockExitData(BlockExitType exitType) => _exitType = exitType;
    }
}
