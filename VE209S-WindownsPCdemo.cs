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
        //private string dimmer2RxRevData = "";
        //private string dimmer3RxRevData = "";
        //private string dimmer4RxRevData = "";
        //private string dimmer5RxRevData = "";
        //private static bool led1_status = false;
        private const byte ve209s_tx_repeaTimes = 3;
        private const int ve209s_tx_repeaTime = 3000;
        private byte dimmer1_status = 0x00;
        private byte dimmer2_status = 0x00;
        private byte dimmer3_status = 0x00;
        private byte dimmer4_status = 0x00;
        private byte dimmer5_status = 0x00;
        private byte dimmer1Clockwise_repeaTimes = 0;
        private byte dimmer2Clockwise_repeaTimes = 0;
        private byte dimmer3Clockwise_repeaTimes = 0;
        private byte dimmer4Clockwise_repeaTimes = 0;
        private byte dimmer5Clockwise_repeaTimes = 0;
        private byte dimmer1CounterClockwise_repeaTimes = 0;
        private byte dimmer2CounterClockwise_repeaTimes = 0;
        private byte dimmer3CounterClockwise_repeaTimes = 0;
        private byte dimmer4CounterClockwise_repeaTimes = 0;
        private byte dimmer5CounterClockwise_repeaTimes = 0;
        private System.Timers.Timer dimmer1Clockwise_timer = null;
        private System.Timers.Timer dimmer2Clockwise_timer = null;
        private System.Timers.Timer dimmer3Clockwise_timer = null;
        private System.Timers.Timer dimmer4Clockwise_timer = null;
        private System.Timers.Timer dimmer5Clockwise_timer = null;
        private System.Timers.Timer dimmer1CounterClockwise_timer = null;
        private System.Timers.Timer dimmer2CounterClockwise_timer = null;
        private System.Timers.Timer dimmer3CounterClockwise_timer = null;
        private System.Timers.Timer dimmer4CounterClockwise_timer = null;
        private System.Timers.Timer dimmer5CounterClockwise_timer = null;       

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
                    dimmer2Clockwise_timer = new System.Timers.Timer(2000);
                    dimmer2Clockwise_timer.Elapsed += new ElapsedEventHandler(dimmer2Clockwise_timerEvent);
                    dimmer2Clockwise_timer.Interval = 2000;
                    dimmer3Clockwise_timer = new System.Timers.Timer(2000);
                    dimmer3Clockwise_timer.Elapsed += new ElapsedEventHandler(dimmer3Clockwise_timerEvent);
                    dimmer3Clockwise_timer.Interval = 2000;
                    dimmer4Clockwise_timer = new System.Timers.Timer(2000);
                    dimmer4Clockwise_timer.Elapsed += new ElapsedEventHandler(dimmer4Clockwise_timerEvent);
                    dimmer4Clockwise_timer.Interval = 2000;
                    dimmer5Clockwise_timer = new System.Timers.Timer(2000);
                    dimmer5Clockwise_timer.Elapsed += new ElapsedEventHandler(dimmer5Clockwise_timerEvent);
                    dimmer5Clockwise_timer.Interval = 2000;

                    dimmer1CounterClockwise_timer = new System.Timers.Timer(ve209s_tx_repeaTime);
                    dimmer1CounterClockwise_timer.Elapsed += new ElapsedEventHandler(dimmer1CounterClockwise_timerEvent);
                    dimmer1CounterClockwise_timer.Interval = ve209s_tx_repeaTime;
                    dimmer2CounterClockwise_timer = new System.Timers.Timer(ve209s_tx_repeaTime);
                    dimmer2CounterClockwise_timer.Elapsed += new ElapsedEventHandler(dimmer2CounterClockwise_timerEvent);
                    dimmer2CounterClockwise_timer.Interval = ve209s_tx_repeaTime;
                    dimmer3CounterClockwise_timer = new System.Timers.Timer(ve209s_tx_repeaTime);
                    dimmer3CounterClockwise_timer.Elapsed += new ElapsedEventHandler(dimmer3CounterClockwise_timerEvent);
                    dimmer3CounterClockwise_timer.Interval = ve209s_tx_repeaTime;
                    dimmer4CounterClockwise_timer = new System.Timers.Timer(ve209s_tx_repeaTime);
                    dimmer4CounterClockwise_timer.Elapsed += new ElapsedEventHandler(dimmer4CounterClockwise_timerEvent);
                    dimmer4CounterClockwise_timer.Interval = ve209s_tx_repeaTime;
                    dimmer5CounterClockwise_timer = new System.Timers.Timer(ve209s_tx_repeaTime);
                    dimmer5CounterClockwise_timer.Elapsed += new ElapsedEventHandler(dimmer5CounterClockwise_timerEvent);
                    dimmer5CounterClockwise_timer.Interval = ve209s_tx_repeaTime;
                    
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
            textBox1.Text = rxRevData;
            textBox2.Text = dimmer1CounterClockwise_repeaTimes.ToString();
            textBox3.Text = dimmer1Clockwise_repeaTimes.ToString();

            dimmer1CounterClockwise_timer.Stop();
            dimmer2CounterClockwise_timer.Stop();
            dimmer3CounterClockwise_timer.Stop();
            dimmer4CounterClockwise_timer.Stop();
            dimmer5CounterClockwise_timer.Stop();
            dimmer1Clockwise_timer.Stop();
            dimmer2Clockwise_timer.Stop();
            dimmer3Clockwise_timer.Stop();
            dimmer4Clockwise_timer.Stop();
            dimmer5Clockwise_timer.Stop();

            dimmer1CounterClockwise.Enabled = true;
            dimmer2CounterClockwise.Enabled = true;
            dimmer3CounterClockwise.Enabled = true;
            dimmer4CounterClockwise.Enabled = true;
            dimmer5CounterClockwise.Enabled = true;
            dimmer1Clockwise.Enabled = true;
            dimmer2Clockwise.Enabled = true;
            dimmer3Clockwise.Enabled = true;
            dimmer4Clockwise.Enabled = true;
            dimmer5Clockwise.Enabled = true;

            dimmer1Clockwise_repeaTimes = 0;
            dimmer2Clockwise_repeaTimes = 0;
            dimmer3Clockwise_repeaTimes = 0;
            dimmer4Clockwise_repeaTimes = 0;
            dimmer5Clockwise_repeaTimes = 0;
            dimmer1CounterClockwise_repeaTimes = 0;
            dimmer2CounterClockwise_repeaTimes = 0;
            dimmer3CounterClockwise_repeaTimes = 0;
            dimmer4CounterClockwise_repeaTimes = 0;
            dimmer5CounterClockwise_repeaTimes = 0;

            switch (rxRevData)
            {
                case "ON 1\r":
                    rxRevData = "";
                    //dimmer1CounterClockwise_timer.Stop();

                    led1.Image = VE209S_WindowsPC.Properties.Resources.light_bulb_on0;

                    dimmer1_status = 0x01;
                    dimmer1.Image = VE209S_WindowsPC.Properties.Resources.tuner1;

                    //dimmer1CounterClockwise.Enabled = true; 
                    break;
                case "ON 2\r":
                    rxRevData = "";
                    //dimmer1CounterClockwise_timer.Stop();

                    led2.Image = VE209S_WindowsPC.Properties.Resources.light_bulb_on0;

                    dimmer2_status = 0x01;
                    dimmer2.Image = VE209S_WindowsPC.Properties.Resources.tuner1;

                    //dimmer1CounterClockwise.Enabled = true; 
                    break;
                case "ON 3\r":
                    rxRevData = "";
                    //dimmer1CounterClockwise_timer.Stop();

                    led3.Image = VE209S_WindowsPC.Properties.Resources.light_bulb_on0;

                    dimmer3_status = 0x01;
                    dimmer3.Image = VE209S_WindowsPC.Properties.Resources.tuner1;

                    //dimmer1CounterClockwise.Enabled = true; 
                    break;
                case "ON 4\r":
                    rxRevData = "";
                    //dimmer1CounterClockwise_timer.Stop();

                    led4.Image = VE209S_WindowsPC.Properties.Resources.light_bulb_on0;

                    dimmer4_status = 0x01;
                    dimmer4.Image = VE209S_WindowsPC.Properties.Resources.tuner1;

                    //dimmer1CounterClockwise.Enabled = true; 
                    break;
                case "ON 5\r":
                    rxRevData = "";
                    //dimmer1CounterClockwise_timer.Stop();

                    led5.Image = VE209S_WindowsPC.Properties.Resources.light_bulb_on0;

                    dimmer5_status = 0x01;
                    dimmer5.Image = VE209S_WindowsPC.Properties.Resources.tuner1;

                    //dimmer1CounterClockwise.Enabled = true; 
                    break;
                case "OFF 1\r":
                    rxRevData = "";
                    //dimmer1CounterClockwise_timer.Stop();

                    led1.Image = VE209S_WindowsPC.Properties.Resources.light_bulb_off;

                    dimmer1_status = 0x00;
                    dimmer1.Image = VE209S_WindowsPC.Properties.Resources.tuner0;

                    //dimmer1CounterClockwise.Enabled = true;
                    break;
                case "OFF 2\r":
                    rxRevData = "";
                    //dimmer1CounterClockwise_timer.Stop();

                    led2.Image = VE209S_WindowsPC.Properties.Resources.light_bulb_off;

                    dimmer2_status = 0x00;
                    dimmer2.Image = VE209S_WindowsPC.Properties.Resources.tuner0;

                    //dimmer1CounterClockwise.Enabled = true;
                    break;
                case "OFF 3\r":
                    rxRevData = "";
                    //dimmer1CounterClockwise_timer.Stop();

                    led3.Image = VE209S_WindowsPC.Properties.Resources.light_bulb_off;

                    dimmer3_status = 0x00;
                    dimmer3.Image = VE209S_WindowsPC.Properties.Resources.tuner0;

                    //dimmer1CounterClockwise.Enabled = true;
                    break;
                case "OFF 4\r":
                    rxRevData = "";
                    //dimmer1CounterClockwise_timer.Stop();

                    led4.Image = VE209S_WindowsPC.Properties.Resources.light_bulb_off;

                    dimmer4_status = 0x00;
                    dimmer4.Image = VE209S_WindowsPC.Properties.Resources.tuner0;

                    //dimmer1CounterClockwise.Enabled = true;
                    break;
                case "OFF 5\r":
                    rxRevData = "";
                    //dimmer1CounterClockwise_timer.Stop();

                    led5.Image = VE209S_WindowsPC.Properties.Resources.light_bulb_off;

                    dimmer5_status = 0x00;
                    dimmer5.Image = VE209S_WindowsPC.Properties.Resources.tuner0;

                    //dimmer1CounterClockwise.Enabled = true;
                    break;
                case "DIM+ 1\r":
                    rxRevData = "";
                    //dimmer1CounterClockwise_timer.Stop();

                    switch (dimmer1_status)
                    {
                        case 0x01:
                            dimmer1_status = 0x02;
                            led1.Image = VE209S_WindowsPC.Properties.Resources.light_bulb_on1;
                            //dimmer1.Image = VE209S_WindowsPC.Properties.Resources.tuner2;
                            break;
                        case 0x02:
                            dimmer1_status = 0x03;
                            led1.Image = VE209S_WindowsPC.Properties.Resources.light_bulb_on2;
                            //dimmer1.Image = VE209S_WindowsPC.Properties.Resources.tuner3;
                            break;
                        case 0x03:
                            dimmer1_status = 0x04;
                            led1.Image = VE209S_WindowsPC.Properties.Resources.light_bulb_on3;
                            //dimmer1.Image = VE209S_WindowsPC.Properties.Resources.tuner4;
                            break;
                        default:
                            break;
                    }

                    //dimmer1CounterClockwise.Enabled = true;
                    break;
                case "DIM+ 2\r":
                    rxRevData = "";
                    //dimmer1CounterClockwise_timer.Stop();

                    switch (dimmer2_status)
                    {
                        case 0x01:
                            dimmer2_status = 0x02;
                            led2.Image = VE209S_WindowsPC.Properties.Resources.light_bulb_on1;
                            //dimmer1.Image = VE209S_WindowsPC.Properties.Resources.tuner2;
                            break;
                        case 0x02:
                            dimmer2_status = 0x03;
                            led2.Image = VE209S_WindowsPC.Properties.Resources.light_bulb_on2;
                            //dimmer1.Image = VE209S_WindowsPC.Properties.Resources.tuner3;
                            break;
                        case 0x03:
                            dimmer2_status = 0x04;
                            led2.Image = VE209S_WindowsPC.Properties.Resources.light_bulb_on3;
                            //dimmer1.Image = VE209S_WindowsPC.Properties.Resources.tuner4;
                            break;
                        default:
                            break;
                    }

                    //dimmer1CounterClockwise.Enabled = true;
                    break;
                case "DIM+ 3\r":
                    rxRevData = "";
                    //dimmer1CounterClockwise_timer.Stop();

                    switch (dimmer3_status)
                    {
                        case 0x01:
                            dimmer3_status = 0x02;
                            led3.Image = VE209S_WindowsPC.Properties.Resources.light_bulb_on1;
                            //dimmer1.Image = VE209S_WindowsPC.Properties.Resources.tuner2;
                            break;
                        case 0x02:
                            dimmer3_status = 0x03;
                            led3.Image = VE209S_WindowsPC.Properties.Resources.light_bulb_on2;
                            //dimmer1.Image = VE209S_WindowsPC.Properties.Resources.tuner3;
                            break;
                        case 0x03:
                            dimmer3_status = 0x04;
                            led3.Image = VE209S_WindowsPC.Properties.Resources.light_bulb_on3;
                            //dimmer1.Image = VE209S_WindowsPC.Properties.Resources.tuner4;
                            break;
                        default:
                            break;
                    }

                    //dimmer1CounterClockwise.Enabled = true;
                    break;
                case "DIM+ 4\r":
                    rxRevData = "";
                    //dimmer1CounterClockwise_timer.Stop();

                    switch (dimmer4_status)
                    {
                        case 0x01:
                            dimmer4_status = 0x02;
                            led4.Image = VE209S_WindowsPC.Properties.Resources.light_bulb_on1;
                            //dimmer1.Image = VE209S_WindowsPC.Properties.Resources.tuner2;
                            break;
                        case 0x02:
                            dimmer4_status = 0x03;
                            led4.Image = VE209S_WindowsPC.Properties.Resources.light_bulb_on2;
                            //dimmer1.Image = VE209S_WindowsPC.Properties.Resources.tuner3;
                            break;
                        case 0x03:
                            dimmer4_status = 0x04;
                            led4.Image = VE209S_WindowsPC.Properties.Resources.light_bulb_on3;
                            //dimmer1.Image = VE209S_WindowsPC.Properties.Resources.tuner4;
                            break;
                        default:
                            break;
                    }

                    //dimmer1CounterClockwise.Enabled = true;
                    break;
                case "DIM+ 5\r":
                    rxRevData = "";
                    //dimmer1CounterClockwise_timer.Stop();

                    switch (dimmer5_status)
                    {
                        case 0x01:
                            dimmer5_status = 0x02;
                            led5.Image = VE209S_WindowsPC.Properties.Resources.light_bulb_on1;
                            //dimmer1.Image = VE209S_WindowsPC.Properties.Resources.tuner2;
                            break;
                        case 0x02:
                            dimmer5_status = 0x03;
                            led5.Image = VE209S_WindowsPC.Properties.Resources.light_bulb_on2;
                            //dimmer1.Image = VE209S_WindowsPC.Properties.Resources.tuner3;
                            break;
                        case 0x03:
                            dimmer5_status = 0x04;
                            led5.Image = VE209S_WindowsPC.Properties.Resources.light_bulb_on3;
                            //dimmer1.Image = VE209S_WindowsPC.Properties.Resources.tuner4;
                            break;
                        default:
                            break;
                    }

                    //dimmer1CounterClockwise.Enabled = true;
                    break;
                case "DIM- 1\r":
                    rxRevData = "";
                    //dimmer1CounterClockwise_timer.Stop();

                    switch (dimmer1_status)
                    {
                        case 0x02:
                            dimmer1_status = 0x01;
                            led1.Image = VE209S_WindowsPC.Properties.Resources.light_bulb_on0;
                            //dimmer1.Image = VE209S_WindowsPC.Properties.Resources.tuner1;
                            break;
                        case 0x03:
                            dimmer1_status = 0x02;
                            led1.Image = VE209S_WindowsPC.Properties.Resources.light_bulb_on1;
                            //dimmer1.Image = VE209S_WindowsPC.Properties.Resources.tuner2;
                            break;
                        case 0x04:
                            dimmer1_status = 0x03;
                            led1.Image = VE209S_WindowsPC.Properties.Resources.light_bulb_on2;
                            //dimmer1.Image = VE209S_WindowsPC.Properties.Resources.tuner3;
                            break;
                        default:
                            break;
                    }

                    //dimmer1CounterClockwise.Enabled = true;
                    break;
                case "DIM- 2\r":
                    rxRevData = "";
                    //dimmer1CounterClockwise_timer.Stop();

                    switch (dimmer2_status)
                    {
                        case 0x02:
                            dimmer2_status = 0x01;
                            led2.Image = VE209S_WindowsPC.Properties.Resources.light_bulb_on0;
                            //dimmer1.Image = VE209S_WindowsPC.Properties.Resources.tuner1;
                            break;
                        case 0x03:
                            dimmer2_status = 0x02;
                            led2.Image = VE209S_WindowsPC.Properties.Resources.light_bulb_on1;
                            //dimmer1.Image = VE209S_WindowsPC.Properties.Resources.tuner2;
                            break;
                        case 0x04:
                            dimmer2_status = 0x03;
                            led2.Image = VE209S_WindowsPC.Properties.Resources.light_bulb_on2;
                            //dimmer1.Image = VE209S_WindowsPC.Properties.Resources.tuner3;
                            break;
                        default:
                            break;
                    }

                    //dimmer1CounterClockwise.Enabled = true;
                    break;
                case "DIM- 3\r":
                    rxRevData = "";
                    //dimmer1CounterClockwise_timer.Stop();

                    switch (dimmer3_status)
                    {
                        case 0x02:
                            dimmer3_status = 0x01;
                            led3.Image = VE209S_WindowsPC.Properties.Resources.light_bulb_on0;
                            //dimmer1.Image = VE209S_WindowsPC.Properties.Resources.tuner1;
                            break;
                        case 0x03:
                            dimmer3_status = 0x02;
                            led3.Image = VE209S_WindowsPC.Properties.Resources.light_bulb_on1;
                            //dimmer1.Image = VE209S_WindowsPC.Properties.Resources.tuner2;
                            break;
                        case 0x04:
                            dimmer3_status = 0x03;
                            led3.Image = VE209S_WindowsPC.Properties.Resources.light_bulb_on2;
                            //dimmer1.Image = VE209S_WindowsPC.Properties.Resources.tuner3;
                            break;
                        default:
                            break;
                    }

                    //dimmer1CounterClockwise.Enabled = true;
                    break;
                case "DIM- 4\r":
                    rxRevData = "";
                    //dimmer1CounterClockwise_timer.Stop();

                    switch (dimmer4_status)
                    {
                        case 0x02:
                            dimmer4_status = 0x01;
                            led4.Image = VE209S_WindowsPC.Properties.Resources.light_bulb_on0;
                            //dimmer1.Image = VE209S_WindowsPC.Properties.Resources.tuner1;
                            break;
                        case 0x03:
                            dimmer4_status = 0x02;
                            led4.Image = VE209S_WindowsPC.Properties.Resources.light_bulb_on1;
                            //dimmer1.Image = VE209S_WindowsPC.Properties.Resources.tuner2;
                            break;
                        case 0x04:
                            dimmer4_status = 0x03;
                            led4.Image = VE209S_WindowsPC.Properties.Resources.light_bulb_on2;
                            //dimmer1.Image = VE209S_WindowsPC.Properties.Resources.tuner3;
                            break;
                        default:
                            break;
                    }

                    //dimmer1CounterClockwise.Enabled = true;
                    break;
                case "DIM- 5\r":
                    rxRevData = "";
                    //dimmer1CounterClockwise_timer.Stop();

                    switch (dimmer5_status)
                    {
                        case 0x02:
                            dimmer5_status = 0x01;
                            led5.Image = VE209S_WindowsPC.Properties.Resources.light_bulb_on0;
                            //dimmer1.Image = VE209S_WindowsPC.Properties.Resources.tuner1;
                            break;
                        case 0x03:
                            dimmer5_status = 0x02;
                            led5.Image = VE209S_WindowsPC.Properties.Resources.light_bulb_on1;
                            //dimmer1.Image = VE209S_WindowsPC.Properties.Resources.tuner2;
                            break;
                        case 0x04:
                            dimmer5_status = 0x03;
                            led5.Image = VE209S_WindowsPC.Properties.Resources.light_bulb_on2;
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

        private void dimmer2Clockwise_timerEvent(object source, ElapsedEventArgs e)
        {
            if (++dimmer2Clockwise_repeaTimes < ve209s_tx_repeaTimes)
            {
                this.Invoke(new EventHandler(dimmer2Clockwise_repeatTX));
            }
            else
            {
                this.Invoke(new EventHandler(dimmer2Clockwise_failTX));
            }
        }

        private void dimmer3Clockwise_timerEvent(object source, ElapsedEventArgs e)
        {
            if (++dimmer3Clockwise_repeaTimes < ve209s_tx_repeaTimes)
            {
                this.Invoke(new EventHandler(dimmer3Clockwise_repeatTX));
            }
            else
            {
                this.Invoke(new EventHandler(dimmer3Clockwise_failTX));
            }
        }

        private void dimmer4Clockwise_timerEvent(object source, ElapsedEventArgs e)
        {
            if (++dimmer4Clockwise_repeaTimes < ve209s_tx_repeaTimes)
            {
                this.Invoke(new EventHandler(dimmer4Clockwise_repeatTX));
            }
            else
            {
                this.Invoke(new EventHandler(dimmer4Clockwise_failTX));
            }
        }

        private void dimmer5Clockwise_timerEvent(object source, ElapsedEventArgs e)
        {
            if (++dimmer5Clockwise_repeaTimes < ve209s_tx_repeaTimes)
            {
                this.Invoke(new EventHandler(dimmer5Clockwise_repeatTX));
            }
            else
            {
                this.Invoke(new EventHandler(dimmer5Clockwise_failTX));
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

        private void dimmer2CounterClockwise_timerEvent(object source, ElapsedEventArgs e)
        {
            if (++dimmer2CounterClockwise_repeaTimes < ve209s_tx_repeaTimes)
            {
                this.Invoke(new EventHandler(dimmer2CounterClockwise_repeatTX));
            }
            else
            {
                this.Invoke(new EventHandler(dimmer2CounterClockwise_failTX));
            }
        }

        private void dimmer3CounterClockwise_timerEvent(object source, ElapsedEventArgs e)
        {
            if (++dimmer3CounterClockwise_repeaTimes < ve209s_tx_repeaTimes)
            {
                this.Invoke(new EventHandler(dimmer3CounterClockwise_repeatTX));
            }
            else
            {
                this.Invoke(new EventHandler(dimmer3CounterClockwise_failTX));
            }
        }

        private void dimmer4CounterClockwise_timerEvent(object source, ElapsedEventArgs e)
        {
            if (++dimmer4CounterClockwise_repeaTimes < ve209s_tx_repeaTimes)
            {
                this.Invoke(new EventHandler(dimmer4CounterClockwise_repeatTX));
            }
            else
            {
                this.Invoke(new EventHandler(dimmer4CounterClockwise_failTX));
            }
        }

        private void dimmer5CounterClockwise_timerEvent(object source, ElapsedEventArgs e)
        {
            if (++dimmer5CounterClockwise_repeaTimes < ve209s_tx_repeaTimes)
            {
                this.Invoke(new EventHandler(dimmer5CounterClockwise_repeatTX));
            }
            else
            {
                this.Invoke(new EventHandler(dimmer5CounterClockwise_failTX));
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

        private void dimmer2CounterClockwise_repeatTX(object sender, EventArgs e)
        {
            switch (dimmer2_status)
            {
                case 0x00:
                    ve209s_usb_serial_port_tx("ON 2");
                    break;
                case 0x01:
                case 0x02:
                case 0x03:
                    ve209s_usb_serial_port_tx("DIM+ 2");
                    break;
                default:
                    break;
            }
        }

        private void dimmer3CounterClockwise_repeatTX(object sender, EventArgs e)
        {
            switch (dimmer3_status)
            {
                case 0x00:
                    ve209s_usb_serial_port_tx("ON 3");
                    break;
                case 0x01:
                case 0x02:
                case 0x03:
                    ve209s_usb_serial_port_tx("DIM+ 3");
                    break;
                default:
                    break;
            }
        }

        private void dimmer4CounterClockwise_repeatTX(object sender, EventArgs e)
        {
            switch (dimmer4_status)
            {
                case 0x00:
                    ve209s_usb_serial_port_tx("ON 4");
                    break;
                case 0x01:
                case 0x02:
                case 0x03:
                    ve209s_usb_serial_port_tx("DIM+ 4");
                    break;
                default:
                    break;
            }
        }

        private void dimmer5CounterClockwise_repeatTX(object sender, EventArgs e)
        {
            switch (dimmer5_status)
            {
                case 0x00:
                    ve209s_usb_serial_port_tx("ON 5");
                    break;
                case 0x01:
                case 0x02:
                case 0x03:
                    ve209s_usb_serial_port_tx("DIM+ 5");
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

        private void dimmer2Clockwise_repeatTX(object sender, EventArgs e)
        {
            switch (dimmer2_status)
            {
                case 0x01:
                    ve209s_usb_serial_port_tx("OFF 2");
                    break;
                case 0x02:
                case 0x03:
                case 0x04:
                    ve209s_usb_serial_port_tx("DIM- 2");
                    break;
                default:
                    break;
            }
        }

        private void dimmer3Clockwise_repeatTX(object sender, EventArgs e)
        {
            switch (dimmer3_status)
            {
                case 0x01:
                    ve209s_usb_serial_port_tx("OFF 3");
                    break;
                case 0x02:
                case 0x03:
                case 0x04:
                    ve209s_usb_serial_port_tx("DIM- 3");
                    break;
                default:
                    break;
            }
        }

        private void dimmer4Clockwise_repeatTX(object sender, EventArgs e)
        {
            switch (dimmer4_status)
            {
                case 0x01:
                    ve209s_usb_serial_port_tx("OFF 4");
                    break;
                case 0x02:
                case 0x03:
                case 0x04:
                    ve209s_usb_serial_port_tx("DIM- 4");
                    break;
                default:
                    break;
            }
        }

        private void dimmer5Clockwise_repeatTX(object sender, EventArgs e)
        {
            switch (dimmer5_status)
            {
                case 0x01:
                    ve209s_usb_serial_port_tx("OFF 5");
                    break;
                case 0x02:
                case 0x03:
                case 0x04:
                    ve209s_usb_serial_port_tx("DIM- 5");
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

        private void dimmer2CounterClockwise_failTX(object sender, EventArgs e)
        {
            dimmer2CounterClockwise_timer.Stop();

            dimmer2CounterClockwise_repeaTimes = 0;

            dimmer2CounterClockwise.Enabled = true;

            switch (dimmer2_status)
            {
                case 0x00:
                    dimmer2.Image = VE209S_WindowsPC.Properties.Resources.tuner0;
                    break;
                case 0x01:
                    dimmer2.Image = VE209S_WindowsPC.Properties.Resources.tuner1;
                    break;
                case 0x02:
                    dimmer2.Image = VE209S_WindowsPC.Properties.Resources.tuner2;
                    break;
                case 0x03:
                    dimmer2.Image = VE209S_WindowsPC.Properties.Resources.tuner3;
                    break;
                case 0x04:
                    dimmer2.Image = VE209S_WindowsPC.Properties.Resources.tuner4;
                    break;
                default:
                    break;
            }
        }

        private void dimmer3CounterClockwise_failTX(object sender, EventArgs e)
        {
            dimmer3CounterClockwise_timer.Stop();

            dimmer3CounterClockwise_repeaTimes = 0;

            dimmer3CounterClockwise.Enabled = true;

            switch (dimmer3_status)
            {
                case 0x00:
                    dimmer3.Image = VE209S_WindowsPC.Properties.Resources.tuner0;
                    break;
                case 0x01:
                    dimmer3.Image = VE209S_WindowsPC.Properties.Resources.tuner1;
                    break;
                case 0x02:
                    dimmer3.Image = VE209S_WindowsPC.Properties.Resources.tuner2;
                    break;
                case 0x03:
                    dimmer3.Image = VE209S_WindowsPC.Properties.Resources.tuner3;
                    break;
                case 0x04:
                    dimmer3.Image = VE209S_WindowsPC.Properties.Resources.tuner4;
                    break;
                default:
                    break;
            }
        }

        private void dimmer4CounterClockwise_failTX(object sender, EventArgs e)
        {
            dimmer4CounterClockwise_timer.Stop();

            dimmer4CounterClockwise_repeaTimes = 0;

            dimmer4CounterClockwise.Enabled = true;

            switch (dimmer4_status)
            {
                case 0x00:
                    dimmer4.Image = VE209S_WindowsPC.Properties.Resources.tuner0;
                    break;
                case 0x01:
                    dimmer4.Image = VE209S_WindowsPC.Properties.Resources.tuner1;
                    break;
                case 0x02:
                    dimmer4.Image = VE209S_WindowsPC.Properties.Resources.tuner2;
                    break;
                case 0x03:
                    dimmer4.Image = VE209S_WindowsPC.Properties.Resources.tuner3;
                    break;
                case 0x04:
                    dimmer4.Image = VE209S_WindowsPC.Properties.Resources.tuner4;
                    break;
                default:
                    break;
            }
        }

        private void dimmer5CounterClockwise_failTX(object sender, EventArgs e)
        {
            dimmer5CounterClockwise_timer.Stop();

            dimmer5CounterClockwise_repeaTimes = 0;

            dimmer5CounterClockwise.Enabled = true;

            switch (dimmer5_status)
            {
                case 0x00:
                    dimmer5.Image = VE209S_WindowsPC.Properties.Resources.tuner0;
                    break;
                case 0x01:
                    dimmer5.Image = VE209S_WindowsPC.Properties.Resources.tuner1;
                    break;
                case 0x02:
                    dimmer5.Image = VE209S_WindowsPC.Properties.Resources.tuner2;
                    break;
                case 0x03:
                    dimmer5.Image = VE209S_WindowsPC.Properties.Resources.tuner3;
                    break;
                case 0x04:
                    dimmer5.Image = VE209S_WindowsPC.Properties.Resources.tuner4;
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

        private void dimmer2Clockwise_failTX(object sender, EventArgs e)
        {
            dimmer2Clockwise_timer.Stop();

            dimmer2Clockwise_repeaTimes = 0;

            dimmer2Clockwise.Enabled = true;

            switch (dimmer2_status)
            {
                case 0x00:
                    dimmer2.Image = VE209S_WindowsPC.Properties.Resources.tuner0;
                    break;
                case 0x01:
                    dimmer2.Image = VE209S_WindowsPC.Properties.Resources.tuner1;
                    break;
                case 0x02:
                    dimmer2.Image = VE209S_WindowsPC.Properties.Resources.tuner2;
                    break;
                case 0x03:
                    dimmer2.Image = VE209S_WindowsPC.Properties.Resources.tuner3;
                    break;
                case 0x04:
                    dimmer2.Image = VE209S_WindowsPC.Properties.Resources.tuner4;
                    break;
                default:
                    break;
            }
        }

        private void dimmer3Clockwise_failTX(object sender, EventArgs e)
        {
            dimmer3Clockwise_timer.Stop();

            dimmer3Clockwise_repeaTimes = 0;

            dimmer3Clockwise.Enabled = true;

            switch (dimmer3_status)
            {
                case 0x00:
                    dimmer3.Image = VE209S_WindowsPC.Properties.Resources.tuner0;
                    break;
                case 0x01:
                    dimmer3.Image = VE209S_WindowsPC.Properties.Resources.tuner1;
                    break;
                case 0x02:
                    dimmer3.Image = VE209S_WindowsPC.Properties.Resources.tuner2;
                    break;
                case 0x03:
                    dimmer3.Image = VE209S_WindowsPC.Properties.Resources.tuner3;
                    break;
                case 0x04:
                    dimmer3.Image = VE209S_WindowsPC.Properties.Resources.tuner4;
                    break;
                default:
                    break;
            }
        }

        private void dimmer4Clockwise_failTX(object sender, EventArgs e)
        {
            dimmer4Clockwise_timer.Stop();

            dimmer4Clockwise_repeaTimes = 0;

            dimmer4Clockwise.Enabled = true;

            switch (dimmer4_status)
            {
                case 0x00:
                    dimmer4.Image = VE209S_WindowsPC.Properties.Resources.tuner0;
                    break;
                case 0x01:
                    dimmer4.Image = VE209S_WindowsPC.Properties.Resources.tuner1;
                    break;
                case 0x02:
                    dimmer4.Image = VE209S_WindowsPC.Properties.Resources.tuner2;
                    break;
                case 0x03:
                    dimmer4.Image = VE209S_WindowsPC.Properties.Resources.tuner3;
                    break;
                case 0x04:
                    dimmer4.Image = VE209S_WindowsPC.Properties.Resources.tuner4;
                    break;
                default:
                    break;
            }
        }

        private void dimmer5Clockwise_failTX(object sender, EventArgs e)
        {
            dimmer5Clockwise_timer.Stop();

            dimmer5Clockwise_repeaTimes = 0;

            dimmer5Clockwise.Enabled = true;

            switch (dimmer5_status)
            {
                case 0x00:
                    dimmer5.Image = VE209S_WindowsPC.Properties.Resources.tuner0;
                    break;
                case 0x01:
                    dimmer5.Image = VE209S_WindowsPC.Properties.Resources.tuner1;
                    break;
                case 0x02:
                    dimmer5.Image = VE209S_WindowsPC.Properties.Resources.tuner2;
                    break;
                case 0x03:
                    dimmer5.Image = VE209S_WindowsPC.Properties.Resources.tuner3;
                    break;
                case 0x04:
                    dimmer5.Image = VE209S_WindowsPC.Properties.Resources.tuner4;
                    break;
                default:
                    break;
            }
        }

        private void dimmer1CounterClockwise_Click(object sender, EventArgs e)
        {
            //rxRevData = "";

            //ve209s_usb_serial_port.DiscardInBuffer();

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
            //rxRevData = "";

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

        private void dimmer2CounterClockwise_Click(object sender, EventArgs e)
        {
            //rxRevData = "";

            //ve209s_usb_serial_port.DiscardInBuffer();

            dimmer2CounterClockwise_repeaTimes = 0;

            switch (dimmer2_status)
            {
                case 0x00:
                    dimmer2.Image = VE209S_WindowsPC.Properties.Resources.tuner1;

                    ve209s_usb_serial_port_tx("ON 2");

                    //dimmer1_status = 0x01;

                    dimmer2CounterClockwise_timer.Start();

                    dimmer2CounterClockwise.Enabled = false;
                    break;
                case 0x01:
                    dimmer2.Image = VE209S_WindowsPC.Properties.Resources.tuner2;

                    ve209s_usb_serial_port_tx("DIM+ 2");

                    //dimmer1_status = 0x02;

                    dimmer2CounterClockwise_timer.Start();

                    dimmer2CounterClockwise.Enabled = false;
                    break;
                case 0x02:
                    dimmer2.Image = VE209S_WindowsPC.Properties.Resources.tuner3;

                    ve209s_usb_serial_port_tx("DIM+ 2");

                    //dimmer1_status = 0x02;

                    dimmer2CounterClockwise_timer.Start();

                    dimmer2CounterClockwise.Enabled = false;
                    break;
                case 0x03:
                    dimmer2.Image = VE209S_WindowsPC.Properties.Resources.tuner4;

                    ve209s_usb_serial_port_tx("DIM+ 2");

                    //dimmer1_status = 0x02;

                    dimmer2CounterClockwise_timer.Start();

                    dimmer2CounterClockwise.Enabled = false;
                    break;
                default:
                    break;
            }
        }

        private void dimmer2Clockwise_Click(object sender, EventArgs e)
        {
            //rxRevData = "";

            dimmer2Clockwise_repeaTimes = 0;

            switch (dimmer2_status)
            {
                case 0x01:
                    dimmer2.Image = VE209S_WindowsPC.Properties.Resources.tuner0;

                    ve209s_usb_serial_port_tx("OFF 2");

                    //dimmer1_status = 0x00;

                    dimmer2Clockwise_timer.Start();

                    dimmer2Clockwise.Enabled = false;
                    break;
                case 0x02:
                    dimmer2.Image = VE209S_WindowsPC.Properties.Resources.tuner1;

                    ve209s_usb_serial_port_tx("DIM- 2");

                    //dimmer1_status = 0x01;

                    dimmer2Clockwise_timer.Start();

                    dimmer2Clockwise.Enabled = false;
                    break;
                case 0x03:
                    dimmer2.Image = VE209S_WindowsPC.Properties.Resources.tuner2;

                    ve209s_usb_serial_port_tx("DIM- 2");

                    //dimmer1_status = 0x02;

                    dimmer2Clockwise_timer.Start();

                    dimmer2Clockwise.Enabled = false;
                    break;
                case 0x04:
                    dimmer2.Image = VE209S_WindowsPC.Properties.Resources.tuner3;

                    ve209s_usb_serial_port_tx("DIM- 2");

                    //dimmer1_status = 0x03;

                    dimmer2Clockwise_timer.Start();

                    dimmer2Clockwise.Enabled = false;
                    break;
                default:
                    break;
            }
        }

        private void dimmer3Clockwise_Click(object sender, EventArgs e)
        {
            //rxRevData = "";

            dimmer3Clockwise_repeaTimes = 0;

            switch (dimmer3_status)
            {
                case 0x01:
                    dimmer3.Image = VE209S_WindowsPC.Properties.Resources.tuner0;

                    ve209s_usb_serial_port_tx("OFF 3");

                    dimmer3Clockwise_timer.Start();

                    dimmer3Clockwise.Enabled = false;
                    break;
                case 0x02:
                    dimmer3.Image = VE209S_WindowsPC.Properties.Resources.tuner1;

                    ve209s_usb_serial_port_tx("DIM- 3");

                    dimmer3Clockwise_timer.Start();

                    dimmer3Clockwise.Enabled = false;
                    break;
                case 0x03:
                    dimmer3.Image = VE209S_WindowsPC.Properties.Resources.tuner2;

                    ve209s_usb_serial_port_tx("DIM- 3");

                    dimmer3Clockwise_timer.Start();

                    dimmer3Clockwise.Enabled = false;
                    break;
                case 0x04:
                    dimmer3.Image = VE209S_WindowsPC.Properties.Resources.tuner3;

                    ve209s_usb_serial_port_tx("DIM- 3");

                    dimmer3Clockwise_timer.Start();

                    dimmer3Clockwise.Enabled = false;
                    break;
                default:
                    break;
            }
        }

        private void dimmer3CounterClockwise_Click(object sender, EventArgs e)
        {
            //rxRevData = "";

            //ve209s_usb_serial_port.DiscardInBuffer();

            dimmer3CounterClockwise_repeaTimes = 0;

            switch (dimmer3_status)
            {
                case 0x00:
                    dimmer3.Image = VE209S_WindowsPC.Properties.Resources.tuner1;

                    ve209s_usb_serial_port_tx("ON 3");

                    //dimmer1_status = 0x01;

                    dimmer3CounterClockwise_timer.Start();

                    dimmer3CounterClockwise.Enabled = false;
                    break;
                case 0x01:
                    dimmer3.Image = VE209S_WindowsPC.Properties.Resources.tuner2;

                    ve209s_usb_serial_port_tx("DIM+ 3");

                    //dimmer1_status = 0x02;

                    dimmer3CounterClockwise_timer.Start();

                    dimmer3CounterClockwise.Enabled = false;
                    break;
                case 0x02:
                    dimmer3.Image = VE209S_WindowsPC.Properties.Resources.tuner3;

                    ve209s_usb_serial_port_tx("DIM+ 3");

                    //dimmer1_status = 0x02;

                    dimmer3CounterClockwise_timer.Start();

                    dimmer3CounterClockwise.Enabled = false;
                    break;
                case 0x03:
                    dimmer3.Image = VE209S_WindowsPC.Properties.Resources.tuner4;

                    ve209s_usb_serial_port_tx("DIM+ 3");

                    //dimmer1_status = 0x02;

                    dimmer3CounterClockwise_timer.Start();

                    dimmer3CounterClockwise.Enabled = false;
                    break;
                default:
                    break;
            }
        }

        private void dimmer4CounterClockwise_Click(object sender, EventArgs e)
        {
            //rxRevData = "";

            //ve209s_usb_serial_port.DiscardInBuffer();

            dimmer4CounterClockwise_repeaTimes = 0;

            switch (dimmer4_status)
            {
                case 0x00:
                    dimmer4.Image = VE209S_WindowsPC.Properties.Resources.tuner1;

                    ve209s_usb_serial_port_tx("ON 4");

                    //dimmer1_status = 0x01;

                    dimmer4CounterClockwise_timer.Start();

                    dimmer4CounterClockwise.Enabled = false;
                    break;
                case 0x01:
                    dimmer4.Image = VE209S_WindowsPC.Properties.Resources.tuner2;

                    ve209s_usb_serial_port_tx("DIM+ 4");

                    //dimmer1_status = 0x02;

                    dimmer4CounterClockwise_timer.Start();

                    dimmer4CounterClockwise.Enabled = false;
                    break;
                case 0x02:
                    dimmer4.Image = VE209S_WindowsPC.Properties.Resources.tuner3;

                    ve209s_usb_serial_port_tx("DIM+ 4");

                    //dimmer1_status = 0x02;

                    dimmer4CounterClockwise_timer.Start();

                    dimmer4CounterClockwise.Enabled = false;
                    break;
                case 0x03:
                    dimmer4.Image = VE209S_WindowsPC.Properties.Resources.tuner4;

                    ve209s_usb_serial_port_tx("DIM+ 4");

                    //dimmer1_status = 0x02;

                    dimmer4CounterClockwise_timer.Start();

                    dimmer4CounterClockwise.Enabled = false;
                    break;
                default:
                    break;
            }
        }

        private void dimmer4Clockwise_Click(object sender, EventArgs e)
        {
            //rxRevData = "";

            dimmer4Clockwise_repeaTimes = 0;

            switch (dimmer4_status)
            {
                case 0x01:
                    dimmer4.Image = VE209S_WindowsPC.Properties.Resources.tuner0;

                    ve209s_usb_serial_port_tx("OFF 4");

                    dimmer4Clockwise_timer.Start();

                    dimmer4Clockwise.Enabled = false;
                    break;
                case 0x02:
                    dimmer4.Image = VE209S_WindowsPC.Properties.Resources.tuner1;

                    ve209s_usb_serial_port_tx("DIM- 4");

                    dimmer4Clockwise_timer.Start();

                    dimmer4Clockwise.Enabled = false;
                    break;
                case 0x03:
                    dimmer4.Image = VE209S_WindowsPC.Properties.Resources.tuner2;

                    ve209s_usb_serial_port_tx("DIM- 4");

                    dimmer4Clockwise_timer.Start();

                    dimmer4Clockwise.Enabled = false;
                    break;
                case 0x04:
                    dimmer4.Image = VE209S_WindowsPC.Properties.Resources.tuner3;

                    ve209s_usb_serial_port_tx("DIM- 4");

                    dimmer4Clockwise_timer.Start();

                    dimmer4Clockwise.Enabled = false;
                    break;
                default:
                    break;
            }
        }

        private void dimmer5CounterClockwise_Click(object sender, EventArgs e)
        {
            //rxRevData = "";

            //ve209s_usb_serial_port.DiscardInBuffer();

            dimmer5CounterClockwise_repeaTimes = 0;

            switch (dimmer5_status)
            {
                case 0x00:
                    dimmer5.Image = VE209S_WindowsPC.Properties.Resources.tuner1;

                    ve209s_usb_serial_port_tx("ON 5");

                    //dimmer1_status = 0x01;

                    dimmer5CounterClockwise_timer.Start();

                    dimmer5CounterClockwise.Enabled = false;
                    break;
                case 0x01:
                    dimmer5.Image = VE209S_WindowsPC.Properties.Resources.tuner2;

                    ve209s_usb_serial_port_tx("DIM+ 5");

                    //dimmer1_status = 0x02;

                    dimmer5CounterClockwise_timer.Start();

                    dimmer5CounterClockwise.Enabled = false;
                    break;
                case 0x02:
                    dimmer5.Image = VE209S_WindowsPC.Properties.Resources.tuner3;

                    ve209s_usb_serial_port_tx("DIM+ 5");

                    //dimmer1_status = 0x02;

                    dimmer5CounterClockwise_timer.Start();

                    dimmer5CounterClockwise.Enabled = false;
                    break;
                case 0x03:
                    dimmer5.Image = VE209S_WindowsPC.Properties.Resources.tuner4;

                    ve209s_usb_serial_port_tx("DIM+ 5");

                    //dimmer1_status = 0x02;

                    dimmer5CounterClockwise_timer.Start();

                    dimmer5CounterClockwise.Enabled = false;
                    break;
                default:
                    break;
            }
        }

        private void dimmer5Clockwise_Click(object sender, EventArgs e)
        {
            //rxRevData = "";

            dimmer5Clockwise_repeaTimes = 0;

            switch (dimmer5_status)
            {
                case 0x01:
                    dimmer5.Image = VE209S_WindowsPC.Properties.Resources.tuner0;

                    ve209s_usb_serial_port_tx("OFF 5");

                    dimmer5Clockwise_timer.Start();

                    dimmer5Clockwise.Enabled = false;
                    break;
                case 0x02:
                    dimmer5.Image = VE209S_WindowsPC.Properties.Resources.tuner1;

                    ve209s_usb_serial_port_tx("DIM- 5");

                    dimmer5Clockwise_timer.Start();

                    dimmer5Clockwise.Enabled = false;
                    break;
                case 0x03:
                    dimmer5.Image = VE209S_WindowsPC.Properties.Resources.tuner2;

                    ve209s_usb_serial_port_tx("DIM- 5");

                    dimmer5Clockwise_timer.Start();

                    dimmer5Clockwise.Enabled = false;
                    break;
                case 0x04:
                    dimmer5.Image = VE209S_WindowsPC.Properties.Resources.tuner3;

                    ve209s_usb_serial_port_tx("DIM- 5");

                    dimmer5Clockwise_timer.Start();

                    dimmer5Clockwise.Enabled = false;
                    break;
                default:
                    break;
            }
        }
    }
}