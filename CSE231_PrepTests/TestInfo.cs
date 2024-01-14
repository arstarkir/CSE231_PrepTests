using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.ComTypes;
using System.Security.Policy;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;


namespace CSE231_PrepTests
{
    internal class TestInfo
    {
        public string name="Not Found";
        string genInfo;
        public Dictionary<int, string> answers = new Dictionary<int, string>();
        public int numOfQuestions;
        public bool wasStarted = false;
        public bool wasFinished = false;
        public List<Question> questions = new List<Question>();
        public void DissectTest(string filePath, string text)
        {
            filePath = filePath.Remove(0, filePath.LastIndexOf("\\") + "\\".Length);
            this.name = filePath.Substring(0, filePath.Length-4);
            text = text.Remove(0, text.IndexOf("Directions:")-1);
            genInfo = text.Substring(0, text.IndexOf("Form ") -1);
            text = text.Remove(0, text.IndexOf("Form ") + "Form ".Length);
            text = text.Remove(0, text.IndexOf("\n") + "\n".Length);
            int numLines = text.Substring(0, text.IndexOf("\n\f\n0") + "\n\f\n0".Length).Split('\n').Length - 1;
            for (int i = 0; i < (int)(numLines/3); i++)
            {
                string temp = text.Substring(0, text.IndexOf("\n"));
                temp = Regex.Replace(temp, @"\s+", " ");
                List<string> nums = temp.Split(' ').ToList();
                text = text.Remove(0, text.IndexOf("\n") + "\n".Length);

                temp = text.Substring(0, text.IndexOf("\n"));
                temp = Regex.Replace(temp, @"\s+", " ");
                List<string> ans = temp.Split(' ').ToList();
                text = text.Remove(0, text.IndexOf("\n") + "\n".Length + 1);
                if (nums[0] == "")
                    nums.RemoveAt(0);
                if (ans[0] == "")
                    ans.RemoveAt(0);
                for (int j = 0; j < nums.Count; j++)
                {
                    answers[Int32.Parse(nums[j])] = ans[j];
                    numOfQuestions = Int32.Parse(nums[j]);
                }
                text.Remove(0, text.IndexOf("\n") + "\n".Length + 1);
            }
            var numToAns = new Dictionary<int, string> { [0] = "A)", [1] = "B)", [2] = "C)", [3] = "D)", [4] = "E)" };
            string prevFig = "";
            for (int i = 1; i < numOfQuestions+1; i++)
            {
                bool isPartOfMQ = false;
                string questionText = text.Substring(0, text.IndexOf(numToAns[0]) -3 );
                int questionNum = Int32.Parse(questionText.Substring(questionText.IndexOf("\n") + "\n".Length, questionText.IndexOf(".")-1));
                if (questionText.Contains("Figure"))
                {
                    isPartOfMQ = true;
                    questionText = prevFig + questionText;
                }
                text = text.Remove(0, text.IndexOf(numToAns[0]) - 1);
                Dictionary<string, bool> options = new Dictionary<string, bool>();
                for (int j = 0; j < 6; j++)
                {
                    string numOfNextQ = (i + 1).ToString();
                    numOfNextQ = (numOfNextQ.Length == 1 ? ("\n0" + numOfNextQ) : "\n" + numOfNextQ);
                    if (j == 5) 
                    {
                        if((numOfQuestions) != i)
                        {
                            int a = text.IndexOf("############");
                            int b = text.IndexOf(numOfNextQ);
                            if (i == 25)
                                i = 25;

                            if (b < a || a == -1)
                            {
                                options[text.Substring(0, b)] = false;
                                text = text.Remove(0, b);
                            }
                            else
                            {
                                options[text.Substring(0, a)] = false;
                                text = text.Remove(0, a);
                                prevFig = text.Substring(0, text.IndexOf(numOfNextQ));
                                text = text.Remove(0, text.IndexOf(numOfNextQ));
                            }
                        }
                        else
                        {
                            options[text] = false;
                            text = "";
                        }
                    }
                    else
                    {
                        if (text.IndexOf(numOfNextQ) > text.IndexOf(numToAns[j]) - 1 || (numOfQuestions) == i)
                        {
                            if (text.Substring(0, text.IndexOf(numToAns[j]) - 1) == "")
                            {
                                text = text.Remove(0, text.IndexOf(numToAns[j]) - 1);
                                continue;
                            }
                            options[text.Substring(0, text.IndexOf(numToAns[j]) - 1)] = false;
                            text = text.Remove(0, text.IndexOf(numToAns[j]) - 1);
                        }
                        else
                            continue;
                    }
                }
                questions.Add(new Question(questionText, options, isPartOfMQ, questionNum));
            }
        }
    }
}
