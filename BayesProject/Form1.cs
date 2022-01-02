using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace BayesProject
{
    public partial class Form1 : Form
    {
        public BayesNetwork bayesNetwork;
        public List<RadioButton> checks;
        public List<ComboBox> comboBoxes;
        public Form1()
        {
            InitializeComponent();
            checks = new List<RadioButton>();
            comboBoxes = new List<ComboBox>();
            bayesNetwork = new BayesNetwork(@"test.txt");
        }

        int k = 1;
        public TextBox AddTextBox(string NodeID)
        {
            TextBox txt = new TextBox();
            txt.Text = NodeID;
            txt.Font = new Font("Modern No. 20", 10, FontStyle.Bold);
            txt.Size = new Size(200, 300);
            txt.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;

            ComboBox comboBox = new ComboBox();
            comboBox.Font = new Font("Modern No. 20", 10, FontStyle.Bold);
            comboBox.Items.Add("Yes");
            comboBox.Items.Add("No");
            comboBox.Items.Add("NotPresent");

            RadioButton radioButton = new RadioButton();

            this.Controls.Add(txt);
            this.Controls.Add(comboBox);
            this.Controls.Add(radioButton);

            radioButton.Enabled = false;
            checks.Add(radioButton);
            radioButton.Top = k * 45;
            radioButton.Left = 420;

            comboBox.Top = k * 45;
            comboBox.Left = 250;
            comboBox.Enabled = false;
            comboBoxes.Add(comboBox);

            txt.Top = k * 45;
            txt.Left = 30;
            k++;
            return txt;
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            this.FormBorderStyle = FormBorderStyle.FixedSingle;
            CenterToScreen();
            foreach (var i in bayesNetwork.getNetworkGraph.GetNodes)
                AddTextBox(i.NodeID + " -> Parents [" + i.ParentsToString() + "]");
        }

        private void button1_Click(object sender, EventArgs e)
        {
            bool checkRadioButton = checks.Any(item => item.Checked == true);
            int indexChecked = checks.FindIndex(index => index.Checked == true);
            if (checkRadioButton)
            {
                // Functie de interogare
                bayesNetwork.getNetworkGraph.GetNodes[indexChecked].Evidence = (TypeOfEvidence)Enum.Parse(typeof(TypeOfEvidence), comboBoxes[indexChecked].Text);
                RichTextBox richTextBox = new RichTextBox();
                richTextBox.Size = new Size(700, 100);
                richTextBox.Left = 15;
                richTextBox.Top = 280;
                this.Controls.Add(richTextBox);
                richTextBox.Font = new Font("Modern No. 20", 18, FontStyle.Bold);
                richTextBox.ForeColor = Color.Red;
                richTextBox.AppendText("The node {" + bayesNetwork.getNetworkGraph.GetNodes[indexChecked].NodeID + "} was queried with evidence {" + bayesNetwork.getNetworkGraph.GetNodes[indexChecked].Evidence + "}.");

                Console.WriteLine("The node {" + bayesNetwork.getNetworkGraph.GetNodes[indexChecked].NodeID + "} was queried with evidence {" + bayesNetwork.getNetworkGraph.GetNodes[indexChecked].Evidence + "}.");

                InferenceByEnumeration inf = new InferenceByEnumeration(bayesNetwork);
                inf.EnumerationAsk(bayesNetwork.getNetworkGraph.GetNodes[indexChecked].NodeID);
            }
            else
                MessageBox.Show("You didn't select any node! You must select one for querying!", "Warning");
        }

        private void button2_Click(object sender, EventArgs e)
        {
            checks.ForEach(action => action.Enabled = true);
            comboBoxes.ForEach(action => action.Enabled = true);
            comboBoxes.ForEach(item => item.Text = "NotPresent");
            button1.Enabled = true;
        }
    }
}
