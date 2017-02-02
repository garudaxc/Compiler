using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Compiler
{
    class LLPaser
    {
        Lexer lex_;
        Token currentToken_;
        InstructionSet set_;
        HashSet<string> symbols_;

        public LLPaser()
        {
        }

        void Error(string fmt, params Object[] args)
        {
            string str = string.Format(fmt, args);
            Console.WriteLine(str);
            throw new Exception();
        }
        
        void Eat(TokenType type)
        {
            if (currentToken_.Type == type)
            {
                currentToken_ = lex_.GetNextToken();
            }
            else
            {
                Error("wrong token type {0} expect {1}", currentToken_.TypeName, Enum.GetName(typeof(TokenType), type));
            }            
        }

        void Block(int lable)
        {
            Eat(TokenType.LBrace);
            while (currentToken_.Type != TokenType.RBrace)
            {
                Statement(lable);
            }
            Eat(TokenType.RBrace);
        }

        void Statement(int lable)
        {
            switch (currentToken_.Type)
            {
                case TokenType.Variable:
                    Assignment();
                    break;
                case TokenType.If:
                    IfStatement(lable);
                    break;
                default:
                    Error("wrong token {0} expect statement", currentToken_.TypeName);
                    break;
            }
        }

        void Assignment()
        {
            string var = currentToken_.str;
            Eat(TokenType.Variable);
            Eat(TokenType.Equal);
            E();
            set_.EmitStore(var, Instruction.Oper.R0); 
            Eat(TokenType.Semicolon);
            symbols_.Add(var);
        }

        void IfStatement(int lable)
        {
            int newLable = set_.NewLable();
            Eat(TokenType.If);
            BExpression();
            Block(lable);
            set_.AddLable(newLable);
        }
        
        void BExpression()
        {
            BTerm();
            BExpressionPrime();
        }

        void BExpressionPrime()
        {
            switch (currentToken_.Type)
            {
                case TokenType.Or:
                    Eat(TokenType.Or);
                    BTerm();
                    BExpressionPrime();

                    break;
                default:
                    break;
            }            
        }

        
        void BTerm()
        {
            BFactor();
            BTermPrime();
        }

        void BTermPrime()
        {
            switch (currentToken_.Type)
            {
                case TokenType.And:
                    Eat(TokenType.And);
                    BFactor();
                    BTermPrime();

                    break;
                default:
                    break;
            }
        }
               

        void BFactor()
        {
            switch (currentToken_.Type)
            {
                case TokenType.Not:
                    Eat(TokenType.Not);
                    BFactor();
                    break;
                default:
                    E();
                    InequalOp();
                    break;
            }
        }

        void InequalOp()
        {
            switch (currentToken_.Type)
            {
                case TokenType.EqualEqual:
                    Eat(TokenType.EqualEqual);
                    set_.EmitPush(Instruction.Oper.R0);
                    E();
                    set_.EmitSub(Instruction.Oper.R0, Instruction.Oper.SP);
                    set_.EmitPop(Instruction.Oper.SP);
                    set_.EmitAnd(Instruction.Oper.R0, 0xffffffff);
                    break;
                case TokenType.NotEqual:
                    Eat(TokenType.NotEqual);
                    E();
                    break;
                case TokenType.Less:
                    Eat(TokenType.Less);
                    E();
                    break;
                case TokenType.LessEqual:
                    Eat(TokenType.LessEqual);
                    E();
                    break;
                case TokenType.Great:
                    Eat(TokenType.Great);
                    E();
                    break;
                case TokenType.GreatEqual:
                    Eat(TokenType.GreatEqual);
                    E();
                    break;
                default:
                    break;
            }

        }

        float E()
        {
            set_.EmitMove(Instruction.Oper.R0, 0);
            float a = T(); a = EPrime(a);
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
                case TokenType.Minus:
                    Eat(TokenType.Minus);
                    F();
                    set_.EmitMove(Instruction.Oper.R1, 0);
                    set_.EmitSub(Instruction.Oper.R1, Instruction.Oper.R0);
                    break;
                case TokenType.Number:
                    float val = currentToken_.value;
                    set_.EmitMove(Instruction.Oper.R0, val);
                    Eat(TokenType.Number);
                    return val;
                case TokenType.Variable:
                    if (!symbols_.Contains(currentToken_.str))
                    {
                        Error("symbol {0} has not been defined!", currentToken_.str);
                    }
                    set_.EmitLoad(Instruction.Oper.R0, currentToken_.str);
                    Eat(TokenType.Variable);                                        
                    break;
                case TokenType.LParenthesis:
                    Eat(TokenType.LParenthesis); float a = T(); a = EPrime(a); Eat(TokenType.RParenthesis);
                    return a;
                default:
                    Error("wrong token {0} expect number/left parentthesis", currentToken_.TypeName);
                    break;

            }
            return 0.0f;
        }

        public InstructionSet Parse(Lexer lex)
        {
            set_ = new InstructionSet();
            symbols_ = new HashSet<string>();
            lex_ = lex;
            currentToken_ = lex_.GetNextToken();
            Block(-1);

            //Console.WriteLine(result.ToString() + " ok");

            return set_;
        }

    }
}
