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
    
    public partial class LoginAttempt
    {
        public int LoginAttemptId { get; set; }
        public int UserId { get; set; }
        public string ServerVariables { get; set; }
    
        public virtual User User { get; set; }
    }
}