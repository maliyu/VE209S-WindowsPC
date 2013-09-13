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
    public partial class VE209S_WindownsPCdemo : Form
    {
        private SerialPort ve209s_usb_serial_port = null;
        private VE209S_WindowsPCCOMselection form_VE209S_WindowsPCCOMselection = null;
        private bool led1_status = false;

        public VE209S_WindownsPCdemo()
        {
            InitializeComponent();
        }

        public VE209S_WindownsPCdemo(object parrent_form)
        {
            InitializeComponent();

            form_VE209S_WindowsPCCOMselection = parrent_form as VE209S_WindowsPCCOMselection;
        }

        private void VE209S_WindownsPCdemo_Load(object sender, EventArgs e)
        {
            if (ve209s_usb_serial_port == null)
            {
                ve209s_usb_serial_port = new SerialPort();
            }
            else
            {
                ve209s_usb_serial_port.Close();
            }

            // Port name can be identified by checking the ports
            // section in Device Manager after connecting your device
            ve209s_usb_serial_port.PortName = form_VE209S_WindowsPCCOMselection.Get_ve209s_usb_serial_port_name();
            ve209s_usb_serial_port.BaudRate = 9600;
            ve209s_usb_serial_port.DataBits = 8;
            ve209s_usb_serial_port.Parity = Parity.None;
            ve209s_usb_serial_port.StopBits = StopBits.One;
            ve209s_usb_serial_port.Handshake = Handshake.None;
            ve209s_usb_serial_port.ReadTimeout = 500;
            ve209s_usb_serial_port.WriteTimeout = 500;

            if (ve209s_usb_serial_port.IsOpen)
            {
                MessageBox.Show("COM Port is occupied!");

                this.Dispose();
            }
            else
            {
                try
                {
                    ve209s_usb_serial_port.Open();
                }
                catch (System.Exception ex)
                {
                    MessageBox.Show(ex.Message);
                    this.Dispose();
                    form_VE209S_WindowsPCCOMselection.Dispose();
                }
                finally
                {
                    ve209s_usb_serial_port.DataReceived += new SerialDataReceivedEventHandler(ve209s_usb_serial_port_DataReceived);
                }
            }
        }

        private void ve209s_usb_serial_port_DataReceived(object sender, System.IO.Ports.SerialDataReceivedEventArgs e)
        {
            string rx_buffer = ve209s_usb_serial_port.ReadExisting();

            switch (rx_buffer)
            {
                case "ON 1\r":
                    led1.Image = VE209S_WindowsPC.Properties.Resources.light_bulb_on;
                    break;
                case "OFF 1\r":
                    led1.Image = VE209S_WindowsPC.Properties.Resources.light_bulb_off;
                    break;
                default:
                    break;
            }
        }

        private void switch1_Click(object sender, EventArgs e)
        {
            if (led1_status)
            {
                // led 1 is on now, so click is to turn it off
                switch1.Image = VE209S_WindowsPC.Properties.Resources.Light_Switch_Off_clip_art_medium;

                ve209s_usb_serial_port_tx("OFF 1");

                led1_status = false;
            } 
            else
            {
                // led 1 is off now, so click is to turn it on
                switch1.Image = VE209S_WindowsPC.Properties.Resources.Light_Switch_On_clip_art_medium;
                ve209s_usb_serial_port_tx("ON 1");

                led1_status = true;
            }
        }

        private void VE209S_WindownsPCdemo_FormClosing(object sender, FormClosingEventArgs e)
        {
            form_VE209S_WindowsPCCOMselection.Dispose();
        }

        private void ve209s_usb_serial_port_tx(string tx_data)
        {
            byte[] tx_buff = new byte[5+tx_data.Length];
            tx_buff[0] = 0x02;  // STX
            tx_buff[1] = 0x00;  // high byte of package size
            tx_buff[2] = (byte)(tx_data.Length);    // low byte of package size
            tx_buff[3 + tx_data.Length] = 0x03;   // ETX
            tx_buff[4 + tx_data.Length] = 0x0A;   // new line
            byte[] ascii_data = Encoding.ASCII.GetBytes(tx_data); // ascii code of tx data
            int i = 3;
            foreach (byte ascii_b in ascii_data)
            {
                tx_buff.SetValue(ascii_b, i++);
            }

            ve209s_usb_serial_port.Write(tx_buff, 0, tx_buff.Length);
        }
    }
}