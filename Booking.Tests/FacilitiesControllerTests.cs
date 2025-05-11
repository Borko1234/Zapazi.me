using NUnit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Booking.Data;
using Booking.Data.Entities;
using Booking.Controllers;
using Booking.Data.Identity.Users;
using Microsoft.AspNetCore.Identity;
using Moq;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Security.Claims;

namespace Booking.Tests
{
    public class FacilitiesControllerTests
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

            context.Facilities.AddRange(new[]
            {
                new Facility { Id = Guid.NewGuid(), Name = "Gym", Mobile = "123", Address = "Main St", OwnerId="test-user-id" },
                new Facility { Id = Guid.NewGuid(), Name = "Pool", Mobile = "456", Address = "Ocean Dr", OwnerId="test-user-id" }
            });
            context.SaveChanges();
            return context;
        }

        [Test]
        public async Task Index_ReturnsViewWithAllFacilities()
        {
            var context = GetInMemoryContext();
            var userManager = GetMockUserManager();
            var controller = new FacilitiesController(context, userManager);

            var result = await controller.Index(null) as ViewResult;
            var model = result?.Model as IEnumerable<Facility>;

            Assert.That(model?.Count(), Is.EqualTo(2));
        }

        [Test]
        public async Task Index_WithSearchTerm_ReturnsFilteredFacilities()
        {
            var context = GetInMemoryContext();
            var userManager = GetMockUserManager();
            var controller = new FacilitiesController(context, userManager);

            var result = await controller.Index("Gym") as ViewResult;
            var model = result?.Model as IEnumerable<Facility>;

            Assert.That(model?.Single().Name, Is.EqualTo("Gym"));
        }

        [Test]
        public async Task Details_ReturnsNotFound_WhenIdIsNull()
        {
            var context = GetInMemoryContext();
            var userManager = GetMockUserManager();
            var controller = new FacilitiesController(context, userManager);

            var result = await controller.Details(null);

            Assert.IsInstanceOf<NotFoundResult>(result);
        }

        [Test]
        public async Task Details_ReturnsViewResult_WhenIdIsValid()
        {
            var context = GetInMemoryContext();
            var facility = context.Facilities.First();
            var userManager = GetMockUserManager();
            var controller = new FacilitiesController(context, userManager);

            var result = await controller.Details(facility.Id);

            var view = result as ViewResult;
            var model = view?.Model as Facility;

            Assert.That(model?.Name, Is.EqualTo(facility.Name));
        }

        [Test]
        public async Task Create_Post_AddsFacility_AndRedirects()
        {
            var context = GetInMemoryContext();
            var userManager = GetMockUserManager();
            var controller = new FacilitiesController(context, userManager);
            var facility = new Facility { Name = "Library", Mobile = "789", Address = "Elm St" };

            var result = await controller.Create(facility);

            Assert.IsInstanceOf<RedirectToActionResult>(result);
            Assert.That(context.Facilities.Count(), Is.EqualTo(3));
        }

        [Test]
        public async Task Edit_Post_UpdatesFacility()
        {
            var context = GetInMemoryContext();
            var userManager = GetMockUserManager();
            var controller = new FacilitiesController(context, userManager);
            var facility = context.Facilities.First();
            facility.Name = "Updated";

            var result = await controller.Edit(facility.Id, facility);

            Assert.IsInstanceOf<RedirectToActionResult>(result);
            Assert.That((await context.Facilities.FindAsync(facility.Id))?.Name, Is.EqualTo("Updated"));
        }

        [Test]
        public async Task Delete_Post_DeletesFacility()
        {
            var context = GetInMemoryContext();
            var userManager = GetMockUserManager();
            var controller = new FacilitiesController(context, userManager);
            var facility = context.Facilities.First();

            var result = await controller.Delete(facility.Id);

            Assert.IsInstanceOf<RedirectToActionResult>(result);
            Assert.That(context.Facilities.Count(), Is.EqualTo(1));
        }

        [Test]
        public async Task DetailsBySearch_ReturnsNotFound_WhenNoMatch()
        {
            var context = GetInMemoryContext();
            var userManager = GetMockUserManager();
            var controller = new FacilitiesController(context, userManager);

            var result = await controller.DetailsBySearch("NonExistent");

            Assert.IsInstanceOf<NotFoundResult>(result);
        }

        [Test]
        public async Task DetailsBySearch_ReturnsFacility_WhenMatchFound()
        {
            var context = GetInMemoryContext();
            var userManager = GetMockUserManager();
            var controller = new FacilitiesController(context, userManager);

            var result = await controller.DetailsBySearch("Gym");

            var view = result as ViewResult;
            var model = view?.Model as Facility;

            Assert.That(model?.Name, Is.EqualTo("Gym"));
        }
    }
}