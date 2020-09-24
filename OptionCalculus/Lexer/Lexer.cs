using System.Collections.Generic;
using System.Linq;

namespace OptionCalculus.Lexer {
    public sealed class Lexer {
        private string source;
        private int start;
        private int pos;
        private List<Token> tokens;
        private uint line;
        private uint column;

        public Lexer(string source) {
            this.source = source;
            tokens = new List<Token>();
        }

        private string current() {
            return source.Substring(start, pos - start);
        }

        private void emit(TokenType type) {
            tokens.Add(new Token(type, current(), line + 1, (uint)(column - (pos - start))));
            start = pos;
        }

        private char next() {
            if (!more()) {
                return (char)0;
            }

            var c = source[pos];
            pos++;
            column++;
            return c;
        }

        private void prev() {
            pos--;
            column--;
        }

        private char peek() {
            if (!more()) {
                return (char)0;
            }

            return source[pos];
        }

        private void ignore() {
            start = pos;
        }

        private bool accept(string valid) {
            if (!more()) {
                return false;
            }

            if (valid.Contains(next())) {
                return true;
            }

            prev();
            return false;
        }

        private void acceptRun(string valid) {
            while (more() && valid.Contains(peek())) {
                next();
            }
        }

        private bool acceptSeq(string seq) {
            int savedPos = pos;
            uint savedColumn = column;

            for (int i = 0; i < seq.Length; i++) {
                if (seq[i] != next()) {
                    pos = savedPos;
                    column = savedColumn;
                    return false;
                }
            }

            return true;
        }

        private bool more() {
            return pos < source.Length;
        }

        public List<Token> Tokenize() {
            bool running = true;
            while (running) {
                var c = next();
                switch (c) {
                    case (char) 0:
                        running = false;
                        break;
                    case '\n':
                        ignore();
                        line++;
                        column = 0;
                        break;
                    case '|':
                        emit(TokenType.OPTION);
                        break;
                    case '=':
                        emit(TokenType.EQUALS);
                        break;
                    case ':':
                        emit(TokenType.COLON);
                        break;
                    case '?':
                        emit(TokenType.QUESTION);
                        break;
                    case '(':
                        emit(TokenType.LPAREN);
                        break;
                    case ')':
                        emit(TokenType.RPAREN);
                        break;
                    default:
                        if (char.IsLetter(c) || c == '_') {
                            while (more() && (char.IsLetterOrDigit(peek()) || peek() == '_')) {
                                next();
                            }

                            emit(TokenType.IDENT);
                        } else if (char.IsWhiteSpace(c)) {
                            while (more() && char.IsWhiteSpace(peek())) {
                                if (peek() == '\n') {
                                    line++;
                                    column = 0;
                                }

                                next();
                            }
                            ignore();
                        } else {
                            throw new LexException("Unexpected character '" + c + "'");
                        }
                        break;
                }
            }

            return tokens;
        }
    }
}