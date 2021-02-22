using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Semaphore
{
    public partial class Form1 : Form
    {
        private static System.Threading.Semaphore Sem;
        private int num;
        private int prev;
        private static Dictionary<int, int> list;
        List<Thread> thd;
        public Form1()
        {
            InitializeComponent();
            listBox1.Items.Add(1);
            Set_max();
            list = new Dictionary<int, int>();
            thd = new List<Thread>();
            prev = (int)numericUpDown1.Value;

            Sem = new System.Threading.Semaphore(prev, 10);
            num = 0;

            for (int num = 0; num < 5; num++)
            {
                thd.Add(new Thread(Method));

                thd[num].Start(num);
            }

            Thread.Sleep(500);
        }

        private static void Method(object obj)
        {
            while (true)
            {
                Sem.WaitOne();
                if (!list.ContainsKey((int)obj))
                    list[(int)obj] = 1;
                else
                    list[(int)obj] = list[(int)obj] + 1;

                Console.WriteLine((int)obj + "   " + list[(int)obj]);

                Thread.Sleep(1000);
                Sem.Release();
            }
        }


        private void Change_position()
        {
            button1.Top = this.Size.Height - button1.Size.Height - 55;
            numericUpDown1.Top = this.Size.Height - numericUpDown1.Size.Height - 55;
            label4.Top = numericUpDown1.Location.Y - label4.Size.Height - 15;
        }

        private void Set_max()
        {
            int c = 15;
            int max = listBox1.Items.Count;
            if (max < listBox2.Items.Count)
                max = listBox2.Items.Count;

            if (max < listBox3.Items.Count)
                max = listBox3.Items.Count;

            if (max == 1)
                c = 20;

            listBox1.Size = new Size(listBox1.Size.Width, max * c);
            listBox2.Size = new Size(listBox2.Size.Width, max * c);
            listBox3.Size = new Size(listBox3.Size.Width, max * c);

            this.Size = new Size(this.Size.Width, listBox1.Size.Height + 150);

            Change_position();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            listBox1.Items.Add(1);
            Set_max();
        }

        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            MessageBox.Show("temp3");
        }

        private void numericUpDown1_ValueChanged(object sender, EventArgs e)
        {
            if ((int)numericUpDown1.Value <= 100)
            {
                Sem.Dispose();
                Sem = new System.Threading.Semaphore((int)numericUpDown1.Value, 10);
                Sem.Release((int)numericUpDown1.Value);
                //Sem.Release();
                Console.WriteLine(Sem.Release());
                foreach(Thread temp in thd)
                {
                    Console.WriteLine(temp.ThreadState);
                }
            }
        }
    }
}
