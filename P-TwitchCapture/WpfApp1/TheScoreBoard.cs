using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace PTwitchCapture
{
    class TheScoreBoard
    {
        public static string path1 = "score.txt";
        public static List<Data_UserData> list_user_all = new List<Data_UserData>();
        //public static List<Data_UserData> list_user_top10 = new List<Data_UserData>();//sorted, 0 as dummy




        public static void SaveAll()
        {
            List<string> s = new List<string>();
            foreach (Data_UserData u in list_user_all)
            {
                s.Add(u.total + "," + u.p1_cheer + "," + u.p1_jeer + "," + u.p2_cheer + "," + u.p2_jeer + "," + u.username);
            }
            TheTool.exportCSV_orTXT(path1, s, false);
            TheTool.log("SAVEEEEEEEEEE");
        }
        
        public static void UpdateTop10()
        {
            list_user_all = list_user_all.OrderByDescending(o => o.total).ToList();
            int i = 1;
            string url = "https://p-library.com/t/lab/ftg/db_update10.php?";
            foreach(Data_UserData d in list_user_all)
            {
                if (i > 1) { url += "&"; }
                url += "u" + i + "=" + d.username + "&s" + i + "=" + d.total;
                i++;
            }
            TheTool.log("URLCALL " + url);



            //Uri contoso = new Uri(url);
            //WebRequest wr = WebRequest.Create(contoso);

            try
            {
                HttpWebRequest myRequest = (HttpWebRequest)WebRequest.Create(url);
                //myRequest.Method = "GET";
                var resp = (HttpWebResponse)myRequest.GetResponse();
                resp.Close();
            }
            catch (Exception ex) { TheTool.log("WEBBBB " + ex); }

            //var data = new WebClient().DownloadString(url);
        }

        public static void LoadAll()
        {
            try
            {
                list_user_all.Clear();
                List<string> list_s = TheTool.read_File_getListString(path1);
                foreach (string s in list_s)
                {
                    TheTool.log(s + "READDDDDDD");
                    string[] s2 = TheTool.splitText(s, ",");
                    if (s2.Length >= 6)
                    {
                        Data_UserData d = new Data_UserData();
                        d.total = TheTool.getInt(s2[0]);
                        d.p1_cheer = TheTool.getInt(s2[1]);
                        d.p1_jeer = TheTool.getInt(s2[2]);
                        d.p2_cheer = TheTool.getInt(s2[3]);
                        d.p2_jeer = TheTool.getInt(s2[4]);

                        for (int i = 5; i < s2.Length; i++)
                        {
                            d.username += s2[i];
                        }
                        list_user_all.Add(d);
                    }
                }
            }
            catch (Exception ex) { TheTool.log(ex); }
        }

        //--------------------------------

        /* cheer P1 = +1
         * jeer O2 = -2
         * -1
         * +1
         */ 

        public static void UpdateUser(string username0, int type)
        {
            TheTool.log("UPDATEEEEE" + username0 + " " + type);
            Data_UserData target = null;
            foreach (Data_UserData d in list_user_all)
            {
                if (d.username == username0)
                {
                    target = d;
                    break;
                }
            }
            if (target == null)
            {
                target = new Data_UserData();
                target.username = username0;
                list_user_all.Add(target);
            }
            if (target != null)
            {
                if (type == 1)
                {
                    target.p1_cheer++;
                }
                else if (type == 2)
                {
                    target.p2_cheer++;
                }
                else if (type == -1)
                {
                    target.p1_jeer++;
                }
                else if (type == -2)
                {
                    target.p2_jeer++;
                }
                target.total++;
            }
        }
    }



        //--------------------------------





    }







    class Data_UserData
    {
        public String username = "";
        public int total = 0;
        public int p1_cheer = 0;
        public int p1_jeer = 0;
        public int p2_cheer = 0;
        public int p2_jeer = 0;
    }






