namespace OptionCalculus.Parser.Tree {
    public interface INodeVisitable {
        void Accept(INodeVisitor visitor);
    }
}