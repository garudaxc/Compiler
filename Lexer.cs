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
        LessEqual,
        GreatEqual,
        EqualEqual,
        NotEqual,
        True,
        False,
        Not,
        And,
        Or,
        Semicolon,
        If,
        Else,
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
        public static Token LessEqual = new Token(TokenType.LessEqual);
        public static Token GreatEqual = new Token(TokenType.GreatEqual);
        public static Token EqualEqual = new Token(TokenType.EqualEqual);
        public static Token NotEqual = new Token(TokenType.NotEqual);
        public static Token True = new Token(TokenType.True);
        public static Token False = new Token(TokenType.False);
        public static Token Not = new Token(TokenType.Not);
        public static Token And = new Token(TokenType.And);
        public static Token Or = new Token(TokenType.Or);
        public static Token If = new Token(TokenType.If);
        public static Token Else = new Token(TokenType.Else);
        public static Token While = new Token(TokenType.While);
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

        public static bool Match(string s, int start, string sub)
        {
            if (s.Length < start + sub.Length)
            {
                return false;
            }

            for(int i = 0; i < sub.Length; i++)
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
                if (s[cntChar_] == '+')
                {
                    tok = Token.Add;
                    cntChar_++;
                    break;
                }
                else if (s[cntChar_] == '-')
                {
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
                else if (Match(s, cntChar_, "<="))
                {
                    tok = Token.LessEqual;
                    cntChar_ += 2;
                    break;
                }
                else if (Match(s, cntChar_, ">="))
                {
                    tok = Token.GreatEqual;
                    cntChar_ += 2;
                    break;
                }
                else if (Match(s, cntChar_, "=="))
                {
                    tok = Token.EqualEqual;
                    cntChar_ += 2;
                    break;
                }
                else if (Match(s, cntChar_, "!="))
                {
                    tok = Token.NotEqual;
                    cntChar_ += 2;
                    break;
                }
                else if (Match(s, cntChar_, "&&"))
                {
                    tok = Token.And;
                    cntChar_ += 2;
                    break;
                }
                else if (Match(s, cntChar_, "||"))
                {
                    tok = Token.Or;
                    cntChar_ += 2;
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
                    tok.i4 = int.Parse(s.Substring(cntChar_, j - cntChar_));
                    cntChar_ = j;
                    break;
                }
                else if (s.Equals("if"))
                {
                    tok = Token.If;
                    cntString_++;
                    cntChar_ = 0;
                    break;
                }
                else if (s.Equals("else"))
                {
                    tok = Token.Else;
                    cntString_++;
                    cntChar_ = 0;
                    break;
                }
                else if (s.Equals("while"))
                {
                    tok = Token.While;
                    cntString_++;
                    cntChar_ = 0;
                    break;
                }
                else if (s.Equals("true"))
                {
                    tok = new Token(TokenType.True);
                    cntString_++;
                    cntChar_ = 0;
                    break;
                }
                else if (s.Equals("false"))
                {
                    tok = new Token(TokenType.False);
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
