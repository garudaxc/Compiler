using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Compiler
{
    class VM
    {
        object r0;
        object r1;
        int zf;
        int sf;
        int current;
        InstructionSet set_;

        Stack<object> stack = new Stack<object>();
        Dictionary<string, object> symbols = new Dictionary<string, object>();

        
        void Error(string fmt, params Object[] args)
        {
            string str = string.Format(fmt, args);
            Console.WriteLine(str);
            throw new Exception();
        }

        public void Run(InstructionSet set)
        {
            set_ = set;
            for (current = 0; current < set.Count;)
            {
                Instruction ins = set[current];
                current = Execute(ins);
            }

            Console.WriteLine("run result");
            foreach(var p in symbols)
            {
                Console.WriteLine("{0} = {1}", p.Key, p.Value);
            }
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

        void SetValue(Instruction.Oper oper, object val)
        {
            if (oper == Instruction.Oper.R0)
            {
                r0 = val;
            }
            else if (oper == Instruction.Oper.R1)
            {
                r1 = val;
            }
        }

        int Execute(Instruction ins)
        {
            switch(ins.op)
            {
                case Instruction.Op.Mov:
                    {
                        SetValue(ins.o0, ins.val);
                    }
                    return current + 1;
                case Instruction.Op.Add:
                    {
                        object a = GetOper(ins.o0);
                        object b = GetOper(ins.o1);
                        int v = (int)a + (int)b;
                        SetValue(ins.o0, v);
                    }
                    return current + 1;
                case Instruction.Op.NEG:
                    {
                        object a = GetOper(ins.o0);
                        SetValue(ins.o0, -(int)a);
                    }
                    return current + 1;
                case Instruction.Op.Mul:
                    {
                        object a = GetOper(ins.o0);
                        object b = GetOper(ins.o1);
                        int v = (int)a * (int)b;
                        SetValue(ins.o0, v);
                    }
                    return current + 1;
                case Instruction.Op.Div:
                    {
                        object a = GetOper(ins.o0);
                        object b = GetOper(ins.o1);
                        int v = (int)a / (int)b;
                        SetValue(ins.o0, v);
                    }
                    return current + 1;
                case Instruction.Op.Push:
                    {
                        object a = GetOper(ins.o0);
                        stack.Push(a);
                    }
                    return current + 1;
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
                    return current + 1;
                case Instruction.Op.Load:
                    {
                        object val;
                        if (!symbols.TryGetValue((string)ins.val, out val))
                        {
                            Error("can not find varble {0}", ins.val);
                        }

                        SetValue(ins.o0, val);
                    }
                    return current + 1;
                case Instruction.Op.Store:
                    {
                        object a = GetOper(ins.o0);
                        symbols[(string)ins.val] = a;
                    }
                    return current + 1;
                case Instruction.Op.CMP:
                    {
                        object a = GetOper(ins.o0);
                        object b = GetOper(ins.o1);
                        int v = (int)a - (int)b;
                        if (v == 0)
                        {
                            zf = 1;
                        }
                        else
                        {
                            zf = 0;
                        }

                        if (v < 0)
                        {
                            sf = 1;
                        }
                        else
                        {
                            sf = 0;
                        }
                    }
                    return current + 1;
                case Instruction.Op.TEST:
                    {
                        object a = GetOper(ins.o0);
                        object b = GetOper(ins.o1);
                        int v = (int)a & (int)b;
                        if (v == 0)
                        {
                            zf = 1;
                        }
                        else
                        {
                            zf = 0;
                        }

                        if (v < 0)
                        {
                            sf = 1;
                        }
                        else
                        {
                            sf = 0;
                        }
                    }
                    return current + 1;
                case Instruction.Op.AND:
                    {
                        object a = GetOper(ins.o0);
                        object b = GetOper(ins.o1);
                        int v = (int)a & (int)b;
                        SetValue(ins.o0, v);
                    }
                    return current + 1;
                case Instruction.Op.OR:
                    {
                        object a = GetOper(ins.o0);
                        object b = GetOper(ins.o1);
                        int v = (int)a | (int)b;
                        SetValue(ins.o0, v);
                    }
                    return current + 1;
                case Instruction.Op.JMP:
                    return set_.GetLable(ins.i4);
                case Instruction.Op.JZ:
                    if (zf == 1)
                    {
                        return set_.GetLable(ins.i4);
                    }
                    break;
                case Instruction.Op.JNZ:
                    if (zf == 0)
                    {
                        return set_.GetLable(ins.i4);
                    }
                    break;
                case Instruction.Op.JA:
                    if (zf == 0 && sf == 0)
                    {
                        return set_.GetLable(ins.i4);
                    }
                    break;
                case Instruction.Op.JAE:
                    if (sf == 0 || zf == 1)
                    {
                        return set_.GetLable(ins.i4);
                    }
                    break;
                case Instruction.Op.JB:
                    if (zf == 0 && sf == 1)
                    {
                        return set_.GetLable(ins.i4);
                    }
                    break;
                case Instruction.Op.JBE:
                    if (sf == 1 || zf == 1)
                    {
                        return set_.GetLable(ins.i4);
                    }
                    break;

                case Instruction.Op.LZ:
                    r0 = zf;
                    return current + 1;
                case Instruction.Op.LNZ:
                    r0 = zf ^ 0x1;
                    return current + 1;
                case Instruction.Op.LA:
                    r0 = (zf ^ 0x1) & (sf ^ 0x1);
                    return current + 1;
                case Instruction.Op.LAE:
                    r0 = (sf ^ 0x1) | zf;
                    return current + 1;
                case Instruction.Op.LB:
                    r0 = (zf ^ 0x1) & sf;
                    return current + 1;
                case Instruction.Op.LBE:
                    r0 = sf | zf;
                    return current + 1;
                default:
                    Error("not valid instructon {0}", Enum.GetName(typeof(Instruction.Op), ins.op));
                    break;
            }
            return current + 1;
        }

    }
}
