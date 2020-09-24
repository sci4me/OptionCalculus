namespace OptionCalculus.Parser.Tree {
    public interface INodeVisitor {
        void VisitOption(OptionNode node);

        void VisitApplication(ApplicationNode node);

        void VisitIdent(IdentNode node);
    }
}