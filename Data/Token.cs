//------------------------------------------------------------------------------
// <auto-generated>
//    This code was generated from a template.
//
//    Manual changes to this file may cause unexpected behavior in your application.
//    Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace AuthQ.SSO.Data
{
    using System;
    using System.Collections.Generic;
    
    public partial class Token
    {
        public Token() {}

        public Token(Login login, Trust trust)
        {
            AccessToken = Guid.NewGuid().ToString("N");
            Expire = DateTime.Now.AddMinutes(1440);
            Login = login;
            Trust1 = trust;
            RefreshToken = Guid.NewGuid().ToString("N");
        }

        public int Id { get; set; }
        public string AccessToken { get; set; }
        public string RefreshToken { get; set; }
        public System.DateTime Expire { get; set; }
        public Nullable<long> Trust { get; set; }
    
        public virtual Login Login { get; set; }
        public virtual Trust Trust1 { get; set; }
    }
}
