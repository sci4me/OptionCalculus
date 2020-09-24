using System;
using System.IO;
using OptionCalculus.Compiler;
using OptionCalculus.Lexer;
using OptionCalculus.Parser;

public static class Program {
    public static void Main(string[] args) {
        var source = File.ReadAllText(args[0]);

        var lexer = new Lexer(source);
        var parser = new Parser(lexer.Tokenize());
        var ast = parser.Parse();

        var compiler = new Compiler();
        var program = compiler.Compile(ast);

        Console.WriteLine(program());
    }
}