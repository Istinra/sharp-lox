namespace Lox;

public interface IExprVisitor<TR>
{
    TR VisitAssignExpr(AssignExpr assignExpr);
    TR VisitBinaryExpr(BinaryExpr binaryExpr);
    TR VisitGroupingExpr(GroupingExpr groupingExpr);
    TR VisitLiteralExpr(LiteralExpr literalExpr);
    TR VisitUnaryExpr(UnaryExpr unaryExpr);
    TR VisitVariableExpr(VariableExpr variableExpr);
}

public interface IStmtVisitor
{
    void VisitExprStmt(ExprStmt exprStmt);
    void VisitPrintStmt(PrintStmt exprStmt);
    void VisitVarStatement(VarStatement exprStmt);
}