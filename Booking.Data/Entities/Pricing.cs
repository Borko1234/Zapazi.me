using Booking.Data.Entities.Abstractions;
using Booking.Data.Identity.Users;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Booking.Data.Entities
{
    public class Pricing : BaseEntity
    {
        [ForeignKey(nameof(Facility))]
        public Guid FacilityId { get; set; }
        public Facility? Facility { get; set; }

        public decimal PricePerHour { get; set; }
    }
}
