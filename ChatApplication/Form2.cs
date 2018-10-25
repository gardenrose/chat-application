using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;

namespace ChatApplication
{
    public partial class Form2 : Form
    {


        public Form2()
        {
            InitializeComponent();
        }

        private void button2_Click(object sender, EventArgs e)
        {


            if (string.IsNullOrEmpty(textBox3.Text))
            {
                MessageBox.Show("Username empty !");
            }
            else if (string.IsNullOrEmpty(textBox4.Text))
            {
                MessageBox.Show("Password empty !");
            }

            else if ((textBox4.Text != textBox5.Text))
            {
                MessageBox.Show("Passwords don't match !");
            }

            else
            {
                try
                {

                    using (System.IO.StreamWriter file =
                    new System.IO.StreamWriter(System.IO.Directory.GetCurrentDirectory() + "\\info.txt", true))
                    {

                        file.WriteLine(textBox3.Text + "," + textBox4.Text.GetHashCode());
                    }



                }
                catch (Exception exception) { }

                MessageBox.Show("Username and password set successfully !");

            }

        }

        private void button1_Click(object sender, EventArgs e)
        {
            

            string s;
            bool id = true;
            bool passw = true;
            try
            {
                System.IO.StreamReader file = new System.IO.StreamReader(System.IO.Directory.GetCurrentDirectory() + "\\info.txt");

                while ((s = file.ReadLine()) != null)
                {
                    if (textBox1.Text != s.Split(',')[0])
                    {
                        id = false;
                    }

                    else if (textBox2.Text.GetHashCode().ToString() != s.Split(',')[1].Trim())
                    {
                        passw = false;
                    }
                    else
                    {
                        id = true;
                        passw = true;
                        Form1 frm1 = new Form1(s.Split(',')[0], this);
                        frm1.Show();
                        this.Hide();
                        break;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }

            if (id == false)
            {
                MessageBox.Show("Username doesn't exist !");
            }
            else if (passw == false)
            {
                MessageBox.Show("Incorrect password !");
            }



        }


    }
}
