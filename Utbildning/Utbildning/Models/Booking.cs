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
        public string Firstname { get; set; }
        [Required(ErrorMessage = "Efternamn krävs.")]
        public string Lastname { get; set; }
        [Required(ErrorMessage = "Email krävs.")]
        public string Email { get; set; }
        [Required]
        [ForeignKey("CourseOccasion")]
        public int CourseOccasionId { get; set; }
        public CourseOccasion CourseOccasion { get; set; }
        [Required(ErrorMessage = "Telefonnummer krävs.")]
        public string PhoneNumber { get; set; }

        public string Company { get; set; }
        [Required(ErrorMessage = "Faktureringsaddress krävs.")]
        public string BillingAddress { get; set; }
        [Required(ErrorMessage = "Postnummer krävs.")]
        public string PostalCode { get; set; }
        [Required(ErrorMessage = "Ort krävs.")]
        public string City { get; set; }
        [Required(ErrorMessage = "Antal bokningar krävs.")]
        public int Bookings { get; set; }
        [DataType(DataType.MultilineText)]
        public string Message { get; set; }
        public string DiscountCode { get; set; }
        public DateTime BookingDate { get; set; }
    }
}
