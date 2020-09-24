namespace OptionCalculus.Runtime {
    public abstract class Option {
        public Option(string key, uint id) {
            ID = id;
            Key = key;
        }

        public abstract Option Eval(Scope<Option> scope, Option param);

        public uint ID { get; }

        public string Key { get; }

        public override string ToString() {
            return ID.ToString();
        }
    }
}