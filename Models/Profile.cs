using System.ComponentModel;
using System.Runtime.Serialization;
using Newtonsoft.Json;

namespace AuthiQ.SSO.Models
{
    [DataContract(Name = "profile")]
    [JsonObject(MemberSerialization.OptIn)]
    public class Profile
    {
        public Profile()
        {
            Success = true;
        }

        [DataMember(Name = "id")]
        [JsonProperty("id", NullValueHandling = NullValueHandling.Ignore)]
        public int ID { get; set; }

        [DataMember(Name = "username")]
        [JsonProperty("username", NullValueHandling = NullValueHandling.Ignore)]
        public string Username { get; set; }

        [DataMember(Name = "full_name")]
        [JsonProperty("full_name", NullValueHandling = NullValueHandling.Ignore)]
        public string FullName { get; set; }

        [DataMember(Name = "mail_address")]
        [JsonProperty("mail_address", NullValueHandling = NullValueHandling.Ignore)]
        public string MailAddress { get; set; }
        
        [DataMember(Name = "error")]
        [JsonProperty("error", NullValueHandling = NullValueHandling.Ignore)]
        public string Error { get; set; }

        [DefaultValue(true)]
        [DataMember(Name = "success")]
        [JsonProperty("success")]
        public bool Success { get; set; }
    }
}