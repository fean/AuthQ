//------------------------------------------------------------------------------
// <auto-generated>
//    This code was generated from a template.
//
//    Manual changes to this file may cause unexpected behavior in your application.
//    Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace AuthiQ.SSO.Data
{
    using System;
    using System.Collections.Generic;
    
    public partial class TeamMember
    {
        public TeamMember()
        {
            this.Logins = new HashSet<Login>();
        }
    
        public int Id { get; set; }
        public string Name { get; set; }
        public string Rolename { get; set; }
        public string PhotoURL { get; set; }
        public string Speciality { get; set; }
        public int Motto_id { get; set; }
        public int Group_id { get; set; }
    
        public virtual ICollection<Login> Logins { get; set; }
    }
}
