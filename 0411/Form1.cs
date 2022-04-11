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
using System.Threading;
using Microsoft.VisualBasic.PowerPacks;

namespace _0411
{
    public partial class Form1 : Form
    {
        Thread Th;
        UdpClient U;
        //NUget Microsoft.VisualBasic.PowerPacks.Vs 類似於pip install
        ShapeContainer C;
        ShapeContainer D;//畫布物件
        Point stP;//繪圖起點
        string p;//繪圖座標

        ShapeContainer C;//NUget 類似於pip install
        public Form1()
        {
            InitializeComponent();
        }

        private void Listen()
        {
            int port = int.Parse(txtbListenPort.Text);//接聽埠號
            U = new UdpClient(port);

            IPEndPoint EP = new IPEndPoint(IPAddress.Parse("127.0.0.1"), port);//本機接聽

            while (true)
            {
                byte[] B = U.Receive(ref EP);//收到EP存入B裡
                string A = Encoding.Default.GetString(B);//將B翻譯成字串
                string[] Z = A.Split('_');//取得_前的數字 e.g:p=1_80,70/2_50,70/....
                string[] Q = A.Split('/');//分割'/'所紀錄的位置
                string[] Q1=Q[0].Split('_');
                Q[0] = Q1[1];
                Point[] R = new Point[Q.Length];//建立Point物件
                for(int i = 0; i < Q.Length; i++)
                {
                    string[] K = Q[i].Split(',');//分割',' 0=X,1=Y座標
                    R[i].X = int.Parse(K[0]);
                    R[i].Y = int.Parse(K[1]);
        }

                for(int i = 0; i < Q.Length - 1; i++)
                {
                    LineShape L = new LineShape();//建立線段物件
                    L.StartPoint = R[i];//起始位置為R[i]
                    L.EndPoint = R[i + 1];//結束位置為下一個的R[i](下次的起始位置)
                    L.Parent = D;//線段加入至畫布D
                    switch (Z[0])
                    {
                        case "1":
                            L.BorderColor = Color.Red;
                            break;
                        case "2":
                            L.BorderColor = Color.Green;
                            break;
                        case "3":
                            L.BorderColor = Color.Blue;
                            break;
                        case "4":
                            L.BorderColor = Color.Black;
                            break;
                    }
                }
                Thread.Sleep(500);//休息500ms
            }

        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            try
            {
                Th.Abort();
                U.Close();
            }
            catch
            {

            }
        }

        private void btnConnent_Click(object sender, EventArgs e)
        {
            Control.CheckForIllegalCrossThreadCalls = false;
            Th = new Thread(Listen);
            Th.Start();
            btnConnent.Enabled = false;
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            C = new ShapeContainer();//建立畫布物件
            this.Controls.Add(C);//加入畫布C在form裡
            D = new ShapeContainer();
            this.Controls.Add(D);//加入畫布D在form裡
        }

        private void Form1_MouseDown(object sender, MouseEventArgs e)
        {
            stP = e.Location;//紀錄滑鼠點下的位置
            p = stP.X.ToString() + "," + stP.Y.ToString();
        }

        private void Form1_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.Button == System.Windows.Forms.MouseButtons.Left)//按下左鍵時
            {
                LineShape L = new LineShape();//建立線段物件
                L.StartPoint = stP;//開始位置
                L.EndPoint = e.Location;//滑鼠移動結束時的位置
                if (radioBtnRed.Checked)
                {
                    L.BorderColor = Color.Red;
                }
                if (radioBtnGreen.Checked)
                {
                    L.BorderColor = Color.Green;
                }
                if (radioBtnBlue.Checked)
                {
                    L.BorderColor = Color.Blue;
                }
                if (radioBtnBlack.Checked)
                {
                    L.BorderColor = Color.Black;
                }
                L.Parent = C;//加入線段到畫布C
                stP = e.Location;//終點設為下次的起點
                p+="/"+ stP.X.ToString() + "," + stP.Y.ToString();//持續記錄座標位置
            }
        }

        private void Form1_MouseUp(object sender, MouseEventArgs e)
        {
            //直接傳給對方
            int port = int.Parse(txtbTargetport.Text);//對方埠號
            UdpClient S = new UdpClient(txtbTargetIP.Text,port);
            if (radioBtnRed.Checked)
            {
                p = "1_" + p;
            }
            if (radioBtnGreen.Checked)
            {
                p = "2_" + p;
            }
            if (radioBtnBlue.Checked)
            {
                p = "3_" + p;
            }
            if (radioBtnBlack.Checked)
            {
                p = "4_" + p;
            }
            byte[] B = Encoding.Default.GetBytes(p);//p所記錄的位置
            S.Send(B, B.Length);
            S.Close();
        }
    }
}
