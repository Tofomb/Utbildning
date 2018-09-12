using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Utbildning.Models
{
    public class Course
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int Length { get; set; }
        public string Host { get; set; }
        public string Email { get; set; }
        public string Subtitle { get; set; }
        public string Bold { get; set; }
        public string Text { get; set; }
        public string Image { get; set; }
        public string Address { get; set; }
        public string City { get; set; }
        public float Price { get; set; }
    }
}
