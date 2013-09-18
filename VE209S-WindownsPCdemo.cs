using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.IO.Ports;
using System.Timers;

namespace VE209S_WindowsPC
{
    public partial class VE209S_WindownsPCdemo : Form
    {
        private SerialPort ve209s_usb_serial_port = null;
        private VE209S_WindowsPCCOMselection form_VE209S_WindowsPCCOMselection = null;
        private string rxRevData = "";
        //private static bool led1_status = false;
        private const byte ve209s_tx_repeaTimes = 3;
        private const int ve209s_tx_repeaTime = 3000;
        private byte dimmer1_status = 0x00;
        private byte dimmer1Clockwise_repeaTimes = 0;
        private byte dimmer1CounterClockwise_repeaTimes = 0;
        private System.Timers.Timer dimmer1Clockwise_timer = null;
        private System.Timers.Timer dimmer1CounterClockwise_timer = null;
        

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
            ve209s_usb_serial_port.ReadTimeout = 1000;
            ve209s_usb_serial_port.WriteTimeout = 1000;
            ve209s_usb_serial_port.ReadBufferSize = 128;
            ve209s_usb_serial_port.WriteBufferSize = 128;

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

                    dimmer1Clockwise_timer = new System.Timers.Timer(2000);
                    dimmer1Clockwise_timer.Elapsed += new ElapsedEventHandler(dimmer1Clockwise_timerEvent);
                    dimmer1Clockwise_timer.Interval = 2000;

                    dimmer1CounterClockwise_timer = new System.Timers.Timer(ve209s_tx_repeaTime);
                    dimmer1CounterClockwise_timer.Elapsed += new ElapsedEventHandler(dimmer1CounterClockwise_timerEvent);
                    dimmer1CounterClockwise_timer.Interval = ve209s_tx_repeaTime;
                    
                }
            }
        }

        private void ve209s_usb_serial_port_DataReceived(object sender, System.IO.Ports.SerialDataReceivedEventArgs e)
        {
            /*int bytes = ve209s_usb_serial_port.BytesToRead;
            byte[] rx_buffer = new byte[bytes];

            
            ve209s_usb_serial_port.Read(rx_buffer, 0, bytes);

            rxRevData = Encoding.ASCII.GetString(rx_buffer, 0, ve209s_usb_serial_port.BytesToRead);*/

            rxRevData += ve209s_usb_serial_port.ReadExisting();

            this.Invoke(new EventHandler(ve209s_usb_serial_port_revDataCtrl));
        }

        private void ve209s_usb_serial_port_revDataCtrl(object sender, EventArgs e)
        {
            dimmer1CounterClockwise_timer.Stop();
            dimmer1Clockwise_timer.Stop();

            dimmer1CounterClockwise.Enabled = true;
            dimmer1Clockwise.Enabled = true;

            dimmer1Clockwise_repeaTimes = 0;
            dimmer1CounterClockwise_repeaTimes = 0;

            switch (rxRevData)
            {
                case "ON 1\r":
                    //dimmer1CounterClockwise_timer.Stop();

                    led1.Image = VE209S_WindowsPC.Properties.Resources.light_bulb_on;

                    dimmer1_status = 0x01;
                    dimmer1.Image = VE209S_WindowsPC.Properties.Resources.tuner1;

                    //dimmer1CounterClockwise.Enabled = true; 
                    break;
                case "OFF 1\r":
                    //dimmer1CounterClockwise_timer.Stop();

                    led1.Image = VE209S_WindowsPC.Properties.Resources.light_bulb_off;

                    dimmer1_status = 0x00;
                    dimmer1.Image = VE209S_WindowsPC.Properties.Resources.tuner0;

                    //dimmer1CounterClockwise.Enabled = true;
                    break;
                case "DIM+ 1\r":
                
                    //dimmer1CounterClockwise_timer.Stop();

                    switch (dimmer1_status)
                    {
                        case 0x01:
                            dimmer1_status = 0x02;
                            //dimmer1.Image = VE209S_WindowsPC.Properties.Resources.tuner2;
                            break;
                        case 0x02:
                            dimmer1_status = 0x03;
                            //dimmer1.Image = VE209S_WindowsPC.Properties.Resources.tuner3;
                            break;
                        case 0x03:
                            dimmer1_status = 0x04;
                            //dimmer1.Image = VE209S_WindowsPC.Properties.Resources.tuner4;
                            break;
                        default:
                            break;
                    }

                    //dimmer1CounterClockwise.Enabled = true;
                    break;
                case "DIM- 1\r":
                    //dimmer1CounterClockwise_timer.Stop();

                    switch (dimmer1_status)
                    {
                        case 0x02:
                            dimmer1_status = 0x01;
                            //dimmer1.Image = VE209S_WindowsPC.Properties.Resources.tuner1;
                            break;
                        case 0x03:
                            dimmer1_status = 0x02;
                            //dimmer1.Image = VE209S_WindowsPC.Properties.Resources.tuner2;
                            break;
                        case 0x04:
                            dimmer1_status = 0x03;
                            //dimmer1.Image = VE209S_WindowsPC.Properties.Resources.tuner3;
                            break;
                        default:
                            break;
                    }

                    //dimmer1CounterClockwise.Enabled = true;
                    break;
                default:
                    break;
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

        private void dimmer1Clockwise_timerEvent(object source, ElapsedEventArgs e)
        {
            if (++dimmer1Clockwise_repeaTimes < ve209s_tx_repeaTimes)
            {
                this.Invoke(new EventHandler(dimmer1Clockwise_repeatTX));
            }
            else
            {
                this.Invoke(new EventHandler(dimmer1Clockwise_failTX));
            }
        }

        private void dimmer1CounterClockwise_timerEvent(object source, ElapsedEventArgs e)
        {
            if (++dimmer1CounterClockwise_repeaTimes < ve209s_tx_repeaTimes)
            {
                this.Invoke(new EventHandler(dimmer1CounterClockwise_repeatTX));
            } 
            else
            {
                this.Invoke(new EventHandler(dimmer1CounterClockwise_failTX));
            }
        }

        private void dimmer1CounterClockwise_repeatTX(object sender, EventArgs e)
        {
            switch (dimmer1_status)
            {
                case 0x00:
                    ve209s_usb_serial_port_tx("ON 1");
                    break;
                case 0x01:
                case 0x02:
                case 0x03:
                    ve209s_usb_serial_port_tx("DIM+ 1");
                    break;
                default:
                    break;
            }
        }

        private void dimmer1Clockwise_repeatTX(object sender, EventArgs e)
        {
            switch (dimmer1_status)
            {
                case 0x01:
                    ve209s_usb_serial_port_tx("OFF 1");
                    break;
                case 0x02:
                case 0x03:
                case 0x04:
                    ve209s_usb_serial_port_tx("DIM- 1");
                    break;
                default:
                    break;
            }
        }

        private void dimmer1CounterClockwise_failTX(object sender, EventArgs e)
        {
            dimmer1CounterClockwise_timer.Stop();

            dimmer1CounterClockwise_repeaTimes = 0;

            dimmer1CounterClockwise.Enabled = true;

            switch (dimmer1_status)
            {
                case 0x00:
                    dimmer1.Image = VE209S_WindowsPC.Properties.Resources.tuner0;
                    break;
                case 0x01:
                    dimmer1.Image = VE209S_WindowsPC.Properties.Resources.tuner1;
                    break;
                case 0x02:
                    dimmer1.Image = VE209S_WindowsPC.Properties.Resources.tuner2;
                    break;
                case 0x03:
                    dimmer1.Image = VE209S_WindowsPC.Properties.Resources.tuner3;
                    break;
                case 0x04:
                    dimmer1.Image = VE209S_WindowsPC.Properties.Resources.tuner4;
                    break;
                default:
                    break;
            }
        }

        private void dimmer1Clockwise_failTX(object sender, EventArgs e)
        {
            dimmer1Clockwise_timer.Stop();

            dimmer1Clockwise_repeaTimes = 0;

            dimmer1Clockwise.Enabled = true;

            switch (dimmer1_status)
            {
                case 0x00:
                    dimmer1.Image = VE209S_WindowsPC.Properties.Resources.tuner0;
                    break;
                case 0x01:
                    dimmer1.Image = VE209S_WindowsPC.Properties.Resources.tuner1;
                    break;
                case 0x02:
                    dimmer1.Image = VE209S_WindowsPC.Properties.Resources.tuner2;
                    break;
                case 0x03:
                    dimmer1.Image = VE209S_WindowsPC.Properties.Resources.tuner3;
                    break;
                case 0x04:
                    dimmer1.Image = VE209S_WindowsPC.Properties.Resources.tuner4;
                    break;
                default:
                    break;
            }
        }

        private void dimmer1CounterClockwise_Click(object sender, EventArgs e)
        {
            rxRevData = "";

            ve209s_usb_serial_port.DiscardInBuffer();

            dimmer1CounterClockwise_repeaTimes = 0;

            switch (dimmer1_status)
            {
                case 0x00:
                    dimmer1.Image = VE209S_WindowsPC.Properties.Resources.tuner1;

                    ve209s_usb_serial_port_tx("ON 1");

                    //dimmer1_status = 0x01;

                    dimmer1CounterClockwise_timer.Start();

                    dimmer1CounterClockwise.Enabled = false;
                    break;
                case 0x01:
                    dimmer1.Image = VE209S_WindowsPC.Properties.Resources.tuner2;

                    ve209s_usb_serial_port_tx("DIM+ 1");

                    //dimmer1_status = 0x02;

                    dimmer1CounterClockwise_timer.Start();

                    dimmer1CounterClockwise.Enabled = false;
                    break;
                case 0x02:
                    dimmer1.Image = VE209S_WindowsPC.Properties.Resources.tuner3;

                    ve209s_usb_serial_port_tx("DIM+ 1");

                    //dimmer1_status = 0x02;

                    dimmer1CounterClockwise_timer.Start();

                    dimmer1CounterClockwise.Enabled = false;
                    break;
                case 0x03:
                    dimmer1.Image = VE209S_WindowsPC.Properties.Resources.tuner4;

                    ve209s_usb_serial_port_tx("DIM+ 1");

                    //dimmer1_status = 0x02;

                    dimmer1CounterClockwise_timer.Start();

                    dimmer1CounterClockwise.Enabled = false;
                    break;
                default:
                    break;
            }
        }

        private void dimmer1Clockwise_Click(object sender, EventArgs e)
        {
            rxRevData = "";

            dimmer1Clockwise_repeaTimes = 0;

            switch (dimmer1_status)
            {
                case 0x01:
                    dimmer1.Image = VE209S_WindowsPC.Properties.Resources.tuner0;

                    ve209s_usb_serial_port_tx("OFF 1");

                    //dimmer1_status = 0x00;

                    dimmer1Clockwise_timer.Start();

                    dimmer1Clockwise.Enabled = false;
                    break;
                case 0x02:
                    dimmer1.Image = VE209S_WindowsPC.Properties.Resources.tuner1;

                    ve209s_usb_serial_port_tx("DIM- 1");

                    //dimmer1_status = 0x01;

                    dimmer1Clockwise_timer.Start();

                    dimmer1Clockwise.Enabled = false;
                    break;
                case 0x03:
                    dimmer1.Image = VE209S_WindowsPC.Properties.Resources.tuner2;

                    ve209s_usb_serial_port_tx("DIM- 1");

                    //dimmer1_status = 0x02;

                    dimmer1Clockwise_timer.Start();

                    dimmer1Clockwise.Enabled = false;
                    break;
                case 0x04:
                    dimmer1.Image = VE209S_WindowsPC.Properties.Resources.tuner3;

                    ve209s_usb_serial_port_tx("DIM- 1");

                    //dimmer1_status = 0x03;

                    dimmer1Clockwise_timer.Start();

                    dimmer1Clockwise.Enabled = false;
                    break;
                default:
                    break;
            }
        }
    }
}