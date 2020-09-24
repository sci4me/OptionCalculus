using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using OptionCalculus.Lexer;
using OptionCalculus.Parser.Tree;

namespace OptionCalculus.Parser {
    public sealed class Parser {
        private List<Token> tokens;
        private int index;

        public Parser(List<Token> tokens) {
            this.tokens = tokens;
        }

        private void next() {
            index++;
        }

        private void prev() {
            index--;
        }

        private Token current() {
            if (!more()) {
                throw new ParseException("No more tokens");
            }

            return tokens[index];
        }

        private bool more() {
            return index < tokens.Count;
        }

        private bool accept(params TokenType[] types) {
            if (!more()) return false;
            return types.Contains(current().Type);
        }

        private void expect(params TokenType[] types) {
            if (!accept(types)) {
                var sb = new StringBuilder();

                foreach (var type in types) {
                    sb.Append(type);
                    sb.Append(" ");
                }

                throw new ParseException("Unexpected token: " + current() + " Possible types: " + sb.ToString());
            }
        }

        private void expectNot(params TokenType[] types) {
            if (accept(types)) {
                throw new ParseException("Unexpected token: " + current());
            }
        }

        private IdentNode parseIdent() {
            expect(TokenType.IDENT);
            var ident = new IdentNode(current().Data);
            next();
            return ident;
        }

        private OptionNode parseOption() {
            expect(TokenType.OPTION);
            next();

            var key = parseIdent();

            expect(TokenType.EQUALS);
            next();

            var @case = parseExpression();

            expect(TokenType.QUESTION);
            next();

            var caseDecision = parseExpression();

            expect(TokenType.COLON);
            next();

            return new OptionNode(key, @case, caseDecision, parseExpression());
        }

        private ExpressionNode parseAtom() {
            expect(TokenType.LPAREN, TokenType.OPTION, TokenType.IDENT);
            if (accept(TokenType.LPAREN)) {
                next();
                var expr = parseExpression();
                expect(TokenType.RPAREN);
                next();
                return expr;
            }

            if(accept(TokenType.OPTION)) {
                return parseOption();
            }

            return parseIdent();
        }

        private ExpressionNode parseApplication() {
            var ret = parseAtom();

            while (more() && !accept(TokenType.RPAREN, TokenType.COLON, TokenType.QUESTION)) {
                ret = new ApplicationNode(ret, parseAtom());
            }

            return ret;
        }

        private ExpressionNode parseExpression() {
            return parseApplication();
        }

        public ApplicationNode Parse() {
            return (ApplicationNode)parseApplication();
        }
    }
}