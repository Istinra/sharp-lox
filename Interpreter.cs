namespace Lox;

public class Interpreter : IVisitor<object>
{
    public void Interpret(IExpr expression) { 
        try {
            object value = Evaluate(expression);
            Console.WriteLine(value);
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
}

public class RuntimeError : SystemException
{
    public Token Token { get; }

    public RuntimeError(Token token, string message) : base(message)
    {
        Token = token;
    }
}