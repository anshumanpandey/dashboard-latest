//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace NHS.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class FeedBack
    {
        public int FeedBack_ID { get; set; }
        public Nullable<int> PatientID { get; set; }
        public Nullable<bool> FormCompleted { get; set; }
        public Nullable<bool> ComplementsFedBack { get; set; }
        public string CreatedBy { get; set; }
        public Nullable<System.DateTime> CreateDate { get; set; }
        public string UpdatedBy { get; set; }
        public Nullable<System.DateTime> UpdatedDate { get; set; }
    
        public virtual PatientDetails PatientDetails { get; set; }
    }
}
