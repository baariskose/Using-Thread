using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Threading;
using System.Timers;
using System.Diagnostics;

namespace timeThread
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            chart1.Titles.Add("Time - Thread Line Chart");
            chart1.ChartAreas[0].AxisX.Interval = 1;
            chart1.ChartAreas[0].AxisY.Interval = 100;
        }

        public int[] array = new int[32];
        struct Param
        {
            public int index;
            public decimal start;
            public decimal finish;
        }

        static List<decimal> toplamlar;
        static decimal sayi = 10000000000; // 10^9
        static decimal time = 0;

        private void button1_Click(object sender, EventArgs e)
        {
            int x;
            int y;

            createTable(dataGridView1);

            new Thread(gauss).Start();
            for (int a = 1; a <= 32; a++)
            {
                List<Thread> threads = new List<Thread>();
                toplamlar = new List<decimal>();

                Param pr = new Param();
                decimal temp = sayi / a;
                pr.start = 1;
                pr.finish = Math.Floor(temp)  ;

                for (int j = 0; j < a; j++)
                {
                    toplamlar.Add(0);
                    pr.index = j;
                    Param localPr = pr;
                    threads.Add(new Thread(new ParameterizedThreadStart(Toplam)));
                    threads[j].Start(localPr);
                    pr.start = pr.finish + 1;
                    pr.finish = pr.start + temp;
                    pr.finish = pr.finish > sayi ? sayi : pr.finish;
                }

                foreach (var item in threads)
                {
                    item.Join();
                }
                decimal toplam = 0;

                foreach (var item in toplamlar)
                {
                    toplam = toplam + item;
                }

                listBox1.Items.Add(a + " iplik icin Sonuc = " + toplam + " Sure = " + time);
                dataGridView1.Rows[a].Cells[1].Value = time;
            }

            for (int i = 1; i <= 32; i++)
            {
                x = Convert.ToInt32(dataGridView1.Rows[i].Cells[0].Value);
                y = Convert.ToInt32(dataGridView1.Rows[i].Cells[1].Value);
                chart1.Series["time"].Points.AddXY(x, y);
            }
        }

        public void createTable(DataGridView table)
        {
            table.RowCount = 32;
            table.ColumnCount = 2;
            table.Columns[0].Name = "Thread";
            table.Columns[1].Name = "Time";
            table.Columns[0].Width = 65;
            table.Columns[1].Width = 65;
            int i = table.Rows.Add();
            for (i = 1; i <= 32; i++)
            {
                table.Rows[i].Cells[0].Value = i;
                table.Rows[i].Cells[1].Value = array[i-1];
            }
        }

        static void Toplam(object paramaters)
        {
            Param param = (Param)paramaters;
            decimal toplam = 0;
            var sw = Stopwatch.StartNew();
            for (decimal i = param.start; i <= param.finish; i++)
            {
                toplam = toplam + i;
            }
            sw.Stop();
            toplamlar[param.index] = toplam;
            time = sw.ElapsedMilliseconds;
        }

        static void gauss()
        {
            var sw = Stopwatch.StartNew();
            decimal cevap = sayi / 2;
            cevap = cevap * (sayi + 1);
            Console.Write("Gauss Metodu Sonucu = " + cevap + " Sure = ");
            sw.Stop();
            Console.Write(sw.ElapsedMilliseconds + "\n");
        }
    }
}
