using System;
using System.Collections.Generic;
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

        void Test2()
        {
            Lexer lex = new Lexer();
            string s = "12+(2.3-43.)*9/8;";
            //string s = "1+2;";
            lex.Accept(s);
            //Token tok;
            //while((tok = lex.GetNextToken()) != null)
            //{
            //    string name = Enum.GetName(typeof(TokenType), tok.Type);
            //    Console.WriteLine("{0}\t{1}", name, tok.value);
            //}
        }

        public void Run()
        {
            Test1();
            Test2();
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            Lexer lex = new Lexer();
            LLPaser paser = new LLPaser();

            while(true)
            {
                string s = Console.ReadLine();
                if (s.Length == 0)
                {
                    break;
                }

                lex.Accept(s);
                paser.Parse(lex);
            }
        }
    }
}
