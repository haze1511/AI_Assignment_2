using System;
using System.Collections.Generic;
using System.Text;

namespace Interference_Engine
{
    // Every knowledge base will be based off this parent
    public abstract class KnowledgeBase
    {
        private string _inform, _query; // inputs 

        private KnowledgeBaseType _type;

        // Constructor that takes in the query and inform
        public KnowledgeBase(string ask, string tell)
        {
            _query = ask.Trim();
            _inform = tell.Trim();
        }

        // Abstract method to check if the knowledge base contains a goal
        public abstract bool Contains(string g); 

        // Getters and setters for the private fields
        public string Inform { get { return _inform; } set { _inform = value; } }
        public string Query { get { return _query; } set { _query = value; } }
        public KnowledgeBaseType Type { get { return _type; } set { _type = value; } }

        // Abstract method to perform some operation on the knowledge base
        public abstract string Perform();
    }

    // Enum to represent the type of knowledge base
    public enum KnowledgeBaseType { FC, BC, TT }
}
