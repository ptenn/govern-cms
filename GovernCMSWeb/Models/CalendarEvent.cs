//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace GovernCMS.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class CalendarEvent
    {
        public int Id { get; set; }
        public int CalendarId { get; set; }
        public System.DateTime StartDate { get; set; }
        public System.DateTime EndDate { get; set; }
        public string EventUrl { get; set; }
        public System.DateTime CreateDate { get; set; }
        public string EventName { get; set; }
    
        public virtual Calendar Calendar { get; set; }
    }
}