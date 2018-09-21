using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Utbildning.Models
{
    public class FrontpageInfo
    {
        public int Id { get; set; }
        public string Title { get; set; }
        [DataType(DataType.MultilineText)]
        public string Bold { get; set; }
        [DataType(DataType.MultilineText)]
        public string Text { get; set; }
    }
}