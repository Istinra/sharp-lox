namespace Lox;

public interface IExprVisitor<TR>
{
    TR VisitBinaryExpr(BinaryExpr binaryExpr);
    TR VisitGroupingExpr(GroupingExpr groupingExpr);
    TR VisitLiteralExpr(LiteralExpr literalExpr);
    TR VisitUnaryExpr(UnaryExpr unaryExpr);
}

public interface IStmtVisitor
{
    void VisitExprStmt(ExprStmt exprStmt);
    
    void VisitPrintStmt(PrintStmt exprStmt);
}