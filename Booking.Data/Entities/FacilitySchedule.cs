using Booking.Data.Entities.Abstractions;
using Microsoft.Identity.Client;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Booking.Data.Entities
{
    public class FacilitySchedule : BaseEntity
    {
        [ForeignKey(nameof(Facility))]
        public Guid FacilityId { get; set; }
        public Facility? Facility { get; set; }

        [ForeignKey(nameof(Schedule))]
        public Guid ScheduleId { get; set; }
        public Schedule? Schedule { get; set; }
    }
}
