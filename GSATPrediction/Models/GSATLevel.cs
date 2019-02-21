using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace GSATPrediction.Models
{
    public class GSATLevel
    {
        public string ChineseLevel { get; set; } //國文標準
        public string EnglishLevel { get; set; } //英文標準
        public string MathLevel { get; set; } //數學標準
        public string NatureLevel { get; set; } //自然標準
        public string SoeityLevel { get; set; } //社會標準
        public string TotalLevel { get; set; } //總級分標準
    }
}