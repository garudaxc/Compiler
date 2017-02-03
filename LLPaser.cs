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
                case TokenType.While:
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
            int lf = set_.NewLable();
            Eat(TokenType.If);
            BExpression();
            set_.EmitTest(Instruction.Oper.R0, Instruction.Oper.R0);
            set_.EmitJZ(lf);
            Block(lable);
            int le = ElseStat(lable, lf);            
            set_.AddLable(le);
        }

        int ElseStat(int lable, int lf)
        {
            switch (currentToken_.Type)
            {
                case TokenType.Else:
                    {
                        int le = set_.NewLable();
                        set_.EmitJmp(le);
                        Eat(TokenType.Else);
                        set_.AddLable(lf);
                        Block(lable);
                        return le;
                    }
                default:
                    return lf;
            }
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
                    set_.EmitPush(Instruction.Oper.R0);
                    BTerm();
                    BExpressionPrime();
                    set_.EmitOr(Instruction.Oper.R0, Instruction.Oper.SP);
                    set_.EmitPop(Instruction.Oper.SP);
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
                    set_.EmitPush(Instruction.Oper.R0);
                    BFactor();
                    BTermPrime();
                    set_.EmitAnd(Instruction.Oper.R0, Instruction.Oper.SP);
                    set_.EmitPop(Instruction.Oper.SP);
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
                    set_.EmitCmp(Instruction.Oper.SP, Instruction.Oper.R0);
                    set_.EmitPop(Instruction.Oper.SP);
                    set_.EmitLZ();
                    break;
                case TokenType.NotEqual:
                    Eat(TokenType.NotEqual);
                    set_.EmitPush(Instruction.Oper.R0);
                    E();
                    set_.EmitCmp(Instruction.Oper.SP, Instruction.Oper.R0);
                    set_.EmitPop(Instruction.Oper.SP);
                    set_.EmitLNZ();
                    break;
                case TokenType.Less:
                    Eat(TokenType.Less);
                    set_.EmitPush(Instruction.Oper.R0);
                    E();
                    set_.EmitCmp(Instruction.Oper.SP, Instruction.Oper.R0);
                    set_.EmitPop(Instruction.Oper.SP);
                    set_.EmitLB();
                    break;
                case TokenType.LessEqual:
                    Eat(TokenType.LessEqual);
                    set_.EmitPush(Instruction.Oper.R0);
                    E();
                    set_.EmitCmp(Instruction.Oper.SP, Instruction.Oper.R0);
                    set_.EmitPop(Instruction.Oper.SP);
                    set_.EmitLBE();
                    break;
                case TokenType.Great:
                    Eat(TokenType.Great);
                    set_.EmitPush(Instruction.Oper.R0);
                    E();
                    set_.EmitCmp(Instruction.Oper.SP, Instruction.Oper.R0);
                    set_.EmitPop(Instruction.Oper.SP);
                    set_.EmitLA();
                    break;
                case TokenType.GreatEqual:
                    Eat(TokenType.GreatEqual);
                    set_.EmitPush(Instruction.Oper.R0);
                    E();
                    set_.EmitCmp(Instruction.Oper.SP, Instruction.Oper.R0);
                    set_.EmitPop(Instruction.Oper.SP);
                    set_.EmitLAE();
                    break;
                default:
                    break;
            }

        }

        void E()
        {
            T();
            EPrime();
        }

        void EPrime()
        {
            switch(currentToken_.Type)
            {
                case TokenType.Add:
                    Eat(TokenType.Add); 
                    set_.EmitPush(Instruction.Oper.R0);
                    T();
                    set_.EmitAdd(Instruction.Oper.R0, Instruction.Oper.SP);
                    set_.EmitPop(Instruction.Oper.SP);
                    EPrime();
                    break;
                case TokenType.Minus:
                    Eat(TokenType.Minus);
                    set_.EmitPush(Instruction.Oper.R0);
                    T();
                    set_.EmitNeg(Instruction.Oper.R0);
                    set_.EmitAdd(Instruction.Oper.R0, Instruction.Oper.SP);
                    //set_.EmitSub(Instruction.Oper.R0, Instruction.Oper.SP);
                    set_.EmitPop(Instruction.Oper.SP);
                    EPrime();
                    break;
                default:
                    break;
            }            
        }

        void T()
        {
            F(); 
            TPrime();
        }

        void TPrime()
        {
            switch (currentToken_.Type)
            {
                case TokenType.Div:
                    set_.EmitPush(Instruction.Oper.R0);
                    Eat(TokenType.Div); 
                    F();
                    set_.EmitDiv(Instruction.Oper.R0, Instruction.Oper.SP);
                    set_.EmitPop(Instruction.Oper.SP);
                    TPrime();
                    break;
                case TokenType.Multiply:
                    Eat(TokenType.Multiply); 
                    set_.EmitPush(Instruction.Oper.R0);
                    F(); 
                    set_.EmitMul(Instruction.Oper.R0, Instruction.Oper.SP);
                    set_.EmitPop(Instruction.Oper.SP);
                    TPrime();
                    break;
                default:
                    break;
            }
        }

        void F()
        {
            switch(currentToken_.Type)
            {
                case TokenType.Minus:
                    Eat(TokenType.Minus);
                    F();
                    //set_.EmitMove(Instruction.Oper.R1, 0);
                    //set_.EmitSub(Instruction.Oper.R1, Instruction.Oper.R0);
                    set_.EmitNeg(Instruction.Oper.R0);
                    break;
                case TokenType.Number:
                    int val = currentToken_.i4;
                    set_.EmitMove(Instruction.Oper.R0, val);
                    Eat(TokenType.Number);
                    break;
                case TokenType.Variable:
                    if (!symbols_.Contains(currentToken_.str))
                    {
                        Error("symbol {0} has not been defined!", currentToken_.str);
                    }
                    set_.EmitLoad(Instruction.Oper.R0, currentToken_.str);
                    Eat(TokenType.Variable);                                        
                    break;
                case TokenType.LParenthesis:
                    Eat(TokenType.LParenthesis); 
                    T();
                    EPrime(); 
                    Eat(TokenType.RParenthesis);
                    break;
                default:
                    Error("wrong token {0} expect number/left parentthesis", currentToken_.TypeName);
                    break;

            }
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
