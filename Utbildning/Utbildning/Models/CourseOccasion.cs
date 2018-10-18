using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using Utbildning.Classes;

namespace Utbildning.Models
{
    public class CourseOccasion
    {
        public int Id { get; set; }
        [ForeignKey("Course")]
        public int CourseId { get; set; }
        public Course Course { get; set; }
        [DataType(DataType.DateTime)]
        [Display(Name = "Kursstart")]
        public DateTime StartDate { get; set; }
        [Display(Name = "Kursslut")]
        public DateTime EndDate { get; set; }
        [Display(Name = "Minst antal deltagare")]
        public int MinPeople { get; set; }
        [Display(Name = "Mest antal deltagare")]
        public int MaxPeople { get; set; }
    }
}
