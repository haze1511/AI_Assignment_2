using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace Interference_Engine
{
    class ForwardChaining : KnowledgeBase
    {
        private List<string> _kb; // Letters or symbols such as A, B, etc.
        private List<string> _statements; // Rules given
        private Dictionary<string, int> _statementcnt; //Dictionary of statement counts
        private Queue<string> _schedule; // A queue of statements that need to be evaluated
        private string _ans; // final answers


        public ForwardChaining(string query, string inform) : base(query, inform)
        {
            Type = KnowledgeBaseType.FC;

            _kb = new List<string>();
            _statements = new List<string>();
            _statementcnt = new Dictionary<string, int>();
            _schedule = new Queue<string>();
        }

        // A method that sets up the knowledge base based on an input string of rules and facts
        public void Setup(string inform)
        {
            // Split the input string into individual statements using ";" as the separator
            string[] statements = inform.Split(";", StringSplitOptions.RemoveEmptyEntries);

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
                    _statementcnt.Add(statement.Trim(), leftcnt);
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
                // If it is, add all the items in the knowledge base to the result string
                result = "YES:";
                for (int j = 0; j < _kb.Count; j++)
                {
                    result = result + " " + _kb[j];
                    if (j < _kb.Count - 1)
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

            _ans = result;

            return result;
        }

        // Check if the knowledge base contains a given fact
        public override bool Contains(string g)
        {
            Setup(Inform);

            // Keep checking the schedule until it has nothing left
            while (_schedule.Count > 0)
            {
                // Get the next fact from the schedule
                string _currentFact = _schedule.Dequeue();

                // If the fact is not already in the knowledge base, add it
                if (!_kb.Contains(_currentFact))
                {
                    _kb.Add(_currentFact);

                    // Check all the statements to see if they imply the current fact
                    foreach (string statement in _statements)
                    {
                        if (Implies(statement, _currentFact))
                        {
                            // If the statement implies the current fact, decrement the count for the statement
                            _statementcnt[statement] = _statementcnt[statement] - 1;
                            int statementcnt = _statementcnt[statement];

                            // If the count for the statement is zero, add the right-hand side of the statement to the schedule
                            if (statementcnt <= 0)
                            {
                                string right = statement.Split("=>", StringSplitOptions.RemoveEmptyEntries)[1];
                                right = right.Trim();

                                _schedule.Enqueue(right);

                                // If the right-hand side of the statement is the goal, add it to the knowledge base and return true
                                if (right == g) // checking if goal 
                                {
                                    _kb.Add(right);
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
