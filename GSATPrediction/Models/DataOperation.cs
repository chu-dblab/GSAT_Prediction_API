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
            int sum = (gsat.Chinese == null ? 0 : Convert.ToInt32(gsat.Chinese) )+ 
                (gsat.English == null ? 0 : Convert.ToInt32(gsat.English)) +(gsat.Math == null ? 0 : Convert.ToInt32(gsat.Math)) +
                (gsat.Science == null ? 0 : Convert.ToInt32(gsat.Science)) + (gsat.Society == null ? 0 : Convert.ToInt32(gsat.Society));

            int[] score104OfGSAT = { gsat.Chinese == null ? 0 : Convert.ToInt32(gsat.Chinese),
                                     gsat.English == null ? 0 : Convert.ToInt32(gsat.English),
                                     gsat.Math == null ? 0 :Convert.ToInt32(gsat.Math),
                                     gsat.Science == null ? 0 : Convert.ToInt32(gsat.Science),
                                     gsat.Society == null ? 0 : Convert.ToInt32(gsat.Society), sum };
        
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

        public Dictionary<string, int> turnToOldScore(Gsat gsat)
        {
            string sqlCom = null;
            SqlDataAdapter buffer = null;
            Dictionary<string,int> oldScore = new Dictionary<string, int>();
            int sum = gsat.Chinese == null ? 0 : Convert.ToInt32(gsat.Chinese) +
                     gsat.English == null ? 0 : Convert.ToInt32(gsat.English) +
                     gsat.Math == null ? 0 : Convert.ToInt32(gsat.Math) +
                     gsat.Science == null ? 0 : Convert.ToInt32(gsat.Science) +
                     gsat.Society == null ? 0 : Convert.ToInt32(gsat.Society);

            int[] newScore = {  gsat.Chinese == null ? 0 : Convert.ToInt32(gsat.Chinese),
                                     gsat.English == null ? 0 : Convert.ToInt32(gsat.English),
                                     gsat.Math == null ? 0 :Convert.ToInt32(gsat.Math),
                                     gsat.Science == null ? 0 : Convert.ToInt32(gsat.Science),
                                     gsat.Society == null ? 0 : Convert.ToInt32(gsat.Society), sum  };

            string[] subject= { "國文", "英文", "數學", "自然", "社會", "總級分" };

            oldScore.Clear();
            this.conn.Open();
            for (int i = 0; i < subject.Length; i++)
            {
                sqlCom = "SELECT Max(OldScoreData.Score) As Score FROM OldScoreData,NewScoreData WHERE OldScoreData.Ename = '" + subject[i] + "' " +
                    "AND NewScoreData.Ename = '" + subject[i] + "' AND NewScoreData.Score = " + newScore[i].ToString() + " AND NewScoreData.Percentage >= OldScoreData.Percentage;";
                buffer = new SqlDataAdapter(sqlCom, this.conn);
                buffer.Fill(dt);
                if (dt.Rows[0].IsNull("Score")) oldScore.Add(subject[i],0);
                else oldScore.Add(subject[i],Convert.ToInt32(dt.Rows[0]["Score"]));
                dt.Clear();
            }
            buffer.Dispose();
            conn.Close();
            return oldScore;
        }

        public Dictionary<string,int> computeAllSubjectCombination(Dictionary<string, int> oldScore)
        {
            Dictionary<string, int> combination = oldScore;
            combination.Add("OCE", oldScore["國文"] + oldScore["英文"]);
            combination.Add("OCM", oldScore["國文"] + oldScore["數學"]);
            combination.Add("OCS", oldScore["國文"] + oldScore["社會"]);
            combination.Add("OCN", oldScore["國文"] + oldScore["自然"]);
            combination.Add("OCT", oldScore["國文"] + oldScore["總級分"]);
            combination.Add("OEM", oldScore["英文"] + oldScore["數學"]);
            combination.Add("OES", oldScore["英文"] + oldScore["社會"]);
            combination.Add("OEN", oldScore["英文"] + oldScore["自然"]);
            combination.Add("OET", oldScore["英文"] + oldScore["總級分"]);
            combination.Add("OMS", oldScore["數學"] + oldScore["社會"]);
            combination.Add("OMN", oldScore["數學"] + oldScore["自然"]);
            combination.Add("OMT", oldScore["數學"] + oldScore["總級分"]);
            combination.Add("OSN", oldScore["社會"] + oldScore["自然"]);
            combination.Add("OST", oldScore["社會"] + oldScore["總級分"]);
            combination.Add("ONT", oldScore["自然"] + oldScore["總級分"]);
            combination.Add("OCEM", oldScore["國文"] + oldScore["英文"] + oldScore["數學"]);
            combination.Add("OCES", oldScore["國文"] + oldScore["英文"] + oldScore["社會"]);
            combination.Add("OCEN", oldScore["國文"] + oldScore["英文"] + oldScore["自然"]);
            combination.Add("OCET", oldScore["國文"] + oldScore["英文"] + oldScore["總級分"]);
            combination.Add("OCMS", oldScore["國文"] + oldScore["數學"] + oldScore["社會"]);
            combination.Add("OCMN", oldScore["國文"] + oldScore["數學"] + oldScore["自然"]);
            combination.Add("OCMT", oldScore["國文"] + oldScore["數學"] + oldScore["總級分"]);
            combination.Add("OCSN", oldScore["國文"] + oldScore["社會"] + oldScore["自然"]);
            combination.Add("OCST", oldScore["國文"] + oldScore["社會"] + oldScore["總級分"]);
            combination.Add("OCNT", oldScore["國文"] + oldScore["自然"] + oldScore["總級分"]);
            combination.Add("OEMS", oldScore["英文"] + oldScore["數學"] + oldScore["社會"]);
            combination.Add("OEMN", oldScore["英文"] + oldScore["數學"] + oldScore["自然"]);
            combination.Add("OEMT", oldScore["英文"] + oldScore["數學"] + oldScore["總級分"]);
            combination.Add("OESN", oldScore["英文"] + oldScore["社會"] + oldScore["自然"]);
            combination.Add("OEST", oldScore["英文"] + oldScore["社會"] + oldScore["總級分"]);
            combination.Add("OENT", oldScore["英文"] + oldScore["自然"] + oldScore["總級分"]);
            combination.Add("OMSN", oldScore["數學"] + oldScore["社會"] + oldScore["自然"]);
            combination.Add("OMST", oldScore["數學"] + oldScore["社會"] + oldScore["總級分"]);
            combination.Add("OMNT", oldScore["數學"] + oldScore["自然"] + oldScore["總級分"]);
            combination.Add("OSNT", oldScore["社會"] + oldScore["自然"] + oldScore["總級分"]);
            combination.Add("OCEMS", oldScore["國文"] + oldScore["英文"] + oldScore["數學"] + oldScore["社會"]);
            combination.Add("OCEMN", oldScore["國文"] + oldScore["英文"] + oldScore["數學"] + oldScore["自然"]);
            combination.Add("OCEMT", oldScore["國文"] + oldScore["英文"] + oldScore["數學"] + oldScore["總級分"]);
            combination.Add("OCESN", oldScore["國文"] + oldScore["英文"] + oldScore["社會"] + oldScore["自然"]);
            combination.Add("OCEST", oldScore["國文"] + oldScore["英文"] + oldScore["社會"] + oldScore["總級分"]);
            combination.Add("OCENT", oldScore["國文"] + oldScore["英文"] + oldScore["自然"] + oldScore["總級分"]);
            combination.Add("OCMSN", oldScore["國文"] + oldScore["數學"] + oldScore["社會"] + oldScore["自然"]);
            combination.Add("OCMST", oldScore["國文"] + oldScore["數學"] + oldScore["社會"] + oldScore["總級分"]);
            combination.Add("OCMNT", oldScore["國文"] + oldScore["數學"] + oldScore["自然"] + oldScore["總級分"]);
            combination.Add("OCSNT", oldScore["國文"] + oldScore["社會"] + oldScore["自然"] + oldScore["總級分"]);
            combination.Add("OEMSN", oldScore["英文"] + oldScore["數學"] + oldScore["社會"] + oldScore["自然"]);
            combination.Add("OEMST", oldScore["英文"] + oldScore["數學"] + oldScore["社會"] + oldScore["總級分"]);
            combination.Add("OEMNT", oldScore["英文"] + oldScore["數學"] + oldScore["自然"] + oldScore["總級分"]);
            combination.Add("OESNT", oldScore["英文"] + oldScore["社會"] + oldScore["自然"] + oldScore["總級分"]);
            combination.Add("OMSNT", oldScore["數學"] + oldScore["社會"] + oldScore["自然"] + oldScore["總級分"]);
            combination.Add("OCEMSN", oldScore["國文"] + oldScore["英文"] + oldScore["數學"] + oldScore["社會"] + oldScore["自然"]);
            combination.Add("OCEMST", oldScore["國文"] + oldScore["英文"] + oldScore["數學"] + oldScore["社會"] + oldScore["總級分"]);
            combination.Add("OCEMNT", oldScore["國文"] + oldScore["英文"] + oldScore["數學"] + oldScore["自然"] + oldScore["總級分"]);
            combination.Add("OCESNT", oldScore["國文"] + oldScore["英文"] + oldScore["社會"] + oldScore["自然"] + oldScore["總級分"]);
            combination.Add("OCMSNT", oldScore["國文"] + oldScore["數學"] + oldScore["社會"] + oldScore["自然"] + oldScore["總級分"]);
            combination.Add("OEMSNT", oldScore["英文"] + oldScore["數學"] + oldScore["社會"] + oldScore["自然"] + oldScore["總級分"]);
            combination.Add("OCEMSNT", oldScore["國文"] + oldScore["英文"] + oldScore["數學"] + oldScore["社會"] + oldScore["自然"] + oldScore["總級分"]);            
            return combination;
        }

        private string appendSQLString(List<string> groups, List<string> cities, Dictionary<string, int> oldScore , ArrayList level, List<string> attributes, string EL, int expectedSalary)
        {
            string group = appendData(groups);
            string city = appendData(cities);
            string attribute = appendData(attributes);
            string command = "SELECT DISTINCT D.DID, D.UName, D.UURL, D.DName, D.DURL, D.Salary, D.SalaryURL, D.lastCriterion, D.rateOfThisYear, D.Change, D.ExamURL,D.PP," +
                "D.C,D.E,D.M,D.S,D.N,D.T,"+
                "D.CE,D.CM,D.CS,D.CN,D.CT,D.EM,D.ES,D.EN,D.ET,D.MS,D.MN,D.MT,D.SN,D.ST,D.NT,"+
                "D.CEM,D.CES,D.CEN,D.CET,D.CMS,D.CMN,D.CMT,D.CSN,D.CST,D.CNT,D.EMS,D.EMN,D.EMT,D.ESN,D.EST,D.ENT,D.MSN,D.MST,D.MNT,D.SNT,"+
                "D.CEMS,D.CEMN,D.CEMT,D.CESN,D.CEST,D.CENT,D.CMSN,D.CMST,D.CMNT,D.CSNT,D.EMSN,D.EMST,D.EMNT,D.ESNT,D.MSNT,"+
                "D.CEMSN,D.CEMST,D.CEMNT,D.CESNT,D.CMSNT,D.EMSNT,D.CEMSNT" +             
                " FROM D,DC,CG WHERE  D.DID=DC.DID AND DC.CNAME=CG.CNAME AND CG.GNAME IN (" + group + ") " + "AND ((D.City IN (" + city + ") "+"AND D.PP IN (" + attribute + ")) "+ "OR D.UName = '中華大學')" +
                    " AND D.ELLEVEL >= '" + EL + "' "+"AND D.TL1 <= " + level[0].ToString() + 
                    " AND D.TL2 <= " + level[1].ToString() + " AND D.TL3 <= " + level[2].ToString() +
                    " AND D.TL5 <= " + level[3].ToString() + " AND D.TL4 <= " + level[4].ToString() +
                    " AND D.TL6 <= " + level[5].ToString() + " AND D.Salary >= " + expectedSalary.ToString() +
                    " AND D.C <= " + Convert.ToString(oldScore["國文"] + 1) + " AND D.E <= " + Convert.ToString(oldScore["英文"] + 1) + " AND D.M <= " + Convert.ToString(oldScore["數學"] + 1) +
                    " AND D.S <= " + Convert.ToString(oldScore["社會"] + 1) + " AND D.N <= " + Convert.ToString(oldScore["自然"] + 1) + " AND D.T <= " + Convert.ToString(oldScore["總級分"] + 1) +
                    " AND D.CE <= " + Convert.ToString(oldScore["OCE"] + 1) + " AND D.CM <= " + Convert.ToString(oldScore["OCM"] + 1) +
                    " AND D.CS <=" + Convert.ToString(oldScore["OCS"] + 1) + " AND D.CE <= " + Convert.ToString(oldScore["OCE"] + 1) +
                   " AND D.CM <= " + Convert.ToString(oldScore["OCM"] + 1) + " AND D.CS <= " + Convert.ToString(oldScore["OCS"] + 1) +
                   " AND D.CN <= " + Convert.ToString(oldScore["OCN"] + 1) + " AND D.CT <= " + Convert.ToString(oldScore["OCT"] + 1) +
                   " AND D.EM <= " + Convert.ToString(oldScore["OEM"] + 1) + " AND D.ES <= " + Convert.ToString(oldScore["OES"] + 1) +
                   " AND D.EN <= " + Convert.ToString(oldScore["OEN"] + 1) + " AND D.ET <= " + Convert.ToString(oldScore["OET"] + 1) +
                   " AND D.MS <= " + Convert.ToString(oldScore["OMS"] + 1) + " AND D.MN <= " + Convert.ToString(oldScore["OMN"] + 1) +
                   " AND D.MT <= " + Convert.ToString(oldScore["OMT"] + 1) + " AND D.SN <= " + Convert.ToString(oldScore["OSN"] + 1) +
                   " AND D.ST <= " + Convert.ToString(oldScore["OST"] + 1) + " AND D.NT <= " + Convert.ToString(oldScore["ONT"] + 1) +
                   " AND D.CEM <= " + Convert.ToString(oldScore["OCEM"] + 1) + " AND D.CES <= " + Convert.ToString(oldScore["OCES"] + 1) +
                   " AND D.CEN <= " + Convert.ToString(oldScore["OCEN"] + 1) + " AND D.CET <= " + Convert.ToString(oldScore["OCET"] + 1) +
                   " AND D.CMS <= " + Convert.ToString(oldScore["OCMS"] + 1) + " AND D.CMN <= " + Convert.ToString(oldScore["OCMN"] + 1) +
                   " AND D.CMT <= " + Convert.ToString(oldScore["OCMT"] + 1) + " AND D.CSN <= " + Convert.ToString(oldScore["OCSN"] + 1) +
                   " AND D.CST <= " + Convert.ToString(oldScore["OCST"] + 1) + " AND D.CNT <= " + Convert.ToString(oldScore["OCNT"] + 1) +
                   " AND D.EMS <= " + Convert.ToString(oldScore["OEMS"] + 1) + " AND D.EMN <= " + Convert.ToString(oldScore["OEMN"] + 1) +
                   " AND D.EMT <= " + Convert.ToString(oldScore["OEMT"] + 1) + " AND D.ESN <= " + Convert.ToString(oldScore["OESN"] + 1) +
                   " AND D.EST <= " + Convert.ToString(oldScore["OEST"] + 1) + " AND D.ENT <= " + Convert.ToString(oldScore["OENT"] + 1) +
                   " AND D.MSN <= " + Convert.ToString(oldScore["OMSN"] + 1) + " AND D.MST <= " + Convert.ToString(oldScore["OMST"] + 1) +
                   " AND D.MNT <= " + Convert.ToString(oldScore["OMNT"] + 1) + " AND D.SNT <= " + Convert.ToString(oldScore["OSNT"] + 1) +
                   " AND D.CEMS <= " + Convert.ToString(oldScore["OCEMS"] + 1) + " AND D.CEMN <= " + Convert.ToString(oldScore["OCEMN"] + 1) +
                   " AND D.CEMT <= " + Convert.ToString(oldScore["OCEMT"] + 1) + " AND D.CESN <= " + Convert.ToString(oldScore["OCESN"] + 1) +
                   " AND D.CEST <= " + Convert.ToString(oldScore["OCEST"] + 1) + " AND D.CENT <= " + Convert.ToString(oldScore["OCENT"] + 1) +
                   " AND D.CMSN <= " + Convert.ToString(oldScore["OCMSN"] + 1) + " AND D.CMST <= " + Convert.ToString(oldScore["OCMST"] + 1) +
                   " AND D.CMNT <= " + Convert.ToString(oldScore["OCMNT"] + 1) + " AND D.CSNT <= " + Convert.ToString(oldScore["OCSNT"] + 1) +
                   " AND D.EMSN <= " + Convert.ToString(oldScore["OEMSN"] + 1) + " AND D.EMST <= " + Convert.ToString(oldScore["OEMST"] + 1) +
                   " AND D.EMNT <= " + Convert.ToString(oldScore["OEMNT"] + 1) + " AND D.ESNT <= " + Convert.ToString(oldScore["OESNT"] + 1) +
                   " AND D.MSNT <= " + Convert.ToString(oldScore["OMSNT"] + 1) + " AND D.CEMSN <= " + Convert.ToString(oldScore["OCEMSN"] + 1) +
                   " AND D.CEMST <= " + Convert.ToString(oldScore["OCEMST"] + 1) + " AND D.CEMNT <= " + Convert.ToString(oldScore["OCEMNT"] + 1) +
                   " AND D.CESNT <= " + Convert.ToString(oldScore["OCESNT"] + 1) + " AND D.CMSNT <= " + Convert.ToString(oldScore["OCMSNT"] + 1) +
                   " AND D.EMSNT <= " + Convert.ToString(oldScore["OEMSNT"] + 1) + " AND D.CEMSNT <= " + Convert.ToString(oldScore["OCEMSNT"] + 1) +
                   " ORDER BY D.Salary DESC;";
            return command;
        }

        private string appendData(List<string> original)
        {
            string temp = null;
            int count = 1;
            var tmp = from t in original select t;
            foreach (string item in tmp)
            {
                if (count == original.Count) temp += "'" + item + "'";
                else temp += "'" + item + "', ";
                count++;
            }
            return temp;
        }

        private List<PredictionResult> computeRisk(List<PredictionResult> originalData, DataTable filter, Dictionary<string, int> scoreData)
        {
            for(int i=0;i< originalData.Count;i++)
            {
                if ((Convert.ToInt32(filter.Rows[i]["C"]) != 0) && (Convert.ToInt32(filter.Rows[i]["C"]) - scoreData["國文"] > 0)) originalData[i].riskIndex = true;
                if ((Convert.ToInt32(filter.Rows[i]["E"]) != 0) && (Convert.ToInt32(filter.Rows[i]["E"]) - scoreData["英文"] > 0)) originalData[i].riskIndex = true;
                if ((Convert.ToInt32(filter.Rows[i]["M"]) != 0) && (Convert.ToInt32(filter.Rows[i]["M"]) - scoreData["數學"] > 0)) originalData[i].riskIndex = true;
                if ((Convert.ToInt32(filter.Rows[i]["N"]) != 0) && (Convert.ToInt32(filter.Rows[i]["N"]) - scoreData["自然"] > 0)) originalData[i].riskIndex = true;
                if ((Convert.ToInt32(filter.Rows[i]["S"]) != 0) && (Convert.ToInt32(filter.Rows[i]["S"]) - scoreData["社會"] > 0)) originalData[i].riskIndex = true;
                if ((Convert.ToInt32(filter.Rows[i]["T"]) != 0) && (Convert.ToInt32(filter.Rows[i]["T"]) - scoreData["總級分"] > 0)) originalData[i].riskIndex = true;
                /***************************************************************************************************************************************************/
                if ((Convert.ToInt32(filter.Rows[i]["CE"]) != 0) && (Convert.ToInt32(filter.Rows[i]["CE"]) - scoreData["OCE"] > 0)) originalData[i].riskIndex = true;
                if ((Convert.ToInt32(filter.Rows[i]["CM"]) != 0) && (Convert.ToInt32(filter.Rows[i]["CM"]) - scoreData["OCM"] > 0)) originalData[i].riskIndex = true;
                if ((Convert.ToInt32(filter.Rows[i]["CN"]) != 0) && (Convert.ToInt32(filter.Rows[i]["CN"]) - scoreData["OCN"] > 0)) originalData[i].riskIndex = true;
                if ((Convert.ToInt32(filter.Rows[i]["CS"]) != 0) && (Convert.ToInt32(filter.Rows[i]["CS"]) - scoreData["OCS"] > 0)) originalData[i].riskIndex = true;
                if ((Convert.ToInt32(filter.Rows[i]["CT"]) != 0) && (Convert.ToInt32(filter.Rows[i]["CT"]) - scoreData["OCT"] > 0)) originalData[i].riskIndex = true;
                if ((Convert.ToInt32(filter.Rows[i]["EM"]) != 0) && (Convert.ToInt32(filter.Rows[i]["EM"]) - scoreData["OEM"] > 0)) originalData[i].riskIndex = true;
                if ((Convert.ToInt32(filter.Rows[i]["EN"]) != 0) && (Convert.ToInt32(filter.Rows[i]["EN"]) - scoreData["OEN"] > 0)) originalData[i].riskIndex = true;
                if ((Convert.ToInt32(filter.Rows[i]["ES"]) != 0) && (Convert.ToInt32(filter.Rows[i]["ES"]) - scoreData["OES"] > 0)) originalData[i].riskIndex = true;
                if ((Convert.ToInt32(filter.Rows[i]["ET"]) != 0) && (Convert.ToInt32(filter.Rows[i]["ET"]) - scoreData["OET"] > 0)) originalData[i].riskIndex = true;
                if ((Convert.ToInt32(filter.Rows[i]["MN"]) != 0) && (Convert.ToInt32(filter.Rows[i]["MN"]) - scoreData["OMN"] > 0)) originalData[i].riskIndex = true;
                if ((Convert.ToInt32(filter.Rows[i]["MS"]) != 0) && (Convert.ToInt32(filter.Rows[i]["MS"]) - scoreData["OMS"] > 0)) originalData[i].riskIndex = true;
                if ((Convert.ToInt32(filter.Rows[i]["MT"]) != 0) && (Convert.ToInt32(filter.Rows[i]["MT"]) - scoreData["OMT"] > 0)) originalData[i].riskIndex = true;
                if ((Convert.ToInt32(filter.Rows[i]["SN"]) != 0) && (Convert.ToInt32(filter.Rows[i]["SN"]) - scoreData["OSN"] > 0)) originalData[i].riskIndex = true;
                if ((Convert.ToInt32(filter.Rows[i]["ST"]) != 0) && (Convert.ToInt32(filter.Rows[i]["ST"]) - scoreData["OST"] > 0)) originalData[i].riskIndex = true;
                if ((Convert.ToInt32(filter.Rows[i]["NT"]) != 0) && (Convert.ToInt32(filter.Rows[i]["NT"]) - scoreData["ONT"] > 0)) originalData[i].riskIndex = true;
                /*****************************************************************************************************************************************************/
                if ((Convert.ToInt32(filter.Rows[i]["CEM"]) != 0) && (Convert.ToInt32(filter.Rows[i]["CEM"]) - scoreData["OCEM"] > 0)) originalData[i].riskIndex = true;
                if ((Convert.ToInt32(filter.Rows[i]["CES"]) != 0) && (Convert.ToInt32(filter.Rows[i]["CES"]) - scoreData["OCES"] > 0)) originalData[i].riskIndex = true;
                if ((Convert.ToInt32(filter.Rows[i]["CEN"]) != 0) && (Convert.ToInt32(filter.Rows[i]["CEN"]) - scoreData["OCEN"] > 0)) originalData[i].riskIndex = true;
                if ((Convert.ToInt32(filter.Rows[i]["CET"]) != 0) && (Convert.ToInt32(filter.Rows[i]["CET"]) - scoreData["OCET"] > 0)) originalData[i].riskIndex = true;
                if ((Convert.ToInt32(filter.Rows[i]["CMS"]) != 0) && (Convert.ToInt32(filter.Rows[i]["CMS"]) - scoreData["OCMS"] > 0)) originalData[i].riskIndex = true;
                if ((Convert.ToInt32(filter.Rows[i]["CMN"]) != 0) && (Convert.ToInt32(filter.Rows[i]["CMN"]) - scoreData["OCMN"] > 0)) originalData[i].riskIndex = true;
                if ((Convert.ToInt32(filter.Rows[i]["CMT"]) != 0) && (Convert.ToInt32(filter.Rows[i]["CMT"]) - scoreData["OCMT"] > 0)) originalData[i].riskIndex = true;
                if ((Convert.ToInt32(filter.Rows[i]["CSN"]) != 0) && (Convert.ToInt32(filter.Rows[i]["CSN"]) - scoreData["OCSN"] > 0)) originalData[i].riskIndex = true;
                if ((Convert.ToInt32(filter.Rows[i]["CST"]) != 0) && (Convert.ToInt32(filter.Rows[i]["CST"]) - scoreData["OCST"] > 0)) originalData[i].riskIndex = true;
                if ((Convert.ToInt32(filter.Rows[i]["CNT"]) != 0) && (Convert.ToInt32(filter.Rows[i]["CNT"]) - scoreData["OCNT"] > 0)) originalData[i].riskIndex = true;
                if ((Convert.ToInt32(filter.Rows[i]["EMS"]) != 0) && (Convert.ToInt32(filter.Rows[i]["EMS"]) - scoreData["OEMS"] > 0)) originalData[i].riskIndex = true;
                if ((Convert.ToInt32(filter.Rows[i]["EMN"]) != 0) && (Convert.ToInt32(filter.Rows[i]["EMN"]) - scoreData["OEMN"] > 0)) originalData[i].riskIndex = true;
                if ((Convert.ToInt32(filter.Rows[i]["EMT"]) != 0) && (Convert.ToInt32(filter.Rows[i]["EMT"]) - scoreData["OEMT"] > 0)) originalData[i].riskIndex = true;
                if ((Convert.ToInt32(filter.Rows[i]["ESN"]) != 0) && (Convert.ToInt32(filter.Rows[i]["ESN"]) - scoreData["OESN"] > 0)) originalData[i].riskIndex = true;
                if ((Convert.ToInt32(filter.Rows[i]["EST"]) != 0) && (Convert.ToInt32(filter.Rows[i]["EST"]) - scoreData["OEST"] > 0)) originalData[i].riskIndex = true;
                if ((Convert.ToInt32(filter.Rows[i]["ENT"]) != 0) && (Convert.ToInt32(filter.Rows[i]["ENT"]) - scoreData["OENT"] > 0)) originalData[i].riskIndex = true;
                if ((Convert.ToInt32(filter.Rows[i]["MSN"]) != 0) && (Convert.ToInt32(filter.Rows[i]["MSN"]) - scoreData["OMSN"] > 0)) originalData[i].riskIndex = true;
                if ((Convert.ToInt32(filter.Rows[i]["MST"]) != 0) && (Convert.ToInt32(filter.Rows[i]["MST"]) - scoreData["OMST"] > 0)) originalData[i].riskIndex = true;
                if ((Convert.ToInt32(filter.Rows[i]["MNT"]) != 0) && (Convert.ToInt32(filter.Rows[i]["MNT"]) - scoreData["OMNT"] > 0)) originalData[i].riskIndex = true;
                if ((Convert.ToInt32(filter.Rows[i]["SNT"]) != 0) && (Convert.ToInt32(filter.Rows[i]["SNT"]) - scoreData["OSNT"] > 0)) originalData[i].riskIndex = true;
                /*****************************************************************************************************************************************************/
                if ((Convert.ToInt32(filter.Rows[i]["CEMS"]) != 0) && (Convert.ToInt32(filter.Rows[i]["CEMS"]) - scoreData["OCEMS"] > 0)) originalData[i].riskIndex = true;
                if ((Convert.ToInt32(filter.Rows[i]["CEMN"]) != 0) && (Convert.ToInt32(filter.Rows[i]["CEMN"]) - scoreData["OCEMN"] > 0)) originalData[i].riskIndex = true;
                if ((Convert.ToInt32(filter.Rows[i]["CEMT"]) != 0) && (Convert.ToInt32(filter.Rows[i]["CEMT"]) - scoreData["OCEMT"] > 0)) originalData[i].riskIndex = true;
                if ((Convert.ToInt32(filter.Rows[i]["CESN"]) != 0) && (Convert.ToInt32(filter.Rows[i]["CESN"]) - scoreData["OCESN"] > 0)) originalData[i].riskIndex = true;
                if ((Convert.ToInt32(filter.Rows[i]["CEST"]) != 0) && (Convert.ToInt32(filter.Rows[i]["CEST"]) - scoreData["OCEST"] > 0)) originalData[i].riskIndex = true;
                if ((Convert.ToInt32(filter.Rows[i]["CENT"]) != 0) && (Convert.ToInt32(filter.Rows[i]["CENT"]) - scoreData["OCENT"] > 0)) originalData[i].riskIndex = true;
                if ((Convert.ToInt32(filter.Rows[i]["CMSN"]) != 0) && (Convert.ToInt32(filter.Rows[i]["CMSN"]) - scoreData["OCMSN"] > 0)) originalData[i].riskIndex = true;
                if ((Convert.ToInt32(filter.Rows[i]["CMST"]) != 0) && (Convert.ToInt32(filter.Rows[i]["CMST"]) - scoreData["OCMST"] > 0)) originalData[i].riskIndex = true;
                if ((Convert.ToInt32(filter.Rows[i]["CMNT"]) != 0) && (Convert.ToInt32(filter.Rows[i]["CMNT"]) - scoreData["OCMNT"] > 0)) originalData[i].riskIndex = true;
                if ((Convert.ToInt32(filter.Rows[i]["CSNT"]) != 0) && (Convert.ToInt32(filter.Rows[i]["CSNT"]) - scoreData["OCSNT"] > 0)) originalData[i].riskIndex = true;
                if ((Convert.ToInt32(filter.Rows[i]["EMSN"]) != 0) && (Convert.ToInt32(filter.Rows[i]["EMSN"]) - scoreData["OEMSN"] > 0)) originalData[i].riskIndex = true;
                if ((Convert.ToInt32(filter.Rows[i]["EMST"]) != 0) && (Convert.ToInt32(filter.Rows[i]["EMST"]) - scoreData["OEMST"] > 0)) originalData[i].riskIndex = true;
                if ((Convert.ToInt32(filter.Rows[i]["EMNT"]) != 0) && (Convert.ToInt32(filter.Rows[i]["EMNT"]) - scoreData["OEMNT"] > 0)) originalData[i].riskIndex = true;
                if ((Convert.ToInt32(filter.Rows[i]["ESNT"]) != 0) && (Convert.ToInt32(filter.Rows[i]["ESNT"]) - scoreData["OESNT"] > 0)) originalData[i].riskIndex = true;
                if ((Convert.ToInt32(filter.Rows[i]["MSNT"]) != 0) && (Convert.ToInt32(filter.Rows[i]["MSNT"]) - scoreData["OMSNT"] > 0)) originalData[i].riskIndex = true;
                /*****************************************************************************************************************************************************/
                if ((Convert.ToInt32(filter.Rows[i]["CEMSN"]) != 0) && (Convert.ToInt32(filter.Rows[i]["CEMSN"]) - scoreData["OCEMSN"] > 0)) originalData[i].riskIndex = true;
                if ((Convert.ToInt32(filter.Rows[i]["CEMST"]) != 0) && (Convert.ToInt32(filter.Rows[i]["CEMST"]) - scoreData["OCEMST"] > 0)) originalData[i].riskIndex = true;
                if ((Convert.ToInt32(filter.Rows[i]["CEMNT"]) != 0) && (Convert.ToInt32(filter.Rows[i]["CEMNT"]) - scoreData["OCEMNT"] > 0)) originalData[i].riskIndex = true;
                if ((Convert.ToInt32(filter.Rows[i]["CESNT"]) != 0) && (Convert.ToInt32(filter.Rows[i]["CESNT"]) - scoreData["OCESNT"] > 0)) originalData[i].riskIndex = true;
                if ((Convert.ToInt32(filter.Rows[i]["CMSNT"]) != 0) && (Convert.ToInt32(filter.Rows[i]["CMSNT"]) - scoreData["OCMSNT"] > 0)) originalData[i].riskIndex = true;
                if ((Convert.ToInt32(filter.Rows[i]["EMSNT"]) != 0) && (Convert.ToInt32(filter.Rows[i]["EMSNT"]) - scoreData["OEMSNT"] > 0)) originalData[i].riskIndex = true;
                if ((Convert.ToInt32(filter.Rows[i]["CEMSNT"]) != 0) && (Convert.ToInt32(filter.Rows[i]["CEMSNT"]) - scoreData["OCEMSNT"] > 0)) originalData[i].riskIndex = true;
            }
            return originalData;
        }

        public List<PredictionResult> SearchResult(Input data)
        {
            List<PredictionResult> list = new List<PredictionResult>();
            SqlDataAdapter buffer = null;
            string sqlCom = null;
            ArrayList level = changeScoreOfGSAT2Level(data.grades.gsat);
            if (this.conn.State == ConnectionState.Open) conn.Close();
            try
            {
                Dictionary<string, int> SCORE= turnToOldScore(data.grades.gsat);
                Dictionary<string, int> SCORECOMBIN= computeAllSubjectCombination(SCORE);
                sqlCom = appendSQLString(data.groups, data.location, SCORECOMBIN, level, data.property, data.grades.gsat.EngListeningLevel, data.expect_salary);
                SqlCommand SqlCmd = new SqlCommand(sqlCom, this.conn);
                SqlCmd.CommandTimeout = 60;
                this.conn.Open();
                buffer = new SqlDataAdapter(SqlCmd);
                buffer.Fill(dt);
                PredictionResult resultData;
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    resultData = new PredictionResult();
                    //將搜尋到的東西封裝成Object
                    resultData.did = dt.Rows[i]["DID"].ToString();
                    resultData.uname = dt.Rows[i]["UName"].ToString().Trim();
                    resultData.uurl = dt.Rows[i]["UURL"].ToString().Trim();
                    resultData.dname = dt.Rows[i]["DName"].ToString().Trim();
                    resultData.durl = dt.Rows[i]["DURL"].ToString().Trim();
                    resultData.salary = Convert.ToInt32(dt.Rows[i]["Salary"]);
                    resultData.salaryUrl = (dt.Rows[i]["SalaryURL"].ToString().Trim() == Convert.ToString(0) ? null : dt.Rows[i]["SalaryURL"].ToString().Trim());
                    resultData.change = (dt.Rows[i]["Change"].Equals("") ? null : dt.Rows[i]["Change"].ToString());
                    resultData.lastCriterion = dt.Rows[i]["lastCriterion"].ToString();
                    resultData.rateOfThisYear = dt.Rows[i]["rateOfThisYear"].ToString();
                    resultData.examURL = dt.Rows[i]["ExamURL"].ToString();
                    resultData.riskIndex = false;
                    list.Add(resultData);  //放到List中
                }
                this.conn.Close();
                list = computeRisk(list, dt, SCORECOMBIN);
                dt.Clear();
            }
            catch (SqlException ex)
            {
                dt.Clear();
                this.conn.Close();
            }
            return list;
        }
    }
}