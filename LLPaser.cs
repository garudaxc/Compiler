using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Compiler
{
    class FunctionInfo
    {
        public string name;
        public int numParam;
        public int lable;
    }

    class DummyInfo
    {
    }

    class LLPaser
    {
        Lexer lex_;
        Token currentToken_;
        InstructionSet set_;
        Dictionary<string, object> symbols_;

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
        
        void Declration()
        {
            switch(currentToken_.Type)
            {
                case TokenType.Variable:
                    AssignmentOrCall();
                    break;
                case TokenType.Function:
                    Function();
                    break;
                default:
                    Error("wrong token {0} expect declration", currentToken_.TypeName);
                    break;
            }
        }

        void Block(int start, int end)
        {
            Eat(TokenType.LBrace);
            while (currentToken_.Type != TokenType.RBrace)
            {
                Statement(start, end);
            }
            Eat(TokenType.RBrace);
        }

        void Statement(int start, int end)
        {
            switch (currentToken_.Type)
            {
                case TokenType.Variable:
                    AssignmentOrCall();
                    break;
                case TokenType.If:
                    IfStatement(start, end);
                    break;
                case TokenType.While:
                    WhileStatement();
                    break;
                case TokenType.Continue:
                    Eat(TokenType.Continue);
                    Eat(TokenType.Semicolon);
                    set_.EmitJmp(start);
                    break;
                case TokenType.Break:
                    Eat(TokenType.Break);
                    Eat(TokenType.Semicolon);
                    set_.EmitJmp(end);
                    break;
                case TokenType.Function:
                    Eat(TokenType.Function);
                    break;
                case TokenType.Return:
                    Eat(TokenType.Return);

                    if (currentToken_.Type != TokenType.Semicolon)
                    {
                        E();
                    }

                    set_.EmitRet(0);
                    Eat(TokenType.Semicolon);
                    break;
                case TokenType.Print:
                    Eat(TokenType.Print);
                    Eat(TokenType.Quote);
                    {
                        string s = currentToken_.str;
                        set_.EmitPut(s);
                    }
                    Eat(TokenType.Variable);
                    Eat(TokenType.Quote);
                    Eat(TokenType.Semicolon);
                    break;
                default:
                    Error("wrong token {0} expect statement", currentToken_.TypeName);
                    break;
            }
        }

        void AssignmentOrCall()
        {
            string var = currentToken_.str;
            Eat(TokenType.Variable);

            switch(currentToken_.Type)
            {
                case TokenType.Equal:
                    Eat(TokenType.Equal);
                    E();
                    set_.EmitStore(var, Instruction.Oper.R0);
                    Eat(TokenType.Semicolon);
                    if (!symbols_.ContainsKey(var))
                    {
                        symbols_.Add(var, null);
                    }
                    break;
                case TokenType.LParenthesis:
                    FunctionCall(var);
                    Eat(TokenType.Semicolon);
                    break;
                default:
                    break;
            }
        }

        void FunctionCall(string name)
        {
            Eat(TokenType.LParenthesis);

            int numParam = 0;
            while (currentToken_.Type != TokenType.RParenthesis)
            {
                E();
                set_.EmitPush(Instruction.Oper.R0);
                numParam++;
                if(currentToken_.Type == TokenType.Comma)
                {
                    Eat(TokenType.Comma);
                }
            }
            // push parameter
            Eat(TokenType.RParenthesis);
            object obj;
            if (!symbols_.TryGetValue(name, out obj))
            {
                Error("function {0} not defined!", name);
            }
            FunctionInfo info = obj as FunctionInfo;
            set_.EmitCall(info.lable);
        }

        void Function()
        {
            Eat(TokenType.Function);
            string name = currentToken_.str;
            if (symbols_.ContainsKey(name))
            {
                Error("funtion {0} redefine", name);
            }

            int lable = set_.NewLable();
            set_.AddLable(lable);

            FunctionInfo info = new FunctionInfo();
            info.name = name;
            info.numParam = 0;
            info.lable = lable;

            symbols_.Add(name, info);
            if (name == "main")
            {
                set_.EnterPoint = lable;
            }

            Eat(TokenType.Variable);
            Eat(TokenType.LParenthesis);
            // parameter list
            Eat(TokenType.RParenthesis);
            Block(-1, -1);
            set_.EmitRet(0);
        }

        void ParameterList()
        {

        }

        void IfStatement(int start, int end)
        {
            int lf = set_.NewLable();
            Eat(TokenType.If);
            BExpression();
            set_.EmitTest(Instruction.Oper.R0, Instruction.Oper.R0);
            set_.EmitJZ(lf);
            Block(start, end);
            int le = ElseStat(start, end, lf);            
            set_.AddLable(le);
        }

        void WhileStatement()
        {
            int start = set_.NewLable();
            int end = set_.NewLable();
            Eat(TokenType.While);
            set_.AddLable(start);
            BExpression();
            set_.EmitTest(Instruction.Oper.R0, Instruction.Oper.R0);
            set_.EmitJZ(end);
            Block(start, end);
            set_.EmitJmp(start);
            set_.AddLable(end);
        }

        int ElseStat(int start, int end, int lf)
        {
            switch (currentToken_.Type)
            {
                case TokenType.Else:
                    {
                        int le = set_.NewLable();
                        set_.EmitJmp(le);
                        Eat(TokenType.Else);
                        set_.AddLable(lf);
                        Block(start, end);
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
                    {
                        string name = currentToken_.str;
                        if (!symbols_.ContainsKey(name))
                        {
                            Error("symbol {0} has not been defined!", name);
                        }
                        Eat(TokenType.Variable);

                        if (currentToken_.Type == TokenType.LParenthesis)
                        {
                            FunctionCall(name);
                        }
                        else
                        {
                            set_.EmitLoad(Instruction.Oper.R0, name);
                        }
                    }
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
            symbols_ = new Dictionary<string, object>();
            lex_ = lex;
            currentToken_ = lex_.GetNextToken();

            while(currentToken_.Type != TokenType.End)
            {
                Declration();
            }

            //Block(-1, -1);
            //Console.WriteLine(result.ToString() + " ok");

            return set_;
        }

    }
}
