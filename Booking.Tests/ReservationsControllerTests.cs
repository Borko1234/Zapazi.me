using NUnit.Framework;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Booking.Data;
using Booking.Data.Entities;
using Booking.Controllers;
using Booking.Data.Identity.Users;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Booking.Tests
{
    public class ReservationsControllerTests
    {
        private ApplicationDbContext GetInMemoryContext()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            var context = new ApplicationDbContext(options);

            var user = new BookingUser { Id = "user1", UserName = "testuser" };
            var facility = new Facility { Id = Guid.NewGuid(), Name = "Gym", Mobile = "123", Address = "Main St" };

            context.Users.Add(user);
            context.Facilities.Add(facility);

            context.Reservations.Add(new Reservation
            {
                Id = Guid.NewGuid(),
                FacilityId = facility.Id,
                UserId = user.Id,
                Date = DateTime.Today,
                Duration = TimeSpan.Parse("45"),
                Description = "Test Reservation"
            });

            context.SaveChanges();
            return context;
        }

        [Test]
        public async Task Index_ReturnsAllReservations()
        {
            var context = GetInMemoryContext();
            var controller = new ReservationsController(context, null);

            var result = await controller.Index(null) as ViewResult;
            var model = result.Model as List<Reservation>;

            Assert.That(model.Count, Is.EqualTo(1));
        }

        [Test]
        public async Task Index_WithSearchTerm_FiltersResults()
        {
            var context = GetInMemoryContext();
            var controller = new ReservationsController(context, null);

            var result = await controller.Index("Gym") as ViewResult;
            var model = result.Model as List<Reservation>;

            Assert.That(model.Count, Is.EqualTo(1));
        }

        [Test]
        public async Task Details_ReturnsReservation_WhenIdIsValid()
        {
            var context = GetInMemoryContext();
            var controller = new ReservationsController(context, null);
            var reservation = context.Reservations.First();

            var result = await controller.Details(reservation.Id) as ViewResult;
            var model = result.Model as Reservation;

            Assert.That(model.Description, Is.EqualTo("Test Reservation"));
        }

        [Test]
        public async Task Details_ReturnsNotFound_WhenIdIsInvalid()
        {
            var context = GetInMemoryContext();
            var controller = new ReservationsController(context, null);

            var result = await controller.Details(Guid.NewGuid());

            Assert.IsInstanceOf<NotFoundResult>(result);
        }

        [Test]
        public async Task Create_Post_AddsReservation_AndRedirects()
        {
            var context = GetInMemoryContext();
            var controller = new ReservationsController(context, null);

            var facilityId = context.Facilities.First().Id;
            var reservation = new Reservation
            {
                FacilityId = facilityId,
                Date = DateTime.Today.AddDays(1),
                Duration = TimeSpan.Parse("45"),
                Description = "New Reservation",
                UserId = "user1" // Simulate logged-in user
            };

            controller.ModelState.Remove("UserId"); // Match controller behavior

            var result = await controller.Create(reservation);

            Assert.IsInstanceOf<RedirectToActionResult>(result);
            Assert.That(context.Reservations.Count(), Is.EqualTo(2));
        }

        [Test]
        public async Task Edit_Post_UpdatesReservation()
        {
            var context = GetInMemoryContext();
            var controller = new ReservationsController(context, null);
            var reservation = context.Reservations.First();
            reservation.Description = "Updated Description";

            var result = await controller.Edit(reservation.Id, reservation);

            Assert.IsInstanceOf<RedirectToActionResult>(result);
            Assert.That((await context.Reservations.FindAsync(reservation.Id)).Description, Is.EqualTo("Updated Description"));
        }

        [Test]
        public async Task DeleteConfirmed_DeletesReservation()
        {
            var context = GetInMemoryContext();
            var controller = new ReservationsController(context, null);
            var reservation = context.Reservations.First();

            var result = await controller.DeleteConfirmed(reservation.Id);

            Assert.IsInstanceOf<RedirectToActionResult>(result);
            Assert.That(context.Reservations.Any(r => r.Id == reservation.Id), Is.False);
        }
    }
}
