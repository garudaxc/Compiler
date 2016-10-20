using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Compiler
{
    class LLPaser
    {
        Lexer lex_;
        Token currentToken_;

        public LLPaser()
        {
        }

        void Error(string str)
        {
            Console.WriteLine("error : " + str);            
        }

        void Eat(TokenType type)
        {
            if (currentToken_.Type == type)
            {
                currentToken_ = lex_.GetNextToken();
            }
            else
            {
                Error(string.Format("wrong token type {0} expect {1}", currentToken_.TypeName, Enum.GetName(typeof(TokenType), type)));
            }            
        }

        float E()
        {
            T(); EPrime(); Eat(TokenType.Semicolon);
            return 0.0f;
        }

        float EPrime()
        {
            switch(currentToken_.Type)
            {
                case TokenType.Add:
                    Eat(TokenType.Add); T(); EPrime();
                    break;
                case TokenType.Minus:
                    Eat(TokenType.Minus); T(); EPrime();
                    break;
                default:
                    break;
            }
            
            return 0.0f;
        }

        float T()
        {
            F(); TPrime();

            return 0.0f;
        }

        float TPrime()
        {
            switch (currentToken_.Type)
            {
                case TokenType.Div:
                    Eat(TokenType.Div); F(); TPrime();
                    break;
                case TokenType.Multiply:
                    Eat(TokenType.Multiply); F(); TPrime();
                    break;
                default:
                    break;
            }

            return 0.0f;
        }

        float F()
        {
            switch(currentToken_.Type)
            {
                case TokenType.Number:
                    Eat(TokenType.Number);
                    break;
                case TokenType.LParenthesis:
                    Eat(TokenType.LParenthesis); T(); EPrime(); Eat(TokenType.RParenthesis);
                    break;
                default:
                    Error(string.Format("wrong token {0} expect number/left parentthesis", currentToken_.TypeName));
                    break;

            }
            return 0.0f;
        }

        public void Parse(Lexer lex)
        {
            lex_ = lex;
            currentToken_ = lex_.GetNextToken();
            E();

            Console.WriteLine("ok");
        }

    }
}
