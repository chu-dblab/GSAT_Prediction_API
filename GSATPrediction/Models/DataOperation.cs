using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;
using System.Collections;

namespace PredictionAPI.Models
{
    public class DataOperation
    {
        private string conStr = ConfigurationManager.ConnectionStrings["PredictionADO"].ConnectionString;
        private SqlConnection conn;
        private DataTable dt;
        public DataOperation()
        {
            this.conn = new SqlConnection(conStr);
            dt = new DataTable();
        }

        /// <summary>
        /// 將學測級分轉換成等級
        /// </summary>
        /// <param name="gsat">學測成績</param>
        /// <returns>學測等級</returns>
        public ArrayList changeScoreOfGSAT2Level(Gsat gsat)
        {
            string sqlCom = null;
            int LV = 0;
            int sum = gsat.Chinese + gsat.English + gsat.Math + gsat.Science + gsat.Society;
            int[] score104OfGSAT = {gsat.Chinese, gsat.English, gsat.Math, gsat.Science, gsat.Society,sum };
            string[] subjectOfGSAT = { "國文", "英文", "數學", "自然", "社會" ,"總級分"};
            SqlDataAdapter buffer = null;
            ArrayList level = new ArrayList();
            this.conn.Open();
            for (int i = 0; i < 6; i++)
            {
                sqlCom = "SELECT Grade1,Grade2,Grade3,Grade4,Grade5 " +
                    "FROM T WHERE Tname = '" + subjectOfGSAT[i] + "'";
                buffer = new SqlDataAdapter(sqlCom, this.conn);
                buffer.Fill(dt);
                this.conn.Close();

                if (score104OfGSAT[i] < Convert.ToInt32(dt.Rows[0]["Grade1"].ToString())) LV = 0;
                else if (score104OfGSAT[i] < Convert.ToInt32(dt.Rows[0]["Grade2"].ToString())) LV = 1;
                else if (score104OfGSAT[i] < Convert.ToInt32(dt.Rows[0]["Grade3"].ToString())) LV = 2;
                else if (score104OfGSAT[i] < Convert.ToInt32(dt.Rows[0]["Grade4"].ToString())) LV = 3;
                else if (score104OfGSAT[i] < Convert.ToInt32(dt.Rows[0]["Grade5"].ToString())) LV = 4;
                else LV = 5;
                level.Add(LV);
                dt.Clear();
            }
            buffer.Dispose();
            this.conn.Close();
            return level;
        }
    }
}