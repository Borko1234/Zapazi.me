using System.Diagnostics;
using Booking.Data;
using Booking.Data.Entities;
using Booking.Models;
using Microsoft.AspNetCore.Mvc;

namespace Booking.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ApplicationDbContext _context;

        public HomeController(ILogger<HomeController> logger, ApplicationDbContext context)
        {
            _logger = logger;
            _context = context;
        }

        public IActionResult Index()
        {
            var today = DateTime.Today;

            // Get all facility schedules for today (assuming one schedule per facility per day)
            var facilities = (from f in _context.Facilities
                              join fs in _context.FacilitySchedules on f.Id equals fs.FacilityId
                              join s in _context.Schedules on fs.ScheduleId equals s.Id
                              select new
                              {
                                  f.Id,
                                  f.Name,
                                  f.Address,
                                  WorkStart = s.Open,
                                  WorkEnd = s.Close,
                                  ReservationsToday = _context.Reservations
                                      .Where(r => r.FacilityId == f.Id && r.Date.Date == today)
                                      .Select(r => r.Duration)
                                      .ToList()
                              })
                              .AsEnumerable()
                              .Select(f =>
                              {
                                  var totalMinutes = (f.WorkEnd - f.WorkStart).TotalMinutes;
                                  var reservedMinutes = f.ReservationsToday.Sum(d => d.TotalMinutes);
                                  var freeMinutes = Math.Max(0, totalMinutes - reservedMinutes);

                                  var reservationCount = f.ReservationsToday.Count;
                                  var interest = reservationCount switch
                                  {
                                      <= 2 => "Нисък",
                                      <= 5 => "Среден",
                                      _ => "Висок"
                                  };

                                  return new
                                  {
                                      f.Id,
                                      f.Name,
                                      f.Address,
                                      Interest = interest,
                                      FreeSlots = freeMinutes // <-- Use FreeSlots instead of FreeMinutes
                                  };
                              })
                              .ToList();

            ViewBag.FacilityCount = _context.Facilities.Count();
            ViewBag.ReservationCount = _context.Reservations.Count();
            ViewBag.Facilities = facilities; // facilities is a List of objects with Id, Name, Address, Interest, FreeSlots

            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
