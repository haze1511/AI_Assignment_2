using System;
using System.Collections.Generic;
using System.Linq;

namespace Interference_Engine
{
    class ForwardChaining : KnowledgeBase
    {
        // Private fields
        private List<string> _symbol; // Letters or symbols such as A, B, etc.
        private List<string> _statements; // Rules given
        private Dictionary<string, int> _statementcount; //Dictionary of statement counts
        private Queue<string> _schedule; // A queue of statements that need to be evaluated

        // Constructor that sets up the private fields and calls the base constructor from the KnowledgeBase class
        public ForwardChaining(string query, string inform) : base(query, inform)
        {
            Type = KnowledgeBaseType.FC; // Set the type of the knowledge base to forward chaining
            _symbol = new List<string>(); // Initialize the list of symbols
            _statements = new List<string>(); // Initialize the list of rules
            _statementcount = new Dictionary<string, int>(); // Initialize the dictionary of statement counts
            _schedule = new Queue<string>(); // Initialize the queue of statements to evaluate
        }

        // A method that sets up the knowledge base based on an input string of rules and facts
        public void Setup(string inform)
        {
            // Split the input string into individual statements using ";" as the separator
            string[] statements = inform.Split(";", StringSplitOptions.RemoveEmptyEntries);

            // Loop over each statement
            foreach (string statement in statements)
            {
                // If the statement contains the "=>" symbol, it is a rule
                if (statement.Contains("=>"))
                {
                    // Add the rule to the list of rules
                    _statements.Add(statement.Trim());

                    // Split the rule into left side and right side
                    string left = statement.Split("=>", StringSplitOptions.RemoveEmptyEntries)[0];

                    // Count the number of symbols
                    int leftcnt = left.Split("&", StringSplitOptions.RemoveEmptyEntries).Count();

                    // Add the rule and the count to the dictionary
                    _statementcount.Add(statement.Trim(), leftcnt);
                }
                // If the statement does not contain the "=>" symbol, it is a fact
                else
                {
                    // Add the fact to the schedule for evaluation
                    _schedule.Enqueue(statement.Trim());
                }
            }
        }

        public override string Perform()
        {
            string result = "";

            // Check if the query is already in the knowledge base
            if (Contains(Query))
            {
                result = "YES:";
                // If it is, add all the items in the knowledge base to the result string
                for (int j = 0; j < _symbol.Count; j++)
                {
                    result = result + " " + _symbol[j];
                    if (j < _symbol.Count - 1)
                    {
                        result = result + ",";
                    }
                }
            }
            else
            {
                // If the query is not in the knowledge base, return "NO"
                result = "NO";
            }

            // Return the result string
            return result;
        }

        // Check if the knowledge base contains a given fact
        public override bool Contains(string g)
        {
            // Initialize the knowledge base and the schedule
            Setup(Inform);

            // Keep checking the schedule until it has nothing left
            while (_schedule.Count > 0)
            {
                // Get the next fact from the schedule
                string _currentFact = _schedule.Dequeue();

                // If the fact is not already in the knowledge base, add it
                if (!_symbol.Contains(_currentFact))
                {
                    _symbol.Add(_currentFact);

                    // Check all the statements to see if they imply the current fact
                    foreach (string statement in _statements)
                    {
                        if (Implies(statement, _currentFact))
                        {
                            // If the statement implies the current fact, decrement the count for the statement
                            _statementcount[statement] = _statementcount[statement] - 1;
                            int statementcnt = _statementcount[statement];

                            // If the count for the statement is zero, add the right-hand side of the statement to the schedule
                            if (statementcnt <= 0)
                            {
                                string right = statement.Split("=>", StringSplitOptions.RemoveEmptyEntries)[1];
                                right = right.Trim();

                                _schedule.Enqueue(right);

                                // If the right-hand side of the statement is the goal, add it to the knowledge base and return true
                                if (right == g) // checking if goal 
                                {
                                    _symbol.Add(right);
                                    return true;
                                }
                            }
                        }
                    }
                }
            }

            // If none of the statements lead to the goal, return false
            return false;
        }

        // Check if a given statement implies a given fact
        public bool Implies(string statement, string fact)
        {
            string[] left = statement.Split("=>", StringSplitOptions.RemoveEmptyEntries);
            if (left[0].Contains(fact))
            {
                return true;
            }
            return false;
        }
    }
}
