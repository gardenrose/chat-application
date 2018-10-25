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
using System.Net.Sockets;
using System.Diagnostics;
namespace ChatApplication
{
    public partial class Form1 : Form
    {
        private byte[] bytearray = null;
        private Dictionary<string, Person> persons;
        Person selected = null;
        private string user;
        Form form1;
        public Form1() { }
        public Form1(string user, Form form1)
        {

            InitializeComponent();
            this.user = user;
            this.form1 = form1;
            persons = new Dictionary<string, Person>();
            int PORT = 15000;
            UdpClient udpClient = new UdpClient();
            udpClient.Client.Bind(new IPEndPoint(IPAddress.Any, PORT));
            richTextBox1.Enabled = false;

            var from = new IPEndPoint(0, 0);
            Task.Run(() =>
            {
                while (true)
                {
                    try
                    {
                        bytearray = udpClient.Receive(ref from);
                        var parts = Encoding.UTF8.GetString(bytearray).Split(new[] { ':' }, 2);
                        if ((parts[1] == "Online") && (from.Address.ToString() != getLocalIP()))
                        {
                            AddListViewItem(parts[0], from.Address);
                        }

                        if ((parts[1] == "Offline") && (from.Address.ToString() != getLocalIP()))
                        {
                            Person p;
                            bool b = persons.TryGetValue(from.Address.ToString(), out p);

                            if (b)
                            {
                                listView1.Invoke(new Action(() => listView1.Items.Remove(p.item)));
                                persons.Remove(from.Address.ToString());
                            }
                        }

                        if ((parts[0] == "Message"))
                        {
                            Person p;
                            persons.TryGetValue(from.Address.ToString(), out p);

                            p.messages.Add("[ " + p.name + " : " + from.Address.ToString() + " ]  " + parts[1]);
                            if (selected != null)
                            {
                                richTextBox1.Invoke(new Action(() => richTextBox1.Text += selected.messages.Last<string>() + "\n\n"));
                            }
                            if (selected == null || selected.ip != p.ip)
                            {
                                listView1.Invoke(new Action(() => p.item.Text += "  NEW MESSAGE"));
                            }
                        }
                    }
                    catch (Exception e) { MessageBox.Show(e.ToString()); }
                }
            });
            try
            {
                UdpClient client = new UdpClient(new IPEndPoint(IPAddress.Parse(getLocalIP()), 15000));
                IPEndPoint ip = new IPEndPoint(IPAddress.Broadcast, 15000);
                byte[] bytes = Encoding.ASCII.GetBytes(this.user + ":Online");
                client.Send(bytes, bytes.Length, ip);
                client.Close();
            }
            catch (Exception e)
            {
                MessageBox.Show(e.ToString());
            }

        }

        private string getLocalIP()
        {
            string localIP;
            using (Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, 0))
            {
                socket.Connect("8.8.8.8", 65530);
                IPEndPoint endPoint = socket.LocalEndPoint as IPEndPoint;
                localIP = endPoint.Address.ToString();
            }
            return localIP;
        }



        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            UdpClient client = new UdpClient(new IPEndPoint(IPAddress.Parse(getLocalIP()), 15000));
            IPEndPoint ip = new IPEndPoint(IPAddress.Broadcast, 15000);
            byte[] bytes = Encoding.ASCII.GetBytes(this.user + ":Offline");
            client.Send(bytes, bytes.Length, ip);
            client.Close();
            this.form1.Close();

        }

        private void AddListViewItem(string name, IPAddress ipaddress)
        {
            Person tmp;
            if (!persons.TryGetValue(ipaddress.ToString(), out tmp))
            {
                Person p = new Person();
                p.name = name;
                p.item = new ListViewItem(p.name);
                p.item.Tag = p;
                listView1.Invoke(new Action(() => listView1.Items.Add(p.item)));
                persons.Add(ipaddress.ToString(), p);
                p.ip = ipaddress.ToString();
                string s = getLocalIP();
                UdpClient client = new UdpClient(new IPEndPoint(IPAddress.Parse(s), 15000));
                byte[] bytes = Encoding.ASCII.GetBytes(this.user + ":Online");
                client.Send(bytes, bytes.Length, new IPEndPoint(ipaddress, 15000));
                client.Close();
            }
        }

        private void listView1_DoubleClick(object sender, EventArgs e)
        {
            selected = (Person)listView1.SelectedItems[0].Tag;
            richTextBox1.Clear();
            button1.Enabled = true;
            selected.item.Text = selected.name;
            foreach (string message in selected.messages)
            {
                richTextBox1.Text += message + "\n\n";
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (textBox1.Text != "")
            {
                UdpClient client = new UdpClient(new IPEndPoint(IPAddress.Parse(getLocalIP()), 15000));
                byte[] bytes = Encoding.ASCII.GetBytes("Message:" + textBox1.Text);
                client.Send(bytes, bytes.Length, new IPEndPoint(IPAddress.Parse(selected.ip), 15000));
                selected.messages.Add("[ You ]  " + textBox1.Text);
                client.Close();
                richTextBox1.Text += "[ You ]  " + textBox1.Text + "\n\n";
                textBox1.Text = "";
            }
        }
    }
}
