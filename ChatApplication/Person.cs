using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;
using System.Windows.Forms;

namespace ChatApplication
{
    public class Person
    {
        public string name;
        public ListViewItem item;
        public string ip;
        public List<string> messages;


        public Person()
        {
            messages = new List<string>();
        }
    }

}
