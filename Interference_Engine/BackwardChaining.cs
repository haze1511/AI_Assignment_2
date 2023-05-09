using System;
using System.Collections.Generic;
using System.Linq;

namespace Interference_Engine
{
    public class BackwardChaining : KnowledgeBase
    {
        // Fields to store facts, rules, agenda, and inferred statements
        private readonly HashSet<string> _facts;
        private readonly List<string> _rules;
        private readonly Queue<string> _agenda;
        private readonly List<string> _inferred;

        // Constructor that initializes the fields and sets up the knowledge base
        public BackwardChaining(string query, string information) : base(query, information)
        {
            _facts = new HashSet<string>();
            _rules = new List<string>();
            _agenda = new Queue<string>();
            _inferred = new List<string>();
            Setup(information);
        }

        // Override of the Perform method that executes the backward chaining algorithm
        public override string Perform()
        {
            return Infer(Query) ? GeneratePositiveResult() : "NO";
        }

        // Override of the Contains method that checks if a goal has been inferred
        public override bool Contains(string goal)
        {
            return _inferred.Contains(goal);
        }

        // Method that sets up the knowledge base by parsing the information string
        private void Setup(string information)
        {
            _agenda.Enqueue(Query);
            var statements = information.Split(";", StringSplitOptions.RemoveEmptyEntries);

            foreach (var statement in statements)
            {
                if (statement.Contains("=>"))
                    _rules.Add(statement.Trim());
                else
                    _facts.Add(statement.Trim());
            }
        }

        // Method that performs backward chaining to infer the query
        private bool Infer(string goal)
        {
            while (_agenda.Any())
            {
                var ask = _agenda.Dequeue();
                _inferred.Add(ask);

                if (!_facts.Contains(ask))
                {
                    var matchingRules = FindRulesThatConclude(ask);
                    if (!matchingRules.Any())
                        return false;

                    EnqueueNewGoals(matchingRules);
                }
            }

            return true;
        }

        // Method that finds the rules that conclude a given fact and returns the premises
        private IEnumerable<string> FindRulesThatConclude(string fact)
        {
            return _rules.Where(rule => rule.Split("=>")[1].Trim() == fact)
                         .SelectMany(rule => rule.Split("=>")[0].Split("&").Select(s => s.Trim()));
        }

        // Method that enqueues new goals to the agenda if they haven't already been inferred
        private void EnqueueNewGoals(IEnumerable<string> goals)
        {
            foreach (var goal in goals)
                if (!_inferred.Contains(goal))
                    _agenda.Enqueue(goal);
        }

        // Method that generates the positive result string
        private string GeneratePositiveResult()
        {
            return "YES: " + string.Join(", ", _inferred);
        }
    }
}