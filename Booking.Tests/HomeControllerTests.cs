using NUnit.Framework;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Mvc;
using Booking.Controllers;
using Booking.Data;
using Booking.Data.Entities;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Booking.Tests
{
    public class HomeControllerTests
    {
        private ApplicationDbContext GetInMemoryContext()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            var context = new ApplicationDbContext(options);

            var facility = new Facility
            {
                Id = Guid.NewGuid(),
                Name = "Gym",
                Address = "123 Main St",
                Mobile = "123456"
            };

            var schedule = new Schedule
            {
                Id = Guid.NewGuid(),
                Open = TimeOnly.Parse("08:00"),
                Close = TimeOnly.Parse("18:00")
            };

            var facilitySchedule = new FacilitySchedule
            {
                Id = Guid.NewGuid(),
                FacilityId = facility.Id,
                ScheduleId = schedule.Id
            };

            var reservation = new Reservation
            {
                Id = Guid.NewGuid(),
                FacilityId = facility.Id,
                Date = DateTime.Today,
                Duration = TimeSpan.FromHours(2),
                Description = "Morning session",
                UserId = Guid.NewGuid().ToString()
            };

            context.Facilities.Add(facility);
            context.Schedules.Add(schedule);
            context.FacilitySchedules.Add(facilitySchedule);
            context.Reservations.Add(reservation);
            context.SaveChanges();

            return context;
        }

        [Test]
        public void Index_ReturnsViewWithCorrectFacilityStats()
        {
            var context = GetInMemoryContext();
            var logger = new LoggerFactory().CreateLogger<HomeController>();
            var controller = new HomeController(logger, context);

            var result = controller.Index() as ViewResult;

            Assert.IsNotNull(result);
            Assert.IsNotNull(result.ViewData);

            var facilityCount = (int)controller.ViewBag.FacilityCount;
            var reservationCount = (int)controller.ViewBag.ReservationCount;
            var facilities = controller.ViewBag.Facilities as dynamic;

            Assert.That(facilityCount, Is.EqualTo(1));
            Assert.That(reservationCount, Is.EqualTo(1));
            Assert.That(facilities.Count, Is.EqualTo(1));
            Assert.That((int)facilities[0].FreeSlots, Is.EqualTo(600 - 120)); // 10 hours - 2 reserved = 480 min
            Assert.That((string)facilities[0].Interest, Is.EqualTo("Нисък"));
        }

        [Test]
        public void Privacy_ReturnsView()
        {
            var context = GetInMemoryContext();
            var logger = new LoggerFactory().CreateLogger<HomeController>();
            var controller = new HomeController(logger, context);

            var result = controller.Privacy();

            Assert.IsInstanceOf<ViewResult>(result);
        }

        [Test]
        public void Error_ReturnsViewWithErrorModel()
        {
            var context = GetInMemoryContext();
            var logger = new LoggerFactory().CreateLogger<HomeController>();
            var controller = new HomeController(logger, context);

            var result = controller.Error() as ViewResult;

            Assert.IsInstanceOf<ViewResult>(result);
            Assert.IsInstanceOf<Booking.Models.ErrorViewModel>(result.Model);
        }
    }
}
