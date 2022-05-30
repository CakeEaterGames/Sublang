using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sublang
{
    class VM
    {
        public string inputProgram;

        public int pc = 0;
        public int[] cell;

        public Queue<int> inputs = new Queue<int>();
        public Queue<int> outputs = new Queue<int>();

        public bool pause = false;
        public bool done = false;

        public bool immedeateOutput = true;
        public bool toTrace = false;

        public void SetProgram(string inp)
        {
            inp = inp.Replace("\r", " ").Replace("\n", " ");
            while (inp.Contains("  "))
            {
                inp = inp.Replace("  ", " ");
            }
            inp = inp.Trim();
            inputProgram = inp;
        }

        public void Init()
        {
            //Log("Reset computer");

            cell = inputProgram.Split(' ').Select(Int32.Parse).ToArray();

            pc = 0;
            done = false;
            pause = false;

            outputs = new Queue<int>();
            inputs = new Queue<int>();
        }
        public void Run()
        {
            pause = false;
            while (!pause && !done)
            {
                if (pc < 0 || cell.Length < pc + 3)
                {
                    //Console.WriteLine("Program ended");
                    done = true;
                    break;
                }

                int B = cell[pc + 0];
                int A = cell[pc + 1];
                int C = cell[pc + 2];

                if (toTrace)
                {
                    StringBuilder tr = new StringBuilder();
                    for (int i = 0; i < cell.Length; i++)
                    {
                        if (i == pc)
                        {
                            tr.Append("[");
                        }
                        if (i == A || i==B)
                        {
                            tr.Append("(");
                        }
                        tr.Append(cell[i] + " ");
                        if (i == A || i == B)
                        {
                            tr.Append(")");
                        }
                        if (i == pc+2)
                        {
                            tr.Append("]");
                        }

                    }
                    Console.WriteLine();
                    if (A ==-1)
                    {
                        Console.WriteLine("Write mem[{0}] {1} ({2})", B, cell[B], (char)cell[B]);
                    }
                    else if (B == -1)
                    {
                        
                    }
                    else
                    {
                        Console.WriteLine("mem[{0}] = mem[{0}] - mem[{1}]", A, B);
                        Console.WriteLine("mem[{0}] = {1} - {2} = {3}", A, cell[A], cell[B], cell[A] - cell[B]);
                    }
                    
                    if (A==B)
                    {
                        Console.WriteLine("Clear cell "+A);
                    }
                    if (C!=pc+3)
                    {
                        if (A==B)
                        {

                            Console.WriteLine("jump to " + C);
                        }
                        else
                        {
                            Console.WriteLine("if <=0 jump to " + C);
                        }
                    }
                    Console.WriteLine(tr);
                    Console.ReadLine();
                }

                //Console.WriteLine("{0} {1} {2}", A, B, C);
                
                //Console.WriteLine(pc);
                //Console.WriteLine("op: {0} {1} {2}",A,B,C);
                //Console.WriteLine("Cells: " + String.Join(", ", cell) + " ");
                if (A == -1 && B == -1)
                {
                    if (inputs.Count == 0)
                    {
                        pause = true;
                        break;
                    }
                    if (immedeateOutput)
                    {
                        Console.Write((char)inputs.Dequeue());
                    }
                    else
                    {
                        outputs.Enqueue(inputs.Dequeue());
                    }
                    pc += 3;
                }
                else if (A == -1)
                {
                    if (immedeateOutput)
                    {
                        Console.Write((char)cell[B]);
                    }
                    else
                    {
                        outputs.Enqueue(cell[B]);
                    }
                   
                    pc += 3;
                }
                else if (B == -1)
                {
                    if (inputs.Count == 0)
                    {
                        pause = true;
                        break;
                    }
                    cell[A] -= inputs.Dequeue();
                }
                else
                {
                    cell[A] -= cell[B];
                }

                if (A != -1)
                {
                    if (cell[A] <= 0)
                    {
                        pc = C;
                    }
                    else
                    {
                        pc += 3;
                    }
                }

            }
        }
    }


}
