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
            Add,
            Sub,
            Mul,
            Div,
            Push,
            Pop,
            Jmp,
            GE,
            LE,
            Call
        }

        struct OpInfo
        {
            int numOper;

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
        public Type type;

        override public string ToString()
        {
            string s = null;
            if (op == Op.Mov)
            {
                s = string.Format("{0}\t{1}, {2}", Enum.GetName(typeof(Op), op),
                    Enum.GetName(typeof(Oper), o0), val);
            } 
            else if(op == Op.Push || op == Op.Pop)
            {
                s = string.Format("{0}\t{1}", Enum.GetName(typeof(Op), op),
                    Enum.GetName(typeof(Oper), o0));
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
