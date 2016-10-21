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
            float a = T(); a = EPrime(a); Eat(TokenType.Semicolon);
            return a;
        }

        float EPrime(float a)
        {
            float b;
            switch(currentToken_.Type)
            {
                case TokenType.Add:
                    Eat(TokenType.Add); 
                    b = T(); 
                    return EPrime(a + b);
                case TokenType.Minus:
                    Eat(TokenType.Minus);
                    b = T(); 
                    return EPrime(a - b);
                default:
                    return a;
            }
            
        }

        float T()
        {
            float a = F(); 
            return TPrime(a);
        }

        float TPrime(float a)
        {
            float b;
            switch (currentToken_.Type)
            {
                case TokenType.Div:
                    Eat(TokenType.Div); 
                    b = F(); 
                    return TPrime(a / b);
                case TokenType.Multiply:
                    Eat(TokenType.Multiply); 
                    b = F(); 
                    return TPrime(a * b);
                default:
                    return a;
            }

        }

        float F()
        {
            switch(currentToken_.Type)
            {
                case TokenType.Number:
                    float val = currentToken_.value;
                    Eat(TokenType.Number);
                    return val;
                case TokenType.LParenthesis:
                    Eat(TokenType.LParenthesis); float a = T(); a = EPrime(a); Eat(TokenType.RParenthesis);
                    return a;
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
            float result = E();

            Console.WriteLine(result.ToString() + " ok");
        }

    }
}
