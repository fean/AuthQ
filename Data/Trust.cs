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
    
    public partial class Trust
    {
        public Trust()
        {
            this.Tokens = new HashSet<Token>();
            this.TrustedDomains = new HashSet<TrustedDomain>();
        }
    
        public long TrustId { get; set; }
        public string Secret { get; set; }
        public string TrustName { get; set; }
        public string returnUrl { get; set; }
    
        public virtual ICollection<Token> Tokens { get; set; }
        public virtual ICollection<TrustedDomain> TrustedDomains { get; set; }
    }
}
