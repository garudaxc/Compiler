﻿using System;
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
                //if (index < 0 || index > Count)
                //{
                    
                //}

                return list[index];
            }
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
            ins.o0 = oper1;
            list.Add(ins);
        }

        public void EmitSub(Instruction.Oper oper0, Instruction.Oper oper1)
        {
            Instruction ins = new Instruction();
            ins.op = Instruction.Op.Sub;
            ins.o0 = oper0;
            ins.o0 = oper1;
            list.Add(ins);
        }

        public void EmitMul(Instruction.Oper oper0, Instruction.Oper oper1)
        {
            Instruction ins = new Instruction();
            ins.op = Instruction.Op.Mul;
            ins.o0 = oper0;
            ins.o0 = oper1;
            list.Add(ins);
        }
        public void EmitDiv(Instruction.Oper oper0, Instruction.Oper oper1)
        {
            Instruction ins = new Instruction();
            ins.op = Instruction.Op.Div;
            ins.o0 = oper0;
            ins.o0 = oper1;
            list.Add(ins);
        }

        public void Print()
        {
            foreach(var ins in list)
            {
                Console.WriteLine(ins.ToString());
            }
        }
    
    }
}
