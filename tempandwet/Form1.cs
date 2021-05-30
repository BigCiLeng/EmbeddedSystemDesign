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
using System.IO.Ports;//SerialPort 命名空间
using System.IO;

namespace tempandwet
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            serialPort1.Encoding = Encoding.GetEncoding("GB2312");   //串口编码引入GB2312编码(汉字编码)
            //防止跨线程操作空间异常
            Control.CheckForIllegalCrossThreadCalls = false;   //取消跨线程检查
            this.zedbind();
            timer1.Start();
        }
        PointPairList list1 = new PointPairList();
        PointPairList list2 = new PointPairList();
        PointPairList list3 = new PointPairList();
        PointPairList list4 = new PointPairList();

        //端口号扫描按钮
        private void button1_Click_1(object sender, EventArgs e)
        {
            ReflashPortToComboBox(serialPort1, comboBox1);
        }

        //自动扫描可用串口并添加到串口号列表上
        private void ReflashPortToComboBox(SerialPort serialPort, ComboBox comboBox)
        {                                                               //将可用端口号添加到ComboBox
            if (!serialPort.IsOpen)//串口处于关闭状态
            {
                comboBox.Items.Clear();
                comboBox2.Items.Clear();
                string[] str = SerialPort.GetPortNames();
                if (str == null)
                {
                    MessageBox.Show("本机没有串口！", "Error");
                    return;
                }
                //添加串口
                foreach (string s in str)
                {
                    comboBox.Items.Add(s);
                    Console.WriteLine(s);
                }
                comboBox2.Items.Add("9600");
            }
            else
            {
                MessageBox.Show("串口处于打开状态不能刷新串口列表", "Error");
            }
        }
        //串口打开按钮
        private void button2_Click(object sender, EventArgs e)
        {
            Int32 iBaudRate = Convert.ToInt32(comboBox2.SelectedItem.ToString());//获取波特率下拉框里选中的波特率数据从字符串转为int32

            serialPort1.PortName = comboBox1.SelectedItem.ToString();//串口号
            serialPort1.BaudRate = iBaudRate;//波特率
            try
            {
                serialPort1.Open();
            }
            catch (Exception ex)
            {
                MessageBox.Show("串口打开失败" + ex, "error");
            }
        }


        double co=0, ch4=0;
        double heat_temp = 0;
        double temp = 0, humi = 0;
        double light = 0, pre = 0;
        //串口数据接受事件
        private void serialPort1_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            Thread.Sleep(100);
            try
            {
                string content = serialPort1.ReadExisting();//从串口控件读取输入流返回为string
                string[] strdata1 = content.Split(' ');
                co=double.Parse(strdata1[3]);
                ch4=double.Parse(strdata1[4]);
                heat_temp = double.Parse(strdata1[7]);
                temp = double.Parse(strdata1[1]);
                humi = double.Parse(strdata1[2]);
                light = double.Parse(strdata1[5]);
                pre = double.Parse(strdata1[6]);
            }
            catch (Exception ex)
            {
                MessageBox.Show("数据接受出错" + ex, "error");
            }
        }

        //发送数据按钮
        private void senddata()
        {
            string flag="a";
            if (radioButton1.Checked)
            {
                flag = "a";
            }else if (radioButton2.Checked)
            {
                flag = "b";
            }else if (radioButton3.Checked)
            {
                flag = "c";
            }else if (radioButton4.Checked)
            {
                flag = "d";
            }
            if (serialPort1.IsOpen)//判断串口是否打开，如果打开执行下一步操作
            {
                        try
                        {
                            serialPort1.Write(flag);//发送数据
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show(ex.Message, "串口数据写入错误");//出错提示
                            serialPort1.Close();
                        }

            }
        }


        double Average1(double d1,double d2,double b)
        {
            double O1=0.0;
            O1 = 0.9 * d1 + (1 - 0.9) * d2;
            O1 = O1 / (1 - b);
            return O1;
        }
        double data1 = 0.0;
        double data2 = 0.0;
        double data3 = 0.0;
        double miao = 0;
        private void readdata()
        {
            try
            {
                miao = (double)new XDate(DateTime.Now.AddMilliseconds(-50));
                data1 = co;
                data2 = ch4;
                data3 = heat_temp;
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
                list3.Add(miao, data3);
                list4.Add(miao, data1);
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
        bool timerflag = false;
        private void timer1_Tick(object sender, EventArgs e)
        {
            if (!timerflag)
            {
                senddata();
                timerflag = true;
            }
            else
            {
                readdata();
                timerflag = false;
            }
        }
    }
}
