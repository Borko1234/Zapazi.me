using Booking.Controllers;
using Booking.Data.Entities;
using Booking.Data.Identity.Users;
using Booking.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Security.Claims;

namespace Booking.Tests
{
    [TestFixture]
    public class ReservationsControllerTests
    {
        private UserManager<BookingUser> GetMockUserManager()
        {
            var store = new Mock<IUserStore<BookingUser>>();
            var userManagerMock = new Mock<UserManager<BookingUser>>(
                store.Object,
                null,
                null,
                new List<IUserValidator<BookingUser>>(),
                new List<IPasswordValidator<BookingUser>>(),
                null,
                null,
                null,
                null
            );

            userManagerMock
                .Setup(um => um.GetUserAsync(It.IsAny<ClaimsPrincipal>()))
                .ReturnsAsync(new BookingUser { Id = "test-user-id" });

            return userManagerMock.Object;
        }
        private ApplicationDbContext GetInMemoryContext()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            var context = new ApplicationDbContext(options);

            var user1 = new BookingUser { Id = Guid.NewGuid().ToString(), UserName = "user1", Name = "name1" };
            var user2 = new BookingUser { Id = Guid.NewGuid().ToString(), UserName = "user2", Name = "name1" };

            var facility1 = new Facility { Id = Guid.NewGuid(), Name = "Gym", Address = "123", Mobile = "111", OwnerId = user1.Id };
            var facility2 = new Facility { Id = Guid.NewGuid(), Name = "Pool", Address = "456", Mobile = "222", OwnerId = user2.Id };

            context.Users.AddRange(user1, user2);
            context.Facilities.AddRange(facility1, facility2);
            context.Reservations.AddRange(
                new Reservation { Id = Guid.NewGuid(), FacilityId = facility1.Id, UserId = user1.Id, Date = DateTime.Now, Duration = TimeSpan.Parse("02:00"), Description = "Gym session" },
                new Reservation { Id = Guid.NewGuid(), FacilityId = facility2.Id, UserId = user2.Id, Date = DateTime.Now.AddHours(2), Duration = TimeSpan.Parse("01:00"), Description = "Pool session" }
            );

            context.SaveChanges();
            return context;
        }

        [Test]
        public async Task Index_ReturnsAllReservations()
        {
            var context = GetInMemoryContext();
            var userManager = GetMockUserManager();
            var controller = new ReservationsController(context, userManager);

            var result = await controller.Index(null) as ViewResult;
            var model = result?.Model as List<Reservation>;

            Assert.IsNotNull(model);
            Assert.That(model.Count, Is.EqualTo(2));
        }

        [Test]
        public async Task Index_WithSearchTerm_FiltersReservations()
        {
            var context = GetInMemoryContext();
            var userManager = GetMockUserManager();
            var controller = new ReservationsController(context, userManager);

            var result = await controller.Index("Gym") as ViewResult;
            var model = result?.Model as List<Reservation>;

            Assert.IsNotNull(model);
            Assert.That(model.Count, Is.EqualTo(1));
            Assert.That(model.First().Facility.Name, Is.EqualTo("Gym"));
        }

        [Test]
        public async Task Details_ReturnsReservation_WhenIdIsValid()
        {
            var context = GetInMemoryContext();
            var reservation = context.Reservations.First();
            var userManager = GetMockUserManager();
            var controller = new ReservationsController(context, userManager);

            var result = await controller.Details(reservation.Id) as ViewResult;
            var model = result?.Model as Reservation;

            Assert.IsNotNull(model);
            Assert.That(model.Id, Is.EqualTo(reservation.Id));
        }

        [Test]
        public async Task Details_ReturnsNotFound_WhenIdIsInvalid()
        {
            var context = GetInMemoryContext();
            var userManager = GetMockUserManager();
            var controller = new ReservationsController(context, userManager);

            var result = await controller.Details(Guid.NewGuid());

            Assert.IsInstanceOf<NotFoundResult>(result);
        }

        [Test]
        public void Create_Get_ReturnsView()
        {
            var context = GetInMemoryContext();
            var userManager = GetMockUserManager();
            var controller = new ReservationsController(context, userManager);

            var result = controller.Create((Guid?)null) as ViewResult;

            Assert.IsNotNull(result);
            Assert.IsTrue(result.ViewData.ContainsKey("FacilityId"));
        }


        [Test]
        public async Task Edit_Get_ReturnsView_WhenIdIsValid()
        {
            var context = GetInMemoryContext();
            var reservation = context.Reservations.First();
            var userManager = GetMockUserManager();
            var controller = new ReservationsController(context, userManager);

            var result = await controller.Edit(reservation.Id) as ViewResult;
            var model = result?.Model as Reservation;

            Assert.IsNotNull(model);
            Assert.That(model.Id, Is.EqualTo(reservation.Id));
        }

        [Test]
        public async Task Edit_Post_UpdatesReservation_AndRedirects()
        {
            var context = GetInMemoryContext();
            var reservation = context.Reservations.First();
            reservation.Description = "Updated description";

            var userManager = GetMockUserManager();
            var controller = new ReservationsController(context, userManager);
            var user = new BookingUser { Id = Guid.NewGuid().ToString(), UserName = "user3" };
            //userManagerMock.Setup(um => um.GetUserAsync(It.IsAny<ClaimsPrincipal>())).ReturnsAsync(user);

            var result = await controller.Edit(reservation.Id, reservation);

            Assert.IsInstanceOf<RedirectToActionResult>(result);
            Assert.That(context.Reservations.Find(reservation.Id).Description, Is.EqualTo("Updated description"));
        }

        [Test]
        public async Task DeleteConfirmed_RemovesReservation_AndRedirects()
        {
            var context = GetInMemoryContext();
            var reservation = context.Reservations.First();
            var userManager = GetMockUserManager();
            var controller = new ReservationsController(context, userManager);

            var result = await controller.DeleteConfirmed(reservation.Id);

            Assert.IsInstanceOf<RedirectToActionResult>(result);
            Assert.That(context.Reservations.Any(r => r.Id == reservation.Id), Is.False);
        }
    }

}
