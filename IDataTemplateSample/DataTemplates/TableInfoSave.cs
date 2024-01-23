using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSE231_PrepTests
{
    internal class TableInfoSave
    {
        public string name;
        public string numOfQ;
        //public Button button;
        public TestInfo test;
        public TableInfoSave(string name, string numOfQ, /*Button button,*/ ref TestInfo test)
        {
            this.name = name;   
            this.numOfQ = numOfQ;
            //this.button = button;
            this.test = test;
        }
    }
}
