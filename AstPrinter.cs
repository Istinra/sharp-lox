using System.Text;

namespace Lox;

// public class AstPrinter : IExprVisitor<string>
// {
//     public string VisitBinaryExpr(BinaryExpr binaryExpr)
//     {
//         return Parenthesize(binaryExpr.Op.Lexeme, binaryExpr.Left, binaryExpr.Right);
//     }
//
//     public string VisitGroupingExpr(GroupingExpr groupingExpr)
//     {
//         return Parenthesize("group", groupingExpr.Expression);
//     }
//
//     public string VisitLiteralExpr(LiteralExpr literalExpr)
//     {
//         return literalExpr.Value?.ToString() ?? "nil";
//     }
//
//     public string VisitUnaryExpr(UnaryExpr unaryExpr)
//     {
//         return Parenthesize(unaryExpr.Op.Lexeme, unaryExpr.Right);
//     }
//     
//     private string Parenthesize(string name, params IExpr[] exprs) {
//         StringBuilder builder = new StringBuilder();
//
//         builder.Append('(').Append(name);
//         foreach (IExpr expr in exprs)
//         {
//             builder.Append(' ');
//             builder.Append(expr.Accept(this));   
//         }
//         builder.Append(')');
//
//         return builder.ToString();
//     }
// }