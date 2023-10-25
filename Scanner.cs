namespace Lox;

public class Scanner
{
    private readonly string _source;
    private readonly List<Token> _tokens = new List<Token>();
    private int _start = 0;
    private int _current = 0;
    private int _line = 1;

    private static readonly Dictionary<string, TokenType> Identifiers
        = new Dictionary<string, TokenType>
        {
            {"and",    TokenType.AND},
            {"class",  TokenType.CLASS},
            {"else",   TokenType.ELSE},
            {"false",  TokenType.FALSE},
            {"for",    TokenType.FOR},
            {"fun",    TokenType.FUN},
            {"if",     TokenType.IF},
            {"nil",    TokenType.NIL},
            {"or",     TokenType.OR},
            {"print",  TokenType.PRINT},
            {"return", TokenType.RETURN},
            {"super",  TokenType.SUPER},
            {"this",   TokenType.THIS},
            {"true",   TokenType.TRUE},
            {"var",    TokenType.VAR},
            {"while",  TokenType.WHILE}
        };
    
    public Scanner(string source)
    {
        this._source = source;
    }

    public List<Token> ScanTokens()
    {
        while (!IsAtEnd())
        {
            // We are at the beginning of the next lexeme.
            _start = _current;
            ScanToken();
        }

        return _tokens;
    }

    private bool IsAtEnd()
    {
        return _current >= _source.Length;
    }

    private void ScanToken()
    {
        char c = Advance();
        switch (c) {
            case '(': AddToken(TokenType.LEFT_PAREN); break;
            case ')': AddToken(TokenType.RIGHT_PAREN); break;
            case '{': AddToken(TokenType.LEFT_BRACE); break;
            case '}': AddToken(TokenType.RIGHT_BRACE); break;
            case ',': AddToken(TokenType.COMMA); break;
            case '.': AddToken(TokenType.DOT); break;
            case '-': AddToken(TokenType.MINUS); break;
            case '+': AddToken(TokenType.PLUS); break;
            case ';': AddToken(TokenType.SEMICOLON); break;
            case '*': AddToken(TokenType.STAR); break;
            case '!':
                AddToken(Match('=') ? TokenType.BANG_EQUAL : TokenType.BANG);
                break;
            case '=':
                AddToken(Match('=') ? TokenType.EQUAL_EQUAL : TokenType.EQUAL);
                break;
            case '<':
                AddToken(Match('=') ? TokenType.LESS_EQUAL : TokenType.LESS);
                break;
            case '>':
                AddToken(Match('=') ? TokenType.GREATER_EQUAL : TokenType.GREATER);
                break;
            case '/':
                if (Match('/')) {
                    // A comment goes until the end of the line.
                    while (Peek() != '\n' && !IsAtEnd()) _current++;
                } else {
                    AddToken(TokenType.SLASH);
                }
                break;
            case '"': 
                ReadString();
                break;
            case ' ':
            case '\r':
            case '\t':
                // Ignore whitespace.
                break;
            case '\n':
                _line++;
                break;
            default:
                if (char.IsDigit(c))
                {
                    ReadNumber();
                    break;
                }
                else if (char.IsLetter(c))
                {
                    ReadIdentifier();
                    break;
                }
                Lox.Error(_line, "Unexpected character: " + c);
                break;
        }
    }

    private void ReadString()
    {
        char c;
        while ((c = Peek()) != '"' )
        {
            if (IsAtEnd())
            {
                Lox.Error(_line,  "Unterminated string.");
                return;
            }
            if (c == '\n')
                _line++;
            Advance();
        }

        //Skip closing "
        Advance();
        
        string value = _source.Substring(_start + 1, _current - _start -2);
        AddToken(TokenType.STRING, value);
    }

    private void ReadNumber()
    {
        while (char.IsDigit(Peek()))
            Advance();
        if ('.' == Peek() && char.IsDigit(Peek()))
        {
            Advance();
            while (char.IsDigit(Peek()))
                Advance();
        }
        AddToken(TokenType.NUMBER, double.Parse(_source.Substring(_start, _current - _start)));
    }

    private void ReadIdentifier()
    {
        while (char.IsLetterOrDigit(Peek()))
            Advance();
        string s = _source.Substring(_start, _current - _start);
        TokenType type = Identifiers.TryGetValue(s, out TokenType identifier) ? identifier : TokenType.IDENTIFIER;
        AddToken(type);
    }

    private char Advance()
    {
        return _source[_current++];
    }

    private void AddToken(TokenType type, object? literal = null)
    {
        string text = _source.Substring(_start, _current - _start);
        _tokens.Add(new Token(type, text, literal, _line));
    }

    private bool Match(char expected)
    {
        if (IsAtEnd() || _source[_current] != _current) return false;
        _current++;
        return true;
    }

    private char Peek()
    {
        return IsAtEnd() ? '\0' : _source[_current];
    }
    
    private char PeekNext()
    {
        return _current + 1 >= _source.Length ? '\0' : _source[_current + 1];
    } 
}