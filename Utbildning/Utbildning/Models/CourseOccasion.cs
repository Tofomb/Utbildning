using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Utbildning.Models
{
    public class CourseOccasion
    {
        public int Id { get; set; }
        [ForeignKey("Course")]
        public int CourseId { get; set; }
        public Course Course { get; set; }
        public DateTime StartDate { get; set; }
        public string AltHost { get; set; }
        public string AltAddress { get; set; }
        public string AltCity { get; set; }
        public string AltMail { get; set; }
        public string AltProfilePicture{ get; set; }
        public int MinPeople { get; set; }
        public int MaxPeople { get; set; }
    }
}
