using Booking.Data.Entities.Abstractions;
using Booking.Data.Identity.Users;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Booking.Data.Entities
{
    public class Facility : BaseEntity
    {
        [Required]
        [StringLength(50)]
        public string Name { get; set; }
        [Required]
        [StringLength(15)]
        public string Mobile { get; set; }
        [Required]
        public string Address { get; set; }

        public string OwnerId { get; set; }
        public BookingUser? Owner { get; set; }

        public virtual ICollection<FacilitySchedule>? FacilitySchedules { get; set; }
        public virtual ICollection<Pricing>? Pricings { get; set; }
        public virtual ICollection<Reservation>? Reservations { get; set; }
    }
}
