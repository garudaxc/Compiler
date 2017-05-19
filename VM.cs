using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Compiler
{
    static class ExtentClass
    {
        public static void Push(this List<object> list, object obj)
        {
            list.Add(obj);
        }

        public static object Pop(this List<object> list)
        {
            object obj = list[list.Count - 1];
            list.RemoveAt(list.Count - 1);
            return obj;
        }

        public static void Pop(this List<object> list, int count)
        {
            if (count == 0)
            {
                return;
            }
            list.RemoveRange(list.Count - count, count);
        }
        
        public static object Peek(this List<object> list)
        {
            object obj = list[list.Count - 1];
            return obj;
        }
    }

    class NativeInvok
    {
        public static void StringLength(VM vm)
        {
            string s = (string)vm.PopStack();
            vm.PushStack(s.Length);
        }

        public static void Print(VM vm)
        {
            object o = vm.PopStack();
            if (o.GetType().IsGenericType && o.GetType() == typeof(List<object>))
            {
                List<object> l = (List<object>)o;
                Console.Write("[");
                l.ForEach((i) => Console.Write(i + ", "));
                Console.WriteLine("] size " + l.Count);
            }
            else
            {
                Console.WriteLine(o);
            }
            vm.PushStack(0);
        }

        public static void ArrayAppend(VM vm)
        {
            object o = vm.PopStack();
            List<object> list = (List<object>)vm.PopStack();
            list.Add(o);
            vm.PushStack(list);
        }

        public static void NewArray(VM vm)
        {
            int size = (int)vm.PopStack();
            List<object> list = new List<object>();
            while(size-- > 0)
            {
                list.Insert(0, vm.PopStack());
            }
            vm.PushStack(list);
        }

    }

    class VM
    {
        object r0;
        object r1;
        int zf;
        int sf;
        int current;
        InstructionSet set_;
        int stackFrame = 0;

        List<object> stack = new List<object>();

        // global storage

        Dictionary<string, Action<VM>> nativeInvoke = new Dictionary<string, Action<VM>>();

        public object PopStack()
        {
            object o = stack.Pop();
            return o;
        }

        public void PushStack(object o)
        {
            stack.Push(o);
        }

        public VM()
        {
            InitNativeInvoke();
        }

        public void AddNativeInvoke(string name, Action<VM> func)
        {
            nativeInvoke.Add(name, func);
        }

        void InitNativeInvoke()
        {
            AddNativeInvoke("strlen", NativeInvok.StringLength);
            AddNativeInvoke("print", NativeInvok.Print);
            AddNativeInvoke("append", NativeInvok.ArrayAppend);
            AddNativeInvoke("new_array", NativeInvok.NewArray);
        }
        
        void Error(string fmt, params Object[] args)
        {
            string str = string.Format(fmt, args);
            Console.WriteLine(str);
            throw new Exception();
        }

        public void Run(InstructionSet set)
        {
            set_ = set;
            if (set.EnterPoint == -1)
            {
                Console.WriteLine("no entery point(main) founded!");
                return;
            }

            stackFrame = -1;
            stack.Push(stackFrame);
            stack.Push(-2);
            stackFrame = stack.Count - 1;

            current = set.GetLablePosition(set.EnterPoint);

            //return;
            while (current != -1)
            {
                Instruction ins = set[current];
                current = Execute(ins);
            }

            //Console.WriteLine("run result");
            //foreach(var p in symbols)
            //{
            //    Console.WriteLine("{0} = {1}", p.Key, p.Value);
            //}
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
                case Instruction.Op.Halt:
                    return -1;
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
                        object val = stack[stackFrame + ins.i4];
                        SetValue(ins.o0, val);
                    }
                    return current + 1;
                case Instruction.Op.Store:
                    {
                        object val = GetOper(ins.o0);
                        stack[stackFrame + ins.i4] = val;
                    }
                    return current + 1;
                case Instruction.Op.LoadArray:
                    {
                        List<object> val = (List<object>)stack[stackFrame + ins.i4];
                        int index = (int)GetOper(Instruction.Oper.R0);
                        SetValue(ins.o0, val[index]);
                    }
                    return current + 1;
                case Instruction.Op.StoreArray:
                    {
                        List<object> val = (List<object>)stack[stackFrame + ins.i4];
                        int index = (int)GetOper(Instruction.Oper.R0);
                        val[index] = GetOper(ins.o0);
                    }
                    return current + 1;
                case Instruction.Op.Call:
                    {
                        stack.Push(stackFrame);
                        stack.Push(current);
                        stackFrame = stack.Count - 1;
                    }
                    return set_.GetLablePosition(ins.i4);
                case Instruction.Op.Ret:
                    {
                        int ps = (int)stack[stackFrame];
                        stackFrame = (int)stack[stackFrame - 1];
                        int i = ins.i4;
                        stack.Pop(i + 2);

                        return ps + 1;
                    }
                case Instruction.Op.PInvoke:
                    {
                        string name = (string)ins.val;
                        Action<VM> func;
                        if (!nativeInvoke.TryGetValue(name, out func))
                        {
                            Error("can not find function {0}", name);
                        }
                        func(this);
                        r0 = stack.Pop();
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
                    return set_.GetLablePosition(ins.i4);
                case Instruction.Op.JZ:
                    if (zf == 1)
                    {
                        return set_.GetLablePosition(ins.i4);
                    }
                    break;
                case Instruction.Op.JNZ:
                    if (zf == 0)
                    {
                        return set_.GetLablePosition(ins.i4);
                    }
                    break;
                case Instruction.Op.JA:
                    if (zf == 0 && sf == 0)
                    {
                        return set_.GetLablePosition(ins.i4);
                    }
                    break;
                case Instruction.Op.JAE:
                    if (sf == 0 || zf == 1)
                    {
                        return set_.GetLablePosition(ins.i4);
                    }
                    break;
                case Instruction.Op.JB:
                    if (zf == 0 && sf == 1)
                    {
                        return set_.GetLablePosition(ins.i4);
                    }
                    break;
                case Instruction.Op.JBE:
                    if (sf == 1 || zf == 1)
                    {
                        return set_.GetLablePosition(ins.i4);
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
