using System;
using System.Collections.Generic;
using System.Text;

namespace Interference_Engine
{
    class BackwardChaining : KnowledgeBase
    {
        // same initialisation variables as ForwardChaining

        private List<string> _facts; // letters or symbols such as A, B, etc.
        private List<string> _kb; // knowledge base
        private List<string> _statements; // B => D, A & C => F, etc.
        private Queue<string> _schedule; // future statements to evaluate
        private string _ans; // final answers

        public BackwardChaining(string query, string inform) : base(query, inform)
        {
            Type = KnowledgeBaseType.BC;

            _kb = new List<string>();
            _statements = new List<string>();
            _facts = new List<string>();
            _schedule = new Queue<string>();
        }

        // Given a logical statement of the form "A & B => C", returns a list of the individual
        // propositions (in this example, "A" and "B").
        public List<string> getSigns(string statement)
        {
            string left = statement.Split("=>")[0].Trim();

            string[] leftSigns = left.Split("&");

            List<string> result = new List<string>();

            foreach (string sign in leftSigns)
            {
                result.Add(sign.Trim());
            }

            return result;
        }

        // Determines whether a given proposition is true, using backward chaining.
        // Returns true if the proposition is true, false otherwise.
        public override bool Contains(string g)
        {
            Setup(Inform);

            while (_schedule.Count > 0)
            {
                string ask = _schedule.Dequeue();
                _kb.Add(ask);

                if (!_facts.Contains(ask))
                {
                    // statements which need to checked
                    List<string> checkStatements = new List<string>(); 

                    // Check all statements in the knowledge base to see if they imply the current proposition.
                    foreach (string statement in _statements)
                    {
                        if (Finished(statement, ask))
                        {
                            List<string> signs = getSigns(statement);

                            // Add any new propositions we can derive from the current statement to the list of statements
                            // we need to check next.
                            foreach (string sign in signs)
                            {
                                checkStatements.Add(sign);
                            }
                        }
                    }

                    if (checkStatements.Count == 0)
                    {
                        return false;
                    }
                    else
                    {
                        // Add any propositions we need to check next to the schedule.
                        foreach (string sign in checkStatements)
                        {
                            if (!_kb.Contains(sign)) { _schedule.Enqueue(sign); }
                        }
                    }
                }
            }

            return true;
        }

        // Determines whether a given statement implies a given proposition.
        private bool Finished(string statement, string fact)
        {
            string[] left = statement.Split("=>", StringSplitOptions.RemoveEmptyEntries);
            if (left[1].Contains(fact))
            {
                return true;
            }
            return false;
        }

        // Parses the given string of logical statements and adds them to the knowledge base.
        // Also adds the query to the schedule.
        public void Setup(string inform)
        {
            _schedule.Enqueue(Query);
            string[] statements = inform.Split(";", StringSplitOptions.RemoveEmptyEntries);

            foreach (string statement in statements)
            {
                if (statement.Contains("=>"))
                {
                    // If the statement contains "=>", it is a sentence.
                    _statements.Add(statement.Trim());
                }
                else
                {
                    // If the statement doesn't contain "=>", it is a fact.
                    // We should add it to the schedule
                    _facts.Add(statement.Trim());
                }
            }
        }

        public override string Perform()
        {
            // Initialize an empty string variable named "result".
            string result = "";
            // Check if the knowledge base contains the query.
            if (Contains(Query))
            {
                result = "YES:";

                // Iterate through the list of statements in the knowledge base.
                for (int j = 0; j < _kb.Count; j++)
                {
                    // Append each statement to the "result" string with a space and a comma separator between them.
                    result = result + " " + _kb[j];

                    // If the statement is the last one, then it doesn't append a comma.
                    if (j < _kb.Count - 1)
                    {
                        result = result + ",";
                    }
                }
            }
            else
            {
                // If the knowledge base doesn't contain the query, then set the "result" to "NO".
                result = "NO";
            }

            // Set the "_ans" variable to the "result".
            _ans = result;

            // Return the "result".
            return result;
        }


    }
}
