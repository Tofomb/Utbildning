using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Utbildning.Models
{
    public class Course
    {
        public int Id { get; set; }
        [Display(Name = "Namn")]
        public string Name { get; set; }
        [Display(Name = "Längd")]
        public int Length { get; set; }
        [Display(Name = "Kursledare")]
        public string Host { get; set; }
        [Display(Name = "Email")]
        public string Email { get; set; }
        [Display(Name = "Underrubrik")]
        [DataType(DataType.MultilineText)]
        public string Subtitle { get; set; }
        [Display(Name = "Tjock text")]
        [DataType(DataType.MultilineText)]
        public string Bold { get; set; }
        [Display(Name = "Text")]
        [DataType(DataType.MultilineText)]
        public string Text { get; set; }
        [Display(Name = "Bild")]
        public string Image { get; set; }
        [Display(Name = "Address")]
        public string Address { get; set; }
        [Display(Name = "Ort")]
        public string City { get; set; }
        [Display(Name = "Pris")]
        public float Price { get; set; }
    }
}
