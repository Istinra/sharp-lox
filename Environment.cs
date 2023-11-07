namespace Lox;

public class Environment
{
    private readonly IDictionary<string, object?> _values = new Dictionary<string, object?>();

    public void Define(string name, object? value)
    {
        _values[name] = value;
    }

    public object? Get(Token name)
    {
        if (!_values.ContainsKey(name.Lexeme))
            throw new Exception("Variable undefined " + name.Lexeme);
        return _values[name.Lexeme];
    }
}