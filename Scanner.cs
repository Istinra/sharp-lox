namespace Lox;

public class Scanner
{
    private readonly string source;

    public Scanner(string source)
    {
        this.source = source;
    }

    public List<Token> ScanTokens()
    {
        return new List<Token>();
    }
}