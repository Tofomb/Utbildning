using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Utbildning.Models
{
    public class SiteConfiguration
    {
        public int Id { get; set; }
        public string ContactTitle { get; set; }
        public string ContactText { get; set; }
        public string ContactCompanyName { get; set; }
        public string ContactAddress { get; set; }
        public string ContactPhoneNumber { get; set; }
        public string ContactEmail { get; set; }
        public string AboutTitle { get; set; }
        public string AboutBold { get; set; }
        public string AboutText { get; set; }        
        public int ExpirationTime { get; set; }
    }
}