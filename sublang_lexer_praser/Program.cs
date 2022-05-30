﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace Sublang
{
    class Program
    {
        static bool toRun = false;
        static bool toCompile = false;
        static bool toPrint = false;
        static bool toTrace = false;
        static string inputFilePath = "";
        static string outputFilePath = "";

        static void Main(string[] args)
        {
           

            if (args.Length == 0)
            {
                var assembly = Assembly.GetExecutingAssembly();
                var resourceName = "Sublang.help.txt";
                Stream stream = assembly.GetManifestResourceStream(resourceName);
                StreamReader reader = new StreamReader(stream);
                string result = reader.ReadToEnd();
                reader.Close();
                Console.WriteLine(result);


#if DEBUG
                toRun = true;
                toCompile = true;
                toPrint = false;
                toTrace = false;
                inputFilePath = "input.txt";
                outputFilePath = "output.txt";


#else
                return;
#endif

            }
            else
            {

                for (int i = 0; i < args.Length; i++)
                {
                    switch (args[i])
                    {
                        case "-c":
                            toCompile = true;
                            break;
                        case "-p":
                            toPrint = true;
                            break;
                        case "-r":
                            toRun = true;
                            break;
                        case "-t":
                            toTrace = true;
                            break;
                        case "-i":
                            inputFilePath = args[i + 1];
                            i++;
                            break;
                        case "-o":
                            outputFilePath = args[i + 1];
                            i++;
                            break;
                        case "-h":
                            var page = int.Parse(args[i + 1]);
                            i++;
                            if (page>=1 && page<=7)
                            { 
                                var assembly = Assembly.GetExecutingAssembly();
                                var resourceName = "Sublang.Tutorial."+page+".txt";
                                Stream stream = assembly.GetManifestResourceStream(resourceName);
                                StreamReader reader = new StreamReader(stream);
                                string result = reader.ReadToEnd();
                                reader.Close();
                                Console.WriteLine(result);
                                return;
                            }
                            else
                            {
                                Console.WriteLine("There is no tutorial with page "+page);
                                return;
                            }
                            break;
                        default:
                            Console.WriteLine(String.Join(", ", args));
                            throw new Exception("An error occured while reading the parameters");
                            break;
                    }
                }


            }

            /*
            -i InputFilePath Read program from this file
            -o OutputFilePath Write the result to this file
            -p Print the compiled code to the console
            -c Compile the input file first
            -r Run the program
            */


            Program p = new Program();
            //Console.ReadLine(  );

            
        }

        public Program()
        {
            string compiled = "";
            if (toCompile)
            {
                var allCode = Includer.Init(inputFilePath);

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

                new TokenDefinition(@"#", TokenType.constVarDef),

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
                compiled = comp.result.ToString();

                if (outputFilePath != "")
                {
                    StreamWriter cout = new StreamWriter(outputFilePath);
                    cout.Write(compiled);
                    cout.Flush();
                    cout.Close();
                }

              
            }
            else
            {
                StreamReader sr = new StreamReader(inputFilePath);
                compiled = sr.ReadToEnd();
                sr.Close();
            }

            if (toPrint)
            {
                Console.WriteLine(compiled);
            }
            /*
            while (true)
            {
                var a =Keyboard.GetDownKeys();
                Console.WriteLine(String.Join(", ",a));
                Thread.Sleep(1);
            }
            */
            if (toRun)
            {
                VM vm = new VM();
                vm.toTrace = toTrace;

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
                    
                    //Console.CursorLeft = 0;
                    //Console.CursorTop = 0;
                    
                    Console.Write(outp);

                    if (vm.done) break;

                    
                    var inp = Console.ReadLine();
                    
                    foreach (var c in inp)
                    {
                        vm.inputs.Enqueue(c);
                    }
                    
                    //vm.inputs.Enqueue(Console.ReadKey().KeyChar);
                    vm.inputs.Enqueue('\n');
                    //vm.inputs.Enqueue(0);
                    //Thread.Sleep(27);

                }
            }

          
        }
    }


    class Includer
    {
        public static string Init(string filename)
        {
 

            StreamReader sr = new StreamReader(filename);
            string text = sr.ReadToEnd();
            sr.Close();

            int incPos = text.IndexOf("!include");
            while (incPos>=0)
            {
                int start = text.IndexOf("\"", incPos);
                int end = text.IndexOf("\"", start+1);
                string fname = text.Substring(start+1, end - start-1);
                //Console.WriteLine(fname);
                StreamReader sr2 = new StreamReader(fname);
                var inc = sr2.ReadToEnd();
                sr2.Close();
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

