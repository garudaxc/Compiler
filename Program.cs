using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Compiler
{
    class Test
    {
        void Test1()
        {
            
            //lex.Accept(s);

            //string[] ss = { "129", "23sdds", "sjof23", "34.", "343.45fvd" };
            //Array.ForEach(ss, (str) =>
            //{
            //    int i = 0;
            //    bool b = Lexer.IsNumber(str, 0, out i);
            //    if (b)
            //    {
            //        Console.WriteLine(string.Format("{0}\ttrue\t{1}", str, str.Substring(0, i)));
            //    }
            //    else
            //    {
            //        Console.WriteLine(str + " false");
            //    }
            //});
        }

        public void LexerTest()
        {
            Lexer lex = new Lexer();
            //string s = "12+(2.3-43.)*9/8;";
            //string s = "1+2;";


            string[] str = File.ReadAllLines("testcase.txt");


            lex.Accept(str);
            Token tok;
            while ((tok = lex.GetNextToken()) != Token.End)
            {
                string name = Enum.GetName(typeof(TokenType), tok.Type);
                Console.WriteLine("{0}\t{1}", name, tok.i4);
            }

        }


        public void FullTest()
        {
            string[] str = File.ReadAllLines("testcase.txt");

            Lexer lex = new Lexer();
            LLPaser paser = new LLPaser();
            VM vm = new VM();

            lex.Accept(str);
            InstructionSet set = paser.Parse(lex);
            set.Print();
            vm.Run(set);
        }

        public void ParserTest()
        {
            string[] str = File.ReadAllLines("testcase.txt");

            Lexer lex = new Lexer();
            LLPaser paser = new LLPaser();
            VM vm = new VM();

            lex.Accept(str);
            InstructionSet set = paser.Parse(lex);
            set.Print();
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            Test test = new Test();
            //test.LexerTest();
            test.FullTest();
            //test.ParserTest();
        }
    }
}
