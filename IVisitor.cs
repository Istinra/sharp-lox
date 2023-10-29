namespace Lox;

public interface IVisitor<TR>
{
    TR VisitBinaryExpr(BinaryExpr binaryExpr);
    TR VisitGroupingExpr(GroupingExpr groupingExpr);
    TR VisitLiteralExpr(LiteralExpr literalExpr);
    TR VisitUnaryExpr(UnaryExpr unaryExpr);
}