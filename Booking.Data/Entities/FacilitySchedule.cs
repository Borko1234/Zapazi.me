using Microsoft.Identity.Client;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Booking.Data.Entities
{
    public class FacilitySchedule
    {
        [ForeignKey(nameof(Facility))]
        public virtual Guid FacilityId { get; set; }
        public virtual Facility? Facility { get; set; }

        [ForeignKey(nameof(Schedule))]
        public virtual Guid ScheduleId { get; set; }
        public virtual Schedule? Schedule { get; set; }
    }
}
