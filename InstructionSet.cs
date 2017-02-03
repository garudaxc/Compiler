using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Compiler
{
    class InstructionSet
    {
        List<Instruction> list = new List<Instruction>();
        List<int> lables = new List<int>();

        public int Count
        {
            get
            {
                return list.Count;
            }
        }

        public Instruction this[int index]
        {
            get
            {
                return list[index];
            }
        }

        public int GetLable(int l)
        {
            return lables[l];
        }

        static int lableIndex_ = 0;
        public int NewLable()
        {
            return lableIndex_++;
        }

        public void AddLable(int lable)
        {
            while(lables.Count <= lable)
            {
                lables.Add(-1);
            }

            lables[lable] = list.Count;
        }

        public void Emit(Instruction ins)
        {
            list.Add(ins);
        }

        public void EmitMove(Instruction.Oper oper, object constValue)
        {
            Instruction ins = new Instruction();
            ins.op = Instruction.Op.Mov;
            ins.o0 = oper;
            ins.val = constValue;
            list.Add(ins);
        }

        public void EmitLoad(Instruction.Oper oper, string var)
        {
            Instruction ins = new Instruction();
            ins.op = Instruction.Op.Load;
            ins.o0 = oper;
            ins.val = var;
            list.Add(ins);            
        }

        public void EmitStore(string var, Instruction.Oper oper)
        {
            Instruction ins = new Instruction();
            ins.op = Instruction.Op.Store;
            ins.o0 = oper;
            ins.val = var;
            list.Add(ins);
        }

        public void EmitPush(Instruction.Oper oper)
        {
            Instruction ins = new Instruction();
            ins.op = Instruction.Op.Push;
            ins.o0 = oper;
            list.Add(ins);
        }

        public void EmitPop(Instruction.Oper oper)
        {
            Instruction ins = new Instruction();
            ins.op = Instruction.Op.Pop;
            ins.o0 = oper;
            list.Add(ins);
        }

        public void EmitAdd(Instruction.Oper oper0, Instruction.Oper oper1)
        {
            Instruction ins = new Instruction();
            ins.op = Instruction.Op.Add;
            ins.o0 = oper0;
            ins.o1 = oper1;
            list.Add(ins);
        }

        //public void EmitSub(Instruction.Oper oper0, Instruction.Oper oper1)
        //{
        //    Instruction ins = new Instruction();
        //    ins.op = Instruction.Op.Sub;
        //    ins.o0 = oper0;
        //    ins.o1 = oper1;
        //    list.Add(ins);
        //}

        public void EmitMul(Instruction.Oper oper0, Instruction.Oper oper1)
        {
            Instruction ins = new Instruction();
            ins.op = Instruction.Op.Mul;
            ins.o0 = oper0;
            ins.o1 = oper1;
            list.Add(ins);
        }
        public void EmitDiv(Instruction.Oper oper0, Instruction.Oper oper1)
        {
            Instruction ins = new Instruction();
            ins.op = Instruction.Op.Div;
            ins.o0 = oper0;
            ins.o1 = oper1;
            list.Add(ins);
        }

        public void EmitNeg(Instruction.Oper oper0)
        {
            Instruction ins = new Instruction();
            ins.op = Instruction.Op.NEG;
            ins.o0 = oper0;
            list.Add(ins);
        }

        public void EmitJmp(int lable)
        {
            Instruction ins = new Instruction();
            ins.op = Instruction.Op.JMP;
            ins.i4 = lable;
            list.Add(ins);
        }

        public void EmitJZ(int lable)
        {
            Instruction ins = new Instruction();
            ins.op = Instruction.Op.JZ;
            ins.i4 = lable;
            list.Add(ins);
        }

        public void EmitJNZ(int lable)
        {
            Instruction ins = new Instruction();
            ins.op = Instruction.Op.JNZ;
            ins.i4 = lable;
            list.Add(ins);
        }

        public void EmitJA(int lable)
        {
            Instruction ins = new Instruction();
            ins.op = Instruction.Op.JA;
            ins.i4 = lable;
            list.Add(ins);
        }

        public void EmitJAE(int lable)
        {
            Instruction ins = new Instruction();
            ins.op = Instruction.Op.JAE;
            ins.i4 = lable;
            list.Add(ins);
        }

        public void EmitJB(int lable)
        {
            Instruction ins = new Instruction();
            ins.op = Instruction.Op.JB;
            ins.i4 = lable;
            list.Add(ins);
        }
        public void EmitJBE(int lable)
        {
            Instruction ins = new Instruction();
            ins.op = Instruction.Op.JBE;
            ins.i4 = lable;
            list.Add(ins);
        }

        public void EmitLZ()
        {
            Instruction ins = new Instruction();
            ins.op = Instruction.Op.LZ;
            list.Add(ins);
        }

        public void EmitLNZ()
        {
            Instruction ins = new Instruction();
            ins.op = Instruction.Op.LNZ;
            list.Add(ins);
        }

        public void EmitLA()
        {
            Instruction ins = new Instruction();
            ins.op = Instruction.Op.LA;
            list.Add(ins);
        }

        public void EmitLAE()
        {
            Instruction ins = new Instruction();
            ins.op = Instruction.Op.LAE;
            list.Add(ins);
        }
        public void EmitLB()
        {
            Instruction ins = new Instruction();
            ins.op = Instruction.Op.LB;
            list.Add(ins);
        }
        public void EmitLBE()
        {
            Instruction ins = new Instruction();
            ins.op = Instruction.Op.LBE;
            list.Add(ins);
        }
        
        public void EmitOr(Instruction.Oper oper0, Instruction.Oper oper1)
        {
            Instruction ins = new Instruction();
            ins.op = Instruction.Op.OR;
            ins.o0 = oper0;
            ins.o1 = oper1;
            list.Add(ins);
        }

        public void EmitXor(Instruction.Oper oper, object value)
        {

        }

        public void EmitAnd(Instruction.Oper oper0, Instruction.Oper oper1)
        {
            Instruction ins = new Instruction();
            ins.op = Instruction.Op.AND;
            ins.o0 = oper0;
            ins.o1 = oper1;
            list.Add(ins);
        }

        public void EmitCmp(Instruction.Oper oper0, Instruction.Oper oper1)
        {
            Instruction ins = new Instruction();
            ins.op = Instruction.Op.CMP;
            ins.o0 = oper0;
            ins.o1 = oper1;
            list.Add(ins);
        }

        public void EmitTest(Instruction.Oper oper0, Instruction.Oper oper1)
        {
            Instruction ins = new Instruction();
            ins.op = Instruction.Op.TEST;
            ins.o0 = oper0;
            ins.o1 = oper1;
            list.Add(ins);
        }


        public void Print()
        {
            int i = 0;
            for (int j = 0; j < lables.Count; j++)
            {
                while ( i < lables[j] )
                {
                    Console.Write(i);
                    Console.Write('\t');
                    Console.WriteLine(list[i]);
                    i++;
                }
                Console.WriteLine(string.Format("lable {0}: {1}", j, lables[j]));
            }

            for (; i < list.Count; i++)
            {
                Console.Write(i);
                Console.Write('\t');
                Console.WriteLine(list[i]);
            }
        }
    
    }
}
