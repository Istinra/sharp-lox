namespace Lox;

public interface IExpr
{
     TR Accept<TR>(IExprVisitor<TR> exprVisitor);
}

public record AssignExpr(Token Name, IExpr Value) : IExpr
{
     public TR Accept<TR>(IExprVisitor<TR> exprVisitor)
     {
          return exprVisitor.VisitAssignExpr(this);
     }
}

public record BinaryExpr(IExpr Left, Token Op, IExpr Right) : IExpr
{
     public TR Accept<TR>(IExprVisitor<TR> exprVisitor)
     {
          return exprVisitor.VisitBinaryExpr(this);
     }
}

public record GroupingExpr(IExpr Expression) : IExpr
{
     public TR Accept<TR>(IExprVisitor<TR> exprVisitor)
     {
          return exprVisitor.VisitGroupingExpr(this);
     } 
}

public record LiteralExpr(object Value) : IExpr
{
     public TR Accept<TR>(IExprVisitor<TR> exprVisitor)
     {
          return exprVisitor.VisitLiteralExpr(this);
     }
}

public record UnaryExpr(Token Op, IExpr Right) : IExpr
{
     public TR Accept<TR>(IExprVisitor<TR> exprVisitor)
     {
          return exprVisitor.VisitUnaryExpr(this);
     }
}

public record VariableExpr(Token Name) : IExpr
{
     public TR Accept<TR>(IExprVisitor<TR> exprVisitor)
     {
          return exprVisitor.VisitVariableExpr(this);
     }
}

public interface IStmt
{
     void Accept(IStmtVisitor visitor);
}

public record ExprStmt(IExpr Expression) : IStmt
{
     public void Accept(IStmtVisitor visitor)
     {
          visitor.VisitExprStmt(this);
     }
}

public record IfStmt(IExpr Condition, IStmt ThenBranch, IStmt? ElseBranch) : IStmt
{
     public void Accept(IStmtVisitor visitor)
     {
          visitor.VisitIfStmt(this);
     }
}

public record PrintStmt(IExpr Expression) : IStmt
{
     public void Accept(IStmtVisitor visitor)
     {
          visitor.VisitPrintStmt(this);
     }
}

public record VarStatement(Token Name, IExpr? Initializer) : IStmt
{
     public void Accept(IStmtVisitor visitor)
     {
          visitor.VisitVarStatement(this);
     }
}

public record BlockStmt(List<IStmt> Statements) : IStmt
{
     public void Accept(IStmtVisitor visitor)
     {
          visitor.VisitBlockStatement(this);
     }
}