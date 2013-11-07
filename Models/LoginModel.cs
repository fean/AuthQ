using System.ComponentModel.DataAnnotations;

namespace AuthiQ.SSO.Models
{
    public class LoginModel
    {
        public string RequestToken { get; set; }

        [Required]
        public string Username { get; set; }

        [Required]
        public string Password { get; set; }

        [Display(Name = "Remember me")]
        public bool RememberMe { get; set; }

        public string ErrorMessage { get; set; }

        public long TrustId { get; set; }

        public string Challenge { get; set; }
    }
}