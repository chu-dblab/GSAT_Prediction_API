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
using System.Web.Http.Cors;

namespace GSATPrediction.Controllers
{
    public class GSATController : ApiController
    {
        private StandarLevel level = new StandarLevel();
        private DataOperation op;

        [HttpPost]
        public HttpResponseMessage aquireAllSubjectStandar([FromBody]JObject point)
        {
            try
            {
                Enter input = JsonConvert.DeserializeObject<Enter>(point.ToString());

                //查學測成績的標準
                op = new DataOperation();
                ArrayList list = op.changeScoreOfGSAT2Level(input.grades.gsat);

                //轉換成人可以看的標準
                string[] judge = { "未達標","底標", "後標", "均標", "前標", "頂標" };
                string[] subject = { "Chinese", "English", "Math", "Science", "Society", "TotalScore" };
                Dictionary<string, string> standar = new Dictionary<string, string>();
                for (int i = 0; i < list.Count; i++)
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
            catch(Exception ex)
            {
                var result = new HttpResponseMessage(HttpStatusCode.BadRequest)
                {
                    Content = new ObjectContent<JObject>(
                       new JObject(
                       new JProperty("status", HttpStatusCode.BadRequest),
                       new JProperty("input", point),
                       new JProperty("Message", "解析錯誤~!!")), new JsonMediaTypeFormatter())
                };
                return result;
            }
            
            
        }

        [HttpPost]
        public HttpResponseMessage analysis([FromBody] JObject data)
        {
            try
            {
                Input obj = JsonConvert.DeserializeObject<Input>(data.ToString());
                op = new DataOperation();
                List<PredictionResult> list = op.SearchResult(obj);
                List<PredictionResult> listCHU = op.SearchResultCHU(obj);
                Output rootData = new Output();
                rootData.status = Convert.ToInt32(HttpStatusCode.OK);
                rootData.input = obj;
                rootData.result = list;
                rootData.resultCHU = listCHU;
                rootData.message = "Success~!!";
                JObject jsonData = JsonConvert.DeserializeObject<JObject>(JsonConvert.SerializeObject(rootData));
                var result = new HttpResponseMessage(HttpStatusCode.OK)
                {
                    Content = new ObjectContent<JObject>(jsonData, new JsonMediaTypeFormatter())
                };
                return result;
            }
            catch (Exception ex)
            {
                var result = new HttpResponseMessage(HttpStatusCode.BadRequest)
                {
                    Content = new ObjectContent<JObject>(
                       new JObject(
                       new JProperty("status", HttpStatusCode.BadRequest),
                       new JProperty("input", data),
                       new JProperty("Message", "解析錯誤~!!")), new JsonMediaTypeFormatter())
                };
                return result;
            }
        }
    }
}
