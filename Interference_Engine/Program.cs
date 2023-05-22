using System;
using System.Collections.Generic;
using System.IO;

namespace Interference_Engine
{
    class Program
    {
        static void Main(string[] args)
        {
            // Assignment of variables using ternary operators
            // If less than 2 arguments are provided, prompt the user to select an inference algorithm
            // Otherwise, assign the first argument to 'method' and convert it to lowercase
            string method = args.Length < 2 ? GetInferenceMethod() : args[0].ToLower();
            // If less than 2 arguments are provided, set 'argumentToSelect' to 0
            // Otherwise, set 'argumentToSelect' to 1
            int argumentToSelect = args.Length < 2 ? 0 : 1;

            // Parse the input file
            Parser parser = new Parser(args[argumentToSelect]);

            // Print the query and knowledge base
            Console.WriteLine($"TELL: {parser.Inform}");
            Console.WriteLine($"ASK: {parser.Query}\n");

            // Create a knowledge base based on the selected inference method.
            KnowledgeBase kb = CreateKnowledgeBase(method, parser.Query, parser.Inform);

            // Evaluate the knowledge base and print the result.
            string result = kb.Perform();
            Console.WriteLine("RESULT:");
            Console.WriteLine(result);

            // Wait for the user to press a key before exiting.
            Console.WriteLine("\nPress any key to exit.");
            Console.ReadKey();
        }

        // Create a knowledge base based on the specified inference method.
        private static KnowledgeBase CreateKnowledgeBase(string method, string query, string inform)
        {
            switch (method)
            {
                case "bc":
                    return new BackwardChaining(query, inform);
                case "tt":
                    return new TruthTable(query, inform);
                case "fc":
                default:
                    return new ForwardChaining(query, inform);
            }
        }
        
        private static string GetInferenceMethod()
        {
            // Define the available inference algorithms.
            Dictionary<string, string> methods = new Dictionary<string, string>
            {
                { "bc", "Backward Chaining" },
                { "fc", "Forward Chaining" },
                { "tt", "Truth Table" }
            };

            // Display the list of available inference algorithms.
            Console.WriteLine("AVAILABLE INFERENCE ALGORITHMS:");
            foreach (KeyValuePair<string, string> method in methods)
            {
                Console.WriteLine($"- {method.Key.ToUpper()}: {method.Value}");
            }

            // Prompt the user to select an inference algorithm.
            Console.Write("\nEnter the code of the inference algorithm you want to use: ");
            string input = Console.ReadLine().ToLower();

            // Validate the user input and return the selected inference algorithm.
            while (!methods.ContainsKey(input))
            {
                Console.Write("Invalid input. Please enter a valid algorithm code: ");
                input = Console.ReadLine().ToLower();
            }
            return input;
        }
    }
}