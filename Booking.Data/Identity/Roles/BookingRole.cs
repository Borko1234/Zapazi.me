using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Booking.Data.Identity.Roles
{
    public class BookingRole : IdentityRole
    {
        public BookingRole() : base()
        {
        }
        public BookingRole(string roleName) : base(roleName)
        {
        }
    }
}
