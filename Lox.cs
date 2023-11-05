namespace Lox;

public class Lox
{
    private static bool _hadError = false;
    private static bool _hadRuntimeError = false;
    private static readonly Interpreter Interpreter = new Interpreter();

    public static int RunFile(string file)
    {
        string allText = File.ReadAllText(file);
        Run(allText);
        if (_hadRuntimeError)
            return 70;
        return _hadError ? 65 : 0;
    }

    public static void Run(string source)
    {
        Scanner scanner = new Scanner(source);
        List<Token> tokens = scanner.ScanTokens();
        Parser parser = new Parser(tokens);
        List<IStmt> stmts = parser.Parse();
        Interpreter.Interpret(stmts);
        _hadError = false;
    }

    public static void Error(int line, string message)
    {
        Report(line, "", message);
    }

    public static void Error(Token token, string message)
    {
        if (token.Type == TokenType.EOF)
        {
            Report(token.Line, " at end", message);
        }
        else
        {
            Report(token.Line, " at '" + token.Lexeme + "'", message);
        }
    }

    public static void RuntimeError(RuntimeError error)
    {
        Console.WriteLine(error.Message +
                          "\n[line " + error.Token.Line + "]");
        _hadRuntimeError = true;
    }

    private static void Report(int line, string where, string message)
    {
        Console.WriteLine("[line " + line + "] Error" + where + ": " + message);
        _hadError = true;
    }
}