﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace BayesProject
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            BayesNetwork bayesNetwork = new BayesNetwork(@"test.txt");
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Form1());
        }
    }
}
