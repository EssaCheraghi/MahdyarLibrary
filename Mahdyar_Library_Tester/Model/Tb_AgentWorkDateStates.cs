//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Mahdyar_Library_Tester.Model
{
    using System;
    using System.Collections.Generic;
    
    public partial class Tb_AgentWorkDateStates
    {
        public int xid_pk { get; set; }
        public string xIdentityNo { get; set; }
        public Nullable<long> WorkHeaderId { get; set; }
        public Nullable<int> xPersonelId_pk { get; set; }
        public Nullable<System.DateTime> xDate { get; set; }
        public Nullable<bool> xIsSpecialDay { get; set; }
        public Nullable<int> WorkDurationInMinutes { get; set; }
        public Nullable<int> TimeToGoNextWorkInMinutes { get; set; }
        public Nullable<int> WorkCapacityInThisDateByMinutes { get; set; }
    }
}
