using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Net.Mime.MediaTypeNames;

namespace CSE231_PrepTests
{
    public partial class Form1 : Form
    {
        //Question visuals
        Label questionText = new System.Windows.Forms.Label();
        CheckedListBox checkedListOptions = new System.Windows.Forms.CheckedListBox();
        Button prev = new System.Windows.Forms.Button();
        Button next = new System.Windows.Forms.Button();
        Button checkAns = new System.Windows.Forms.Button();

        //Question + Test info
        List<TestInfo> tests = new List<TestInfo>();
        TestInfo curTest = null;
        Question curQ = null;

        //Info Vault

        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            TestInfo tempTest = new TestInfo();
            var fileContent = string.Empty;
            var filePath = string.Empty;

            using (OpenFileDialog openFileDialog = new OpenFileDialog())
            {
                openFileDialog.InitialDirectory = "c:\\";
                openFileDialog.Filter = "txt files (*.txt)|*.txt|All files (*.*)|*.*";
                openFileDialog.FilterIndex = 2;
                openFileDialog.RestoreDirectory = true;

                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    //Get the path of specified file
                    filePath = openFileDialog.FileName;

                    //Read the contents of the file into a stream
                    var fileStream = openFileDialog.OpenFile();

                    using (StreamReader reader = new StreamReader(fileStream))
                    {
                        fileContent = reader.ReadToEnd();
                    }
                }
            }
            try
            {
                tempTest.DissectTest(filePath, fileContent);
                MessageBox.Show("Yep, that file works","", MessageBoxButtons.OK);
                tests.Add(tempTest);
                updateTable();
            }
            catch 
            {
                MessageBox.Show("Something went wrong(","" , MessageBoxButtons.OK);
            }
        }
        void updateTable()
        {
            System.Windows.Forms.Button button = new System.Windows.Forms.Button();
            button.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            button.Font = new System.Drawing.Font("Microsoft Sans Serif", 16.64F);
            button.Name = "button";
            button.Size = new System.Drawing.Size(102, 40);
            button.TabIndex = 3;
            button.Text = "Start Test"+ (tests.Count - 1).ToString();
            button.UseVisualStyleBackColor = true;
            System.EventHandler eventHandler = new System.EventHandler(start_Click);
            //eventHandler.
            button.Click += new System.EventHandler(start_Click);
            AddItem(tests[tests.Count-1].name, tests[tests.Count-1].numOfQuestions.ToString(), button, "idk.... Good");
        }
        private void AddItem( string name, string numOfQ, System.Windows.Forms.Button button, string progres)
        {
            RowStyle temp = tableLayoutPanel2.RowStyles[tableLayoutPanel2.RowCount - 1];
            tableLayoutPanel2.RowCount++;
            tableLayoutPanel2.RowStyles.Add(new RowStyle(temp.SizeType, temp.Height));
            tableLayoutPanel2.Controls.Add(new Label() { Text = name }, 0, tableLayoutPanel2.RowCount - 1);
            tableLayoutPanel2.Controls.Add(new Label() { Text = numOfQ }, 1, tableLayoutPanel2.RowCount - 1);
            tableLayoutPanel2.Controls.Add(button, 2, tableLayoutPanel2.RowCount - 1);
            tableLayoutPanel2.Controls.Add(new Label() { Text = progres }, 2, tableLayoutPanel2.RowCount - 1);

        }

        private void tableLayoutPanel2_Paint(object sender, PaintEventArgs e)
        {

        }

        private void start_Click(object sender, EventArgs e)
        {
            curTest = tests[Int32.Parse(sender.ToString().Remove(0, sender.ToString().LastIndexOf("t") + "t".Length))];
            curQ = GetUnfinQ(curTest);
            genQ();
        }
        void genQ()
        {
            Size TempCS = ClientSize;
            this.Controls.Clear();
            // 
            // label1
            // 
            questionText = new Label();
            questionText.AutoSize = true;
            questionText.Location = new System.Drawing.Point(3, 0);
            questionText.Font =  new System.Drawing.Font("Microsoft Sans Serif", 15F);
            questionText.Name = "label1";
            questionText.Size = new System.Drawing.Size(172, 20);
            questionText.TabIndex = 6;
            questionText.Text = curQ.questionText;
            questionText.BackColor = System.Drawing.Color.Transparent;
            questionText.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            questionText.Click += new System.EventHandler(label1_Click);
            Controls.Add(questionText);
            // 
            // checkedListBox1
            // 
            checkedListOptions = new CheckedListBox();
            checkedListOptions.Anchor = System.Windows.Forms.AnchorStyles.Top;
            checkedListOptions.BackColor = System.Drawing.SystemColors.Control;
            checkedListOptions.Font = new System.Drawing.Font("Microsoft Sans Serif", 15F);
            checkedListOptions.FormattingEnabled = true;
            checkedListOptions.Location = new System.Drawing.Point(3, questionText.Location.Y + questionText.Size.Height);
            checkedListOptions.Name = "checkedListBox1";
            checkedListOptions.RightToLeft = System.Windows.Forms.RightToLeft.No;
            checkedListOptions.Size = new System.Drawing.Size(770, 229);
            checkedListOptions.TabIndex = 3;

            
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
            prev.Font = new System.Drawing.Font("Microsoft Sans Serif", 16.64F);
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
                prev.Text = "Prev";
                prev.UseVisualStyleBackColor = true;
                prev.Click += new System.EventHandler(prev_Click);
            }
            Controls.Add(prev);
            // 
            // button3
            // 
            checkAns = new Button();
            checkAns.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            checkAns.Font = new System.Drawing.Font("Microsoft Sans Serif", 16.64F);
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
            next.Font = new System.Drawing.Font("Microsoft Sans Serif", 16.64F);
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
            Controls.Add(next);
            this.InitializeComponent();
            Controls.Remove(button1);
            Controls.Remove(tableLayoutPanel2);
            ClientSize = TempCS;
        }
        Question GetUnfinQ(TestInfo test)
        {
            Question unfinQ = null;
            if (test.wasStarted)
            {
                foreach (Question question in test.questions)
                {
                    if (question.options.Values.All<bool>(b => b == false))
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

        private void menu_Click(object sender, EventArgs e)
        {
            this.Controls.Clear();
            this.InitializeComponent();
            Controls.Remove(questionText);
            Controls.Remove(checkedListOptions);
            Controls.Remove(prev);
            Controls.Remove(next);
            Controls.Remove(checkAns);
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
                            checkedListOptions.BackColor = Color.Green;
                        }
                        else
                        {
                            checkedListOptions.BackColor = Color.Red;
                        }
                    }
                }
            if(checkedListOptions.BackColor == System.Drawing.SystemColors.Control)
            {
                checkedListOptions.BackColor = Color.Yellow;
            }
        }

        private void checkedListBox1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void label1_Click(object sender, EventArgs e)
        {

        }
    }
}
