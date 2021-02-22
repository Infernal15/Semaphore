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
        private int numeric;
        private int prev;
        private static Dictionary<int, int> list;
        List<Thread> thd;
        public Form1()
        {
            InitializeComponent();
            Set_max();
            list = new Dictionary<int, int>();
            thd = new List<Thread>();
            prev = (int)numericUpDown1.Value;

            Sem = new System.Threading.Semaphore(prev, 10);
            numeric = 0;
        }

        private void Method(object obj)
        {
            listBox2.Invoke((Action)delegate() { listBox2.Items.Remove("Поток " + (int)obj + " --> Очікує"); });
            listBox1.Invoke((Action)delegate () { listBox1.Items.Add("Поток " + (int)obj + " --> 1"); });
            while (true)
            {
                Sem.WaitOne();
                if (!list.ContainsKey((int)obj))
                    list[(int)obj] = 1;
                else
                {
                    try
                    {
                        listBox1.Invoke((Action)delegate () { listBox1.Items.Remove("Поток " + (int)obj + $" --> {list[(int)obj]}"); });
                        list[(int)obj] = list[(int)obj] + 1;
                        listBox1.Invoke((Action)delegate () { listBox1.Items.Add("Поток " + (int)obj + $" --> {list[(int)obj]}"); });
                    }
                    catch
                    {

                    }
                }

                Sem.Release();
                Thread.Sleep(1000);
            }
        }

        private static void Down()
        {
            Sem.WaitOne();
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
            thd.Add(new Thread(Method));
            thd[numeric].Name = $"Поток {numeric + 1}";
            listBox3.Items.Add(thd[numeric].Name + " --> Створений");
            numeric++;
            Set_max();
        }

        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (listBox1.SelectedItem != null)
            {
                int temp = Convert.ToInt32(listBox1.SelectedItem.ToString().Split(' ')[1]);
                listBox1.Items.Remove(listBox1.SelectedItem.ToString());
                thd[temp - 1].Abort();
            }
            Set_max();
        }

        private void numericUpDown1_ValueChanged(object sender, EventArgs e)
        {
            if ((int)numericUpDown1.Value <= 100)
            {
                if (prev > (int)numericUpDown1.Value)
                {
                    Thread term = new Thread(Down);
                    term.Start();
                }
                else if (prev < (int)numericUpDown1.Value)
                {
                    Sem.Release();
                }
                prev = (int)numericUpDown1.Value;
                foreach (Thread temp in thd)
                {
                    Console.WriteLine(temp.ThreadState);
                }
            }
        }

        private void listBox3_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (listBox3.SelectedItem != null)
            {
                int temp = Convert.ToInt32(listBox3.SelectedItem.ToString().Split(' ')[1]);
                listBox2.Items.Add(thd[temp - 1].Name + " --> Очікує");
                listBox3.Items.Remove(listBox3.SelectedItem.ToString());
                thd[temp - 1].Start(temp);
            }
            Set_max();
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            for (int i = 0; i < thd.Count; i++)
            {
                thd[i].Abort();
            }
            Sem.Dispose();
        }

        private void listBox2_DisplayMemberChanged(object sender, EventArgs e)
        {
            Set_max();
        }
    }
}
