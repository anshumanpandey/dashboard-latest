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
    
    public partial class DataManagement
    {
        public int DataId { get; set; }
        public string SourceSystem { get; set; }
        public string DataSet { get; set; }
        public Nullable<int> DQReq { get; set; }
        public Nullable<System.DateTime> UpdateDate { get; set; }
        public Nullable<System.TimeSpan> UpdateTime { get; set; }
    }
}
