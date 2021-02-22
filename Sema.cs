using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Semaphore
{
    class Sema
    {
        private static System.Threading.Semaphore Sem;
        private static Dictionary<int, int> list;
        List<Thread> thd;
        public Sema(int prev, Dictionary<int, int> rem, List<Thread> tpd)
        {
            Sem = new System.Threading.Semaphore(prev, 10);
            list = rem;
            thd = tpd;
            for (int num = 0; num < 5; num++)
            {
                if(thd.Count <= num)
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

        public void Up()
        {
            Sem.Release();
        }

        public void Down()
        {
            Sem.WaitOne();
        }
    }
}
