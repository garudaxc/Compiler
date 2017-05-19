using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Compiler
{
    struct Instruction
    {
        public enum Op
        {
            Mov,
            Load,
            Store,
            LoadArray,
            StoreArray,
            Add,
            Sub,
            Mul,
            Div,
            NEG,
            INC,
            DEC,
            Push,
            Pop,
            AND,
            OR,
            XOR,
            NOT,
            CMP,
            TEST,
            JMP,
            JZ,
            JNZ,
            JA,
            JAE,
            JB,
            JBE,

            LZ,
            LNZ,
            LA,
            LAE,
            LB,
            LBE,
            Call,
            Ret,
            Halt,
            PInvoke,
        }

        public enum Oper
        {
            R0,
            R1,
            SP,
        }

        public Op op;
        public Oper o0;
        public Oper o1;
        public object val;
        public int i4;
        public float f4;
        public short s2;
        public char c1;


        override public string ToString()
        {
            string s = null;

            if (op >= Op.JMP && op <= Op.JBE)
            {
                s = string.Format("{0}\tlable{1}", Enum.GetName(typeof(Op), op), i4);
                return s;
            }

            if (op == Op.Mov)
            {
                s = string.Format("{0}\t{1}, {2}", Enum.GetName(typeof(Op), op),
                    Enum.GetName(typeof(Oper), o0), val);
            } 
            else if (op == Op.Store || op == Op.Load)
            {
                s = string.Format("{0}\t{1}, sp${2}", Enum.GetName(typeof(Op), op),
                    Enum.GetName(typeof(Oper), o0), i4);
            }
            else if(op == Op.Push || op == Op.Pop)
            {
                s = string.Format("{0}\t{1}", Enum.GetName(typeof(Op), op),
                    Enum.GetName(typeof(Oper), o0));
            }
            else if(op == Op.PInvoke)
            {
                s = string.Format("{0}\t{1}", Enum.GetName(typeof(Op), op), val);
            }
            else if (op == Op.Ret)
            {
                s = string.Format("{0}\t{1}", Enum.GetName(typeof(Op), op), i4);
            }
            else
            {
                s = string.Format("{0}\t{1}, {2}", Enum.GetName(typeof(Op), op),
                    Enum.GetName(typeof(Oper), o0), Enum.GetName(typeof(Oper), o1));
            }

            return s;
        }

    }
}
