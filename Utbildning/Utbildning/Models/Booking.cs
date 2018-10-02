using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Utbildning.Models
{    
    public class Booking
    {
        [Key]
        public int Id { get; set; }
        [Required(ErrorMessage="Förnamn krävs.")]
        [Display(Name = "Förnamn")]
        public string Firstname { get; set; }
        [Required(ErrorMessage = "Efternamn krävs.")]
        [Display(Name = "Efternamn")]
        public string Lastname { get; set; }
        [Required(ErrorMessage = "Email krävs.")]
        [Display(Name = "Email")]
        public string Email { get; set; }
        [Required]
        [ForeignKey("CourseOccasion")]
        public int CourseOccasionId { get; set; }
        public CourseOccasion CourseOccasion { get; set; }
        [Required(ErrorMessage = "Telefonnummer krävs.")]
        [Display(Name = "Telefonnummer")]
        public string PhoneNumber { get; set; }
        [Display(Name = "Företag")]
        public string Company { get; set; }
        [Required(ErrorMessage = "Faktureringsaddress krävs.")]
        [Display(Name = "Faktureringsaddress")]
        public string BillingAddress { get; set; }
        [Required(ErrorMessage = "Postnummer krävs.")]
        [Display(Name = "Postnummer")]
        public string PostalCode { get; set; }
        [Required(ErrorMessage = "Ort krävs.")]
        [Display(Name = "Ort")]
        public string City { get; set; }
        [Required(ErrorMessage = "Antal bokningar krävs.")]
        [Display(Name = "Antal bokningar")]
        public int Bookings { get; set; }
        [DataType(DataType.MultilineText)]
        [Display(Name = "Meddelande")]
        public string Message { get; set; }
        [Display(Name = "Rabattkod")]
        public string DiscountCode { get; set; }
        public DateTime BookingDate { get; set; }
    }
}
