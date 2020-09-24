using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using OptionCalculus.Parser.Tree;
using OptionCalculus.Runtime;

namespace OptionCalculus.Compiler {
    public sealed class Compiler {
        private readonly List<CompilerPass> passes;

        public Compiler() {
            var name = new AssemblyName {Name = "OptionCalculusCompiled"};
            AssemblyBuilder = AppDomain.CurrentDomain.DefineDynamicAssembly(name, AssemblyBuilderAccess.RunAndCollect);
            ModuleBuilder = AssemblyBuilder.DefineDynamicModule(name.Name, name.Name + ".dll", true);

            OptionTypes = new Dictionary<uint, OptionData>();

            passes = new List<CompilerPass> {new TypeDefinitionPass(this), new TypeImplementationPass(this)};
        }

        public AssemblyBuilder AssemblyBuilder { get; }

        public ModuleBuilder ModuleBuilder { get; }

        public Dictionary<uint, OptionData> OptionTypes { get; }

        public Func<Option> Compile(ApplicationNode node) {
            if (!(node.Option is OptionNode) || !(node.Operand is OptionNode)) {
                throw new Exception("Invalid AST");
            }

            passes.ForEach(node.Accept);
            foreach (var tb in OptionTypes.Values) tb.TypeBuilder.CreateType();

            //AssemblyBuilder.Save(AssemblyBuilder.GetName().Name + ".dll");

            var optionID = ((OptionNode) node.Option).ID;
            var operandID = ((OptionNode) node.Operand).ID;

            var optionType = AssemblyBuilder.GetType("OptionCalculusCompiled.Option" + optionID);
            var operandType = AssemblyBuilder.GetType("OptionCalculusCompiled.Option" + operandID);

            var option = (Option) Activator.CreateInstance(optionType);
            var operand = (Option) Activator.CreateInstance(operandType);

            return () => {
                var scope = new Scope<Option>();
                scope.Bind(option.Key, operand);
                return option.Eval(scope, operand);
            };
        }
    }
}