using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Compiler
{
    class VM
    {
        object programe;
        object r0;
        object r1;

        Stack<object> stack = new Stack<object>();


        public void Run(InstructionSet set)
        {
            for (int i = 0; i < set.Count; i++)
            {
                Instruction ins = set[i];
                Execute(ins);
            }

            Console.WriteLine("run result {0}", r0);
        }


        object GetOper(Instruction.Oper oper)
        {
            if (oper == Instruction.Oper.R0)
            {
                return r0;
            }
            else if (oper == Instruction.Oper.R1)
            {
                return r1;
            }
            else if (oper == Instruction.Oper.SP)
            {
                return stack.Peek();
            }

            return null;
        }

        void Execute(Instruction ins)
        {
            switch(ins.op)
            {
                case Instruction.Op.Mov:
                    {
                        r0 = ins.val;
                    }
                    break;
                case Instruction.Op.Add:
                    {
                        object a = GetOper(ins.o0);
                        object b = GetOper(ins.o1);
                        r0 = (float)a + (float)b;
                    }

                    break;
                case Instruction.Op.Sub:
                    {
                        object a = GetOper(ins.o0);
                        object b = GetOper(ins.o1);
                        r0 = (float)a - (float)b;
                    }
                    break;
                case Instruction.Op.Mul:
                    {
                        object a = GetOper(ins.o0);
                        object b = GetOper(ins.o1);
                        r0 = (float)a * (float)b;
                    }
                    break;
                case Instruction.Op.Div:
                    {
                        object a = GetOper(ins.o0);
                        object b = GetOper(ins.o1);
                        r0 = (float)a / (float)b;
                    }
                    break;
                case Instruction.Op.Push:
                    {
                        object a = GetOper(ins.o0);
                        stack.Push(a);
                    }
                    break;
                case Instruction.Op.Pop:
                    {
                        if (ins.o0 == Instruction.Oper.R0)
                        {
                            r0 = stack.Pop();
                        }
                        else if (ins.o0 == Instruction.Oper.R1)
                        {
                            r1 = stack.Pop();
                        }
                        else if (ins.o0 == Instruction.Oper.SP)
                        {
                            stack.Pop();
                        }
                    }
                    break;
            }
        }

    }
}
