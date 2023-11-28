namespace Lox;

public class Interpreter : IExprVisitor<object>, IStmtVisitor
{
    public readonly Environment Globals = new Environment();
    private Environment _environment;

    public Interpreter()
    {
        _environment = Globals;
        Globals.Define("clock", new ClockCallable(), true);
    }

    public void Interpret(List<IStmt> statements) { 
        try
        {
            foreach (IStmt statement in statements)
            {
                Execute(statement);
            }
        } catch (RuntimeError error) {
            Lox.RuntimeError(error);
        }
    }
    
    public object VisitBinaryExpr(BinaryExpr binaryExpr)
    {
        object left = Evaluate(binaryExpr.Left);
        object right = Evaluate(binaryExpr.Right);

        switch (binaryExpr.Op.Type)
        {
            case TokenType.STAR:
                CheckNumberOperands(binaryExpr.Op, left, right);
                return (double)left * (double)right;
            case TokenType.SLASH:
                CheckNumberOperands(binaryExpr.Op, left, right);
                return (double)left / (double)right;
            case TokenType.MINUS:
                CheckNumberOperands(binaryExpr.Op, left, right);
                return (double)left - (double)right;
            case TokenType.PLUS:
                if (left is double l && right is double r)
                    return l + r;
                return left + "" + right;
            case TokenType.LESS:
                CheckNumberOperands(binaryExpr.Op, left, right);
                return (double)left < (double)right;
            case TokenType.LESS_EQUAL:
                CheckNumberOperands(binaryExpr.Op, left, right);
                return (double)left <= (double)right;
            case TokenType.GREATER:
                CheckNumberOperands(binaryExpr.Op, left, right);
                return (double)left > (double)right;
            case TokenType.GREATER_EQUAL:
                CheckNumberOperands(binaryExpr.Op, left, right);
                return (double)left >= (double)right;
            case TokenType.EQUAL_EQUAL:
                return IsEqual(left, right);
            case TokenType.BANG_EQUAL:
                return IsEqual(left, right);
        }

        throw new RuntimeError(binaryExpr.Op, "Invalid binary operation");
    }

    public object VisitCallExpr(CallExpr callExpr)
    {
        object callee = Evaluate(callExpr.Callee);
        if (callee is not ILoxCallable callable) {
            throw new RuntimeError(callExpr.Paren, "Can only call functions and classes.");
        }
        List<object> args = callExpr.Exprs.Select(Evaluate).ToList();
        if (args.Count != callable.Arity())
        {
            throw new RuntimeError(callExpr.Paren, "Expected " + callable.Arity() + " arguments but got " +
                                                   args.Count + ".");
        }
        return callable.Call(this, args);
    }

    public object VisitGroupingExpr(GroupingExpr groupingExpr)
    {
        return Evaluate(groupingExpr.Expression);
    }

    public object VisitLiteralExpr(LiteralExpr literalExpr)
    {
        return literalExpr.Value;
    }

    public object VisitUnaryExpr(UnaryExpr unaryExpr)
    {
        object right = Evaluate(unaryExpr.Right);
        switch (unaryExpr.Op.Type)
        {
            case TokenType.BANG:
                return !IsTruthy(right);
            case TokenType.MINUS:
                CheckNumberOperands(unaryExpr.Op, right);
                return -(double)right;
        }

        throw new RuntimeError(unaryExpr.Op, "Invalid unary operation");
    }

    private object Evaluate(IExpr expr)
    {
        return expr.Accept(this);
    }

    private static bool IsTruthy(object? o)
    {
        if (o == null) return false;
        if (o is bool b) return b;
        return true;
    }

    private static bool IsEqual(object? a, object? b)
    {
        if (a == null && b == null) return true;
        if (a == null) return false;

        return a == b;
    }

    private static void CheckNumberOperands(Token op, params object[] operand)
    {
        if (operand.Any(o => o is not double))
            throw new RuntimeError(op, "Operand must be a number.");
    }

    public void VisitExprStmt(ExprStmt exprStmt)
    {
        Evaluate(exprStmt.Expression);
    }

    public void VisitBlockStatement(BlockStmt exprStmt)
    {
        ExecuteBlock(exprStmt.Statements, new Environment(_environment));
    }

    public void ExecuteBlock(List<IStmt> statements, Environment environment)
    {
        Environment previous = _environment;
        try {
            _environment = environment;

            foreach (IStmt statement in statements)
            {
                Execute(statement);
            }
            
        } finally {
            _environment = previous;
        }
    }
    
    private void Execute(IStmt stmt) {
        stmt.Accept(this);
    }

    public void VisitFunctionStmt(FunctionStmt functionStmt)
    {
        LoxFunction loxFunction = new LoxFunction(functionStmt);
        _environment.Define(functionStmt.Name.Lexeme, loxFunction, true);
    }

    public void VisitIfStmt(IfStmt ifStmt)
    {
        if (IsTruthy(ifStmt.Condition))
        {
            Execute(ifStmt.ThenBranch);
        }
        else
        {
            ifStmt.ElseBranch?.Accept(this);
        }
    }

    public object VisitLogicalExpr(LogicalExpr logicalExpr)
    {
        object left = Evaluate(logicalExpr.Left);

        if (logicalExpr.Op.Type == TokenType.OR)
            if (IsTruthy(left))
                return left;
        if (logicalExpr.Op.Type == TokenType.AND)
            if (!IsTruthy(left))
                return left;
        
        return Evaluate(logicalExpr.Right);
    }

    public void VisitPrintStmt(PrintStmt exprStmt)
    {
        object result = Evaluate(exprStmt.Expression);
        Console.WriteLine(result?.ToString());
    }

    public void VisitVarStatement(VarStatement exprStmt)
    {
        bool initialised = exprStmt.Initializer != null;
        _environment.Define(exprStmt.Name.Lexeme, initialised ? Evaluate(exprStmt.Initializer!) : null, initialised);
    }

    public void VisitWhileStmt(WhileStmt whileStmt)
    {
        while (IsTruthy(Evaluate(whileStmt.Condition)))
        {
            Execute(whileStmt.Body);
        }
    }

    public object VisitVariableExpr(VariableExpr variableExpr)
    {
        return _environment.Get(variableExpr.Name)!;
    }

    public object VisitAssignExpr(AssignExpr assignExpr)
    {
        object value = Evaluate(assignExpr.Value);
        _environment.Assign(assignExpr.Name, value);
        return value;
    }
}

public class RuntimeError : SystemException
{
    public Token Token { get; }

    public RuntimeError(Token token, string message) : base(message)
    {
        Token = token;
    }
}