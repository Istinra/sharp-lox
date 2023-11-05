﻿namespace Lox;

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
            stmts.Add(Statement());
        }

        return stmts;
        // try {
        //     return Expression();
        // } catch (ParseError error) {
        //     return null;
        // }
    }
    
    private IStmt Statement() {
        if (Match(TokenType.PRINT)) return PrintStatement();
        return ExpressionStatement();
    }

    private IStmt PrintStatement()
    {
        IExpr expression = Expression();
        Consume(TokenType.SEMICOLON, "Expected ; after print value");
        return new PrintStmt(expression);
    }

    private IStmt ExpressionStatement()
    {
        ExprStmt expressionStatement = new ExprStmt(Expression());
        Consume(TokenType.SEMICOLON, "Expected ; after value");
        return expressionStatement;
    }

    private IExpr Expression() {
        return Equality();
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
            return new LiteralExpr(null);
        }
        if (Match(TokenType.STRING, TokenType.NUMBER))
        {
            return new LiteralExpr(Previous().Literal);
        }

        if (Match(TokenType.LEFT_PAREN))
        {
            IExpr expr = Expression();
            Consume(TokenType.RIGHT_PAREN, "Expect ')' after expression.");
            return new GroupingExpr(expr);
        }
        
        throw Error(Peek(), "Expect expression.");
    }
    
    private void Synchronize() {
        Advance();

        while (!IsAtEnd()) {
            if (Previous().Type == TokenType.SEMICOLON) return;

            switch (Peek().Type) {
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
    
    private bool Check(TokenType type) {
        if (IsAtEnd()) return false;
        return Peek().Type == type;
    }
    
    private Token Advance() {
        if (!IsAtEnd()) _current++;
        return Previous();
    }
    
    private bool IsAtEnd() {
        return Peek().Type == TokenType.EOF;
    }

    private Token Peek() {
        return _tokens[_current];
    }

    private Token Previous() {
        return _tokens[_current - 1];
    }
    
    private Token Consume(TokenType type, string message) {
        if (Check(type)) return Advance();

        throw Error(Peek(), message);
    }
    
    private ParseError Error(Token token, string message) {
        Lox.Error(token, message);
        return new ParseError();
    }
    
    private class ParseError : SystemException {} 
}