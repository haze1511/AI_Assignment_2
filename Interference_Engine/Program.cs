using System;

namespace Interference_Engine
{
    class Program
    {
        static void Main(string[] args)
        {
            Parser parser = new Parser(args[1]); // providing file to parser
            
            // for debugging
            Console.WriteLine("ASK: " + parser.Query);
            Console.WriteLine("TELL: " + parser.Inform);

            KnowledgeBase KB; 
           
            switch (args[0].ToLower()) // based on method 
            {
                case "bc": //backward chaining
                    KB = new BackwardChaining(parser.Query, parser.Inform);
                    break;

                default: //forward chaining
                    KB = new ForwardChaining(parser.Query, parser.Inform);
                    break;

                case "tt": // truth table 
                    KB = new TruthTable(parser.Query, parser.Inform);
                   break;     
            }

            Console.WriteLine(KB.Perform());

            Console.ReadLine();
        }
    }
}
