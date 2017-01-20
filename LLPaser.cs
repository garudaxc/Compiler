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
        InstructionSet set_;

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
            set_.EmitMove(0, Instruction.Oper.R0);
            float a = T(); a = EPrime(a); Eat(TokenType.Semicolon);
            return a;
        }

        float EPrime(float a)
        {
            float b, c;
            switch(currentToken_.Type)
            {
                case TokenType.Add:
                    Eat(TokenType.Add); 
                    set_.EmitPush(Instruction.Oper.R0);
                    b = T();
                    set_.EmitAdd(Instruction.Oper.R0, Instruction.Oper.SP);
                    set_.EmitPop(Instruction.Oper.SP);
                    c = EPrime(a + b);
                    return c;
                case TokenType.Minus:
                    Eat(TokenType.Minus);
                    set_.EmitPush(Instruction.Oper.R0);
                    b = T(); 
                    set_.EmitSub(Instruction.Oper.R0, Instruction.Oper.SP);
                    set_.EmitPop(Instruction.Oper.SP);
                    c = EPrime(a - b);
                    return c;
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
            float b, c;
            switch (currentToken_.Type)
            {
                case TokenType.Div:
                    set_.EmitPush(Instruction.Oper.R0);
                    Eat(TokenType.Div); 
                    b = F();
                    set_.EmitDiv(Instruction.Oper.R0, Instruction.Oper.SP);
                    set_.EmitPop(Instruction.Oper.SP);
                    c = TPrime(a / b);
                    return c;
                case TokenType.Multiply:
                    Eat(TokenType.Multiply); 
                    set_.EmitPush(Instruction.Oper.R0);
                    b = F(); 
                    set_.EmitMul(Instruction.Oper.R0, Instruction.Oper.SP);
                    set_.EmitPop(Instruction.Oper.SP);
                    c = TPrime(a * b);
                    return c;
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
                    set_.EmitMove(val, Instruction.Oper.R0);
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

        public InstructionSet Parse(Lexer lex)
        {
            set_ = new InstructionSet();
            lex_ = lex;
            currentToken_ = lex_.GetNextToken();
            float result = E();

            Console.WriteLine(result.ToString() + " ok");

            return set_;
        }

    }
}
