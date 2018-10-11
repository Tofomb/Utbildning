using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Utbildning.Models
{
    public class CourseListItem
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
        public List<string> Tags { get; set; }

        public CourseListItem(Course course)
        {
            Id = course.Id;
            Name = course.Name;
            Length = course.Length;
            Host = course.Host;
            Email = course.Email;
            Subtitle = course.Subtitle;
            Bold = course.Bold;
            Text = course.Text;
            Image = course.Image;
            Address = course.Address;
            City = course.City;
            Price = course.Price;

            using (ApplicationDbContext db = new ApplicationDbContext())
            {
                Tags = (from x in db.CourseTags.Where(y => y.CourseId == course.Id) select x.Text).ToList();
            }
        }
    }
}