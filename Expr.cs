namespace Lox;

public interface IExpr
{
     TR Accept<TR>(IVisitor<TR> visitor);
}

public record BinaryExpr(IExpr Left, Token Op, IExpr Right) : IExpr
{
     public TR Accept<TR>(IVisitor<TR> visitor)
     {
          return visitor.VisitBinaryExpr(this);
     }
}

public record GroupingExpr(IExpr Expression) : IExpr
{
     public TR Accept<TR>(IVisitor<TR> visitor)
     {
          return visitor.VisitGroupingExpr(this);
     } 
}

public record LiteralExpr(object Value) : IExpr
{
     public TR Accept<TR>(IVisitor<TR> visitor)
     {
          return visitor.VisitLiteralExpr(this);
     }
}

public record UnaryExpr(Token Op, IExpr Right) : IExpr
{
     public TR Accept<TR>(IVisitor<TR> visitor)
     {
          return visitor.VisitUnaryExpr(this);
     }
}