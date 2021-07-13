﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PTwitchCapture
{
    class Analysis2
    {
        List<TMessage> list_msg = new List<TMessage>();
        

        //Index: 0 Total, 1 P1, P2
        public int[] score_pos = { 0, 0, 0 };//positive: Total, P1, P2
        public int[] score_neg = { 0, 0, 0 };
        //
        public double dominance = 0;//[-1,1]
        public int bal_gui = 0;//[-100,100]
        public int bal_hp;
        //
        public double f_p1 = 0;
        public double f_p2 = 0;
        public double f_p1_gui = 0;
        public double f_p2_gui = 0;
        static public MainWindow mainWin1 = new MainWindow();
        int timeWindow = 15000;//15sec

        public void reset()
        {
            list_msg.Clear();

            score_pos = new int[] { 0, 0, 0 };
            score_neg = new int[] { 0, 0, 0 };
            dominance = 0;
            bal_gui = 0;
            bal_hp = 0;
            f_p1 = 0;
            f_p2 = 0;
            f_p1_gui = 0;
            f_p2_gui = 0;
        }

        public void doUpdate()
        {
            while (list_msg.Count > 0)
            {
                TMessage m = list_msg[0];
                TimeSpan tspan = DateTime.Now - list_msg[0].time;
                if (tspan.TotalMilliseconds > timeWindow)
                {
                    processMsg(m,true);
                    Console.WriteLine("Remove: " + m.txt);
                    list_msg.RemoveAt(0);
                }
                else
                {
                    break;
                }
            }
            updateVariables();
        }

        //add or remove value
        void processMsg(TMessage m, Boolean remove)
        {
            int i = 1;
            if (remove) { i = -1; }
            if (m.type == 1)
            {
                score_pos[0] += i;
                score_pos[1] += i;
            }
            else if (m.type == 2)
            {
                score_pos[0] += i;
                score_pos[2] += i;
            }
            else if (m.type == 3)
            {
                score_neg[0] += i;
                score_neg[1] += i;
            }
            else if (m.type == 4)
            {
                score_neg[0] += i;
                score_neg[2] += i;
            }
            //Console.WriteLine("testProcess: " + score_pos[0]);
        }


        public void updateVariables()
        {
            double d1 = score_pos[1] - score_neg[1] - score_pos[2] + score_neg[2];
            double d2 = score_pos[0] + score_neg[0];
            if (d2 > 0) { dominance = d1 / d2; }
            else { dominance = 0; }
            //----------------------


            int targetWinner = MainWindow.favorCharacter;
            //if (targetWinner == 1) { dominance = getRescale(dominance, -1,1, 0, 1); }
            //else if (targetWinner == 2) { dominance = getRescale(dominance, -1, 1, -1, 0); }

            if (targetWinner == 1) { dominance = (dominance+1)/2; }
            else if (targetWinner == 2) { dominance = ((dominance + 1)/ 2)-1; }

            //-------------------------
            bal_gui = (int)Math.Round(dominance * 100);
            bal_hp = (int)Math.Round(dominance * 120);
            f_p1 = -dominance;
            f_p2 = dominance;
            f_p1_gui = bal_gui;
            f_p2_gui = -bal_gui;
        }




        public static double getRescale(double v, double min, double max, double newmin, double newmax)
        {
            double r = max - min;
            double r2 = newmax - newmin;

            double v2 = ((v - min) / r * r2) + newmin; // 0--1 = 1/2*1 = 0.5 + 0 = -0.5          // 0--1 = 1/2*1 = 0.5+ -1
            return v2;
        }

        //public static double getRescale(double v,
        //double min, double max, double newmin, double newmax)
        //{
        //    double rnew = newmax - newmin;
        //    double n = getNormalized(v, min, max);
        //    return 1 * rnew - newmin ;
        //}


        //public static double getNormalized(double v, double min, double max)
        //{
        //    double r = max - min;
        //    if (r > 0)
        //    {
        //        v = v - min;
        //        v = v / r; return v;
        //    }
        //    else { return 0; }
        //}



        //------------

        public void addMsg(string m0, Boolean skipP2)
        {
            string m = m0.Replace("p","P");
            if (m.Contains("1"))
            {
                TMessage tm = new TMessage() { txt = m, type = 1};
                list_msg.Add(tm);
                Console.WriteLine("test P1+++++++++++++++");
                //mainWin1.countExport();
                processMsg(tm, false);

            }
            if (!skipP2 && m.Contains("AutoCheer"))
            {
                TMessage tm = new TMessage() { txt = m, type = 2 };
                list_msg.Add(tm);
                //Console.WriteLine("test P2+++++++++++++++");
                processMsg(tm, false);
            }
            if (!skipP2 && m.Contains("AutoJeer"))
            {
                TMessage tm = new TMessage() { txt = m, type = 3 };
                list_msg.Add(tm);
                //Console.WriteLine("test P1---------------");
                processMsg(tm, false);
            }
            if (m.Contains("2"))
            {
                TMessage tm = new TMessage() { txt = m, type = 4 };
                list_msg.Add(tm);
                Console.WriteLine("test P2---------------");
                //mainWin1.countExport();
                processMsg(tm, false);
            }

            /*string m = m0.Replace("p", "P");
            if (m.Contains("P1+"))
            {
                TMessage tm = new TMessage() { txt = m, type = 1 };
                list_msg.Add(tm);
                Console.WriteLine("test P1+++++++++++++++");
                //mainWin1.countExport();
                processMsg(tm, false);

            }
            if (!skipP2 && m.Contains("P2+"))
            {
                TMessage tm = new TMessage() { txt = m, type = 2 };
                list_msg.Add(tm);
                //Console.WriteLine("test P2+++++++++++++++");
                processMsg(tm, false);
            }
            if (!skipP2 && m.Contains("P1-"))
            {
                TMessage tm = new TMessage() { txt = m, type = 3 };
                list_msg.Add(tm);
                //Console.WriteLine("test P1---------------");
                processMsg(tm, false);
            }
            if (m.Contains("P2-"))
            {
                TMessage tm = new TMessage() { txt = m, type = 4 };
                list_msg.Add(tm);
                Console.WriteLine("test P2---------------");
                //mainWin1.countExport();
                processMsg(tm, false);
            }*/
        }



        public void addMsgVS(string m0)
        {
            string m = m0.Replace("p", "P");
            if (m.Contains("P1+"))
            {
                TMessage tm = new TMessage() { txt = m, type = 1 };
                list_msg.Add(tm);
                Console.WriteLine("test P1+++++++++++++++");
                //mainWin1.countExport();
                processMsg(tm, false);

            }
            if ( m.Contains("P2+"))
            {
                TMessage tm = new TMessage() { txt = m, type = 2 };
                list_msg.Add(tm);
                Console.WriteLine("test P2+++++++++++++++");
                processMsg(tm, false);
            }
            if (m.Contains("P1-"))
            {
                TMessage tm = new TMessage() { txt = m, type = 3 };
                list_msg.Add(tm);
                Console.WriteLine("test P1---------------");
                processMsg(tm, false);
            }
            if (m.Contains("P2-"))
            {
                TMessage tm = new TMessage() { txt = m, type = 4 };
                list_msg.Add(tm);
                Console.WriteLine("test P2---------------");
                //mainWin1.countExport();
                processMsg(tm, false);
            }

            /*string m = m0.Replace("p", "P");
            if (m.Contains("P1+"))
            {
                TMessage tm = new TMessage() { txt = m, type = 1 };
                list_msg.Add(tm);
                Console.WriteLine("test P1+++++++++++++++");
                //mainWin1.countExport();
                processMsg(tm, false);

            }
            if (!skipP2 && m.Contains("P2+"))
            {
                TMessage tm = new TMessage() { txt = m, type = 2 };
                list_msg.Add(tm);
                //Console.WriteLine("test P2+++++++++++++++");
                processMsg(tm, false);
            }
            if (!skipP2 && m.Contains("P1-"))
            {
                TMessage tm = new TMessage() { txt = m, type = 3 };
                list_msg.Add(tm);
                //Console.WriteLine("test P1---------------");
                processMsg(tm, false);
            }
            if (m.Contains("P2-"))
            {
                TMessage tm = new TMessage() { txt = m, type = 4 };
                list_msg.Add(tm);
                Console.WriteLine("test P2---------------");
                //mainWin1.countExport();
                processMsg(tm, false);
            }*/
        }






        public string getMsgVar()
        {
            String s = TheTool.getTime2() + Environment.NewLine;
            s += "Dominance: " + Math.Round(dominance, 2) + Environment.NewLine;
            if (bal_hp >= 0)
            {
                s += "P1 wins by ΔHP " + bal_hp + Environment.NewLine;
            }
            else
            {
                s += "P2 wins by ΔHP " + -bal_hp + Environment.NewLine;
            }
            s += Environment.NewLine;
            s += "Score All: +" + score_pos[0] + " -" + score_neg[0] + Environment.NewLine;
            s += "Score P1: +" + score_pos[1] + " -" + score_neg[1] + Environment.NewLine;
            s += "Score P2: +" + score_pos[2] + " -" + score_neg[2] + Environment.NewLine;
            s += Environment.NewLine;
            s += "P1's F: " + Math.Round(f_p1, 2) + Environment.NewLine;
            s += "P2's F: " + Math.Round(f_p2, 2) + Environment.NewLine;
            return s;
        }

    }



    class TMessage
    {
        public String txt = "";
        public int type = 0;//0,1:P1+,2:P2+,3:P1-,2:P2-
        public DateTime time = DateTime.Now;
    }

   

}
