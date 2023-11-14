namespace Lox;

public class Environment
{
    private readonly IDictionary<string, ValueWrapper> _values = new Dictionary<string, ValueWrapper>();

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
        _values[name] = new ValueWrapper(initialised, value);
    }

    public object? Get(Token name)
    {
        if (_values.TryGetValue(name.Lexeme, out ValueWrapper value))
        {
            if (!value.Initialised) 
                throw new RuntimeError(name, "Uninitialised Variable '" + name.Lexeme + "'.");
            return value.Value;
        }
        if (_enclosing != null)
            return _enclosing.Get(name);

        throw new Exception("Variable undefined " + name.Lexeme);
    }

    public void Assign(Token name, object? value)
    {
        if (_values.TryGetValue(name.Lexeme, out ValueWrapper valueWrapper))
        {
            valueWrapper.Initialised = true;
            valueWrapper.Value = value;
            _values[name.Lexeme] = valueWrapper;
            return;
        }

        if (_enclosing != null)
        {
            _enclosing.Assign(name, value);
            return;
        }

        throw new RuntimeError(name, "Undefined variable '" + name.Lexeme + "'.");
    }
}

internal struct ValueWrapper
{
    public ValueWrapper(bool initialised, object? value)
    {
        Initialised = initialised;
        Value = value;
    }

    public bool Initialised { get; set; }
    public object? Value { get; set; }
}