using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.Common;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;
using System.Windows.Forms;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace CSE231_PrepTests
{
    public partial class Form1 : Form
    {
        bool timerEx = false;
        //Question visuals
        Label questionText = new System.Windows.Forms.Label();
        Label timer = new System.Windows.Forms.Label();
        CheckedListBox checkedListOptions = new System.Windows.Forms.CheckedListBox();
        Button prev = new System.Windows.Forms.Button();
        Button next = new System.Windows.Forms.Button();
        Button checkAns = new System.Windows.Forms.Button();
        Button timerB = new System.Windows.Forms.Button();

        System.Windows.Forms.Timer TestTimer = new System.Windows.Forms.Timer();
        

        //Question + Test info
        List<TestInfo> tests = new List<TestInfo>();
        TestInfo curTest = null;
        Question curQ = null;

        //Info Vault
        List<TableInfoSave> testTableInfoStorage = new List<TableInfoSave>();
        public Form1()
        {
            InitializeComponent();
            TestTimer.Interval = 1000;
            TestTimer.Tick += new EventHandler(OnTimedEvent);
            TestTimer.Start();
            FromSave();
        }
        class AnswerItem
        {
            public int aKey { get; set; }
            public string aVal { get; set; }
        }
        void FromSave()
        {
            string fileName = "SaveData.json";
            string filePath = Path.Combine(Directory.GetCurrentDirectory(), fileName);

            string jsonString = File.ReadAllText(filePath);
            JObject rss = JObject.Parse(jsonString);
            
            string name = (string)rss["name"];
            string numOfQ = (string)rss["numOfQ"];
            Button button = new Button();
            JObject testInfoJson = (JObject)rss["testInfo"];

            var answersList = testInfoJson["answers"].ToObject<List<AnswerItem>>();
            var answersDict = answersList.ToDictionary(item => item.aKey, item => item.aVal);

            string nameT = (string)testInfoJson["name"];
            string genInfo = (string)testInfoJson["genInfo"];

            string totalTimeString = (string)testInfoJson["Time"];

            Dictionary<int, string> answers = answersDict;
            int qFinished = (int)testInfoJson["qFinished"];
            int numOfQuestions = (int)testInfoJson["numOfQuestions"];
            bool wasStarted = (bool)testInfoJson["wasStarted"];

            JArray questionsArray = (JArray)testInfoJson["questions"];
            List<Question> questions = new List<Question>();

            foreach (JObject questionJson in questionsArray)
            {
                string questionText = questionJson.Value<string>("questionText");
                int questionNum = questionJson.Value<int>("questionNum");
                bool isPartOfMQ = questionJson.Value<bool>("isPartOfMQ");
                string stateStr = questionJson["state"].ToString();
                AnswerdQ state = (AnswerdQ)Enum.Parse(typeof(AnswerdQ), stateStr);

                JArray optionsArray = (JArray)questionJson["options"];
                Dictionary<string, bool> options = optionsArray.ToDictionary(
                    opt => opt["qKey"].ToString(),
                    opt => (bool)opt["qVal"]
                );

                Question question = new Question(questionText, options, isPartOfMQ, questionNum);
                question.state = state;
                questions.Add(question);
            }
            TestInfo testInfo = new TestInfo(nameT, genInfo, answers, qFinished, numOfQuestions, wasStarted, questions, totalTimeString);
            tests.Add(testInfo);

            updateTable();
        }

        private void OnTimedEvent(object sender, EventArgs e)
        {
            if(timerEx)
            {
                TimeSpan timeSpan1 = TimeSpan.ParseExact(curTest.prevTimeSpent, @"hh\:mm\:ss\.fffffff", CultureInfo.InvariantCulture);
                timer.Text = (curTest.TimeSpent.Elapsed + timeSpan1).ToString(@"hh\:mm\:ss");
            }
            //curTest.TimeSpent.Elapsed
        }

        private Button CloneButton(Button org, EventHandler whenClick)
        {
            Button tempButton = new Button();
            tempButton.FlatStyle = org.FlatStyle;
            tempButton.Font = org.Font;
            tempButton.Name = org.Name;
            tempButton.Size = org.Size;
            tempButton.TabIndex = org.TabIndex;
            tempButton.Text = org.Text;
            tempButton.UseVisualStyleBackColor = org.UseVisualStyleBackColor;
            tempButton.Click += whenClick;
            return tempButton;
        }
        private void Form1_FormClosing(Object sender, FormClosingEventArgs e)
        {
            
            foreach (TableInfoSave item in testTableInfoStorage)
            {
                TimeSpan timeSpan1 = TimeSpan.ParseExact(item.test.TimeSpent.Elapsed.ToString(), @"hh\:mm\:ss\.fffffff", CultureInfo.InvariantCulture);
                TimeSpan timeSpan2 = TimeSpan.ParseExact(item.test.prevTimeSpent, @"hh\:mm\:ss\.fffffff", CultureInfo.InvariantCulture);
                TimeSpan totalTimeSpan = timeSpan1 + timeSpan2;
                string totalTimeString = totalTimeSpan.ToString(@"hh\:mm\:ss\.fffffff");
                JObject rss =
                new JObject(
                    new JProperty("name", item.name),
                    new JProperty("numOfQ", item.numOfQ),
                new JProperty("testInfo",
                    new JObject(
                        new JProperty("name", item.test.name),
                        new JProperty("genInfo", item.test.genInfo),
                        new JProperty("Time", totalTimeString),
                        new JProperty("answers",
                            new JArray(
                                from a in item.test.answers
                                select new JObject(
                                    new JProperty("aKey", a.Key),
                                    new JProperty("aVal", a.Value)))),
                        new JProperty("qFinished", item.test.qFinished),
                        new JProperty("numOfQuestions", item.test.numOfQuestions),
                        new JProperty("wasStarted", item.test.wasStarted),
                        new JProperty("questions",
                            new JArray(
                                from q in item.test.questions
                                select new JObject(
                                    new JProperty("state", q.state),
                                    new JProperty("isPartOfMQ", q.isPartOfMQ),
                                    new JProperty("questionText", q.questionText),
                                    new JProperty("questionNum", q.questionNum),
                                    new JProperty("options",
                                        new JArray(
                                            from o in q.options
                                            select new JObject(
                                            new JProperty("qKey", o.Key),
                                            new JProperty("qVal", o.Value)
                                            )))))))));

                string jsonContent = rss.ToString();

                // Define the path for the file in the current working directory
                string fileName = "SaveData.json";
                string filePath = Path.Combine(Directory.GetCurrentDirectory(), fileName);

                // Write the JSON string to a file
                File.WriteAllText(filePath, jsonContent);
                //if (!File.Exists("CSE231_PrepTests_save_info.txt")) // If file does not exists
                //{ 
                //    File.Create("CSE231_PrepTests_save_info.txt").Close(); // Create file
                //    using (StreamWriter sw = File.AppendText("CSE231_PrepTests_save_info.txt"))
                //    {
                //        sw.WriteLine(rss.ToString()); // Write text to .txt file
                //    }
                //}
                //else // If file already exists
                //{
                //    // File.WriteAllText("FILENAME.txt", String.Empty); // Clear file
                //    using (StreamWriter sw = File.AppendText("CSE231_PrepTests_save_info.txt"))
                //    {
                //        sw.WriteLine(rss.ToString()); // Write text to .txt file
                //    }
                //}
            }
        }


        private void button1_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog openFileDialog = new OpenFileDialog())
            {
                openFileDialog.InitialDirectory = "c:\\";
                openFileDialog.Filter = "txt files (*.txt)|*.txt|All files (*.*)|*.*";
                openFileDialog.FilterIndex = 2;
                openFileDialog.Multiselect = true;
                openFileDialog.RestoreDirectory = true;

                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    foreach (string filePath in openFileDialog.FileNames)
                    {
                        TestInfo tempTest = new TestInfo();

                        string fileContent;
                        using (StreamReader reader = new StreamReader(filePath))
                        {
                            fileContent = reader.ReadToEnd();
                        }
                        try
                        {
                            tempTest.DissectTest(filePath, fileContent);
                            MessageBox.Show("Yep, that file works", "", MessageBoxButtons.OK);
                            tests.Add(tempTest);
                            updateTable();
                        }
                        catch
                        {
                            MessageBox.Show("Something went wrong(", "", MessageBoxButtons.OK);
                        }
                    }
                }
            }
        }
        void updateTable()
        {
            Button button = new Button();
            button.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            button.Font = new System.Drawing.Font("Microsoft Sans Serif", 16.64F);
            button.Name = "button";
            button.Size = new System.Drawing.Size(102, 40);
            button.TabIndex = 3;
            button.Text = "Start Test"+ (tests.Count - 1).ToString();
            button.UseVisualStyleBackColor = true;
            button.Click += new System.EventHandler(start_Click);
            TestInfo test = tests[tests.Count - 1];
            //AddItemTable(tableLayoutPanel2,tests[tests.Count-1].name, tests[tests.Count-1].numOfQuestions.ToString(), button, test);
            testTableInfoStorage.Add(new TableInfoSave(tests[tests.Count - 1].name, tests[tests.Count - 1].numOfQuestions.ToString(), button, ref test));


            Size TempCS = ClientSize;
            Point TempLoc = Location;

            if (TempCS.Height > 500)
            {
                TempCS.Height = 500;
            }
            this.Controls.Clear();
            for (int i = 0; i < testTableInfoStorage.Count; i++)
            {
                AddItemTable(tableLayoutPanel2, testTableInfoStorage[i].name, testTableInfoStorage[i].numOfQ, testTableInfoStorage[i].button, testTableInfoStorage[i].test);
            }
            Controls.Add(tableLayoutPanel2);
            InitializeComponent();
            ClientSize = TempCS;
            Location = TempLoc;
            Controls.Remove(questionText);
            Controls.Remove(checkedListOptions);
            Controls.Remove(prev);
            Controls.Remove(next);
            Controls.Remove(checkAns);

        }
        private void AddItemTable(TableLayoutPanel table, string name, string numOfQ, Button button, TestInfo test)
        {
            RowStyle temp = table.RowStyles[table.RowCount - 1];
            table.RowCount++;
            table.RowStyles.Add(new RowStyle(temp.SizeType, temp.Height));  
            table.Controls.Add(new Label() { Text = name }, 0, table.RowCount - 1);
            table.Controls.Add(new Label() { Text = numOfQ }, 1, table.RowCount - 1);
            table.Controls.Add(button, 2, table.RowCount - 1);
            table.Controls.Add(new Label() { Text = test.qFinished.ToString() }, 2, table.RowCount - 1);
        }

        private void tableLayoutPanel2_Paint(object sender, PaintEventArgs e)
        {

        }

        private void start_Click(object sender, EventArgs e)
        {
            curTest = tests[Int32.Parse(sender.ToString().Remove(0, sender.ToString().LastIndexOf("t") + "t".Length))];
            curQ = GetUnfinQ(curTest);
            curTest.wasStarted = true;
            TestTimer.Tick += new System.EventHandler(this.timer2_Tick);
            genQ();
        }
        void genQ()
        {
            Size TempCS = ClientSize;
            Point TempLoc = Location;
            this.Controls.Clear();
            // 
            // label1
            // 
            questionText = new Label();
            questionText.AutoSize = true;
            questionText.Location = new System.Drawing.Point(3, 0);
            questionText.Font =  new System.Drawing.Font("Microsoft Sans Serif", 14F);
            questionText.Name = "label1";
            questionText.Size = new System.Drawing.Size(700, 150);
            questionText.TabIndex = 6;
            questionText.Text = curQ.questionText;
            questionText.BackColor = System.Drawing.Color.Transparent;
            questionText.TextAlign = System.Drawing.ContentAlignment.TopLeft;
            questionText.Click += new System.EventHandler(label1_Click);
            Controls.Add(questionText);

            // 
            // label2
            // 
            timer.AutoSize = true;
            timer.Location = new System.Drawing.Point(3, 0);
            timer.Font = new System.Drawing.Font("Microsoft Sans Serif", 14F);
            timer.Name = "label2";
            if(timer.Text== "")
            {
                timer.Text = "00:00:00";
            }
            timer.Text = timer.Text;
            timer.Location = new System.Drawing.Point(questionText.Location.X + 700, questionText.Location.Y);
            //timer.Location = new System.Drawing.Point(questionText.Location.X + questionText.Size.Width, questionText.Location.Y + questionText.Size.Height);
            timer.Size = new System.Drawing.Size(700, 150);
            timer.TabIndex = 6;
            timer.BackColor = System.Drawing.Color.Transparent;
            timer.TextAlign = System.Drawing.ContentAlignment.TopLeft;
            timer.Click += new System.EventHandler(label1_Click);
            Controls.Add(timer);


            // 
            // timerB
            // 
            timerB = new Button();
            timerB.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            timerB.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F);
            timerB.Location = new System.Drawing.Point(timer.Location.X + timer.Size.Width, timer.Location.Y);
            timerB.Name = "timerB";
            timerB.Size = new System.Drawing.Size(50, 30);
            timerB.TabIndex = 5;
            timerB.Text = "Start";
            timerB.UseVisualStyleBackColor = true;
            timerB.Click += new System.EventHandler(timerStart);
            Controls.Add(timerB);

            // 
            // checkedListBox1
            // 
            checkedListOptions = new CheckedListBox();
            checkedListOptions.Anchor = System.Windows.Forms.AnchorStyles.Top;
            checkedListOptions.Font = new System.Drawing.Font("Microsoft Sans Serif", 14F);
            checkedListOptions.FormattingEnabled = true;
            checkedListOptions.Location = new System.Drawing.Point(3, questionText.Location.Y + questionText.Size.Height);
            checkedListOptions.Name = "checkedListBox1";
            checkedListOptions.RightToLeft = System.Windows.Forms.RightToLeft.No;
            checkedListOptions.Size = new System.Drawing.Size(700, 150);
            checkedListOptions.TabIndex = 3;
            switch (curQ.state)
            {
                case AnswerdQ.Started:
                    checkedListOptions.BackColor = Color.Yellow;
                    break;
                case AnswerdQ.Incorrect:
                    checkedListOptions.BackColor = Color.Red;
                    break;
                case AnswerdQ.Correct:
                    checkedListOptions.BackColor = Color.Green;
                    break;
                default:
                    checkedListOptions.BackColor = SystemColors.Control;
                    break;
            }
            foreach (var option in curQ.options)
            {
                checkedListOptions.Items.Add(option.Key,option.Value);
            }
            

            checkedListOptions.SelectedIndexChanged += new System.EventHandler(checkedListBox1_SelectedIndexChanged);
            Controls.Add(checkedListOptions);
            // 
            // button2
            // 
            prev = new Button();
            prev.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            prev.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F);
            prev.Location = new System.Drawing.Point(3, checkedListOptions.Location.Y + checkedListOptions.Size.Height);
            prev.Name = "button2";
            prev.Size = new System.Drawing.Size(150, 70);
            prev.TabIndex = 4;
            prev.UseVisualStyleBackColor = true;
            if (curTest.questions.IndexOf(curQ) == 0)
            {
                prev.Text = "Menu";
                prev.Click += new System.EventHandler(menu_Click);
            }
            else
            {
                prev.Text = "Previous";
                prev.UseVisualStyleBackColor = true;
                prev.Click += new System.EventHandler(prev_Click);
            }
            Controls.Add(prev);
            // 
            // button3
            // 
            checkAns = new Button();
            checkAns.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            checkAns.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F);
            checkAns.Location = new System.Drawing.Point(159, checkedListOptions.Location.Y + checkedListOptions.Size.Height);
            checkAns.Name = "button3";
            checkAns.Size = new System.Drawing.Size(150, 70);
            checkAns.TabIndex = 5;
            checkAns.Text = "Check";
            checkAns.UseVisualStyleBackColor = true;
            checkAns.Click += new System.EventHandler(checkAns_Click);
            Controls.Add(checkAns);
            // 
            // button4
            // 
            next = new Button();
            next.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            next.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F);
            next.Location = new System.Drawing.Point(315, checkedListOptions.Location.Y + checkedListOptions.Size.Height);
            next.Name = "button4";
            next.Size = new System.Drawing.Size(150, 70);
            next.TabIndex = 5;
            next.UseVisualStyleBackColor = true;
            if (curTest.questions.IndexOf(curQ) == curTest.questions.Count - 1)
            {
                next.Text = "Menu";
                next.Click += new System.EventHandler(menu_Click);
            }
            else
            {
                next.Text = "Next";
                next.Click += new System.EventHandler(next_Click);
            }
            if (TempCS.Height > 500)
            {
                TempCS.Height = 500;
            }
            if (TempCS.Height < checkedListOptions.Location.Y + checkedListOptions.Size.Height + 70)
            {
                TempCS.Height = checkedListOptions.Location.Y + checkedListOptions.Size.Height + 70;
            }
            Controls.Add(next);
            InitializeComponent();
            Controls.Remove(button1);
            Controls.Remove(tableLayoutPanel2);
            ClientSize = TempCS;
            Location = TempLoc;
        }
        Question GetUnfinQ(TestInfo test)
        {
            Question unfinQ = null;
            if (test.wasStarted)
            {
                foreach (Question question in test.questions)
                {
                    if (question.state == AnswerdQ.NotStarted || question.state == AnswerdQ.Started)
                    {
                        unfinQ = question;
                        break;
                    }
                }
            }
            else
                unfinQ = test.questions[0];

            return unfinQ;
        }
        private void prev_Click(object sender, EventArgs e)
        {
            int curQind = curTest.questions.IndexOf(curQ);
            if (curQind > 0)
            {
                curQ = curTest.questions[curQind - 1];
                genQ();
            }
        }

        private void next_Click(object sender, EventArgs e)
        {
            int curQind = curTest.questions.IndexOf(curQ);
            if (curQind < curTest.questions.Count)
            {
                curQ = curTest.questions[curQind+1];
                genQ();
            }
        }
        private void timerStart(object sender, EventArgs e)
        {
            timerEx = true;
            curTest.TimeSpent.Start();
            timerB.Click += new System.EventHandler(timerStop);
            timerB.Text = "Stop";
        }
        private void timerStop(object sender, EventArgs e)
        {
            timerEx = false;
            curTest.TimeSpent.Stop();
            timerB.Click += new System.EventHandler(timerStart);
            timerB.Text = "Start";
        }
        private void menu_Click(object sender, EventArgs e)
        {
            Size TempCS = ClientSize;
            Point TempLoc = Location;

            if (TempCS.Height > 500)
            {
                TempCS.Height = 500;
            }
            this.Controls.Clear();
            for (int i = 0; i < testTableInfoStorage.Count; i++)
            {
                AddItemTable(tableLayoutPanel2, testTableInfoStorage[i].name, testTableInfoStorage[i].numOfQ, testTableInfoStorage[i].button, testTableInfoStorage[i].test);
            }
            Controls.Add(tableLayoutPanel2);
            InitializeComponent();
            ClientSize = TempCS;
            Location = TempLoc;
            Controls.Remove(questionText);
            Controls.Remove(checkedListOptions);
            Controls.Remove(prev);
            Controls.Remove(next);
            Controls.Remove(checkAns);
            if(curTest.TimeSpent.IsRunning)
                curTest.TimeSpent.Stop();
        }

        private void checkAns_Click(object sender, EventArgs e)
        {
            string correctAns = curTest.answers[curQ.questionNum];
            var numToAns = new Dictionary<int, string> { [0] = "A", [1] = "B", [2] = "C", [3] = "D", [4] = "E" };
            for (int i = 0; i < curQ.options.Count; i++)
                if (checkedListOptions.GetItemCheckState(i) == CheckState.Checked)
                {
                    foreach (char c in correctAns)
                    {
                        if (correctAns == numToAns[i])
                        {
                            if (checkedListOptions.BackColor == SystemColors.Control|| checkedListOptions.BackColor == Color.Yellow)
                                curTest.qFinished++;
                            curQ.state = AnswerdQ.Correct;
                            checkedListOptions.BackColor = Color.Green;
                        }
                        else
                        {
                            if (checkedListOptions.BackColor == SystemColors.Control || checkedListOptions.BackColor == Color.Yellow)
                                curTest.qFinished++;
                            curQ.state = AnswerdQ.Incorrect;
                            checkedListOptions.BackColor = Color.Red;
                        }
                    }
                }
            if(checkedListOptions.BackColor == System.Drawing.SystemColors.Control)
            {
                curQ.state = AnswerdQ.Started;
                checkedListOptions.BackColor = Color.Yellow;
            }
        }

        private void checkedListBox1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void Form1_Load(object sender, EventArgs e)
        {
            Location = new System.Drawing.Point(0,0);
        }

        private void timer2_Tick(object sender, EventArgs e)
        {
        }

        protected override void OnFormClosed(FormClosedEventArgs e)
        {
            if (curTest != null && curTest.TimeSpent.IsRunning)
            {
                curTest.TimeSpent.Stop();
            }
            
            base.OnFormClosed(e);
        }
    }
}
