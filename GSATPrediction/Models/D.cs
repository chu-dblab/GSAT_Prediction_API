//------------------------------------------------------------------------------
// <auto-generated>
//     這個程式碼是由範本產生。
//
//     對這個檔案進行手動變更可能導致您的應用程式產生未預期的行為。
//     如果重新產生程式碼，將會覆寫對這個檔案的手動變更。
// </auto-generated>
//------------------------------------------------------------------------------

namespace GSATPrediction.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class D
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public D()
        {
            this.DCs = new HashSet<DC>();
        }
    
        public int DID { get; set; }
        public string UName { get; set; }
        public string UURL { get; set; }
        public string DName { get; set; }
        public string DURL { get; set; }
        public int Salary { get; set; }
        public string SalaryURL { get; set; }
        public int TL1 { get; set; }
        public int TL2 { get; set; }
        public int TL3 { get; set; }
        public int TL4 { get; set; }
        public int TL5 { get; set; }
        public int TL6 { get; set; }
        public string ELLEVEL { get; set; }
        public int C { get; set; }
        public int E { get; set; }
        public int M { get; set; }
        public int S { get; set; }
        public int N { get; set; }
        public int T { get; set; }
        public int CE { get; set; }
        public int CM { get; set; }
        public int CS { get; set; }
        public int CN { get; set; }
        public int CT { get; set; }
        public int EM { get; set; }
        public int ES { get; set; }
        public int EN { get; set; }
        public int ET { get; set; }
        public int MS { get; set; }
        public int MN { get; set; }
        public int MT { get; set; }
        public int SN { get; set; }
        public int ST { get; set; }
        public int NT { get; set; }
        public int CEM { get; set; }
        public int CES { get; set; }
        public int CEN { get; set; }
        public int CET { get; set; }
        public int CMS { get; set; }
        public int CMN { get; set; }
        public int CMT { get; set; }
        public int CSN { get; set; }
        public int CST { get; set; }
        public int CNT { get; set; }
        public int EMS { get; set; }
        public int EMN { get; set; }
        public int EMT { get; set; }
        public int ESN { get; set; }
        public int EST { get; set; }
        public int ENT { get; set; }
        public int MSN { get; set; }
        public int MST { get; set; }
        public int MNT { get; set; }
        public int SNT { get; set; }
        public int CEMS { get; set; }
        public int CEMN { get; set; }
        public int CEMT { get; set; }
        public int CESN { get; set; }
        public int CEST { get; set; }
        public int CENT { get; set; }
        public int CMSN { get; set; }
        public int CMST { get; set; }
        public int CMNT { get; set; }
        public int CSNT { get; set; }
        public int EMSN { get; set; }
        public int EMST { get; set; }
        public int EMNT { get; set; }
        public int ESNT { get; set; }
        public int MSNT { get; set; }
        public int CEMSN { get; set; }
        public int CEMST { get; set; }
        public int CEMNT { get; set; }
        public int CESNT { get; set; }
        public int CMSNT { get; set; }
        public int EMSNT { get; set; }
        public int CEMSNT { get; set; }
        public string City { get; set; }
        public string PP { get; set; }
        public string Change { get; set; }
        public string lastCriterion { get; set; }
        public string rateOfThisYear { get; set; }
        public string ExamURL { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<DC> DCs { get; set; }
    }
}
