namespace Lox;

public class Environment
{
    private readonly IDictionary<string, object?> _values = new Dictionary<string, object?>();

    private readonly Environment? _enclosing;


    public Environment()
    {
        _enclosing = null;
    }

    public Environment(Environment enclosing)
    {
        _enclosing = enclosing;
    }

    public void Define(string name, object? value, bool initialised)
    {
        _values[name] = value;
    }

    public object? Get(Token name)
    {
        if (_values.TryGetValue(name.Lexeme, out object? value))
        {
            return value;
        }

        if (_enclosing != null)
            return _enclosing.Get(name);

        throw new Exception("Variable undefined " + name.Lexeme);
    }

    public object? GetAt(int distance, string nameLexeme)
    {
        return Ancestor(distance)._values.TryGetValue(nameLexeme, out object? value) ? value : null;
    }
    
    private Environment Ancestor(int distance) {
        Environment environment = this;
        for (int i = 0; i < distance; i++)
        {
            if (environment._enclosing != null) environment = environment._enclosing;
        }

        return environment;
    }

    public void Assign(Token name, object? value)
    {
        if (_values.ContainsKey(name.Lexeme))
        {
            _values[name.Lexeme] = value;
            return;
        }

        if (_enclosing != null)
        {
            _enclosing.Assign(name, value);
            return;
        }

        throw new RuntimeError(name, "Undefined variable '" + name.Lexeme + "'.");
    }

    public void AssignAt(int distance, Token assignExprName, object value)
    {
        Ancestor(distance)._values[assignExprName.Lexeme] = value;
    }
}