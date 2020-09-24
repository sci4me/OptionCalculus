namespace OptionCalculus.Parser.Tree {
    public abstract class ExpressionNode : INodeVisitable {
        public abstract void Accept(INodeVisitor visitor);
    }
}