namespace Lox;

public class Interpreter : IVisitor<object>
{
    public object VisitBinaryExpr(BinaryExpr binaryExpr)
    {
        object left = Evaluate(binaryExpr.Left);
        object right = Evaluate(binaryExpr.Right);

        switch (binaryExpr.Op.Type)
        {
            case TokenType.STAR:
                return (double)left * (double)right;
            case TokenType.SLASH:
                return (double)left / (double)right;
            case TokenType.MINUS:
                return (double)left - (double)right;
            case TokenType.PLUS:
            {
                if (left is double l && right is double r)
                    return l + r;
            }
            {
                if (left is string l && right is string r)
                    return l + r;
            }
                break;
            case TokenType.LESS:
                return (double)left < (double)right;
            case TokenType.LESS_EQUAL:
                return (double)left <= (double)right;
            case TokenType.GREATER:
                return (double)left > (double)right;
            case TokenType.GREATER_EQUAL:
                return (double)left >= (double)right;
            case TokenType.EQUAL_EQUAL:
                return IsEqual(left, right);
            case TokenType.BANG_EQUAL:
                return IsEqual(left, right);
        }

        return null;
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
                return -(double)right;
        }

        return null;
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
}