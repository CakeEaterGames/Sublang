using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace Sublang
{
    class Program
    {
        static void Main(string[] args)
        {
            Program p = new Program();
            Console.ReadLine();
        }

        public Program()
        {
            var allCode = Includer.Init("input.txt");
 
            var defs = new TokenDefinition[]
            {
                new TokenDefinition(@"([""'])(?:\\\1|.)*?\1", TokenType.str),
                new TokenDefinition(@"//.+?$", TokenType.comment),
                //new TokenDefinition(@"//[^$]*[$]", TokenType.comment),
 
                //new TokenDefinition(@"function\s+([A-Za-z_@]+[A-Za-z0-9_@]*)\s*\(", TokenType.functionDef),
                new TokenDefinition(@"function\s+", TokenType.functionDef),
                new TokenDefinition(@"!define\s+", TokenType.define),

                new TokenDefinition(@"!vars_location\s*", TokenType.varsLocation),
                new TokenDefinition(@"!var\s+", TokenType.varDef),

                new TokenDefinition(@"!stringlz\s+", TokenType.strlzDef),
                new TokenDefinition(@"!stringz\s+", TokenType.strzDef),
                new TokenDefinition(@"!stringl\s+", TokenType.strlDef),
                new TokenDefinition(@"!string\s+", TokenType.strDef),

                new TokenDefinition(@"!zeroes\s+", TokenType.zeroesDef),

                new TokenDefinition(@",", TokenType.comma),

                new TokenDefinition(@"\d+", TokenType.number),
                new TokenDefinition(@"\+", TokenType.plus),
                new TokenDefinition(@"\-", TokenType.minus),
                new TokenDefinition(@"\*", TokenType.mul),
                new TokenDefinition(@"\/", TokenType.div),

                new TokenDefinition(@"\?\+1", TokenType.pointerNext),
                new TokenDefinition(@"\?", TokenType.pointer),

                new TokenDefinition(@"[A-Za-z_@]+[A-Za-z0-9_@]*\s*\(", TokenType.functionCall),
                new TokenDefinition(@"[A-Za-z_@]+[A-Za-z0-9_@]*:", TokenType.label),
                new TokenDefinition(@"[A-Za-z_@]+[A-Za-z0-9_@]*", TokenType.word),

                new TokenDefinition(@"{", TokenType.openc),
                new TokenDefinition(@"}", TokenType.closec),

                new TokenDefinition(@"\(", TokenType.openr),
                new TokenDefinition(@"\)", TokenType.closer),

                new TokenDefinition(@"[\s\t]+", TokenType.space)
            };
       
            TextReader r = new StringReader(allCode);
            Lexer l = new Lexer(r, defs);

            Compiler comp = new Compiler();
            comp.Init(l.GetAllTokens());
            var compiled = comp.result.ToString();

            StreamWriter cout = new StreamWriter("compiled.txt");
            cout.Write(compiled);
            cout.Flush();
            cout.Close();
           

            //StreamReader sr = new StreamReader("badApplesubleq.txt");
            //var compiled = sr.ReadToEnd();

            VM vm = new VM();

            vm.SetProgram(compiled);
            vm.Init();

            while (!vm.done)
            {
                vm.Run();
                StringBuilder outp = new StringBuilder();
                while (vm.outputs.Count > 0)
                {
                    outp.Append((char)vm.outputs.Dequeue());
                }
                /*
                Console.CursorLeft = 0;
                Console.CursorTop = 0;
                */
                Console.Write(outp);
                
                var inp = Console.ReadLine();
                foreach (var c in inp)
                {
                    vm.inputs.Enqueue(c);
                }
                vm.inputs.Enqueue('\n');
                //vm.inputs.Enqueue(0);
                //Thread.Sleep(27);

            }
        }
    }


    class Includer
    {
        public static string Init(string filename)
        {
 

            StreamReader sr = new StreamReader(filename);
            string text = sr.ReadToEnd();

            int incPos = text.IndexOf("!include");
            while (incPos>=0)
            {
                int start = text.IndexOf("\"", incPos);
                int end = text.IndexOf("\"", start+1);
                string fname = text.Substring(start+1, end - start-1);
                //Console.WriteLine(fname);
                StreamReader sr2 = new StreamReader(fname);
                var inc = sr2.ReadToEnd();
                text = text.Substring(0, incPos) + inc + text.Substring(end+1);
                //Console.WriteLine(text);

                incPos = text.IndexOf("!include");
            }


            return text;
        }
    }

}


/*
TODO:
A compiler should also return an object with all label locations and values to which they are pointing 
*/

/*


    Test:
    Input
    Please write a number followed by any character
    double the number






*/
