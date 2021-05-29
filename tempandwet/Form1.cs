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
using ZedGraph;


namespace tempandwet
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            this.zedbind();
            timer1.Start();
        }
        PointPairList list1 = new PointPairList();
        PointPairList list2 = new PointPairList();
        PointPairList list3 = new PointPairList();
        PointPairList list4 = new PointPairList();
        double Average1(double d1,double d2,double b)
        {
            double O1=0.0;
            O1 = 0.9 * d1 + (1 - 0.9) * d2;
            O1 = O1 / (1 - b);
            return O1;
        }
        double data1 = 0.0;
        double data2 = 0.0;
        double miao = 0;
        double buf = 0.0;
        double b = 0.9;
        double o=0.0;
        private void readdata()
        {
            try
            {
                miao = (double)new XDate(DateTime.Now.AddMilliseconds(-50));

                Random rd = new Random();
                buf = data1;

                data1 = 20 + rd.Next(6, 9);
                data2 = 80 + rd.Next(5, 16);
                if (list1.Count > 50)
                    list1.RemoveAt(0);
                if (list2.Count > 50)
                    list2.RemoveAt(0);
                if (list3.Count > 50)
                    list3.RemoveAt(0);
                if (list4.Count > 50)
                    list4.RemoveAt(0);
                list1.Add(miao, data1);
                list2.Add(miao, data2);
                list3.Add(miao, data1);
                list4.Add(miao, data1);
                buf = o;
                b = b * 0.9;
                this.zedrefreshmethod();
                Thread.Sleep(50);
            }
            catch (Exception err)
            {
                MessageBox.Show(err.ToString());
            }

        }
        private void zedbind()
        {
            //Y1
            zedGraphControl1.GraphPane.YAxis.Title.Text = "浓度/ppm";
            zedGraphControl1.GraphPane.YAxis.Scale.FontSpec.FontColor = Color.Black;
            zedGraphControl1.GraphPane.YAxis.Title.FontSpec.FontColor = Color.Black;
            zedGraphControl1.GraphPane.YAxis.Scale.Min =0;
            zedGraphControl1.GraphPane.YAxis.Scale.Max = 255;
            zedGraphControl1.GraphPane.YAxis.Scale.MajorStep = 15;
            zedGraphControl1.GraphPane.YAxis.Scale.MinorStep = 5;
            zedGraphControl1.GraphPane.YAxis.Scale.MinorStepAuto = true;
            zedGraphControl1.GraphPane.YAxis.Scale.MajorStepAuto = true;
            zedGraphControl1.GraphPane.YAxis.Title.FontSpec.Size = 20;  
            //
            //Y2
            zedGraphControl2.GraphPane.YAxis.Title.Text = "温度/℃";
            zedGraphControl2.GraphPane.YAxis.Scale.FontSpec.FontColor = Color.Black;
            zedGraphControl2.GraphPane.YAxis.Title.FontSpec.FontColor = Color.Black;
            zedGraphControl2.GraphPane.YAxis.Scale.Min = 25;
            zedGraphControl2.GraphPane.YAxis.Scale.Max = 35;
            zedGraphControl2.GraphPane.YAxis.Scale.MajorStep = 5;
            zedGraphControl2.GraphPane.YAxis.Scale.MinorStep = 1;
            zedGraphControl2.GraphPane.YAxis.Scale.MinorStepAuto = true;
            zedGraphControl2.GraphPane.YAxis.Scale.MajorStepAuto = true;
            zedGraphControl2.GraphPane.YAxis.Title.FontSpec.Size = 20;
            //
            zedGraphControl1.GraphPane.XAxis.Title.Text = "时间/mm:ss";
            zedGraphControl2.GraphPane.XAxis.Title.Text = "时间/mm:ss";
            
            zedGraphControl1.GraphPane.XAxis.Type = ZedGraph.AxisType.DateAsOrdinal;
            zedGraphControl1.GraphPane.Title.IsVisible=false;
            zedGraphControl1.GraphPane.XAxis.Title.FontSpec.Size = 20;

            zedGraphControl2.GraphPane.XAxis.Type = ZedGraph.AxisType.DateAsOrdinal;
            zedGraphControl2.GraphPane.Title.IsVisible=false;
            zedGraphControl2.GraphPane.XAxis.Title.FontSpec.Size = 20;

            LineItem l1=zedGraphControl1.GraphPane.AddCurve("CO", list1, Color.Blue);
            LineItem l2=zedGraphControl1.GraphPane.AddCurve("CH4", list2, Color.Red);
            LineItem l3=zedGraphControl2.GraphPane.AddCurve("Heat_temp", list3, Color.Green);

        }
        private delegate void ZedRefreshHandle();
        private void zedrefreshmethod()
        {
            if (this.InvokeRequired)
            {
                ZedRefreshHandle zrh = new ZedRefreshHandle(zedrefreshmethod);
                this.Invoke(zrh, new object[] { });
            }
            else
            {
                zedGraphControl1.AxisChange();
                zedGraphControl1.Refresh();
                zedGraphControl2.AxisChange();
                zedGraphControl2.Refresh();
            }
        }

        private void Form1_SizeChanged(object sender, EventArgs e)
        {
            zedGraphControl1.Size = new Size(this.Size.Width - 162, (this.Size.Height - 115) / 2);
            zedGraphControl2.Size = new Size(this.Size.Width - 162, (this.Size.Height - 115) / 2);

        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (serialPort1.IsOpen)
            {
                try
                {
                    serialPort1.Close();
                }
                catch { }
            }
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            readdata();
        }
    }
}
