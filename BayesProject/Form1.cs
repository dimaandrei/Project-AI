using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.IO;
using System.Windows.Forms;

namespace BayesProject
{
    public partial class Form1 : Form
    {
        private BayesNetwork _bayesNetwork;
        private List<RadioButton> _checks;
        private List<ComboBox> _comboBoxes;
        private List<TextBox> _textBoxes;
        private RichTextBox _richTextBox;
        private int _incrementForAlign;
        private const int DEFAULT_INCREMENT = 2;

        /// <summary>
        /// Constructor of class
        /// </summary>
        public Form1()
        {
            InitializeComponent();
            _checks = new List<RadioButton>();
            _comboBoxes = new List<ComboBox>();
            _textBoxes = new List<TextBox>();
            _richTextBox = new RichTextBox();
            _incrementForAlign = DEFAULT_INCREMENT;
        }

        /// <summary>
        /// Function which add the data of each node
        /// </summary>
        /// <param name="nodeID">The name of node</param>
        /// <returns>Returns a textbox</returns>
        public TextBox AddNodeField(string nodeID, List<String> domain)
        {
            // Add a textBox for each node
            TextBox textBox = new TextBox();
            textBox.Text = nodeID;
            textBox.Font = new Font("Modern No. 20", 10, FontStyle.Bold);
            textBox.Size = new Size(280, 300);
            textBox.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;

            // Add a comboBox for each node. The items should be {Yes, No, NotPresent}
            ComboBox comboBox = new ComboBox();
            comboBox.Font = new Font("Modern No. 20", 10, FontStyle.Bold);
            foreach (var i in domain)
            {
                comboBox.Items.Add(i);
            }
            comboBox.Items.Add(Node.NOT_PRESENT);

            // Add a radioButton for each node
            RadioButton radioButton = new RadioButton();

            // Add the objects to Form
            Controls.Add(textBox);
            Controls.Add(comboBox);
            Controls.Add(radioButton);

            // The radioButton and the comboBox should be disabled until you want to select a node to querry
            radioButton.Enabled = false;
            comboBox.Enabled = false;

            // Add the radioButton to an array
            _checks.Add(radioButton);

            // Align radioButton
            radioButton.Top = _incrementForAlign * 45;
            radioButton.Left = 520;

            // Align comboBox
            comboBox.Top = _incrementForAlign * 45;
            comboBox.Left = 350;

            // Add the comboBox to an array
            _comboBoxes.Add(comboBox);

            // Align textBox
            textBox.Top = _incrementForAlign * 45;
            textBox.Left = 30;

            // Increment for parallel align
            _incrementForAlign++;

            _textBoxes.Add(textBox);

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
            FormBorderStyle = FormBorderStyle.FixedSingle;
            // Center the form in the middle of screen
            CenterToScreen();
        }

        /// <summary>
        /// Function for querry button
        /// </summary>
        /// <param name="sender">The sender</param>
        /// <param name="e">The event</param>
        private void button1_Click(object sender, EventArgs e)
        {
            // Clear richTextBox before querry the node
            _richTextBox.Clear();
            _richTextBox.Focus();
            _richTextBox.Show();

            // Check if any radioButton was checked
            bool checkRadioButton = _checks.Any(item => item.Checked == true);

            // Find the index of the querried node
            int indexChecked = _checks.FindIndex(index => index.Checked == true);

            // Calculate the probabilities if a radioButton is checked
            if (checkRadioButton)
            {
                // Querry function

                // Align and design of richTextBox to append the results
                _richTextBox.Size = new Size(700, 100);
                _richTextBox.Left = 15;
                _richTextBox.Top = _incrementForAlign * 49;
                _richTextBox.Font = new Font("Modern No. 20", 10, FontStyle.Bold);
                _richTextBox.ForeColor = Color.Red;

                // Add the richTextBox to the Form
                Controls.Add(_richTextBox);

                // Set the evidence of each node
                for (var i = 0; i < _bayesNetwork.NetworkGraph.Nodes.Count; ++i)
                {
                    _bayesNetwork.NetworkGraph.Nodes[i].Evidence = _comboBoxes[i].Text;

                    // Append to the richTextBox the evidence of each node
                    _richTextBox.AppendText("The node {" + _bayesNetwork.NetworkGraph.Nodes[i].NodeID + "} was set with evidence {" + _bayesNetwork.NetworkGraph.Nodes[i].Evidence + "}." + System.Environment.NewLine);
                    // Display the evidence of each node
                    Console.WriteLine("The node {" + _bayesNetwork.NetworkGraph.Nodes[i].NodeID + "} was set with evidence {" + _bayesNetwork.NetworkGraph.Nodes[i].Evidence + "}.");
                }

                // Append to the richTextBox the evidence of selected node
                _richTextBox.AppendText("The node {" + _bayesNetwork.NetworkGraph.Nodes[indexChecked].NodeID + "} was queried with evidence {" + _bayesNetwork.NetworkGraph.Nodes[indexChecked].Evidence + "}.");
                // Display the evidence of the selected node
                Console.WriteLine("The node {" + _bayesNetwork.NetworkGraph.Nodes[indexChecked].NodeID + "} was queried with evidence {" + _bayesNetwork.NetworkGraph.Nodes[indexChecked].Evidence + "}.");

                InferenceByEnumeration inf = new InferenceByEnumeration(_bayesNetwork);
                var probabilities = inf.EnumerationAsk(_bayesNetwork.NetworkGraph.Nodes[indexChecked].NodeID);
                //Console.WriteLine("Prob: " + prob[0] + " " + prob[1]);

                var message = "";
                var evidences = _bayesNetwork.NetworkGraph.GetNode(_bayesNetwork.NetworkGraph.Nodes[indexChecked].NodeID).GetEvidenceDomain();

                for (int i = 0; i < probabilities.Length; i++)
                {
                    message += "P(" + evidences[i] + ") = " + probabilities[i] + Environment.NewLine;
                }

                MessageBox.Show(message, "Query Results");
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
            if (_bayesNetwork != null)
            {
                // Enable the radioButton and comboBox for each node when press the select button
                _checks.ForEach(action => action.Enabled = true);
                _comboBoxes.ForEach(action => action.Enabled = true);

                // Set the default value of comboBox to NotPresent
                _comboBoxes.ForEach(item => item.Text = "NotPresent");

                // Enable the querry button
                button1.Enabled = true;
            }
            else
            {
                MessageBox.Show("Load a graph first!", "Error");
            }
        }

        private void loadNewGraphToolStripMenuItem_Click(object sender, EventArgs e)
        {
            openFileDialog.Filter = "(*.txt)|*.txt";
            openFileDialog.FilterIndex = 1;
            openFileDialog.RestoreDirectory = true;

            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                _bayesNetwork = new BayesNetwork(openFileDialog.FileName);

                foreach (var textBox in _textBoxes)
                    Controls.Remove(textBox);

                foreach (var check in _checks)
                    Controls.Remove(check);

                foreach (var comboBox in _comboBoxes)
                    Controls.Remove(comboBox);

                _comboBoxes.Clear();
                _checks.Clear();
                _textBoxes.Clear();

                _incrementForAlign = DEFAULT_INCREMENT;

                // Add a field for each node
                foreach (var i in _bayesNetwork.NetworkGraph.Nodes)
                    AddNodeField(i.NodeID + " -> Parents [" + i.ParentsToString() + "]", i.GetEvidenceDomain());

                _richTextBox.Clear();
                _richTextBox.Hide();
            }
        }

        private void quitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void helpToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Nodes: 5\n\n[Yes, No]\nFlu: { }\n0.1 0.9\n\n[Yes, No]\nAbscess: { }\n0.05 0.95\n\n[Yes, No]\nFever: { Flu, Abscess}\nYes Yes 0.8 0.2\nYes No 0.7 0.3\nNo Yes 0.25 0.75\nNo No 0.05 0.95\n\n[Yes, No]\nFatigue: { Fever}\nYes 0.6 0.4\nNo 0.2 0.8\n\n[Yes, No]\nAnorexia: { Fever}\nYes 0.5 0.5\nNo 0.1 0.9", "Exemplu de descriere a unei rețele bayesiene");
        }
    }
}
