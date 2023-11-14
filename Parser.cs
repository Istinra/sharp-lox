namespace Lox;

public class Parser
{
    private readonly List<Token> _tokens;
    private int _current = 0;

    public Parser(List<Token> tokens)
    {
        _tokens = tokens;
    }

    public List<IStmt> Parse()
    {
        List<IStmt> stmts = new List<IStmt>();
        while (!IsAtEnd())
        {
            IStmt? declaration = Declaration();
            if (declaration != null)
            {
                stmts.Add(declaration);
            }
        }

        return stmts;
    }

    private IStmt? Declaration()
    {
        try
        {
            if (Match(TokenType.VAR))
                return VarDeclaration();
            return Statement();
        }
        catch (ParseError)
        {
            Synchronize();
            return null;
        }
    }

    private IStmt VarDeclaration()
    {
        Token id = Consume(TokenType.IDENTIFIER, "Expect variable name.");
        bool init = Match(TokenType.EQUAL);
        VarStatement varStatement = new VarStatement(id, init ? Expression() : null);
        Consume(TokenType.SEMICOLON, "Expected ; after value");
        return varStatement;
    }

    private IStmt Statement()
    {
        if (Match(TokenType.FOR))
            return ForStatement();
        if (Match(TokenType.IF))
            return IfStatement();
        if (Match(TokenType.PRINT))
            return PrintStatement();
        if (Match(TokenType.WHILE))
            return WhileStatement();
        if (Match(TokenType.LEFT_BRACE))
            return new BlockStmt(Block());
        return ExpressionStatement();
    }

    private IStmt ForStatement()
    {
        Consume(TokenType.LEFT_PAREN, "Expect '(' after 'if'.");
        IStmt? initializer;
        if (Match(TokenType.SEMICOLON)) {
            initializer = null;
        } else if (Match(TokenType.VAR)) {
            initializer = VarDeclaration();
        } else {
            initializer = ExpressionStatement();
        }
        
        IExpr? condition = null;
        if (!Check(TokenType.SEMICOLON)) {
            condition = Expression();
        }
        Consume(TokenType.SEMICOLON, "Expect ';' after loop condition.");
        
        IExpr? increment = null;
        if (!Check(TokenType.RIGHT_PAREN)) {
            increment = Expression();
        }
        
        Consume(TokenType.RIGHT_PAREN, "Expect ')' after if condition.");

        IStmt body = Statement();

        if (increment != null)
            body = new BlockStmt(new List<IStmt>() { body, new ExprStmt(increment) });
        if (condition == null)
            condition = new LiteralExpr(true);
        body = new WhileStmt(condition, body);
        if (initializer != null)
            body = new BlockStmt(new List<IStmt>() { initializer, body });
        
        return body;
    }

    private IStmt IfStatement()
    {
        Consume(TokenType.LEFT_PAREN, "Expect '(' after 'if'.");
        IExpr condition = Expression();
        Consume(TokenType.RIGHT_PAREN, "Expect ')' after if condition.");
        IStmt thenBranch = Statement();
        IStmt? elseBranch = null;
        if (Match(TokenType.ELSE))
        {
            elseBranch = Statement();
        }

        return new IfStmt(condition, thenBranch, elseBranch);
    }

    private IStmt PrintStatement()
    {
        IExpr expression = Expression();
        Consume(TokenType.SEMICOLON, "Expected ; after print value");
        return new PrintStmt(expression);
    }

    private IStmt WhileStatement()
    {
        Consume(TokenType.LEFT_PAREN, "Expect '(' after 'if'.");
        IExpr condition = Expression();
        Consume(TokenType.RIGHT_PAREN, "Expect ')' after if condition.");
        return new WhileStmt(condition, Statement());
    }

    private List<IStmt> Block()
    {
        List<IStmt> stmts = new();
        while (!Check(TokenType.RIGHT_BRACE) && !IsAtEnd())
        {
            IStmt? declaration = Declaration();
            if (declaration != null)
                stmts.Add(declaration);
        }

        Consume(TokenType.RIGHT_BRACE, "Expect '}' after block.");
        return stmts;
    }

    private IStmt ExpressionStatement()
    {
        ExprStmt expressionStatement = new ExprStmt(Expression());
        Consume(TokenType.SEMICOLON, "Expected ; after value");
        return expressionStatement;
    }

    private IExpr Expression()
    {
        return Assignment();
    }

    private IExpr Assignment()
    {
        IExpr expr = Or();

        if (Match(TokenType.EQUAL))
        {
            Token eq = Previous();
            IExpr assignment = Assignment();
            if (expr is VariableExpr variableExpr)
            {
                return new AssignExpr(variableExpr.Name, assignment);
            }

            Error(eq, "Invalid assignment target.");
        }

        return expr;
    }

    private IExpr Or()
    {
        IExpr expr = And();
        
        while (Match(TokenType.OR)) {
            Token op = Previous();
            IExpr right = And();
            expr = new LogicalExpr(expr, op, right);
        }

        return expr;
    }

    private IExpr And()
    {
        IExpr expr = Equality();
        
        while (Match(TokenType.AND)) {
            Token op = Previous();
            IExpr right = Equality();
            expr = new LogicalExpr(expr, op, right);
        }

        return expr;
    }

    private IExpr Equality()
    {
        IExpr expr = Comparison();
        while (Match(TokenType.EQUAL_EQUAL, TokenType.BANG_EQUAL))
        {
            Token op = Previous();
            IExpr right = Comparison();
            expr = new BinaryExpr(expr, op, right);
        }

        return expr;
    }

    private IExpr Comparison()
    {
        IExpr expr = Term();
        while (Match(TokenType.LESS, TokenType.GREATER, TokenType.LESS_EQUAL, TokenType.GREATER_EQUAL))
        {
            Token op = Previous();
            IExpr right = Term();
            expr = new BinaryExpr(expr, op, right);
        }

        return expr;
    }

    private IExpr Term()
    {
        IExpr expr = Factor();
        while (Match(TokenType.MINUS, TokenType.PLUS))
        {
            Token op = Previous();
            IExpr right = Factor();
            expr = new BinaryExpr(expr, op, right);
        }

        return expr;
    }

    private IExpr Factor()
    {
        IExpr expr = Unary();
        while (Match(TokenType.SLASH, TokenType.STAR))
        {
            Token op = Previous();
            IExpr right = Unary();
            expr = new BinaryExpr(expr, op, right);
        }

        return expr;
    }

    private IExpr Unary()
    {
        return Match(TokenType.BANG, TokenType.MINUS) ? new UnaryExpr(Previous(), Unary()) : Primary();
    }

    private IExpr Primary()
    {
        if (Match(TokenType.TRUE))
        {
            return new LiteralExpr(true);
        }

        if (Match(TokenType.FALSE))
        {
            return new LiteralExpr(false);
        }

        if (Match(TokenType.NIL))
        {
            return new LiteralExpr(null!);
        }

        if (Match(TokenType.STRING, TokenType.NUMBER))
        {
            return new LiteralExpr(Previous().Literal!);
        }

        if (Match(TokenType.IDENTIFIER))
        {
            return new VariableExpr(Previous());
        }

        if (Match(TokenType.LEFT_PAREN))
        {
            IExpr expr = Expression();
            Consume(TokenType.RIGHT_PAREN, "Expect ')' after expression.");
            return new GroupingExpr(expr);
        }

        throw Error(Peek(), "Expect expression.");
    }

    private void Synchronize()
    {
        Advance();

        while (!IsAtEnd())
        {
            if (Previous().Type == TokenType.SEMICOLON) return;

            switch (Peek().Type)
            {
                case TokenType.CLASS:
                case TokenType.FUN:
                case TokenType.VAR:
                case TokenType.FOR:
                case TokenType.IF:
                case TokenType.WHILE:
                case TokenType.PRINT:
                case TokenType.RETURN:
                    return;
            }

            Advance();
        }
    }

    private bool Match(params TokenType[] types)
    {
        if (!types.Any(Check)) return false;
        Advance();
        return true;
    }

    private bool Check(TokenType type)
    {
        if (IsAtEnd()) return false;
        return Peek().Type == type;
    }

    private Token Advance()
    {
        if (!IsAtEnd()) _current++;
        return Previous();
    }

    private bool IsAtEnd()
    {
        return Peek().Type == TokenType.EOF;
    }

    private Token Peek()
    {
        return _tokens[_current];
    }

    private Token Previous()
    {
        return _tokens[_current - 1];
    }

    private Token Consume(TokenType type, string message)
    {
        if (Check(type)) return Advance();

        throw Error(Peek(), message);
    }

    private ParseError Error(Token token, string message)
    {
        Lox.Error(token, message);
        return new ParseError();
    }

    private class ParseError : SystemException
    {
    }
}