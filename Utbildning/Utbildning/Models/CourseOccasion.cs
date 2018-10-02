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
        [Display(Name = "Tillfällig kursledare")]
        public string AltHost { get; set; }
        [Display(Name = "Tillfällig address")]
        public string AltAddress { get; set; }
        [Display(Name = "Tillfällig ort")]
        public string AltCity { get; set; }
        [Display(Name = "Tillfällig Email")]
        public string AltMail { get; set; }
        [Display(Name = "Tillfällig profilbild")]
        public string AltProfilePicture{ get; set; }
        [Display(Name = "Minst antal människor")]
        public int MinPeople { get; set; }
        [Display(Name = "Mest antal människor")]
        public int MaxPeople { get; set; }
    }
}
