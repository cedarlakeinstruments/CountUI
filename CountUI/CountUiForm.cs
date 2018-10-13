using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.IO.Ports;
using System.Threading;
using CountUI.Properties;

namespace Tick
{
    public partial class CountUiForm : Form
    {
        bool _connected;
        SerialPort _port;

        public CountUiForm()
        {
            InitializeComponent();
            string[] names = SerialPort.GetPortNames();
            if (names.Length  != 0)
            {
                foreach (string name in names)
                {
                    listBoxPorts.Items.Add(name);
                }
            }
            else
            {
                MessageBox.Show("No COM ports found");                
            }

            if (Settings.Default.port != "")
            {
                _connected = connect(Settings.Default.port);
            }
            else
            {
                this.labelStatus.Text = "Not connected";
            }
        }

        bool connect(string portName)
        {
            timer1.Enabled = false;

            if (_port != null && _port.IsOpen)
            {
                _port.Close();
            }

            bool connected = false;
            try
            {
                _port = new SerialPort(portName, 19200);
                _port.Handshake = Handshake.None;
                _port.DataBits = 8;
                _port.StopBits = StopBits.One;
                _port.Parity = Parity.None;
                _port.Encoding = new ASCIIEncoding();
                _port.Open();
                _port.DiscardInBuffer();
                connected = true;
            }
            catch (IOException e)
            {
                this.labelStatus.Text = String.Format("Could not open {0}. {1}",portName, e.Message);
            }

            if (connected)
            {
                this.labelStatus.Text = String.Format("Connected to {0}", portName);
                timer1.Enabled = true;
            }

            return connected;
        }

        void update()
        {
            this._port.WriteLine("c");
            Thread.Sleep(50);
            string returned = this._port.ReadExisting();
            
            this.labelCount.Text = returned;
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            if (_connected == true)
            {
                update();
            }
            else
            {
                this.labelStatus.Text = "Not connected";
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        /// <summary>
        /// Called when a COM port is chosen
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void portSelected(object sender, EventArgs e)
        {
            Settings.Default.port = (string)((ListBox)sender).SelectedItem;
            Settings.Default.Save();
            connect(Settings.Default.port);
        }
    }
}
