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
        LParenthesis,
        RParenthesis,
        LBrace,
        RBrace,
        Less,
        Great,
        Equal,
        Semicolon,
        If,
        While,
        //Bracket,
        End
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

        public float value
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

        public static Token Variable = new Token(TokenType.Variable);
        public static Token Add = new Token(TokenType.Add);
        public static Token Minus = new Token(TokenType.Minus);
        public static Token Multipy = new Token(TokenType.Multiply);
        public static Token Div = new Token(TokenType.Div);
        public static Token LParenthesis = new Token(TokenType.LParenthesis);
        public static Token RParenthesis = new Token(TokenType.RParenthesis);
        public static Token LBrace = new Token(TokenType.LBrace);
        public static Token RBrace = new Token(TokenType.RBrace);
        public static Token Less = new Token(TokenType.Less);
        public static Token Great = new Token(TokenType.Great);
        public static Token Equal = new Token(TokenType.Equal);
        public static Token Semicolon = new Token(TokenType.Semicolon);
        public static Token End = new Token(TokenType.End);
    }



    class Lexer : IEnumerable<Token>
    {
        string[] strings_;
        int cntString_;
        int cntChar_;

        public bool Accept(string str)
        {
            strings_ = str.Split(' ', '\n', '\r');
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


        public Token GetNextToken()
        {
            if (cntString_ == strings_.Length)
            {
                return Token.End;
            }

            string s = strings_[cntString_];

            int j = 0;
            Token tok = Token.End;
            while (true)
            {
                if (s.Length == 0 )
                {
                    cntString_++;
                    s = strings_[cntString_];
                    continue;
                }
                
                if (s[cntChar_] == '+')
                {
                    tok = Token.Add;
                    cntChar_++;
                    break;
                }
                else if (s[cntChar_] == '-')
                {
                    //if (IsNumber(s, cntChar_ + 1, out j))
                    //{
                    //    tok = new Token(TokenType.Number);
                    //    tok.value = float.Parse(s.Substring(cntChar_, j - cntChar_));
                    //    cntChar_ = j;
                    //}
                    //else
                    //{
                    //}
                    tok = Token.Minus;
                    cntChar_++;
                    break;
                }
                else if (s[cntChar_] == '*')
                {
                    tok = Token.Multipy;
                    cntChar_++;
                    break;
                }
                else if (s[cntChar_] == '/')
                {
                    tok = Token.Div;
                    cntChar_++;
                    break;
                }
                else if (s[cntChar_] == '(')
                {
                    tok = Token.LParenthesis;
                    cntChar_++;
                    break;
                }
                else if (s[cntChar_] == ')')
                {
                    tok = Token.RParenthesis;
                    cntChar_++;
                    break;
                }
                else if (s[cntChar_] == '{')
                {
                    tok = Token.LBrace;
                    cntChar_++;
                    break;
                }
                else if (s[cntChar_] == '}')
                {
                    tok = Token.RBrace;
                    cntChar_++;
                    break;
                }
                else if (s[cntChar_] == '<')
                {
                    tok = Token.Less;
                    cntChar_++;
                    break;
                }
                else if (s[cntChar_] == '>')
                {
                    tok = Token.Great;
                    cntChar_++;
                    break;
                }
                else if (s[cntChar_] == '=')
                {
                    tok = Token.Equal;
                    cntChar_++;
                    break;
                }
                else if (s[cntChar_] == ';')
                {
                    tok = Token.Semicolon;
                    cntChar_++;
                    break;
                }
                else if (IsNumber(s, cntChar_, out j))
                {
                    tok = new Token(TokenType.Number);
                    tok.value = float.Parse(s.Substring(cntChar_, j - cntChar_));
                    cntChar_ = j;
                    break;
                }
                else if (s.Equals("if"))
                {
                    tok = new Token(TokenType.If);
                    cntString_++;
                    cntChar_ = 0;
                    break;
                }
                else if (s.Equals("while"))
                {
                    tok = new Token(TokenType.While);
                    cntString_++;
                    cntChar_ = 0;
                    break;
                }
                else
                {
                    if (IsSymbol(s, cntChar_, out j))
                    {
                        tok = new Token(TokenType.Variable);
                        tok.str = s.Substring(cntChar_, j - cntChar_);
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

            if (cntChar_ == s.Length)
            {
                cntString_++;
                cntChar_ = 0;
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
