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

namespace Booking.Tests
{
    public class FacilitySchedulesControllerTests
    {
        private ApplicationDbContext GetInMemoryContext()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            var context = new ApplicationDbContext(options);
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

            context.FacilitySchedules.AddRange(new[]
            {
                new FacilitySchedule { Id = Guid.NewGuid(), FacilityId = context.Facilities.First().Id, ScheduleId = context.Schedules.First().Id },
                new FacilitySchedule { Id = Guid.NewGuid(), FacilityId = context.Facilities.Last().Id, ScheduleId = context.Schedules.Last().Id }
            });
            context.SaveChanges();
            return context;
        }

        [Test]
        public async Task Index_ReturnsViewWithAllFacilitySchedules()
        {
            var context = GetInMemoryContext();
            var controller = new FacilitySchedulesController(context);

            var result = await controller.Index(null) as ViewResult;
            var model = result.Model as IEnumerable<FacilitySchedule>;

            Assert.That(model.Count(), Is.EqualTo(2));
        }

        [Test]
        public async Task Index_WithSearchTerm_ReturnsFilteredFacilitySchedules()
        {
            var context = GetInMemoryContext();
            var controller = new FacilitySchedulesController(context);

            var result = await controller.Index("Gym") as ViewResult;
            var model = result.Model as IEnumerable<FacilitySchedule>;

            Assert.That(model.Count(), Is.EqualTo(1));
            Assert.That(model.First().Facility.Name, Is.EqualTo("Gym"));
        }

        [Test]
        public async Task Details_ReturnsNotFound_WhenIdIsNull()
        {
            var context = GetInMemoryContext();
            var controller = new FacilitySchedulesController(context);

            var result = await controller.Details(null);

            Assert.IsInstanceOf<NotFoundResult>(result);
        }

        [Test]
        public async Task Details_ReturnsView_WhenIdIsValid()
        {
            var context = GetInMemoryContext();
            var facilitySchedule = context.FacilitySchedules.First();
            var controller = new FacilitySchedulesController(context);

            var result = await controller.Details(facilitySchedule.Id);
            var view = result as ViewResult;
            var model = view.Model as FacilitySchedule;

            Assert.That(model.Id, Is.EqualTo(facilitySchedule.Id));
        }

        [Test]
        public async Task Create_Post_AddsFacilitySchedule_AndRedirects()
        {
            var context = GetInMemoryContext();
            var controller = new FacilitySchedulesController(context);
            var facilitySchedule = new FacilitySchedule
            {
                FacilityId = context.Facilities.First().Id,
                ScheduleId = context.Schedules.First().Id
            };

            var result = await controller.Create(facilitySchedule);

            Assert.IsInstanceOf<RedirectToActionResult>(result);
            Assert.That(context.FacilitySchedules.Count(), Is.EqualTo(3));
        }

        [Test]
        public async Task Edit_Post_UpdatesFacilitySchedule_AndRedirects()
        {
            var context = GetInMemoryContext();
            var controller = new FacilitySchedulesController(context);
            var facilitySchedule = context.FacilitySchedules.First();
            facilitySchedule.FacilityId = context.Facilities.Last().Id;

            var result = await controller.Edit(facilitySchedule.Id, facilitySchedule);

            Assert.IsInstanceOf<RedirectToActionResult>(result);
            Assert.That((await context.FacilitySchedules.FindAsync(facilitySchedule.Id)).FacilityId, Is.EqualTo(context.Facilities.Last().Id));
        }

        [Test]
        public async Task Delete_Post_DeletesFacilitySchedule_AndRedirects()
        {
            var context = GetInMemoryContext();
            var controller = new FacilitySchedulesController(context);
            var facilitySchedule = context.FacilitySchedules.First();

            var result = await controller.Delete(facilitySchedule.Id);

            Assert.IsInstanceOf<RedirectToActionResult>(result);
            Assert.That(context.FacilitySchedules.Count(), Is.EqualTo(1)); // One should be deleted
        }

        [Test]
        public async Task DeleteConfirmed_DeletesFacilitySchedule_AndRedirects()
        {
            var context = GetInMemoryContext();
            var controller = new FacilitySchedulesController(context);
            var facilitySchedule = context.FacilitySchedules.First();

            var result = await controller.DeleteConfirmed(facilitySchedule.Id);

            Assert.IsInstanceOf<RedirectToActionResult>(result);
            Assert.That(context.FacilitySchedules.Count(), Is.EqualTo(1)); // One should be deleted
        }
    }
}
