using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using ParkingSystem;
using System.Net.Sockets;
using System.Net;
using System.Threading;
namespace ParkingDemo
{
    public partial class Form1 : Form
    {

        /// <summary>
        /// <remarks>serialPort State</remarks>
        /// </summary>
        private bool commState = false;
        /// <summary>
        /// <remarks>Ethernet State</remarks>
        /// </summary>
        private bool ethNetOpenState = false;
        /// <summary>
        /// <remarks>SerialPort Instantiation</remarks>
        /// </summary>
        public static ParkingSystem.ParkingSerialPort serialPort = new ParkingSerialPort();
        /// <summary>
        /// <remarks>TCP Instantiation</remarks>
        /// </summary>
        public static ParkingSystem.ParkingRemoteTCP wapper = new ParkingRemoteTCP();
        /// <summary>
        /// <remarks>tcp Listener</remarks>
        /// </summary>
        private static TcpListener tcpListener = null;
        /// <summary>
        /// <remarks>IP Address</remarks>
        /// </summary>
        private static IPAddress localIP;
        /// <summary>
        /// <remarks>TCP portNum</remarks>
        /// </summary>
        private static UInt16 portNum;
        /// <summary>
        /// <remarks>Tcp Client</remarks>
        /// </summary>
        private static TcpClient client = new TcpClient();
        /// <summary>
        /// <remarks>Tcp Thread</remarks>
        /// </summary>
        private Thread m_serverThread;
        public Form1()
        {
            InitializeComponent();
        }
        private void Form1_Load(object sender, EventArgs e)
        {
            CommInit();
            EthCommInit();
            ParkingOriginalPacket.EvProcessReceivedPacket += sp_ProcessReceivedPacket;
        }

        #region SerialPort Operation
        /// <summary>
        /// SerialPort Initialize
        /// </summary>
        private void CommInit()
        {
            foreach (string name in serialPort.portNames)
            {
                if (!CommPort_comboBox.Items.Contains(name))
                {
                    CommPort_comboBox.Items.Add(name);
                }
                CommPort_comboBox.Text = name;
            }

            CommBaud_comboBox.Text = "115200";
        }
        /// <summary>
        /// Set SerialPort Open
        /// </summary>
        private void SetOpen()
        {
            OpenClosePort_Button.Text = "Close  Comm";
            CommStatus_label.ForeColor = Color.Lime;
            commState = true;
        }
        /// <summary>
        /// Set SerialPort Close
        /// </summary>
        private void SetClose()
        {
            OpenClosePort_Button.Text = "Open  Comm";
            CommStatus_label.ForeColor = Color.DarkGray;
            commState = false;
        }
        /// <summary>
        /// Open Close SerialPort Oper
        /// </summary>
        /// <param name="sender">sender</param>
        /// <param name="e">e</param>
        private void OpenClosePort_Button_Click(object sender, EventArgs e)
        {
            try
            {
                if (OpenClosePort_Button.Text == "Open  Comm")
                {
                    serialPort = new ParkingSerialPort(CommPort_comboBox.Text, Convert.ToInt32(CommBaud_comboBox.Text));
                    serialPort.Open();
                    SetOpen();
                }
                else if (OpenClosePort_Button.Text == "Close  Comm")
                {
                    if (!serialPort.IsOpen)
                    {
                        SetClose();
                        return;
                    }
                    serialPort.Close();
                    SetClose();

                }


            }
            catch (Exception err)
            {
                MessageBox.Show(err.Message);
                SetClose();
                return;
            }
        }
        #endregion

        #region Device Operation
        
        /// <summary>
        /// Get device name
        /// </summary>
        /// <param name="devname">devicetype</param>
        /// <returns>devicename</returns>
        public string GetDevName(byte byteName)
        {
            string devname = "";
            switch (byteName)
            {
                case 0x01:
                    devname = "WDC-4003";
                    break;
                case 0x02:
                    devname = "WDC-4005";
                    break;
                case 0x03:
                    devname = "WDC-4008";
                    break;
                case 0x04:
                    devname = "WDC-4007";
                    break;
                case 0x05:
                    devname = "WPSD-340S3";
                    break;
                case 0x06:
                    devname = "WPSD-340S5";
                    break;
                case 0x07:
                    devname = "WPSD-340S8";
                    break;
                case 0x08:
                    devname = "WPSD-340S7";
                    break;
                case 0x09:
                    devname = "WPSD-340E3";
                    break;
                case 0x0A:
                    devname = "WPSD-340E5";
                    break;
                case 0x0B:
                    devname = "WPSD-340E8";
                    break;
                case 0x0C:
                    devname = "WPSD-340E7";
                    break;

                default:
                    devname = "WDC-400x";
                    break;
            }
            return devname;
        }
        #endregion

        #region Ethernet Operation
        /// <summary>
        /// Ethernet Initialize
        /// </summary>
        private void EthCommInit()
        {
            string addresses = GetLocalAddresses();
            severIPcomboBox.Items.Clear();
            if (addresses.Length > 0)
            {

                severIPcomboBox.Items.Add(addresses);

                severIPcomboBox.Text = (string)severIPcomboBox.Items[0];
            }
        }
        /// <summary>
        /// Get Local IP Address
        /// </summary>
        public string GetLocalAddresses()
        {
            // 获取主机名
            string strHostName = Dns.GetHostName();
            System.Net.IPAddress addr;
            addr = new System.Net.IPAddress(Dns.GetHostByName(Dns.GetHostName()).AddressList[0].Address);
            return addr.ToString();
        }
        /// <summary>
        /// Set Ethernet Close
        /// </summary>
        private void eth_Setclose()
        {
            connectstatelabel.Text = "Waiting  Connect...";
            ethNetOpenState = false;
            listenstatelabel.ForeColor = Color.DarkGray;
            startListenbutton.Text = "Start  Listen";
            deviceIPstatelabel.Text = "";

        }
        /// <summary>
        /// startListenbutton_Click Oper
        /// </summary>
        /// /// <param name="sender">sender</param>
        /// <param name="e">e</param>
        private void startListenbutton_Click(object sender, EventArgs e)
        {
            deviceIPstatelabel.Text = "";
            try
            {
                if (severPorttextBox.Text != "")
                {
                    if (startListenbutton.Text == "Start  Listen")
                    {
                        portNum = UInt16.Parse(severPorttextBox.Text);
                        if (portNum > 0 && portNum < 65535)
                        {
                            TCP_StartListen();
                            ethNetOpenState = true;
                            startListenbutton.Text = "Stop  Listen";
                            connectstatelabel.Text = "Listening...";
                        }
                        else
                        {
                            MessageBox.Show("Port Range:1~65535!");
                            return;
                        }

                    }
                    else
                    {
                        try
                        {
                            if (connectstatelabel.Text == "Listening...")
                            {
                                tcpListener.Stop();
                            }
                            else
                            {
                                tcpListener.Stop();
                                m_serverThread.Abort();
                                m_serverThread = null; //中止线程
                                
                                client.Close();
                            }
                            eth_Setclose();
                        }
                        catch (SocketException ex)
                        {
                            MessageBox.Show("TCP Server Listen Error!" + ex.Message);
                        }

                    }
                }
                else
                {
                    MessageBox.Show("Please input Port Adress!");
                }
            }
            catch (ThreadAbortException exl)
            {
                MessageBox.Show("TCP Server Button Error:" + exl.ToString());
            }
            catch (SocketException se)           //处理异常
            {
                MessageBox.Show("TCP Server Listen Error:" + se.Message);
            }
            catch (Exception ex)
            {
                tcpListener.Stop();
                client.Close();
                MessageBox.Show("TCP Server Button Error:" + ex.ToString());
            }
        }
        /// <summary>
        /// Start TCP Listening
        /// </summary>
        public void TCP_StartListen()
        {
            try
            {
                localIP = IPAddress.Parse(this.severIPcomboBox.Text);
                tcpListener = new TcpListener(localIP, portNum);
                tcpListener.Start();
                m_serverThread = new Thread(new ThreadStart(ReceiveAccept));
                m_serverThread.Start();
                m_serverThread.IsBackground = true;

            }
            catch (SocketException ex)
            {
                tcpListener.Stop();
                eth_Setclose();
                MessageBox.Show("TCP Server Listen Error:" + ex.Message);
            }
            catch (Exception err)
            {
                MessageBox.Show("TCP Server Listen Error:" + err.Message);
            }
        }
        /// <summary>
        /// TCP Client Accept
        /// </summary>
        private void ReceiveAccept()
        {
            while (true)
            {
                try
                {
                    client = tcpListener.AcceptTcpClient();
                    this.Invoke((EventHandler)delegate
                    {
                        this.listenstatelabel.ForeColor = Color.Lime;
                        this.connectstatelabel.Text = "connect ok";
                        this.deviceIPstatelabel.Text = client.Client.RemoteEndPoint.ToString();
                        ethNetOpenState = true;
                    });
                    wapper = new ParkingRemoteTCP(client);
                }
                catch (Exception ex)
                {
                    //eth_Setclose();
                    // MessageBox.Show(ex.Message, "?", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
        }
        #endregion

        #region Received Process
        /// <summary>
        /// <remarks>Process Received Packet</remarks>
        /// </summary>
        /// <param name="pk">Received Packet</param>
        private void sp_ProcessReceivedPacket(baseReceivedPacket pk)
        {
            try
            {
                byte revType = Convert.ToByte(pk.type_ver >> 8);
                string wpsdid = "";
                string WDCid = "";
                string RSSI = "";
                byte carState = 0;
                string voltage = "";
                string hardVer = "";
                string softVer = "";
                string deviceName = "";
                string hbPeriod = "";
                this.Invoke((EventHandler)delegate
                {
                    #region Senser Heart Beat
                    if (pk is SensorHBeat)
                       {
                           SensorHBeat hb = (SensorHBeat)pk;
                            reshow(hb.recRawData,true);
                            wpsdid = (hb.WPSD_ID).ToString("X2").PadLeft(8, '0');
                            WDCid = (hb.WDC_ID).ToString("X2").PadLeft(8, '0');
                            softVer = "v" + int.Parse(hb.APP_VER.ToString("X2").Substring(0, 1)).ToString() + "." + int.Parse(hb.APP_VER.ToString("X2").Substring(1, 1)).ToString().PadLeft(2, '0');
                            hardVer = ((int)(hb.HARD_VER) + 10).ToString();
                            hardVer = "v" + hardVer.Substring(0, 1) + "." + hardVer.Substring(1, 1);
                            voltage = (Math.Round((decimal)hb.VOLT / 10, 2)).ToString()+"V"; RSSI = ((Int16)hb.RSSI - 30).ToString();
                            hbPeriod = hb.HB_PERIOD.ToString();
                            deviceName = GetDevName(hb.DEV_TYPE);
                            carState = hb.CAR_STATE;
                            if (carState == 0x01)
                            {
                                richTextBox1.AppendText("wpsd id:" + wpsdid + "\nsoft Ver:" + softVer + "\nhard Ver:" + hardVer + "\nvoltage:" + voltage + "\nRSSI:" + RSSI + "\ncar State:Have Car\n");
                            }
                            else
                            {
                                richTextBox1.AppendText("wpsd id:" + wpsdid + "\nsoft Ver:" + softVer + "\nhard Ver:" + hardVer + "\nvoltage:" + voltage + "\nRSSI:" + RSSI + "\ncar State:No Car\n");
                            } 
                    
                    }
                      #endregion
                    #region Senser Detect
                    else if (pk is SensorDetect)
                    {
                        SensorDetect decbeat = (SensorDetect)pk;
                        reshow(decbeat.recRawData, true);
                        wpsdid = (decbeat.WPSD_ID).ToString("X2").PadLeft(8, '0');
                        WDCid = (decbeat.WDC_ID).ToString("X2").PadLeft(8, '0');
                        hardVer = ((int)(decbeat.HARD_VER) + 10).ToString();
                        hardVer = "v" + hardVer.Substring(0, 1) + "." + hardVer.Substring(1, 1);
                        deviceName = GetDevName(decbeat.DEV_TYPE);
                        carState = decbeat.CAR_STATE;
                        if (carState == 0x01)
                        {
                            richTextBox1.AppendText("wpsd id:" + wpsdid + "\nhard Ver:" + hardVer + "\ncar State:Have Car\n"); 
                        }
                        else
                        {
                            richTextBox1.AppendText("wpsd id:" + wpsdid + "\nhard Ver:" + hardVer + "\ncar State:No Car\n");
                        }    
                    }
                    #endregion
                });
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }
        /// <summary>
        /// <remarks>Received Data Show</remarks>
        /// </summary>
        /// <param name="text">Received data</param>
        public void reshow(byte[] text,bool source)
        {
            string restr = "";
            if (text != null)
            {
                for (int i = 0; i < text.Length; i++)
                {
                    restr += text[i].ToString("X2");
                    restr += " ";
                }
            }
            if (source)
            {
                richTextBox1.AppendText(System.DateTime.Now.ToString() + "[Received]:  " + restr + "\n");
            }
            else { richTextBox1.AppendText(System.DateTime.Now.ToString() + "[Send]:  " + restr + "\n"); }
        }
        
        #endregion

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            try
            {

                System.Environment.Exit(System.Environment.ExitCode);
            }
            catch (Exception err)
            {
                MessageBox.Show(err.Message);
            }
        }

       
    }
}
