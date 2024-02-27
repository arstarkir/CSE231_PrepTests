using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CSE231_PrepTests
{
    internal class TableInfoSave
    {
        public string name;
        public string numOfQ;
        public Button button;
        public Button button1;
        public TestInfo test;
        public Stopwatch TimeSpent;
        public TableInfoSave(string name, string numOfQ, Button button, ref TestInfo test, Button button1)
        {
            this.name = name;   
            this.numOfQ = numOfQ;
            this.button = button;
            this.button1 = button1;
            this.test = test;
        }

        public bool SavesThisTest(TestInfo test)
        {
            return (this.test == test);
        }
    }
}
