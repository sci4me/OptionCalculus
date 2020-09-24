using System.Reflection.Emit;

namespace OptionCalculus.Compiler {
    public struct OptionData {
        public OptionData(TypeBuilder typeBuilder, ConstructorBuilder constructorBuilder) {
            TypeBuilder = typeBuilder;
            ConstructorBuilder = constructorBuilder;
        }

        public TypeBuilder TypeBuilder { get; }

        public ConstructorBuilder ConstructorBuilder { get; }
    }
}