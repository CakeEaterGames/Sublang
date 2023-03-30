using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sublang
{

    public struct Token
    {
        public string Value;
        public TokenType Type;
        public int X;
        public int Y;


        public Token(string value, TokenType type, int x, int y)
        {
            Value = value;
            Type = type;
            X = x;
            Y = y;
        }

        public override string ToString()
        {
            return String.Format("{0}:{1} {2} {3}", Y, X, Type, Value);
        }
    }

    public enum TokenType
    {
        number,
        word,
        label,

        space,

        plus,
        minus,
        mul,
        div,
        mod,
        openr,
        closer,
        comment,
        functionDef,
        comma,
        openc,
        closec,
        pointerNext,
        pointer,
        functionCall,
        define,
        str,
        strDef,
        strzDef,
        strlDef,
        strlzDef,
        varDef,
        varsLocation,
        zeroesDef,
        constVarDef,
        binAnd,
        binOr,
        binXor,
        shiftLeft,
        shiftRight,
        binaryNumber,
        hexNumber,
    }

    //A group of tokens that all exist in one cell of the final program 
    //Like "word+number*(word+number)" ect
    //or simply "number"
    public class TokenGroup
    {
        public List<Token> Tokens = new List<Token>();
        public bool isSimple = true;
        public void Add(Token t)
        {
            switch (t.Type)
            {
                case TokenType.word:
                case TokenType.plus:
                case TokenType.minus:
                case TokenType.mul:
                case TokenType.div:
                case TokenType.mod:
                case TokenType.shiftLeft:
                case TokenType.shiftRight:
                case TokenType.binOr:
                case TokenType.binXor:
                case TokenType.binAnd:
                case TokenType.openr:
                case TokenType.closer:
                case TokenType.pointerNext:
                case TokenType.pointer:
                case TokenType.constVarDef:
                    isSimple = false;
                    break;
                case TokenType.number:
                    break;
                default:
                    throw new Exception("Is this suppose to be here " + t);
            }
            Tokens.Add(t);

            if (Tokens.Count == 2)
            {
                if (Tokens[0].Type == TokenType.minus && Tokens[1].Type == TokenType.number)
                {
                    int val = -int.Parse(Tokens[1].Value);
                    Token t2 = new Token(val + "", TokenType.number, Tokens[0].X, Tokens[0].Y);
                    Tokens.Clear();
                    Tokens.Add(t2);
                    isSimple = true;
                }
            }
        }

        public override string ToString()
        {
            return String.Join(", ", Tokens);
        }

        //It evaluates the expression inside this Token group and reduces it to "number"
        public bool Simplify()
        {
            //You have a list of number+-*/()

            while (true)
            {
                //find the deepest set of ()
                int start = 0;
                int end = Tokens.Count - 1;
                int depth = 0;
                int maxD = 0;
                for (int i = 0; i < Tokens.Count; i++)
                {
                    if (Tokens[i].Type == TokenType.openr)
                    {
                        depth++;
                        if (depth >= maxD)
                        {
                            start = i;
                            maxD = depth;
                        }
                    }
                    else if (Tokens[i].Type == TokenType.closer)
                    {
                        if (depth >= maxD)
                        {
                            end = i;
                            maxD = depth;
                        }
                        depth--;
                    }
                }
                if (Tokens[start].Type == TokenType.openr)
                {
                    Tokens.RemoveAt(end);
                    Tokens.RemoveAt(start);
                    end -= 2;
                }
                SimplifyGroup(start, end);
                if (maxD == 0)
                {
                    break;
                }

            }


            return false;
        }
        int SimplifyGroup(int start, int end)
        {
            //Console.WriteLine(start);
            //Console.WriteLine(end);
            //Console.WriteLine(String.Join("\n",Tokens));
            //first replace all a*b
            //etc

            for (int i = start; i <= end; i++)
            {
                if (Tokens[i].Type == TokenType.binAnd)
                {
                    int a = int.Parse(Tokens[i - 1].Value);
                    int b = int.Parse(Tokens[i + 1].Value);
                    int res = a & b;
                    Tokens.RemoveAt(i - 1);
                    Tokens.RemoveAt(i - 1);
                    Tokens.RemoveAt(i - 1);
                    Tokens.Insert(i - 1, new Token(res + "", TokenType.number, 0, 0));
                    end -= 2;
                    i--;
                }
            }
            for (int i = start; i <= end; i++)
            {
                if (Tokens[i].Type == TokenType.binOr)
                {
                    int a = int.Parse(Tokens[i - 1].Value);
                    int b = int.Parse(Tokens[i + 1].Value);
                    int res = a | b;
                    Tokens.RemoveAt(i - 1);
                    Tokens.RemoveAt(i - 1);
                    Tokens.RemoveAt(i - 1);
                    Tokens.Insert(i - 1, new Token(res + "", TokenType.number, 0, 0));
                    end -= 2;
                    i--;
                }
                else if (Tokens[i].Type == TokenType.binXor)
                {
                    int a = int.Parse(Tokens[i - 1].Value);
                    int b = int.Parse(Tokens[i + 1].Value);
                    int res = a ^ b;
                    Tokens.RemoveAt(i - 1);
                    Tokens.RemoveAt(i - 1);
                    Tokens.RemoveAt(i - 1);
                    Tokens.Insert(i - 1, new Token(res + "", TokenType.number, 0, 0));
                    end -= 2;
                    i--;
                }
                 
            }
            for (int i = start; i <= end; i++)
            {
                if (Tokens[i].Type == TokenType.shiftLeft)
                {
                    int a = int.Parse(Tokens[i - 1].Value);
                    int b = int.Parse(Tokens[i + 1].Value);
                    int res = a << b;
                    Tokens.RemoveAt(i - 1);
                    Tokens.RemoveAt(i - 1);
                    Tokens.RemoveAt(i - 1);
                    Tokens.Insert(i - 1, new Token(res + "", TokenType.number, 0, 0));
                    end -= 2;
                    i--;
                }
                else if (Tokens[i].Type == TokenType.shiftRight)
                {
                    int a = int.Parse(Tokens[i - 1].Value);
                    int b = int.Parse(Tokens[i + 1].Value);
                    int res = a >> b;
                    Tokens.RemoveAt(i - 1);
                    Tokens.RemoveAt(i - 1);
                    Tokens.RemoveAt(i - 1);
                    Tokens.Insert(i - 1, new Token(res + "", TokenType.number, 0, 0));
                    end -= 2;
                    i--;
                }
            }
            for (int i = start; i <= end; i++)
            {

                if (Tokens[i].Type == TokenType.mul)
                {
                    int a = int.Parse(Tokens[i - 1].Value);
                    int b = int.Parse(Tokens[i + 1].Value);
                    int res = a * b;
                    Tokens.RemoveAt(i - 1);
                    Tokens.RemoveAt(i - 1);
                    Tokens.RemoveAt(i - 1);
                    Tokens.Insert(i - 1, new Token(res + "", TokenType.number, 0, 0));
                    end -= 2;
                    i--;
                }
                else if (Tokens[i].Type == TokenType.div)
                {
                    int a = int.Parse(Tokens[i - 1].Value);
                    int b = int.Parse(Tokens[i + 1].Value);
                    int res = a / b;
                    Tokens.RemoveAt(i - 1);
                    Tokens.RemoveAt(i - 1);
                    Tokens.RemoveAt(i - 1);
                    Tokens.Insert(i - 1, new Token(res + "", TokenType.number, 0, 0));
                    end -= 2;
                    i--;
                    
                }
                else if (Tokens[i].Type == TokenType.mod)
                {
                    int a = int.Parse(Tokens[i - 1].Value);
                    int b = int.Parse(Tokens[i + 1].Value);
                    int res = a % b;
                    Tokens.RemoveAt(i - 1);
                    Tokens.RemoveAt(i - 1);
                    Tokens.RemoveAt(i - 1);
                    Tokens.Insert(i - 1, new Token(res + "", TokenType.number, 0, 0));
                    end -= 2;
                    i--;
                }
            }
            for (int i = start; i <= end; i++)
            {
                if (Tokens[i].Type == TokenType.minus)
                {
                    int a = int.Parse(Tokens[i - 1].Value);
                    int b = int.Parse(Tokens[i + 1].Value);
                    int res = a - b;
                    Tokens.RemoveAt(i - 1);
                    Tokens.RemoveAt(i - 1);
                    Tokens.RemoveAt(i - 1);
                    Tokens.Insert(i - 1, new Token(res + "", TokenType.number, 0, 0));
                    end -= 2;
                    i--;
                }
                else if (Tokens[i].Type == TokenType.plus)
                {
                    int a = int.Parse(Tokens[i - 1].Value);
                    int b = int.Parse(Tokens[i + 1].Value);
                    int res = a + b;
                    Tokens.RemoveAt(i - 1);
                    Tokens.RemoveAt(i - 1);
                    Tokens.RemoveAt(i - 1);
                    Tokens.Insert(i - 1, new Token(res + "", TokenType.number, 0, 0));
                    end -= 2;
                    i--;
                }
            }


            return 0;
        }

    }

    public struct sbFuncSignature
    {
        public string Name;
        public int paramCnt;

        public sbFuncSignature(string name, int paramCnt)
        {
            Name = name;
            this.paramCnt = paramCnt;
        }
        public override string ToString()
        {
            return String.Format("{0} : {1} parameters ", Name, paramCnt);
        }
    }

    public class sbFunction
    {
        public string Name = "";
        List<string> paramNames = new List<string>();
        public List<TokenGroup> code = new List<TokenGroup>();
        List<Token> raw;
        Dictionary<string, int> constants = new Dictionary<string, int>();
        Dictionary<string, int> labels = new Dictionary<string, int>();

        Compiler comp;
        bool Prepared = false;

        public void Init(string name, List<Token> tokens, List<string> pars, Compiler parent)
        {
            raw = tokens;
            paramNames = pars;
            comp = parent;
            Name = name;
        }

        public void Prepare()
        {
            if (Prepared) return;
            Round1();
            Round2();
            //Console.WriteLine("Prepared "+Name);
            Prepared = true;
        }

        //Go through raw tokens and collect them into TokenGroups
        //Replace all function calls with their evaluations
        //Collect all label names into constants
        void Round1()
        {
            TokenGroup gr = new TokenGroup();
            for (int i = 0; i < raw.Count; i++)
            {

                Token cur = raw[i];
                switch (cur.Type)
                {
                    case TokenType.label:
                        labels[cur.Value] = code.Count;
                        break;

                    case TokenType.comma:
                        if(gr.Tokens.Count>0)
                        code.Add(gr);
                        gr = new TokenGroup();
                        break;

                    case TokenType.functionCall:
                        //comp.Functions[];

                        string fname = cur.Value;
                        cur = raw[++i];
                        //Collect all tokens that are not , or ) into a tokenGroup
                        //If found ) stop

                        List<TokenGroup> fpars = new List<TokenGroup>();
                        int depth = 1;
                        while (cur.Type != TokenType.closer)
                        {
                            TokenGroup par = new TokenGroup();
                            while (cur.Type != TokenType.comma)
                            {
                                if (cur.Type == TokenType.closer)
                                {
                                    depth--;
                                    if (depth != 0)
                                    {
                                        par.Add(replaceConsts(cur));
                                        cur = raw[++i];
                                    }
                                }
                                else if (cur.Type == TokenType.openr)
                                {
                                    depth++;
                                    par.Add(replaceConsts(cur));
                                    cur = raw[++i];
                                }
                                else
                                {
                                    if(cur.Type == TokenType.constVarDef)
                                    {
                                        cur = raw[++i];
                                        string v = cur.Value;
                                        string c = "p" + v;
                                        if (v == "-")
                                        {
                                            cur = raw[++i];
                                            v = "-" + cur.Value;
                                            c = "n" + v;
                                        }
                                        comp.vars[c] = int.Parse(v);
                                        cur = new Token(c, TokenType.word, 0, 0);
                                    }
                                    par.Add(replaceConsts(cur));
                                    cur = raw[++i];
                                }


                                if (depth <= 0)
                                {
                                    break;
                                }

                            }
                            if (cur.Type == TokenType.comma)
                            {
                                cur = raw[++i];
                            }



                            if (par.Tokens.Count > 0)
                            {
                                fpars.Add(par);
                            }
                            else
                            {
                                break;
                            }

                            if (depth <= 0)
                            {
                                break;
                            }
                        }

                        sbFuncSignature sign = new sbFuncSignature(fname, fpars.Count);
                        if (!comp.Functions.ContainsKey(sign))
                        {
                            throw new Exception("No function with such signature " + sign);
                        }

                        //Console.WriteLine("call " + fname);
                        //Console.WriteLine("pars " + string.Join("/// ", fpars));
                        //Console.WriteLine();

                        var func = comp.Functions[sign];
                        func.Prepare();

                        foreach (var ft in func.code)
                        {
                            TokenGroup tg = new TokenGroup();
                            foreach (var t in ft.Tokens)
                            {
                                var t2 = t;

                                if (func.paramNames.Contains(t2.Value))
                                {
                                    var parn = func.paramNames.IndexOf(t2.Value);
                                    var replaceWith = fpars[parn];

                                    if (replaceWith.Tokens.Count > 1)
                                    {
                                        tg.Add(new Token("(", TokenType.openr, t2.X, t2.Y));
                                    }

                                    foreach (var item in replaceWith.Tokens)
                                    {
                                        tg.Add(item);
                                    }

                                    if (replaceWith.Tokens.Count > 1)
                                    {
                                        tg.Add(new Token(")", TokenType.closer, t2.X, t2.Y));
                                    }

                                }
                                else
                                {
                                    if (constants.ContainsKey(t2.Value))
                                    {
                                        t2.Value = "" + constants[t2.Value];
                                        t2.Type = TokenType.number;
                                    }
                                    tg.Add(t2);
                                }


                            }
                            code.Add(tg);

                        }


                        break;

                    case TokenType.define:
                        {
                            cur = raw[++i];
                            string c = cur.Value;
                            cur = raw[++i];
                            string v = cur.Value;
                            if (v == "-")
                            {
                                cur = raw[++i];
                                v = "-" + cur.Value;
                            }
                            constants[c] = int.Parse(v);
                        }
                        break;

                

                    case TokenType.varDef:
                        {
                            cur = raw[++i];
                            string c = cur.Value;
                            cur = raw[++i];
                            string v = cur.Value;
                            if (v == "-")
                            {
                                cur = raw[++i];
                                v = "-" + cur.Value;
                            }
                            comp.vars[c] = int.Parse(v);
                        }
                        break;

                    case TokenType.constVarDef:
                        {
                            cur = raw[++i];
                            string v = cur.Value;
                            string c = "p" + v;
                            if (v == "-")
                            {
                                cur = raw[++i];
                                v = "-" + cur.Value;
                                c = "n" + v;
                            }
                            comp.vars[c] = int.Parse(v);

                            TokenGroup tg = new TokenGroup();
                            tg.Add(new Token(c, TokenType.word, 0, 0));
                            code.Add(tg);
                        }
                        break;

                    case TokenType.varsLocation:
                        {
 
                            foreach (var v in comp.vars)
                            {
                                labels[v.Key] = code.Count;
                                TokenGroup tg = new TokenGroup();
                                tg.Add(new Token(v.Value+"", TokenType.number, 0, 0));
                                code.Add(tg);
                            }
                        }
                        break;
                    case TokenType.zeroesDef:
                        {
                            cur = raw[++i];
                            int n = int.Parse(cur.Value);
                            for (int nn = 0; nn < n; nn++)
                            {
                                TokenGroup tg = new TokenGroup();
                                tg.Add(new Token("0", TokenType.number, 0, 0));
                                code.Add(tg);
                            }
                        }
                        break;
                    case TokenType.strlzDef:
                        strDef(true, true);
                        break;
                    case TokenType.strzDef:
                        strDef(false, true);
                        break;
                    case TokenType.strlDef:
                        strDef(true, false);
                        break;
                    case TokenType.strDef:
                        strDef(false, false);
                        break;
                    default:
                        gr.Add(replaceConsts(cur));
                        break;
                }

                void strDef(bool l, bool z)
                {
                    cur = raw[++i];
                    string str = cur.Value;
                    if (l)
                    {
                        TokenGroup tg = new TokenGroup();
                        tg.Add(new Token(str.Length+"", TokenType.number, 0, 0));
                        code.Add(tg);
                    }
                    foreach (var ch in str)
                    {
                        TokenGroup tg = new TokenGroup();
                        tg.Add(new Token(((int)(ch) + ""), TokenType.number, 0, 0));
                        code.Add(tg);
                    }
                    if (z)
                    {
                        TokenGroup tg = new TokenGroup();
                        tg.Add(new Token("0", TokenType.number, 0, 0));
                        code.Add(tg);
                    }
                   
                }

            }

            if (gr.Tokens.Count > 0)
            {
                code.Add(gr);

            }

        }

        //Go through each TokenGroup and replace all references to local labels with relative constants "?+n"
        void Round2()
        {
            for (int i = 0; i < code.Count; i++)
            {

                for (int j = 0; j < code[i].Tokens.Count; j++)
                {
                    var cur = code[i].Tokens[j].Value;
                    if (labels.ContainsKey(cur))
                    {
                        code[i].Tokens.RemoveAt(j);

                        code[i].Tokens.Insert(j, new Token((labels[cur] - i) + "", TokenType.number, 0, 0));
                        code[i].Tokens.Insert(j, new Token("+", TokenType.plus, 0, 0));
                        code[i].Tokens.Insert(j, new Token("?", TokenType.pointer, 0, 0));
                    }

                }

            }
        }

        Token replaceConsts(Token t)
        {
            if (t.Type == TokenType.word)
            {
                if (constants.ContainsKey(t.Value))
                {
                    t.Value = "" + constants[t.Value];
                    t.Type = TokenType.number;
                }
            }

            return t;
        }

        /*
        //This is called when another function wants to insert this function into itself
        Dictionary<string, TokenGroup> evalParams;
        int evalPos = 0;
        bool evalDone = true;
        public void StartEval(List<TokenGroup> pars)
        {
            evalParams = new Dictionary<string, TokenGroup>();
            evalDone = false;
        }
        

        //Check if the current token groups has function params inside it
        //If it does create and return new TokenGroup where those params are replaced with their values
        public TokenGroup EvalNext()
        {
            return null;
        }
        */


        public void ReplacePointers()
        {
            int pos = 0;
            foreach (var g in code)
            {
                if (!g.isSimple)
                {
                    for (int i = 0; i < g.Tokens.Count; i++)
                    {
                        if (g.Tokens[i].Type == TokenType.pointer)
                        {
                            Token t = new Token(pos + "", TokenType.number, 0, 0);
                            g.Tokens[i] = t;
                        }
                        else if (g.Tokens[i].Type == TokenType.pointerNext)
                        {
                            Token t = new Token((pos + 1) + "", TokenType.number, 0, 0);
                            g.Tokens[i] = t;
                        }
                    }
                }

                pos++;
            }
        }

        public void Simplify()
        {
            foreach (var tg in code)
            {
                tg.Simplify();
            }
        }

        public void CheckForUndefined()
        {
            foreach (var tg in code)
            {
                foreach (var t in tg.Tokens)
                {
                    switch (t.Type)
                    {
                        case TokenType.number:
                        case TokenType.plus:
                        case TokenType.minus:
                        case TokenType.mul:
                        case TokenType.div:
                        case TokenType.mod:

                        case TokenType.shiftLeft:
                        case TokenType.shiftRight:
                        case TokenType.binOr:
                        case TokenType.binXor:
                        case TokenType.binAnd:

                        case TokenType.openr:
                        case TokenType.closer:
                        //case TokenType.constVarDef:
                            break;
                        default:
                            Console.WriteLine(String.Join(", ",tg.Tokens));
                            throw new Exception("Unexpected token "+t);
                             
                    }
                }
                 
            }
        }
    }

    public class Compiler
    {
        public Dictionary<sbFuncSignature, sbFunction> Functions = new Dictionary<sbFuncSignature, sbFunction>();
        public Dictionary<string, int> vars = new Dictionary<string, int>();

        public void Init(List<Token> tokens)
        {
            //Step 1
            //Collect all functions with raw code

            for (int i = 0; i < tokens.Count; i++)
            {
                var cur = tokens[i];

                switch (cur.Type)
                {
                    case TokenType.functionDef:
                        {
                            cur = tokens[++i];
                            string fname = cur.Value;

                            sbFunction func = new sbFunction();

                            List<string> pars = new List<string>();

                            cur = tokens[++i];
                            while (cur.Type != TokenType.closer)
                            {
                                pars.Add(cur.Value);
                                cur = tokens[++i];
                                if (cur.Type == TokenType.comma)
                                {
                                    cur = tokens[++i];
                                }
                            }

                            i++;
                            cur = tokens[++i];

                            List<Token> ftokens = new List<Token>();
                            while (cur.Type != TokenType.closec)
                            {
                                ftokens.Add(cur);
                                cur = tokens[++i];
                            }


                            //Console.WriteLine(fname);
                            //Console.WriteLine(pars.Count);
                            //Console.WriteLine(String.Join(", ", pars));
                            //Console.WriteLine(String.Join(", ", ftokens));


                            sbFuncSignature sign = new sbFuncSignature(fname, pars.Count);
                            func.Init(fname,ftokens, pars, this);
                            Functions[sign] = func;
                        }
                        break;

                    default:
                        throw new Exception("Unusual token " + cur);
                }

            }

            var main = Functions[new sbFuncSignature("main", 0)];

            //Step 2
            //Prepare all functions
            /*
                        foreach (var f in Functions)
                        {
                            f.Value.Prepare();
                        }
                        */
            main.Prepare();


            //Step 3
            //Find the Main() function and replace all ? and ?+1 with numbers which are pointers to the current token location
            //Simplify each TokenGroup
          
            main.ReplacePointers();
            main.CheckForUndefined();
            main.Simplify();

            foreach (var f in Functions)
            {
                /*
                Console.WriteLine(f.Key.Name);
                Console.WriteLine(string.Join("\n", f.Value.code));
                Console.WriteLine();
                */
            }

            //Step 4
            //Collect the final string result   
            result.Append("#");
            foreach (var n in main.code)
            {
                result.Append(n.Tokens[0].Value);
                result.Append(";\n");
            }
            //Console.WriteLine(result);
        }

        public StringBuilder result = new StringBuilder();
    }
}
