using System;
using System.Reflection;
using System.Reflection.Emit;
using OptionCalculus.Parser.Tree;
using OptionCalculus.Runtime;

namespace OptionCalculus.Compiler {
    public sealed class TypeDefinitionPass : CompilerPass {
        public TypeDefinitionPass(Compiler compiler) : base(compiler) {
        }

        private void defineOptionType(OptionNode node) {
            var tb = Compiler.ModuleBuilder.DefineType("OptionCalculusCompiled.Option" + node.ID, TypeAttributes.Public | TypeAttributes.Sealed | TypeAttributes.Class, typeof(Option));

            var cb = tb.DefineConstructor(MethodAttributes.Public | MethodAttributes.ReuseSlot | MethodAttributes.HideBySig, CallingConventions.Standard, new Type[0]);
            var il = cb.GetILGenerator();
            il.Emit(OpCodes.Ldarg_0);
            il.Emit(OpCodes.Ldstr, node.Key.Ident);
            il.Emit(OpCodes.Ldc_I4, node.ID);
            il.Emit(OpCodes.Conv_U4);
            il.Emit(OpCodes.Call, typeof(Option).GetConstructor(new[] {typeof(string), typeof(uint)}));
            il.Emit(OpCodes.Ret);

            Compiler.OptionTypes[node.ID] = new OptionData(tb, cb);
        }

        public override void VisitOption(OptionNode node) {
            node.Case.Accept(this);
            node.CaseDecision.Accept(this);
            node.DefaultDecision.Accept(this);

            defineOptionType(node);
        }

        public override void VisitApplication(ApplicationNode node) {
            node.Option.Accept(this);
            node.Operand.Accept(this);
        }

        public override void VisitIdent(IdentNode node) {
        }
    }
}