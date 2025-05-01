using Booking.Data.Entities.Abstractions;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Booking.Data.Entities
{
    public class Schedule : BaseEntity
    {
        [Required]
        public TimeOnly Open { get; set; }
        [Required]
        public TimeOnly Close { get; set; }

        public virtual ICollection<FacilitySchedule>? FacilitySchedules { get; set; }
    }
}
