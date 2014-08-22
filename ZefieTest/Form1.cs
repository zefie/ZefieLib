using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Resources;

namespace ZefieTest
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Zefie.Prompts.ShowMsg("This is a message test!");
        }

        private void button4_Click(object sender, EventArgs e)
        {
            Zefie.Prompts.ShowError("This is an error test!");
        }

        private void button2_Click(object sender, EventArgs e)
        {
            checkBox1.Checked = Zefie.Prompts.ShowConfirm("This is a confirmation test!");
        }

        private void button3_Click(object sender, EventArgs e)
        {
            textBox1.Text = Zefie.Prompts.ShowPrompt("This is a prompt test!");
        }

        private void button5_Click(object sender, EventArgs e)
        {
            Zefie.Prompts.ShowMsg("This is a message test!", "This message has a title");
        }

        private void button7_Click(object sender, EventArgs e)
        {
            Zefie.Prompts.ShowError("This is an error test!", "This error has a title");
        }

        private void button8_Click(object sender, EventArgs e)
        {
            checkBox1.Checked = Zefie.Prompts.ShowConfirm("bla Bla bla b bla bla bla bla b blla bs b bla bla bla bla bla bla bla bla bla bla bla bla bla Bla bla bla bla bla bla bla bla bla bla Bla bla bla bla bla bla bla bla bla bla Bla bla bla bla bla bla bla bla bla bla Bla bla bla bla bla bla bla bla bla");

        }

        private void button6_Click(object sender, EventArgs e)
        {
            textBox1.Text = Zefie.Prompts.ShowPrompt("bla Bla bla b bla bla bla bla b blla bs b bla bla bla bla bla bla bla bla bla bla bla bla bla Bla bla bla bla bla bla bla bla bla bla Bla bla bla bla bla bla bla bla bla bla Bla bla bla bla bla bla bla bla bla bla Bla bla bla bla bla bla bla bla bla");
        }

        private void button9_Click(object sender, EventArgs e)
        {
            textBox3.Text = Zefie.Bemani.eAmuseCardGen();
        }

        private void button10_Click(object sender, EventArgs e)
        {
            textBox3.Text = Zefie.Math.random(100).ToString();
        }

        private void button11_Click(object sender, EventArgs e)
        {
            textBox3.Text = Zefie.Math.random(100, 200).ToString(); ;
        }

        private void button12_Click(object sender, EventArgs e)
        {
            textBox3.Text = Zefie.Strings.genString(20);
        }

        private void button13_Click(object sender, EventArgs e)
        {
            char[] append = { '$', '%', '@', '!', '&', '(', ')', '[', ']', '|', '.', '?', '<', '>' };
            textBox3.Text = Zefie.Strings.genString(20, append);
        }

        private void button14_Click(object sender, EventArgs e)
        {
            textBox3.Text = Zefie.Cryptography.genCryptoNumber().ToString();
        }

        private void button15_Click(object sender, EventArgs e)
        {
            textBox3.Text = Zefie.Cryptography.genCryptoString(20);
        }

        private void button16_Click(object sender, EventArgs e)
        {
            char[] append = { '$', '%', '@', '!', '&', '(', ')', '[', ']', '|', '.', '?', '<', '>' };
            textBox3.Text = Zefie.Cryptography.genCryptoString(20, append);
        }

        private void button17_Click(object sender, EventArgs e)
        {
            textBox3.Text = Zefie.Cryptography.genCryptoNumber(1, 1000).ToString();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            comboBox1.Items.Add("32-bit");
            comboBox1.Items.Add("64-bit");
            comboBox1.Items.Add("128-bit");
            comboBox1.Items.Add("256-bit");
            comboBox1.Items.Add("512-bit");
            comboBox1.Items.Add("1024-bit");
            comboBox1.Items.Add("2048-bit");
            comboBox1.SelectedIndex = 0;
            pictureBox1.Image = Zefie.Imaging.Scale(Properties.Resources.IMG_0130,320,240);
        }

        private void button18_Click(object sender, EventArgs e)
        {
            textBox3.Text = Zefie.Cryptography.genHash(Convert.ToInt32(comboBox1.Text.Replace("-bit", "")));
        }

        private void button19_Click(object sender, EventArgs e)
        {
            pictureBox1.Image = Zefie.Imaging.invertColors(pictureBox1.Image);
        }

        private void button20_Click(object sender, EventArgs e)
        {
            Zefie.Data.rawISO9660Mode = false;
            string[] test = Zefie.Data.listISO9660Files("D:\\ubuntu-12.04-desktop-i386.iso");
            foreach (string s in test)
                Zefie.Prompts.ShowMsg(s);
        }
    }
}
