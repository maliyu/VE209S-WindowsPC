using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace VE209S_WindowsPC
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new VE209S_WindowsPCCOMselection());
        }
    }
}