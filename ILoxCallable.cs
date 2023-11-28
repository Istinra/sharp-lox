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

    public LoxFunction(FunctionStmt declaration)
    {
        this._declaration = declaration;
    }

    public object Call(Interpreter interpreter, List<object> arguments)
    {
        Environment environment = new Environment(interpreter.Globals);
        for (var i = 0; i < _declaration.Parameters.Count; i++)
        {
            environment.Define(_declaration.Parameters[i].Lexeme, arguments[i], true);
        }
        interpreter.ExecuteBlock(_declaration.Body, environment);
        return null;
    }

    public int Arity()
    {
        return _declaration.Parameters.Count;
    }
}