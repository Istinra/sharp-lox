namespace Lox;

public class Resolver : IExprVisitor<object>, IStmtVisitor
{
    private readonly Interpreter _interpreter;
    private readonly Stack<Dictionary<string, bool>> _scopes = new();
    private FunctionType _currentFunction = FunctionType.NONE;
    
    private enum FunctionType {
        NONE,
        FUNCTION
    }

    public Resolver(Interpreter interpreter)
    {
        _interpreter = interpreter;
    }

    public object VisitAssignExpr(AssignExpr assignExpr)
    {
        Resolve(assignExpr.Value);
        ResolveLocal(assignExpr, assignExpr.Name);
        return null!;
    }

    public object VisitBinaryExpr(BinaryExpr binaryExpr)
    {
        Resolve(binaryExpr.Left);
        Resolve(binaryExpr.Right);
        return null!;
    }

    public object VisitCallExpr(CallExpr callExpr)
    {
        Resolve(callExpr.Callee);
        foreach (IExpr expressions in callExpr.Exprs)
        {
            Resolve(expressions);
        }

        return null!;
    }

    public object VisitGroupingExpr(GroupingExpr groupingExpr)
    {
        Resolve(groupingExpr.Expression);
        return null!;
    }

    public object VisitLiteralExpr(LiteralExpr literalExpr)
    {
        return null!;
    }

    public object VisitUnaryExpr(UnaryExpr unaryExpr)
    {
        Resolve(unaryExpr.Right);
        return null!;
    }

    public object VisitVariableExpr(VariableExpr variableExpr)
    {
        if (_scopes.Count != 0 && _scopes.Peek().TryGetValue(variableExpr.Name.Lexeme, out bool isInitialized) && !isInitialized)
        {
            Lox.Error(variableExpr.Name, "Can't read local variable in its own initializer.");
        }

        ResolveLocal(variableExpr, variableExpr.Name);
        return null!;
    }

    public object VisitLogicalExpr(LogicalExpr logicalExpr)
    {
        Resolve(logicalExpr.Left);
        Resolve(logicalExpr.Right);
        return null!;
    }

    public void VisitExprStmt(ExprStmt exprStmt)
    {
        Resolve(exprStmt.Expression);
    }

    public void VisitPrintStmt(PrintStmt exprStmt)
    {
        Resolve(exprStmt.Expression);
    }

    public void VisitVarStatement(VarStatement exprStmt)
    {
        Declare(exprStmt.Name);
        if (exprStmt.Initializer != null)
        {
            Resolve(exprStmt.Initializer);
        }
        Define(exprStmt.Name);
    }

    public void VisitBlockStatement(BlockStmt exprStmt)
    {
        BeginScope();
        Resolve(exprStmt.Statements);
        EndScope();
    }

    public void VisitFunctionStmt(FunctionStmt functionStmt)
    {
        Declare(functionStmt.Name);
        Define(functionStmt.Name);
        ResolveFunction(functionStmt, FunctionType.FUNCTION);
    }

    public void VisitIfStmt(IfStmt ifStmt)
    {
        Resolve(ifStmt.Condition);
        Resolve(ifStmt.ThenBranch);
        if (ifStmt.ElseBranch != null) Resolve(ifStmt.ElseBranch);
    }

    public void VisitWhileStmt(WhileStmt whileStmt)
    {
        Resolve(whileStmt.Condition);
        Resolve(whileStmt.Body);
    }

    public void VisitReturnStmt(ReturnStmt returnStmt)
    {
        if (_currentFunction == FunctionType.NONE)
        {
            Lox.Error(returnStmt.Keyword, "Can't return from top-level code.");
        }
        if (returnStmt.Expression != null) Resolve(returnStmt.Expression);
    }

    public void Resolve(List<IStmt> statements)
    {
        foreach (IStmt statement in statements)
        {
            Resolve(statement);
        }
    }

    private void Resolve(IStmt stmt)
    {
        stmt.Accept(this);
    }

    private void Resolve(IExpr expr)
    {
        expr.Accept(this);
    }

    private void BeginScope()
    {
        _scopes.Push(new Dictionary<string, bool>());
    }

    private void EndScope()
    {
        _scopes.Pop();
    }

    private void Declare(Token name)
    {
        if (_scopes.Count == 0) return;
        Dictionary<string, bool> scope = _scopes.Peek();
        if (scope.ContainsKey(name.Lexeme))
        {
            Lox.Error(name, "Already a variable with this name in this scope.");
        }
        scope[name.Lexeme] = false;
    }

    private void Define(Token name)
    {
        if (_scopes.Count == 0) return;
        Dictionary<string, bool> scope = _scopes.Peek();
        scope[name.Lexeme] = true;
    }

    private void ResolveLocal(IExpr expr, Token name)
    {
        Dictionary<string, bool>[] scopeArray = _scopes.ToArray();
        for (int i = 0; i < scopeArray.Length; i++)
        {
            if (scopeArray[i].ContainsKey(name.Lexeme))
            {
                _interpreter.Resolve(expr, i);
                return;
            }
        }
    }

    private void ResolveFunction(FunctionStmt function, FunctionType type)
    {
        FunctionType enclosingFunction = _currentFunction;
        _currentFunction = type;
        BeginScope();
        foreach (Token param in function.Parameters)
        {
            Declare(param);
            Define(param);
        }

        Resolve(function.Body);
        EndScope();
        _currentFunction = enclosingFunction;
    }
}