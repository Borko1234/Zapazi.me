using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using Booking.Data;
using Booking.Data.Entities;
using Booking.Data.Identity.Users;
using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

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

    public async Task<IActionResult> MySchedule(Guid? facilityId, DateTime? date)
    {
        var user = await _userManager.GetUserAsync(User);
        var facilities = await _context.Facilities.Where(f => f.OwnerId == user.Id).ToListAsync();
        if (!facilities.Any()) return NotFound("You do not own any facilities.");
        Facility facility;
        if (facilityId.HasValue)
            facility = facilities.FirstOrDefault(f => f.Id == facilityId.Value);
        else
            facility = facilities.First();
        if (facility == null) return NotFound("Facility not found.");
        var openHour = 9;
        var closeHour = 17;
        var schedule = await _context.FacilitySchedules
            .Include(fs => fs.Schedule)
            .FirstOrDefaultAsync(fs => fs.FacilityId == facility.Id);
        
        if (schedule?.Schedule != null)
        {
            openHour = schedule.Schedule.Open.Hour;
            closeHour = schedule.Schedule.Close.Hour;
        }

        var selectedDate = date ?? DateTime.Today;
        var reservations = await _context.Reservations
            .Where(r => r.FacilityId == facility.Id && r.Date.Date == selectedDate.Date)
            .Include(r => r.User)
            .OrderBy(r => r.Date)
            .ToListAsync();

        ViewBag.FacilityName = facility.Name;
        ViewBag.FacilityId = facility.Id;
        ViewBag.Facilities = facilities;
        ViewBag.SelectedDate = selectedDate;
        ViewBag.OpenHour = openHour;
        ViewBag.CloseHour = closeHour;
        
        return View(reservations);
    }
}