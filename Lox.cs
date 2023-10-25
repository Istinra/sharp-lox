namespace Lox;


public class Lox
{
    static bool hadError = false;
    public static int RunFile(string file)
    {
        string allText = File.ReadAllText(file);
        Run(allText);
        return hadError ? 65 : 0;
    }
    
    public static void Run(string source)
    {
        Scanner scanner = new Scanner(source);
        List<Token> tokens = scanner.ScanTokens();
        
        hadError = false;
    }
    
    public static void Error(int line, string message) {
        Report(line, "", message);
    }

    private static void Report(int line, string where, string message) {
        Console.WriteLine("[line " + line + "] Error" + where + ": " + message);
        hadError = true;
    }
}