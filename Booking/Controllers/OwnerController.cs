using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Booking.Data;
using Booking.Data.Entities;
using System;
using System.Linq;
using System.Threading.Tasks;
using Booking.Data.Identity.Users;

namespace Booking.Controllers
{
    [Authorize]
    public class OwnerController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<BookingUser> _userManager;

        public OwnerController(ApplicationDbContext context, UserManager<BookingUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public async Task<IActionResult> MySchedule()
        {
            var user = await _userManager.GetUserAsync(User);

            var facility = await _context.Facilities
                .FirstOrDefaultAsync(f => f.OwnerId == user.Id);

            if (facility == null)
                return NotFound("You do not own a facility.");

            var today = DateTime.Today;
            var reservations = await _context.Reservations
                .Where(r => r.FacilityId == facility.Id && r.Date.Date == today)
                .OrderBy(r => r.Date)
                .ToListAsync();

            ViewBag.Facility = facility;
            return View(reservations);
        }
    }
}