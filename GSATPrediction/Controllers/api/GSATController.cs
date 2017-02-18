using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using PredictionAPI.Models;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Web.Http;

namespace GSATPrediction.Controllers
{
    public class GSATController : ApiController
    {
        private StandarLevel level = new StandarLevel();

        [HttpPost]
        public HttpResponseMessage aquireAllSubjectStandar(JObject point)
        {
            Enter input = JsonConvert.DeserializeObject<Enter>(point.ToString());
            
            //查學測成績的標準
            DataOperation op = new DataOperation();
            ArrayList list = op.changeScoreOfGSAT2Level(input.grades.gsat);

            //轉換成人可以看的標準
            string[] judge = {"底標", "後標", "均標", "前標", "頂標" };
            string[] subject = {"國文", "英文", "數學", "自然", "社會", "總級分"};
            Dictionary<string, string> standar = new Dictionary<string, string>();
            for (int i=0;i<list.Count;i++)
            {
                standar.Add(subject[i], judge[Convert.ToInt32(list[i])]);
            }

            //塞資料
            level.enter = input;
            level.status = Convert.ToInt32(HttpStatusCode.OK);
            level.step = standar;
            
            //轉JSON
            JObject jsonData = JsonConvert.DeserializeObject<JObject>(JsonConvert.SerializeObject(level));
            var result = new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new ObjectContent<JObject>(jsonData, new JsonMediaTypeFormatter())
            };
            return result;
        }
    }
}
