using System;

namespace OptionCalculus.Parser.Tree.Util {
    public sealed class PrintASTVisitor : INodeVisitor {
        private int depth;

        private void writeTabs() {
            Console.Write(new String(' ', depth * 4));
        }

        private void write(string s) {
            Console.Write(s);
        }

        private void writeLine(string s) {
            Console.WriteLine(s);
        }

        public void VisitOption(OptionNode node) {
            writeLine("option {");

            depth++;

            writeTabs();
            writeLine("id: " + node.ID);

            writeTabs();
            write("key: ");
            node.Key.Accept(this);

            writeTabs();
            write("case: ");
            node.Case.Accept(this);

            writeTabs();
            write("case_decision: ");
            node.CaseDecision.Accept(this);

            writeTabs();
            write("case_default: ");
            node.DefaultDecision.Accept(this);

            depth--;

            writeTabs();
            writeLine("}");
        }

        public void VisitApplication(ApplicationNode node) {
            writeLine("application {");

            depth++;

            writeTabs();
            write("option: ");
            node.Option.Accept(this);

            writeTabs();
            write("operand: ");
            node.Operand.Accept(this);

            depth--;

            writeTabs();
            writeLine("}");
        }

        public void VisitIdent(IdentNode node) {
            writeLine(node.Ident);
        }
    }
}