namespace Lox;

public interface IExprVisitor<TR>
{
    TR VisitAssignExpr(AssignExpr assignExpr);
    TR VisitBinaryExpr(BinaryExpr binaryExpr);
    TR VisitCallExpr(CallExpr callExpr);
    TR VisitGroupingExpr(GroupingExpr groupingExpr);
    TR VisitLiteralExpr(LiteralExpr literalExpr);
    TR VisitUnaryExpr(UnaryExpr unaryExpr);
    TR VisitVariableExpr(VariableExpr variableExpr);
    TR VisitLogicalExpr(LogicalExpr logicalExpr);
}

public interface IStmtVisitor
{
    void VisitExprStmt(ExprStmt exprStmt);
    void VisitPrintStmt(PrintStmt exprStmt);
    void VisitVarStatement(VarStatement exprStmt);
    void VisitBlockStatement(BlockStmt exprStmt);
    void VisitIfStmt(IfStmt ifStmt);
    void VisitWhileStmt(WhileStmt whileStmt);
}