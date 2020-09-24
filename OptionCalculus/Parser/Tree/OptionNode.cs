namespace OptionCalculus.Parser.Tree {
    public sealed class OptionNode : ExpressionNode {
        private static uint currentID;

        public OptionNode(IdentNode key, ExpressionNode @case, ExpressionNode caseDecision,
            ExpressionNode defaultDecision) {
            Key = key;
            Case = @case;
            CaseDecision = caseDecision;
            DefaultDecision = defaultDecision;
            ID = currentID++;
        }

        public IdentNode Key { get; }

        public ExpressionNode Case { get; }

        public ExpressionNode CaseDecision { get; }

        public ExpressionNode DefaultDecision { get; }

        public uint ID { get; }

        public override void Accept(INodeVisitor visitor) {
            visitor.VisitOption(this);
        }
    }
}