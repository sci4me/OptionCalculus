using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using OptionCalculus.Parser.Tree;
using OptionCalculus.Runtime;

namespace OptionCalculus.Compiler {
    public sealed class TypeImplementationPass : CompilerPass {
        public TypeImplementationPass(Compiler compiler) : base(compiler) {
        }

        private void generateOption(ILGenerator il, OptionNode node, Dictionary<string, LocalBuilder> locals) {
            var nodeData = Compiler.OptionTypes[node.ID];
            il.Emit(OpCodes.Newobj, nodeData.ConstructorBuilder);
        }

        private void generateApplication(ILGenerator il, ApplicationNode node, Dictionary<string, LocalBuilder> locals) {
            il.Emit(OpCodes.Ldarg_1);
            il.Emit(OpCodes.Call, typeof(Scope<Option>).GetMethod("Push"));

            var optionLb = il.DeclareLocal(typeof(Option));
            optionLb.SetLocalSymInfo("option");

            var operandLb = il.DeclareLocal(typeof(Option));
            operandLb.SetLocalSymInfo("operand");

            generateExpression(il, node.Option, locals);
            il.Emit(OpCodes.Stloc, optionLb);

            generateExpression(il, node.Operand, locals);
            il.Emit(OpCodes.Stloc, operandLb);

            il.Emit(OpCodes.Ldarg_1);
            il.Emit(OpCodes.Ldloc, optionLb);
            il.Emit(OpCodes.Call, typeof(Option).GetMethod("get_Key"));
            il.Emit(OpCodes.Ldloc, operandLb);
            il.Emit(OpCodes.Call, typeof(Scope<Option>).GetMethod("Bind"));

            il.Emit(OpCodes.Ldloc, optionLb);
            il.Emit(OpCodes.Ldarg_1);
            il.Emit(OpCodes.Ldloc, operandLb);
            il.Emit(OpCodes.Callvirt, typeof(Option).GetMethod("Eval"));

            var tempLb = il.DeclareLocal(typeof(Option));
            tempLb.SetLocalSymInfo("temp");

            il.Emit(OpCodes.Stloc, tempLb);

            il.Emit(OpCodes.Ldarg_1);
            il.Emit(OpCodes.Call, typeof(Scope<Option>).GetMethod("Pop"));

            il.Emit(OpCodes.Ldloc, tempLb);
        }

        private void generateIdent(ILGenerator il, IdentNode node, Dictionary<string, LocalBuilder> locals) {
            if (locals.ContainsKey(node.Ident)) {
                il.Emit(OpCodes.Ldloc, locals[node.Ident]);
            } else {
                il.Emit(OpCodes.Ldarg_2);
            }
        }

        private void generateExpression(ILGenerator il, ExpressionNode expr, Dictionary<string, LocalBuilder> locals) {
            if (expr is OptionNode) {
                generateOption(il, (OptionNode) expr, locals);
                return;
            }

            if (expr is ApplicationNode) {
                generateApplication(il, (ApplicationNode) expr, locals);
                return;
            }

            if (expr is IdentNode) {
                generateIdent(il, (IdentNode) expr, locals);
                return;
            }

            throw new Exception(expr?.ToString() ?? "null");
        }

        private Dictionary<string, LocalBuilder> generateLocals(ILGenerator il, OptionNode node) {
            var locals = new Dictionary<string, LocalBuilder>();

            var idents = new List<string>();

            var @case = node.Case as IdentNode;
            if (@case != null && !idents.Contains(@case.Ident)) {
                idents.Add(@case.Ident);
            }

            var caseDecision = node.CaseDecision as IdentNode;
            if (caseDecision != null && !idents.Contains(caseDecision.Ident)) {
                idents.Add(caseDecision.Ident);
            }

            var defaultDecision = node.DefaultDecision as IdentNode;
            if (defaultDecision != null && !idents.Contains(defaultDecision.Ident)) {
                idents.Add(defaultDecision.Ident);
            }

            if (idents.Contains(node.Key.Ident)) {
                idents.Remove(node.Key.Ident);
            }

            foreach (var s in idents) {
                var lb = il.DeclareLocal(typeof(Option));
                lb.SetLocalSymInfo(s);

                il.Emit(OpCodes.Ldarg_1);
                il.Emit(OpCodes.Ldstr, s);
                il.Emit(OpCodes.Call, typeof(Scope<Option>).GetMethod("Lookup"));
                il.Emit(OpCodes.Stloc, lb);

                locals[s] = lb;
            }

            return locals;
        }

        private void implementOptionType(OptionNode node) {
            var nodeData = Compiler.OptionTypes[node.ID];
            var tb = nodeData.TypeBuilder;

            var mb = tb.DefineMethod("Eval", MethodAttributes.Public | MethodAttributes.ReuseSlot | MethodAttributes.HideBySig | MethodAttributes.Virtual, typeof(Option), new[] {typeof(Scope<Option>), typeof(Option)});
            mb.DefineParameter(1, ParameterAttributes.None, "scope");
            mb.DefineParameter(2, ParameterAttributes.None, node.Key.Ident);

            var getID = typeof(Option).GetMethod("get_ID");

            var il = mb.GetILGenerator();

            var locals = generateLocals(il, node);

            var paramNullLabel = il.DefineLabel();

            il.Emit(OpCodes.Ldarg_2);
            il.Emit(OpCodes.Ldnull);
            il.Emit(OpCodes.Beq_S, paramNullLabel);

            var caseDecisionLabel = il.DefineLabel();

            il.Emit(OpCodes.Ldarg_2);
            il.Emit(OpCodes.Call, getID);
            generateExpression(il, node.Case, locals);
            il.Emit(OpCodes.Call, getID);
            il.Emit(OpCodes.Beq_S, caseDecisionLabel);

            generateExpression(il, node.DefaultDecision, locals);
            il.Emit(OpCodes.Ret);

            il.MarkLabel(caseDecisionLabel);
            generateExpression(il, node.CaseDecision, locals);
            il.Emit(OpCodes.Ret);

            il.MarkLabel(paramNullLabel);
            il.Emit(OpCodes.Ldnull);
            il.Emit(OpCodes.Ret);
        }

        public override void VisitOption(OptionNode node) {
            node.Case.Accept(this);
            node.CaseDecision.Accept(this);
            node.DefaultDecision.Accept(this);

            implementOptionType(node);
        }

        public override void VisitApplication(ApplicationNode node) {
            node.Option.Accept(this);
            node.Operand.Accept(this);
        }

        public override void VisitIdent(IdentNode node) {
        }
    }
}