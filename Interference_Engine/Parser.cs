using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace Interference_Engine
{
    // Obtains the input file and extracts ask/tell info
    class Parser
    {
        private string _query, _inform, _filename;

        public Parser(string filename)
        {
            _filename = "resources/" + filename;
            Parse();
        }

        public void Parse()
        {
            try
            {
                StreamReader _read = new StreamReader(_filename);
                string line;

                while (!_read.EndOfStream)
                {
                    line = _read.ReadLine();
                    if (line.ToUpper().Trim() == "TELL")
                    {
                        _inform = _read.ReadLine();
                    }
                    else if (line.ToUpper().Trim() == "ASK") // will be the ASK section
                    {
                        _query = _read.ReadLine();
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("Exception: " + e.Message);
            }
            
        }

        public string Query { get { return _query; } set { _query = value; } }
        public string Inform { get { return _inform; } set { _inform = value; } }
    }
}
