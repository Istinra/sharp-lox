namespace Lox;


public class Lox
{
    static bool _hadError = false;
    public static int RunFile(string file)
    {
        string allText = File.ReadAllText(file);
        Run(allText);
        return _hadError ? 65 : 0;
    }
    
    public static void Run(string source)
    {
        Scanner scanner = new Scanner(source);
        List<Token> tokens = scanner.ScanTokens();
        Parser parser = new Parser(tokens);
        IExpr? expr = parser.Parse();
        Interpreter interpreter = new Interpreter();
        Console.WriteLine(interpreter.VisitBinaryExpr((BinaryExpr)expr));
        _hadError = false;
    }
    
    public static void Error(int line, string message) {
        Report(line, "", message);
    }
    
    public static void Error(Token token, string message) {
        if (token.Type == TokenType.EOF) {
            Report(token.Line, " at end", message);
        } else {
            Report(token.Line, " at '" + token.Lexeme + "'", message);
        }
    }

    private static void Report(int line, string where, string message) {
        Console.WriteLine("[line " + line + "] Error" + where + ": " + message);
        _hadError = true;
    }
}