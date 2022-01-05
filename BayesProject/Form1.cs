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
        RichTextBox richTextBox = new RichTextBox();
        public int incrementForAlign = 1;

        /// <summary>
        /// Constructor of class
        /// </summary>
        public Form1()
        {
            InitializeComponent();
            checks = new List<RadioButton>();
            comboBoxes = new List<ComboBox>();
            bayesNetwork = new BayesNetwork(@"test.txt");
        }

        /// <summary>
        /// Function which add the data of each node
        /// </summary>
        /// <param name="NodeID">The name of node</param>
        /// <returns>Returns a textbox</returns>
        public TextBox AddNodeField(string NodeID)
        {
            // Add a textBox for each node
            TextBox textBox = new TextBox();
            textBox.Text = NodeID;
            textBox.Font = new Font("Modern No. 20", 10, FontStyle.Bold);
            textBox.Size = new Size(200, 300);
            textBox.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;

            // Add a comboBox for each node. The items should be {Yes, No, NotPresent}
            ComboBox comboBox = new ComboBox();
            comboBox.Font = new Font("Modern No. 20", 10, FontStyle.Bold);
            foreach(var i in Enum.GetValues(typeof(TypeOfEvidence)))
            {
                comboBox.Items.Add(i);
            }

            // Add a radioButton for each node
            RadioButton radioButton = new RadioButton();

            // Add the objects to Form
            this.Controls.Add(textBox);
            this.Controls.Add(comboBox);
            this.Controls.Add(radioButton);

            // The radioButton and the comboBox should be disabled until you want to select a node to querry
            radioButton.Enabled = false;
            comboBox.Enabled = false;

            // Add the radioButton to an array
            checks.Add(radioButton);

            // Align radioButton
            radioButton.Top = incrementForAlign * 45;
            radioButton.Left = 420;

            // Align comboBox
            comboBox.Top = incrementForAlign * 45;
            comboBox.Left = 250;

            // Add the comboBox to an array
            comboBoxes.Add(comboBox);

            // Align textBox
            textBox.Top = incrementForAlign * 45;
            textBox.Left = 30;

            // Increment for parallel align
            incrementForAlign++;

            return textBox;
        }

        /// <summary>
        /// Function which load the form
        /// </summary>
        /// <param name="sender">The sender</param>
        /// <param name="e">The event</param>
        private void Form1_Load(object sender, EventArgs e)
        {
            // Fix the style of FormBorder
            this.FormBorderStyle = FormBorderStyle.FixedSingle;
            // Center the form in the middle of screen
            CenterToScreen();
            // Add a field for each node
            foreach (var i in bayesNetwork.getNetworkGraph.GetNodes)
                AddNodeField(i.NodeID + " -> Parents [" + i.ParentsToString() + "]");
        }

        /// <summary>
        /// Function for querry button
        /// </summary>
        /// <param name="sender">The sender</param>
        /// <param name="e">The event</param>
        private void button1_Click(object sender, EventArgs e)
        {
            // Clear richTextBox before querry the node
            richTextBox.Clear();
            richTextBox.Focus();

            // Check if any radioButton was checked
            bool checkRadioButton = checks.Any(item => item.Checked == true);

            // Find the index of the querried node
            int indexChecked = checks.FindIndex(index => index.Checked == true);

            // Calculate the probabilities if a radioButton is checked
            if (checkRadioButton)
            {
                // Querry function

                // Align and design of richTextBox to append the results
                richTextBox.Size = new Size(700, 100);
                richTextBox.Left = 15;
                richTextBox.Top = 280;
                richTextBox.Font = new Font("Modern No. 20", 10, FontStyle.Bold);
                richTextBox.ForeColor = Color.Red;

                // Add the richTextBox to the Form
                this.Controls.Add(richTextBox);

                // Set the evidence of each node
                for (var i = 0; i < bayesNetwork.getNetworkGraph.GetNodes.Count; ++i)
                {
                    bayesNetwork.getNetworkGraph.GetNodes[i].Evidence = (TypeOfEvidence)Enum.Parse(typeof(TypeOfEvidence), comboBoxes[i].Text);

                    // Append to the richTextBox the evidence of each node
                    richTextBox.AppendText("The node {" + bayesNetwork.getNetworkGraph.GetNodes[i].NodeID + "} was set with evidence {" + bayesNetwork.getNetworkGraph.GetNodes[i].Evidence + "}." + System.Environment.NewLine);
                    // Display the evidence of each node
                    Console.WriteLine("The node {" + bayesNetwork.getNetworkGraph.GetNodes[i].NodeID + "} was set with evidence {" + bayesNetwork.getNetworkGraph.GetNodes[i].Evidence + "}.");
                }

                // Append to the richTextBox the evidence of selected node
                richTextBox.AppendText("The node {" + bayesNetwork.getNetworkGraph.GetNodes[indexChecked].NodeID + "} was queried with evidence {" + bayesNetwork.getNetworkGraph.GetNodes[indexChecked].Evidence + "}.");
                // Display the evidence of the selected node
                Console.WriteLine("The node {" + bayesNetwork.getNetworkGraph.GetNodes[indexChecked].NodeID + "} was queried with evidence {" + bayesNetwork.getNetworkGraph.GetNodes[indexChecked].Evidence + "}.");

                InferenceByEnumeration inf = new InferenceByEnumeration(bayesNetwork);
                var prob = inf.EnumerationAsk(bayesNetwork.getNetworkGraph.GetNodes[indexChecked].NodeID);
                Console.WriteLine("Prob: " + prob[0] + " " + prob[1]);
            }
            else
            {
                // Otherwise show a MessageBox like a warning
                MessageBox.Show("You didn't select any node! You must select one for querying!", "Warning");
            }
        }

        /// <summary>
        /// Function for select button
        /// </summary>
        /// <param name="sender">The sender</param>
        /// <param name="e">The event</param>
        private void button2_Click(object sender, EventArgs e)
        {
            // Enable the radioButton and comboBox for each node when press the select button
            checks.ForEach(action => action.Enabled = true);
            comboBoxes.ForEach(action => action.Enabled = true);

            // Set the default value of comboBox to NotPresent
            comboBoxes.ForEach(item => item.Text = "NotPresent");

            // Enable the querry button
            button1.Enabled = true;
        }
    }
}
