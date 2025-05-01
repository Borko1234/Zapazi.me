using Booking.Data.Entities.Abstractions;
using Booking.Data.Identity.Users;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Booking.Data.Entities
{
    public class Reservation : BaseEntity
    {
        public Guid FacilityId { get; set; }
        public Facility? Facility { get; set; }

        public string UserId { get; set; }
        public BookingUser? User { get; set; }


        [Required]
        public DateTime Date { get; set; }
        [Required]
        public TimeSpan Duration { get; set; }
        public string Description { get; set; }
    }
}
