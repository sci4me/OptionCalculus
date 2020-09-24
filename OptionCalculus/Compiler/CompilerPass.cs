using OptionCalculus.Parser.Tree;

namespace OptionCalculus.Compiler {
    public abstract class CompilerPass : INodeVisitor {
        public CompilerPass(Compiler compiler) {
            Compiler = compiler;
        }

        public Compiler Compiler { get; }

        public abstract void VisitOption(OptionNode node);

        public abstract void VisitApplication(ApplicationNode node);

        public abstract void VisitIdent(IdentNode node);
    }
}