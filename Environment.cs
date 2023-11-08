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

    public void Assign(Token name, object? value)
    {
        if (_values.ContainsKey(name.Lexeme))
        {
            _values[name.Lexeme] = value;
            return;
        }

        throw new RuntimeError(name, "Undefined variable '" + name.Lexeme + "'.");
    }
}