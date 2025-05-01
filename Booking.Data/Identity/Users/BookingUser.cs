using Booking.Data.Entities;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Booking.Data.Identity.Users
{
    public class BookingUser : IdentityUser
    {
        [Required]
        [StringLength(30)]
        public string Name { get; set; }
        public virtual ICollection<Reservation>? Reservations { get; set; }
    }
}
