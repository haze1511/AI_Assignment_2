using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Linq;

namespace Interference_Engine
{
    class TruthTable : KnowledgeBase
    {
        private Dictionary<string, List<bool>> _truthTable; // Dictionary of all the TT values

        private List<string> _statements, _signs, _facts;

        private List<string> _guides;
        private List<int> _guideRows;

        private string _ans; // final answer

        public TruthTable(string query, string inform) : base(query, inform)
        {
            Type = KnowledgeBaseType.TT;

            _truthTable = new Dictionary<string, List<bool>>();
            _guides = new List<string>();
            _facts = new List<string>();
            _guideRows = new List<int>();
            _statements = new List<string>();
            _signs = new List<string>();
        }

        // Converts the tell string into a list of statements and adds the facts to the fact list.
        public List<string> ToStatements(string tell)
        {
            List<string> statements = new List<string>();

            string[] separateStatements = tell.Split(";");

            foreach (string statement in separateStatements)
            {
                if (!string.IsNullOrEmpty(statement))
                {
                    if (statement.Contains("=>"))
                    {
                        statements.Add(statement.Trim());
                    }
                    else
                    {
                        _facts.Add(statement.Trim());
                    }
                }
            }

            return statements;
        }

        // Converts the inform string into a list of signs.
        public List<string> ToSigns(string inform)
        {
            List<string> toSigns = new List<string>();

            string[] splitStatements = Regex.Split(inform, @"[\s.,;&=>\\~\\|\\|]+");

            foreach (string statement in splitStatements)
            {
                if (statement != "" && !toSigns.Contains(statement)) {
                    toSigns.Add(statement.Trim());
                }
            }

            return toSigns;
        }

        // Overrides the Contains method of KnowledgeBase to use truth tables to check if the query is entailed by the KB.
        public override bool Contains(string g)
        {
            _statements = ToStatements(Inform);
            _signs = ToSigns(Inform);

            // generating the  truth table signs

            int num_Rows = (int)Math.Pow(2, Signs.Count);

            for (int j = 0; j < Signs.Count; j++)
            {
                List<bool> signColumn = new List<bool>();
                int spot = 0; // place where we are up to 
                int skipRows = (int)(num_Rows / (Math.Pow(2, j + 1)));
                bool curSpot = true; // value of the current spot (i.e. T or F)

                for (int i = 0; i < num_Rows; i++)
                {
                    signColumn.Add(curSpot);
                    spot++;
                    if (spot >= skipRows)
                    {
                        spot = 0;
                        curSpot = !curSpot;
                    }
                }
                _truthTable.Add(Signs[j], signColumn);
            }

            // getting all the statements in the  truth table 
            foreach (string statement in Statements)
            {
                string[] splitStatement = statement.Split("=>");
                splitStatement = splitStatement.Where(s => !string.IsNullOrWhiteSpace(s)).ToArray();
                string left = splitStatement[0].Trim();
                string right = splitStatement[1].Trim();

                // left may have signs so count them 
                List<string> left_signs = ToSigns(left);
                List<bool> statementCol = new List<bool>();

                for (int j = 0; j < num_Rows; j++)
                {
                    // look at all the AND's as they will influence T/F
                    bool anded_Value = true;
                    foreach (string sign in left_signs)
                    {
                        anded_Value = _truthTable[sign][j] && anded_Value;
                    }
                    bool finalVal = (!anded_Value || _truthTable[right][j]);
                    statementCol.Add(finalVal);
                }

                // add new col into TT
                _truthTable.Add(statement, statementCol);
            }

            // Let the truth table doing the asking now
            // there must be at least one fact for the query to be entailed
            if (_facts.Count > 0) 
            {
                for (int index = 0; index < num_Rows; index++)
                {
                    bool satisfied = true;
                    foreach (string fact in _facts)
                    {
                        //satisfied = and.Evaluate(satisfied, _truthTable[fact][index]);
                        satisfied = satisfied && _truthTable[fact][index];
                    }

                    foreach (string statement in Statements)
                    {
                        satisfied = satisfied && _truthTable[statement][index];
                        //satisfied = and.Evaluate(satisfied, _truthTable[statement][index]);
                    }

                    //if (and.Evaluate(satisfied, _truthTable[Query][index]))
                    if (satisfied && _truthTable[Query][index])
                    {
                        string guide = "";

                        foreach (string sign in Signs)
                        {
                            guide += ($" " + sign + ": " + _truthTable[sign][index] + ";");
                        }

                        foreach (string statement in Statements)
                        {
                            guide += ($" " + statement + ": " + _truthTable[statement][index] + ";");
                        }

                        _guides.Add(guide);
                        _guideRows.Add(index);
                        
                    }
                }
            }

            if (_guides.Count > 0)
            {
                return true;
            }
            return false;
        }

        public override string Perform()
        {
            string result = "";

            if (Contains(Query))
            {
                result = "YES: " + _guides.Count;
            }
            else
            {
                result = "NO";
            }
            _ans = result;
            return result;
        }

        public List<string> Signs { get { return _signs;  } set { _signs = value; } }
        public List<string> Statements { get { return _statements; } set { _statements = value; } }
    }
}
