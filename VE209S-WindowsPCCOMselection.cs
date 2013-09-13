using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.IO.Ports;

namespace VE209S_WindowsPC
{
    public partial class VE209S_WindowsPCCOMselection : Form
    {
        public VE209S_WindowsPCCOMselection()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            
        }

        private void buttonDemo_Click(object sender, EventArgs e)
        {
            VE209S_WindownsPCdemo form_ve209s_windowsPCdemo = new VE209S_WindownsPCdemo(this);
            form_ve209s_windowsPCdemo.Show();

            this.Visible = false;
        }

        public string Get_ve209s_usb_serial_port_name()
        {
            return comboBoxCOMPortSelection.Text;
        }
    }
}