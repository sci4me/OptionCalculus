namespace OptionCalculus.Lexer {
    public sealed class Token {
        public Token(TokenType type, string data, uint line, uint column) {
            Type = type;
            Data = data;
            Line = line;
            Column = column;
        }

        public TokenType Type { get; }

        public string Data { get; }

        public uint Line { get; }

        public uint Column { get; }

        public override string ToString() {
            return Data + " (" + Type + ") at line " + Line + ", column " + Column;
        }
    }
}