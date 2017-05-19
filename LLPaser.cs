using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Compiler
{
    class SymbolInfo
    {
        public enum SymbolType
        {
            Variable,
            Function,
        }

        public SymbolType type;
        public string name;
    }

    class FunctionInfo : SymbolInfo
    {
        public FunctionInfo()
        {
            type = SymbolType.Function;
        }

        public int numParam;
        public int numSymbol;
        public int lable;
    }

    class VariableInfo : SymbolInfo
    {
        public enum ValueType
        {
            Integer,
            Float,
            String,
            Array,
        }

        public VariableInfo()
        {
            type = SymbolType.Variable;
        }

        public ValueType valueType;
    }

    class LLPaser
    {
        Lexer lex_;
        Token currentToken_;
        InstructionSet set_;

        SymbolTable table_;

        public LLPaser()
        {
        }

        void Error(string fmt, params Object[] args)
        {
            string str = string.Format(fmt, args);
            Console.WriteLine(str);
            throw new Exception();
        }
        
        void Accept(TokenType type)
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
            Accept(TokenType.LBrace);
            while (currentToken_.Type != TokenType.RBrace)
            {
                Statement(start, end);
            }
            Accept(TokenType.RBrace);
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
                    Accept(TokenType.Continue);
                    Accept(TokenType.Semicolon);
                    set_.EmitJmp(start);
                    break;
                case TokenType.Break:
                    Accept(TokenType.Break);
                    Accept(TokenType.Semicolon);
                    set_.EmitJmp(end);
                    break;
                case TokenType.Return:
                    Accept(TokenType.Return);
                    if (currentToken_.Type != TokenType.Semicolon)
                    {
                        Expr();
                    }
                    set_.EmitRet(table_.NumSymbol);
                    Accept(TokenType.Semicolon);
                    break;
                default:
                    Error("wrong token {0} expect statement", currentToken_.TypeName);
                    break;
            }
        }

        void RightRefValue()
        {
            if (currentToken_.Type == TokenType.LBracket)
            {
                Accept(TokenType.LBracket);

                int numItem = 0;
                while (currentToken_.Type != TokenType.RBracket)
                {
                    Expr();
                    set_.EmitPush(Instruction.Oper.R0);
                    numItem++;
                    if (currentToken_.Type == TokenType.Comma)
                    {
                        Accept(TokenType.Comma);
                    }
                }

                // item list
                Accept(TokenType.RBracket);

                // pcall new array
                set_.EmitMove(Instruction.Oper.R0, numItem);
                set_.EmitPush(Instruction.Oper.R0);
                set_.EmitNativeInvoke("new_array");
            }
            else
            {
                Expr();
            }
        }

        void AssignmentOrCall()
        {
            string var = currentToken_.str;
            Accept(TokenType.Variable);                

            switch(currentToken_.Type)
            {
                case TokenType.Equal:
                    if (!table_.Contains(var))
                    {
                        VariableInfo info = new VariableInfo();
                        info.name = var;

                        table_.Add(var, info);
                        set_.EmitPush(Instruction.Oper.SP);               
                    }

                    Accept(TokenType.Equal);
                    
                    RightRefValue();

                    int offset = table_.GetSymbolOffset(var);
                    //set_.EmitStore(var, Instruction.Oper.R0);
                    set_.EmitStore(Instruction.Oper.R0, offset);

                    Accept(TokenType.Semicolon);

                    break;
                case TokenType.LParenthesis:
                    FunctionCall(var);
                    Accept(TokenType.Semicolon);
                    break;
                case TokenType.LBracket:
                    {
                        Accept(TokenType.LBracket);
                        Expr();
                        set_.EmitPush(Instruction.Oper.R0);
                        Accept(TokenType.RBracket);

                        Accept(TokenType.Equal);

                        RightRefValue();
                        set_.EmitPush(Instruction.Oper.R0);
                        set_.EmitPop(Instruction.Oper.R1);
                        set_.EmitPop(Instruction.Oper.R0);

                        offset = table_.GetSymbolOffset(var);
                        set_.EmitStoreArray(Instruction.Oper.R1, offset);

                        Accept(TokenType.Semicolon);
                    }

                    break;
                default:
                    break;
            }
        }

        void FunctionCall(string name)
        {
            Accept(TokenType.LParenthesis);

            int numParam = 0;
            while (currentToken_.Type != TokenType.RParenthesis)
            {
                Expr();
                set_.EmitPush(Instruction.Oper.R0);
                numParam++;
                if(currentToken_.Type == TokenType.Comma)
                {
                    Accept(TokenType.Comma);
                }
            }
            // push parameter
            Accept(TokenType.RParenthesis);
            object obj;

            if ((obj = table_.GetSymbol(name)) == null)
            {
                //Error("function {0} not defined!", name);
                set_.EmitNativeInvoke(name);
                return;
            }

            FunctionInfo info = obj as FunctionInfo;
            if (numParam != info.numParam)
            {
                Error("function call {0} parameter number not match {1} expect {2}", name, numParam, info.numParam);
            }
            set_.EmitCall(info.lable);
        }


        void Function()
        {
            Accept(TokenType.Function);
            string name = currentToken_.str;

            if (table_.Contains(name))
            {
                Error("funtion {0} redefine", name);
            }

            int lable = set_.NewLable();
            set_.AddLable(lable);

            FunctionInfo info = new FunctionInfo();
            info.name = name;
            info.lable = lable;

            //symbols_.Add(name, info);
            table_.AddGlobal(name, info);

            if (name == "main")
            {
                set_.EnterPoint = lable;
            }

            Accept(TokenType.Variable);

            table_.PushBarrier();

            Accept(TokenType.LParenthesis);
            // parameter list
            info.numParam = FormalParameter();
            table_.SetNumFormalParam(info.numParam);
            Accept(TokenType.RParenthesis);
            Block(-1, -1);

            info.numSymbol = table_.NumSymbol;
            set_.EmitRet(info.numSymbol);

            table_.PopBarrier();
        }

        int FormalParameter()
        {
            int numParam = 0;
            while(currentToken_.Type != TokenType.RParenthesis)
            {
                string name = currentToken_.str;
                Accept(TokenType.Variable);
                if (table_.Contains(name))
                {
                    Error("already have symbol {0}", name);
                }

                table_.Add(name, null);
                numParam++;
                // table add
                if (currentToken_.Type == TokenType.Comma)
                {
                    Accept(TokenType.Comma);
                }
            }
            return numParam;
        }

        void IfStatement(int start, int end)
        {
            int lf = set_.NewLable();
            Accept(TokenType.If);
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
            Accept(TokenType.While);
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
                        Accept(TokenType.Else);
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
                    Accept(TokenType.Or);
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
                    Accept(TokenType.And);
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
                    Accept(TokenType.Not);
                    BFactor();
                    break;
                default:
                    Expr();
                    InequalOp();
                    break;
            }
        }

        void InequalOp()
        {
            switch (currentToken_.Type)
            {
                case TokenType.EqualEqual:
                    Accept(TokenType.EqualEqual);
                    set_.EmitPush(Instruction.Oper.R0);
                    Expr();
                    set_.EmitCmp(Instruction.Oper.SP, Instruction.Oper.R0);
                    set_.EmitPop(Instruction.Oper.SP);
                    set_.EmitLZ();
                    break;
                case TokenType.NotEqual:
                    Accept(TokenType.NotEqual);
                    set_.EmitPush(Instruction.Oper.R0);
                    Expr();
                    set_.EmitCmp(Instruction.Oper.SP, Instruction.Oper.R0);
                    set_.EmitPop(Instruction.Oper.SP);
                    set_.EmitLNZ();
                    break;
                case TokenType.Less:
                    Accept(TokenType.Less);
                    set_.EmitPush(Instruction.Oper.R0);
                    Expr();
                    set_.EmitCmp(Instruction.Oper.SP, Instruction.Oper.R0);
                    set_.EmitPop(Instruction.Oper.SP);
                    set_.EmitLB();
                    break;
                case TokenType.LessEqual:
                    Accept(TokenType.LessEqual);
                    set_.EmitPush(Instruction.Oper.R0);
                    Expr();
                    set_.EmitCmp(Instruction.Oper.SP, Instruction.Oper.R0);
                    set_.EmitPop(Instruction.Oper.SP);
                    set_.EmitLBE();
                    break;
                case TokenType.Great:
                    Accept(TokenType.Great);
                    set_.EmitPush(Instruction.Oper.R0);
                    Expr();
                    set_.EmitCmp(Instruction.Oper.SP, Instruction.Oper.R0);
                    set_.EmitPop(Instruction.Oper.SP);
                    set_.EmitLA();
                    break;
                case TokenType.GreatEqual:
                    Accept(TokenType.GreatEqual);
                    set_.EmitPush(Instruction.Oper.R0);
                    Expr();
                    set_.EmitCmp(Instruction.Oper.SP, Instruction.Oper.R0);
                    set_.EmitPop(Instruction.Oper.SP);
                    set_.EmitLAE();
                    break;
                default:
                    break;
            }

        }

        void Expr()
        {
            T();
            EPrime();
        }

        void EPrime()
        {
            switch(currentToken_.Type)
            {
                case TokenType.Add:
                    Accept(TokenType.Add); 
                    set_.EmitPush(Instruction.Oper.R0);
                    T();
                    set_.EmitAdd(Instruction.Oper.R0, Instruction.Oper.SP);
                    set_.EmitPop(Instruction.Oper.SP);
                    EPrime();
                    break;
                case TokenType.Minus:
                    Accept(TokenType.Minus);
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
                    Accept(TokenType.Div); 
                    F();
                    set_.EmitDiv(Instruction.Oper.R0, Instruction.Oper.SP);
                    set_.EmitPop(Instruction.Oper.SP);
                    TPrime();
                    break;
                case TokenType.Multiply:
                    Accept(TokenType.Multiply); 
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
                    Accept(TokenType.Minus);
                    F();

                    set_.EmitNeg(Instruction.Oper.R0);
                    break;
                case TokenType.Number:
                    {
                        int val = currentToken_.i4;
                        set_.EmitMove(Instruction.Oper.R0, val);
                        Accept(TokenType.Number);
                        break;
                    }
                case TokenType.String:
                    {
                        string val = currentToken_.str;
                        set_.EmitMove(Instruction.Oper.R0, val);
                        Accept(TokenType.String);
                        break;
                    }
                case TokenType.Variable:
                    {
                        string name = currentToken_.str;
                        Accept(TokenType.Variable);

                        if (currentToken_.Type == TokenType.LParenthesis)
                        {
                            FunctionCall(name);
                        }
                        else if(currentToken_.Type == TokenType.LBracket)
                        {
                            Accept(TokenType.LBracket);
                            Expr();
                            Accept(TokenType.RBracket);
                            int offset = table_.GetSymbolOffset(name);
                            set_.EmitLoadArray(Instruction.Oper.R0, offset);
                        }
                        else
                        {
                            int offset = table_.GetSymbolOffset(name);
                            set_.EmitLoad(Instruction.Oper.R0, offset);
                        }
                    }
                    break;
                case TokenType.LParenthesis:
                    Accept(TokenType.LParenthesis); 
                    T();
                    EPrime(); 
                    Accept(TokenType.RParenthesis);
                    break;
                default:
                    Error("wrong token {0} expect number/left parentthesis", currentToken_.TypeName);
                    break;
            }
        }

        public InstructionSet Parse(Lexer lex)
        {
            set_ = new InstructionSet();
            table_ = new SymbolTable();
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
