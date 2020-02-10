using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using Dapper;
using System.Linq;
using System.Web;

namespace GSATPrediction.Models
{
    public class QueryData
    {
        private string conStr;
        private SqlCommand sqlcmd;
        private SqlConnection conn;
        private SqlDataAdapter buffer;
        private DataTable dt;

        public QueryData()
        {
            conStr = ConfigurationManager.ConnectionStrings["PredictionADO"].ConnectionString;
            conn = new SqlConnection(conStr);
            sqlcmd = new SqlCommand();
            buffer = null;
            dt = new DataTable();
        }

        public DataTable search(string cmd)
        {   
            try
            {
                sqlcmd.CommandText = cmd;
                sqlcmd.Connection = conn;
                sqlcmd.CommandTimeout = 60;
                buffer = new SqlDataAdapter(sqlcmd);
                buffer.Fill(dt);
                return dt;
            }
            catch(Exception ex)
            {
                return dt;
            }      
            
        }
    }
}