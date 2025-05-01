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
    public class Reservations : BaseEntity
    {
        [ForeignKey(nameof(Facility))]
        public virtual Guid FacilityId { get; set; }
        public virtual Facility? Facility { get; set; }

        [ForeignKey(nameof(User))]
        public virtual Guid UserId { get; set; }
        public virtual BookingUser? User { get; set; }

        [Required]
        public DateTime Date { get; set; }
        [Required]
        public TimeSpan Duration { get; set; }
        public string Description { get; set; }
    }
}
