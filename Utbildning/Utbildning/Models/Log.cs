using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Utbildning.Models
{
    public class Log
    {
        public int Id { get; set; }
        public string User { get; set; }
        public string Table { get; set; }
        public string Action { get; set; } //Example: Add, Remove, Modify
        public string Before { get; set; } //Value of data before change
        public string After { get; set; } //Value of data after change
        public DateTime Time { get; set; }
    }
}