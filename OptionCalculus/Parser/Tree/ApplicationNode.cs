namespace OptionCalculus.Parser.Tree {
    public sealed class ApplicationNode : ExpressionNode {
        public ApplicationNode(ExpressionNode option, ExpressionNode operand) {
            Option = option;
            Operand = operand;
        }

        public ExpressionNode Option { get; }

        public ExpressionNode Operand { get; }

        public override void Accept(INodeVisitor visitor) {
            visitor.VisitApplication(this);
        }
    }
}