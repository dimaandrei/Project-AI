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
        public System.Windows.Forms.TextBox AddTextBox(string NodeID)
        {
            TextBox txt = new TextBox();
            txt.Text = NodeID;
            txt.Size = new Size(200, 300);
            ComboBox comboBox = new ComboBox();
            comboBox.Items.Add("Yes");
            comboBox.Items.Add("No");
            comboBox.Items.Add("Not present");
            RadioButton radioButton = new RadioButton();
            this.Controls.Add(txt);
            this.Controls.Add(comboBox);
            this.Controls.Add(radioButton);
            radioButton.Enabled = false;
            checks.Add(radioButton);
            radioButton.Top = k * 35;
            radioButton.Left = 420;
            comboBox.Top = k * 35;
            comboBox.Left = 250;
            comboBox.Enabled = false;
            comboBoxes.Add(comboBox);
            txt.Top = k * 35;
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

                Console.WriteLine("S-a interogat nodul " + bayesNetwork.getNetworkGraph.GetNodes[indexChecked].NodeID + " cu evidenta " + bayesNetwork.getNetworkGraph.GetNodes[indexChecked].Evidence);
            }
            else
                throw new Exception("Trebuie selectat nodul ce se doreste a fi interogat si evidenta acestuia!");
        }

        private void button2_Click(object sender, EventArgs e)
        {
            checks.ForEach(action => action.Enabled = true);
            comboBoxes.ForEach(action => action.Enabled = true);
            comboBoxes.ForEach(item => item.Text = "Not present");
            button1.Enabled = true;
        }
    }
}
