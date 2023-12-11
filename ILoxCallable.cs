namespace Lox;

public interface ILoxCallable
{
    object Call(Interpreter interpreter, List<object> arguments);
    int Arity();
}

public class ClockCallable : ILoxCallable
{
    private static readonly DateTime Jan1St1970 = new DateTime
        (1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);

    public static long CurrentTimeMillis()
    {
        return (long) (DateTime.UtcNow - Jan1St1970).TotalMilliseconds;
    }
    
    public object Call(Interpreter interpreter, List<object> arguments)
    {
        return (DateTime.UtcNow - Jan1St1970).TotalMilliseconds / 1000;
    }

    public int Arity()
    {
        return 0;
    }
}

public class LoxFunction : ILoxCallable
{
    private readonly FunctionStmt _declaration;
    private readonly Environment _closure;

    public LoxFunction(FunctionStmt declaration, Environment closure)
    {
        _declaration = declaration;
        _closure = closure;
    }

    public object Call(Interpreter interpreter, List<object> arguments)
    {
        Environment environment = new Environment(_closure);
        for (var i = 0; i < _declaration.Parameters.Count; i++)
        {
            environment.Define(_declaration.Parameters[i].Lexeme, arguments[i], true);
        }

        try
        {
            interpreter.ExecuteBlock(_declaration.Body, environment);
        }
        catch (ReturnException re)
        {
            return re.Value;
        }
        return null;
    }

    public int Arity()
    {
        return _declaration.Parameters.Count;
    }
}

public class ReturnException : SystemException
{
    public object? Value { get; }

    public ReturnException()
    {
    }

    public ReturnException(object? value)
    {
        Value = value;
    }
}