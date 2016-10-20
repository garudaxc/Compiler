using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Compiler
{
    class Program
    {
        static void Main(string[] args)
        {
            Lexer lex = new Lexer();

            //string s = Console.ReadLine();

            string s = "12+(2.3-43.)*9/8;";
            //string s = "(1)3;";
            lex.Accept(s);
            //Token tok;
            //while((tok = lex.GetNextToken()) != null)
            //{
            //    string name = Enum.GetName(typeof(TokenType), tok.Type);
            //    Console.WriteLine("{0}\t{1}", name, tok.value);
            //}

            LLPaser paser = new LLPaser();
            paser.Parse(lex);

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
    }
}
