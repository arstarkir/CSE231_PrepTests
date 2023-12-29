using CSE231_PrepTests.Properties;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Policy;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace CSE231_PrepTests
{
    internal class Question
    {
        bool wasFinished = false;
        bool wasStarted = false;
        bool isPartOfMQ = false;
        public string questionText = "No Question Found";
        public Dictionary<string, bool> options = new Dictionary<string, bool>();
        public Question(string question, Dictionary<string, bool> options, bool isPartOf = false) 
        {
            questionText = question;
            this.options = options;
            isPartOfMQ = isPartOf;
        }
    }
}
