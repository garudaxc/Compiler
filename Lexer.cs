using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Compiler
{

    enum TokenType
    {
        Variable,
        Add,
        Minus,
        Multiply,
        Div,
        Number,
        String,
        LParenthesis,
        RParenthesis,
        LBracket,
        RBracket,
        LBrace,
        RBrace,
        //Quote,
        Less,
        Great,
        Equal,
        LessEqual,
        GreatEqual,
        EqualEqual,
        NotEqual,
        Not,
        And,
        Or,
        Semicolon,
        Comma,
        If,
        Else,
        While,
        Break,
        Continue,
        Function,
        Return,
        End
    }

    struct TokenInfo
    {
        public string s;
        public Token tok;
    }

    class Token
    {
        public Token(TokenType type)
        {
            Type = type;
        }

        public TokenType Type
        {
            set;
            get;
        }

        public string str
        {
            set;
            get;
        }

        public float f32
        {
            set;
            get;
        }

        public int i4
        {
            set;
            get;
        }

        public string TypeName
        {
            get
            {
                return Enum.GetName(typeof(TokenType), Type);
            }
        }

        public static Token Not = new Token(TokenType.Not);
        public static Token End = new Token(TokenType.End);
    }


    class Lexer : IEnumerable<Token>
    {
        string[] strings_;
        int cntString_;
        int cntChar_;

        static TokenInfo[] tokens = new TokenInfo[] {
            new TokenInfo {s = "if", tok = new Token(TokenType.If)},
            new TokenInfo {s = "else", tok = new Token(TokenType.Else)},
            new TokenInfo {s = "while", tok = new Token(TokenType.While)},
            new TokenInfo {s = "break", tok = new Token(TokenType.Break)},
            new TokenInfo {s = "continue", tok = new Token(TokenType.Continue)},
            new TokenInfo {s = "function", tok = new Token(TokenType.Function)},
            new TokenInfo {s = "return", tok = new Token(TokenType.Return)},
            new TokenInfo {s = "<=", tok = new Token(TokenType.LessEqual)},
            new TokenInfo {s = ">=", tok = new Token(TokenType.GreatEqual)},
            new TokenInfo {s = "==", tok = new Token(TokenType.EqualEqual)},
            new TokenInfo {s = "!=", tok = new Token(TokenType.NotEqual)},
            new TokenInfo {s = "&&", tok = new Token(TokenType.And)},
            new TokenInfo {s = "||", tok = new Token(TokenType.Or)},
            new TokenInfo {s = "+", tok = new Token(TokenType.Add)},
            new TokenInfo {s = "-", tok = new Token(TokenType.Minus)},
            new TokenInfo {s = "*", tok = new Token(TokenType.Multiply)},
            new TokenInfo {s = "/", tok = new Token(TokenType.Div)},
            new TokenInfo {s = "(", tok = new Token(TokenType.LParenthesis)},
            new TokenInfo {s = ")", tok = new Token(TokenType.RParenthesis)},
            new TokenInfo {s = "[", tok = new Token(TokenType.LBracket)},
            new TokenInfo {s = "]", tok = new Token(TokenType.RBracket)},
            new TokenInfo {s = "{", tok = new Token(TokenType.LBrace)},
            new TokenInfo {s = "}", tok = new Token(TokenType.RBrace)},
            new TokenInfo {s = ">", tok = new Token(TokenType.Great)},
            new TokenInfo {s = "<", tok = new Token(TokenType.Less)},
            new TokenInfo {s = "=", tok = new Token(TokenType.Equal)},
            new TokenInfo {s = ";", tok = new Token(TokenType.Semicolon)},
            new TokenInfo {s = ",", tok = new Token(TokenType.Comma)},
        };


        public bool Accept(string str)
        {
            strings_ = str.Split(' ', '\n', '\r');
            cntString_ = 0;
            cntChar_ = 0;

            //Array.ForEach(tokens, s => Console.WriteLine(s));

            return true;
        }

        public bool Accept(string[] str)
        {
            StringBuilder builder = new StringBuilder();

            for (int i = 0; i < str.Length; i++)
            {
                if (Match(str[i], 0, "//"))
                {
                    continue;
                }

                builder.Append(str[i]);
                builder.Append(' ');
            }

            string s = builder.ToString();

            strings_ = s.Split(' ', '\t');
            cntString_ = 0;
            cntChar_ = 0;

            //Array.ForEach(tokens, s => Console.WriteLine(s));

            return true;
        }

        public static bool IsNumber(string s, int index, out int i)
        {
            i = index;
            if (!Char.IsDigit(s, i))
            {
                return false;
            }

            while (i < s.Length && Char.IsDigit(s, i))
            {
                i++;
            }

            if (i == s.Length || s[i] != '.')
            {
                return true;
            }

            i++;

            while (i < s.Length && Char.IsDigit(s, i))
            {
                i++;
            }
            return true;
        }

        public static bool IsSymbol(string s, int index, out int i)
        {
            i = index;
            if (!Char.IsLetter(s[i]))
            {
                return false;
            }

            while (i < s.Length && Char.IsLetter(s, i))
            {
                i++;
            }

            while (i < s.Length && Char.IsDigit(s, i))
            {
                i++;
            }

            return true;
        }

        public static bool IsString(string s, int index, out int i, ref string str)
        {
            i = index;
            if (s[i] != '\"')
            {
                return false;
            }

            i++;
            while(i < s.Length && s[i] != '\"')
            {
                i++;
            }

            if (i == s.Length)
            {
                return false;
            }

            i++;
            str = s.Substring(index + 1, i - index - 2);

            return true;
        }

        public static bool Match(string s, int start, string sub)
        {
            if (s.Length < start + sub.Length)
            {
                return false;
            }

            for (int i = 0; i < sub.Length; i++)
            {
                if (s[start + i] != sub[i])
                    return false;
            }

            return true;
        }

        public Token GetNextToken()
        {
            Token tok = Token.End;

            while (cntString_ < strings_.Length && cntChar_ == strings_[cntString_].Length)
            {
                cntString_++;
                cntChar_ = 0;
            }

            if (cntString_ == strings_.Length)
            {
                return tok;
            }

            string s = strings_[cntString_];

            int j = 0;
            while (true)
            {
                for (int i = 0; i < tokens.Length; i++)
                {
                    if (Match(s, cntChar_, tokens[i].s))
                    {
                        tok = tokens[i].tok;
                        cntChar_ += tokens[i].s.Length;
                        return tok;
                    }
                }

                if (IsNumber(s, cntChar_, out j))
                {
                    tok = new Token(TokenType.Number);
                    tok.i4 = int.Parse(s.Substring(cntChar_, j - cntChar_));
                    cntChar_ = j;
                    break;
                }
                else
                {
                    string str = string.Empty;
                    if (IsSymbol(s, cntChar_, out j))
                    {
                        tok = new Token(TokenType.Variable);
                        tok.str = s.Substring(cntChar_, j - cntChar_);
                        cntChar_ = j;
                        break;
                    }
                    else if (IsString(s, cntChar_, out j, ref str))
                    {
                        tok = new Token(TokenType.String);
                        tok.str = str;
                        cntChar_ = j;
                        break;
                    }
                    else
                    {
                        Console.WriteLine("not valid token " + s);
                        return Token.End;
                    }
                }
            }

            return tok;
        }

        public IEnumerator<Token> GetEnumerator()
        {
            throw new NotImplementedException();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            throw new NotImplementedException();
        }
    }
}
