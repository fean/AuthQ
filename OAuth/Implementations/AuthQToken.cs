using System;

namespace AuthQ.SSO.OAuth.Implementations
{
    public class AuthQToken
    {
        public AuthQToken(string name)
        {
            AccessToken = Guid.NewGuid().ToString("N");
            Expire = DateTime.Now.AddMinutes(15);
            Name = name;
            RefreshToken = Guid.NewGuid().ToString("N");
        }

        public string Name { get; set; }

        public string AccessToken { get; set; }
        public string RefreshToken { get; set; }
        public DateTime Expire { get; set; }
            
        public int ExpireSeconds
        {
            get { return (int) Expire.Subtract(DateTime.Now).TotalSeconds; }
        }
        
        public bool IsAccessExpired
        {
            get { return DateTime.Now > Expire; }
        }

        public bool IsRefreshExpired
        {
            get { return DateTime.Now > Expire.AddMinutes(5); }
        }
    }
}
