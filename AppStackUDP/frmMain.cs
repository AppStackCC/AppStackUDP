using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Net;

namespace AppStackUDP
{
    public partial class frmMain : Form
    {
        UDPServer udp = new UDPServer();

        private delegate void PrintResultDelegate(string txt);
        private void PrintResult(string txt)
        {
            if (rtxtResult.InvokeRequired)
            {
                rtxtResult.BeginInvoke(new PrintResultDelegate(PrintResult), txt);
                return;
            }
            rtxtResult.AppendText(txt);
            rtxtResult.ScrollToCaret();
        }

        private void UDPSend()
        {
            IPAddress ip;
            int port;
            try
            {
                ip = IPAddress.Parse(txtDstIP.Text);
            }
            catch
            {
                MessageBox.Show("Invalid destination IP Address.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            try
            {
                port = Convert.ToInt16(txtDstPort.Text);
            }
            catch
            {
                MessageBox.Show("Invalid destination Port.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            try
            {
                if (txtSend.Text.Length == 0)
                {
                    MessageBox.Show("No data to send.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
                udp.Send(txtDstIP.Text, port, Encoding.ASCII.GetBytes(txtSend.Text));

                PrintResult("Send Data [ "+ txtDstIP.Text + ":" + port.ToString() +" ] : " + txtSend.Text + "\r\n");
                txtSend.Clear();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        public frmMain()
        {
            InitializeComponent();
        }

        private void frmMain_Load(object sender, EventArgs e)
        {
            udp.OnReceiveData += udp_OnReceiveData;
        }

        void udp_OnReceiveData(string ip, byte[] data_byte)
        {
            StringBuilder hex = new StringBuilder(data_byte.Length * 5);
            foreach (byte b in data_byte)
                hex.AppendFormat("0x{0:x2} ", b);
             
            PrintResult("Receive ["+ ip + "]["+data_byte.Length.ToString()+"] : " + hex.ToString() + "\r\n");
        }

        private void tsbtnStartStop_Click(object sender, EventArgs e)
        {
            try
            {
                if (tsbtnStartStop.Text == "Start Server")
                {
                    int port = Convert.ToInt32(txtServerPort.Text);
                    udp.Start(port);
                    tsbtnStartStop.Text = "Stop Server";
                    PrintResult("Server Started : Port="+port.ToString() + "\r\n");
                }
                else
                {
                    udp.Stop();
                    tsbtnStartStop.Text = "Start Server";
                    PrintResult("Server Stoped\r\n");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Invalid Server Port.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void frmMain_FormClosing(object sender, FormClosingEventArgs e)
        {
            udp.Stop();
        }

        private void btnSend_Click(object sender, EventArgs e)
        {
            UDPSend();
        }

        private void txtSend_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                e.Handled = true;
                UDPSend();
            }
        }
    }
}
