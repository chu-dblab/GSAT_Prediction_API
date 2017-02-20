using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace PredictionAPI.Models
{
    public class Gsat
    {
        public string Chinese { get; set; }
        public string English { get; set; }
        public string Math { get; set; }
        public string Science { get; set; }
        public string Society { get; set; }
        public string EngListeningLevel { get; set; }
    }

    public class Grades
    {
        public Gsat gsat { get; set; }
    }

    public class Input
    {
        public Grades grades { get; set; }
        public List<string> groups { get; set; }
        public List<string> location { get; set; }
        public List<string> property { get; set; }
        public int expect_salary { get; set; }
    }

    public class PredictionResult
    {
        public string did { get; set; }
        public string uname { get; set; }
        public string uurl { get; set; }
        public string dname { get; set; }
        public string durl { get; set; }
        public int salary { get; set; }
        public string salaryUrl { get; set; }
        public string lastCriterion { get; set; }
        public string rateOfThisYear { get; set; }
        public string change { get; set; }
        public string examURL { get; set; }
        public bool riskIndex { get; set; }
    }

    public class Output
    {
        public int status { get; set; }
        public Input input { get; set; }
        public List<PredictionResult> result { get; set; }
        public string message { get; set; }
    }
    public class Enter
    {
        public Grades grades { get; set; }
    }
    public class StandarLevel
    {
        public int status { get; set; }
        public Enter enter { get; set; }
        public Dictionary<string,string> step { get; set; }
    }
}