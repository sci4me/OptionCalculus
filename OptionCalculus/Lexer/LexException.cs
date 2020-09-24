using System;

namespace OptionCalculus.Lexer {
    [Serializable]
    public sealed class LexException : Exception {
        public LexException(string message) : base(message) {
        }
    }
}