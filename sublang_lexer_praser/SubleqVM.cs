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
        public List<int> cell;

        public Queue<int> inputs = new Queue<int>();
        public Queue<int> outputs = new Queue<int>();

        public bool pause = false;
        public bool done = false;

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

            cell = inputProgram.Split(' ').Select(Int32.Parse).ToList();

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
                if (pc < 0 || cell.Count < pc + 3)
                {
                    //Console.WriteLine("Program ended");
                    done = true;
                    break;
                }

                int B = cell[pc + 0];
                int A = cell[pc + 1];
                int C = cell[pc + 2];

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
                    //Console.Write((char)inputs.Dequeue());
                    outputs.Enqueue(inputs.Dequeue());
                    pc += 3;
                }
                else if (A == -1)
                {
                    Console.Write((char)cell[B]);
                    //Console.WriteLine(cell[B]);
                    //outputs.Enqueue(cell[B]);
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
