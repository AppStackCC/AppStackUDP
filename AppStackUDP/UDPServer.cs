using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Net.Sockets;
using System.Net;

namespace AppStackUDP
{
    public class UDPServer
    {
        private UdpClient listener;
        private UdpClient client;
        private IPEndPoint groupEP;
        private bool _ThreadRunning;
        private Thread ThreadRecieve;
        private byte[] receive_byte_array;

        public delegate void OnRecieveDelegate(string ip, byte[] data_byte);
        public event OnRecieveDelegate OnReceiveData;

        public UDPServer()
        {
            client = new UdpClient();
            this._ThreadRunning = false;
        }

        private void ReceiveData()
        {
            while (_ThreadRunning)
            {
                receive_byte_array = listener.Receive(ref groupEP);
                if (OnReceiveData != null)
                {
                    OnReceiveData(groupEP.Address.ToString(), receive_byte_array);
                }
            }
        }

        public bool Send(string ip , int port , byte[] data)
        {
            try
            {
                client.Send(data, data.Length,ip,port);
                return true;
            }
            catch
            {
                return false;
            }
        }

        public void Start(int port)
        {
            if (listener != null) listener.Close();
           
            listener = new UdpClient(port);
            groupEP = new IPEndPoint(IPAddress.Any, port);
            this._ThreadRunning = true;
            ThreadRecieve = new Thread(new ThreadStart(ReceiveData));
            ThreadRecieve.Start();
        }

        public void Stop()
        {
            try
            {
                this._ThreadRunning = false;
                ThreadRecieve.Abort();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Stop UDP Error : {0}",ex.Message);
            }
        }

    }
}
