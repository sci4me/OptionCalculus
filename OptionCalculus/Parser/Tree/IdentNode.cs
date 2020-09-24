namespace OptionCalculus.Parser.Tree {
    public sealed class IdentNode : ExpressionNode {
        public IdentNode(string ident) {
            Ident = ident;
        }

        public string Ident { get; }

        public override void Accept(INodeVisitor visitor) {
            visitor.VisitIdent(this);
        }
    }
}