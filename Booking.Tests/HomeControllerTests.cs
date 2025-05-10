using NUnit.Framework;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Booking.Controllers;
using Booking.Data;
using Booking.Data.Entities;
using System;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using Booking.Models;

namespace Booking.Tests
{
    public class HomeControllerTests
    {
        // Helper method to get an in-memory context with some sample data
        private ApplicationDbContext GetInMemoryContext()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            var context = new ApplicationDbContext(options);

            // Add some sample facilities, schedules, and reservations
            context.Facilities.AddRange(new[]
            {
                new Facility { Id = Guid.NewGuid(), Name = "Gym", Mobile = "123", Address = "Main St" },
                new Facility { Id = Guid.NewGuid(), Name = "Pool", Mobile = "456", Address = "Ocean Dr" }
            });

            context.Schedules.AddRange(new[]
            {
                new Schedule { Id = Guid.NewGuid(), Open = TimeOnly.Parse("08:00"), Close = TimeOnly.Parse("18:00") },
                new Schedule { Id = Guid.NewGuid(), Open = TimeOnly.Parse("09:00"), Close = TimeOnly.Parse("21:00") }
            });

            var gymScheduleId = context.Schedules.First().Id;
            var poolScheduleId = context.Schedules.Last().Id;

            context.FacilitySchedules.AddRange(new[]
            {
                new FacilitySchedule { Id = Guid.NewGuid(), FacilityId = context.Facilities.First().Id, ScheduleId = gymScheduleId },
                new FacilitySchedule { Id = Guid.NewGuid(), FacilityId = context.Facilities.Last().Id, ScheduleId = poolScheduleId }
            });

            context.Reservations.AddRange(new[]
            {
                new Reservation { Id = Guid.NewGuid(), FacilityId = context.Facilities.First().Id, Date = DateTime.Today, Duration = TimeSpan.FromHours(1) },
                new Reservation { Id = Guid.NewGuid(), FacilityId = context.Facilities.Last().Id, Date = DateTime.Today, Duration = TimeSpan.FromMinutes(90) }
            });

            context.SaveChanges();
            return context;
        }

        [Test]
        public async Task Index_ReturnsViewWithFacilityDetails()
        {
            var context = GetInMemoryContext();

            // Pass null for the ILogger since it's not necessary for testing the logic
            var controller = new HomeController(null, context);

            var result = controller.Index() as ViewResult;
            var model = result?.ViewData["Facilities"] as List<dynamic>;

            Assert.IsNotNull(model);
            Assert.That(model.Count, Is.EqualTo(2));

            var gym = model.First();
            Assert.That(gym.Name, Is.EqualTo("Gym"));
            Assert.That(gym.Interest, Is.EqualTo("Среден")); // Based on the number of reservations (1)
            Assert.That(gym.FreeSlots, Is.GreaterThan(0));
        }

        [Test]
        public async Task Index_HandlesNoReservations()
        {
            // Reset database and add no reservations
            var context = GetInMemoryContext();
            context.Reservations.RemoveRange(context.Reservations);
            context.SaveChanges();

            var controller = new HomeController(null, context);

            var result = controller.Index() as ViewResult;
            var model = result?.ViewData["Facilities"] as List<dynamic>;

            Assert.IsNotNull(model);
            Assert.That(model.Count, Is.EqualTo(2));

            var gym = model.First();
            Assert.That(gym.Name, Is.EqualTo("Gym"));
            Assert.That(gym.Interest, Is.EqualTo("Нисък")); // No reservations, low interest
            Assert.That(gym.FreeSlots, Is.GreaterThan(0));
        }

        [Test]
        public void Privacy_ReturnsPrivacyView()
        {
            var controller = new HomeController(null, null); // Null context here since it's not needed for Privacy view

            var result = controller.Privacy() as ViewResult;

            Assert.IsNotNull(result);
            Assert.AreEqual("Privacy", result.ViewName);
        }

        [Test]
        public void Error_ReturnsErrorView()
        {
            var controller = new HomeController(null, null); // Null context here since it's not needed for Error view

            var result = controller.Error() as ViewResult;
            var model = result?.Model as ErrorViewModel;

            Assert.IsNotNull(result);
            Assert.IsNotNull(model);
            Assert.IsFalse(string.IsNullOrEmpty(model.RequestId));
        }
    }
}
