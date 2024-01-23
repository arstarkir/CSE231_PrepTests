using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Policy;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace CSE231_PrepTests
{
    public enum AnswerdQ
    {
        Correct,
        Incorrect,
        Started,
        NotStarted
    }
    public class Question
    {
        public AnswerdQ state;
        public bool isPartOfMQ = false;
        public string questionText = "No Question Found";
        public int questionNum = 0;
        public Dictionary<string, bool> options = new Dictionary<string, bool>();
        public Question(string question, Dictionary<string, bool> options, bool isPartOf = false,int qNum = 0) 
        {
            questionText = question;
            this.options = options;
            isPartOfMQ = isPartOf;
            questionNum = qNum;
            state = AnswerdQ.NotStarted;
        }
    }
}
