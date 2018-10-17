using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Utbildning.Models
{
    public class SiteConfiguration
    {
        public int Id { get; set; }

        public string Property { get; set; }
        public string Value { get; set; }

        public SiteConfiguration(string Property, string Value)
        {
            this.Property = Property;
            this.Value = Value;
        }
        public SiteConfiguration() { }
    }
}