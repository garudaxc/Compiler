using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Compiler
{
    class SymbolTable
    {        
        List<List<KeyValuePair<string, SymbolInfo>>> symbols = new List<List<KeyValuePair<string, SymbolInfo>>>();
        int numFormalParam = 0;

        public SymbolTable()
        {
            symbols.Add(new List<KeyValuePair<string, SymbolInfo>>());
        }

        public bool Contains(string symbol)
        {
            if (symbols[symbols.Count - 1].FindIndex((s) => s.Key == symbol) != -1)
            {
                return true;
            }
            if (symbols[0].FindIndex((s) => s.Key == symbol) != -1)
            {
                return true;
            }
            return false;
        }
        
        public void Add(string name, SymbolInfo obj)
        {
            if (Contains(name))
            {
                string msg = string.Format("symbol table aleady have symbol {0}", name);
                throw new Exception(msg);
            }
            symbols[symbols.Count - 1].Add(new KeyValuePair<string, SymbolInfo>(name, obj));
        }

        public void AddGlobal(string name, SymbolInfo obj)
        {
            symbols[0].Add(new KeyValuePair<string, SymbolInfo>(name, obj));
        }

        public void PushBarrier()
        {
            symbols.Add(new List<KeyValuePair<string, SymbolInfo>>());
        }

        public void PopBarrier()
        {
            symbols.RemoveAt(symbols.Count - 1);
        }

        public int NumSymbol
        {
            get
            {
                return symbols[symbols.Count - 1].Count;
            }
        }

        public int NumLocalParam
        {
            get
            {
                return NumSymbol - numFormalParam;
            }
        }

        public void SetNumFormalParam(int num)
        {
            numFormalParam = num;
        }

        public int GetSymbolOffset(string symbol)
        {
            int index = symbols[symbols.Count - 1].FindIndex((s) => s.Key == symbol);
            if (index == -1)
            {
                string msg = string.Format("can not find symbol {0}", symbol);
                throw new Exception(msg);
            }

            int offset = index - numFormalParam - 1;
            if (index >= numFormalParam)
            {
                offset += 2;
            }

            return offset;
        }


        public SymbolInfo GetSymbol(string name)
        {
            int index = symbols[symbols.Count - 1].FindIndex((s) => s.Key == name);
            if (index != -1)
            {
                return symbols[symbols.Count - 1][index].Value;
            }

            index = symbols[0].FindIndex((s) => s.Key == name);
            if (index != -1)
            {
                return symbols[0][index].Value;
            }
            return null;
        }

    }
}
