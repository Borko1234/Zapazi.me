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

            var facilities = (from f in _context.Facilities
                              join fs in _context.FacilitySchedules on f.Id equals fs.FacilityId into fsGroup
                              from fs in fsGroup.DefaultIfEmpty()
                              join s in _context.Schedules on fs.ScheduleId equals s.Id into sGroup
                              from s in sGroup.DefaultIfEmpty()
                              select new
                              {
                                  f.Id,
                                  f.Name,
                                  f.Address,
                                  WorkStart = s != null ? s.Open : TimeOnly.MinValue,
                                  WorkEnd = s != null ? s.Close : TimeOnly.MinValue,
                                  ReservationsToday = _context.Reservations
                                      .Where(r => r.FacilityId == f.Id && r.Date.Date == today)
                                      .Select(r => r.Duration)
                                      .ToList()
                              })
                              .AsEnumerable()
                              .Select(f =>
                              {
                                  var hasSchedule = f.WorkStart != TimeOnly.MinValue && f.WorkEnd != TimeOnly.MinValue;
                                  var totalMinutes = hasSchedule ? (f.WorkEnd - f.WorkStart).TotalMinutes : 0;
                                  if (totalMinutes < 0) totalMinutes = 0;
                                  var reservedMinutes = f.ReservationsToday.Any() ? f.ReservationsToday.Sum(ts => ts.TotalMinutes) : 0;
                                  var freeMinutes = hasSchedule ? Math.Max(0, totalMinutes - reservedMinutes) : 0;

                                  var reservationCount = f.ReservationsToday.Count;
                                  var interest = reservationCount switch
                                  {
                                      <= 2 => "Low",
                                      <= 5 => "Midium",
                                      _ => "High"
                                  };

                                  return new
                                  {
                                      f.Id,
                                      f.Name,
                                      f.Address,
                                      Interest = interest,
                                      FreeSlots = freeMinutes
                                  };
                              })
                              .ToList();

            ViewBag.FacilityCount = _context.Facilities.Count();
            ViewBag.ReservationCount = _context.Reservations.Count();
            ViewBag.Facilities = facilities;

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
