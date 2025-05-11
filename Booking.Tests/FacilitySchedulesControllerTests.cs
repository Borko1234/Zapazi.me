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
        private async Task<ApplicationDbContext> GetInMemoryContext()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            var context = new ApplicationDbContext(options);
            context.Facilities.AddRange(new[]
            {
                new Facility { Id = Guid.NewGuid(), Name = "Gym", Mobile = "123", Address = "Main St", OwnerId = "test-user-id" },
                new Facility { Id = Guid.NewGuid(), Name = "Pool", Mobile = "456", Address = "Ocean Dr", OwnerId = "test-user-id" }
            });

            context.Schedules.AddRange(new[]
            {
                new Schedule { Id = Guid.NewGuid(), Open = TimeOnly.Parse("08:00"), Close = TimeOnly.Parse("18:00") },
                new Schedule { Id = Guid.NewGuid(), Open = TimeOnly.Parse("09:00"), Close = TimeOnly.Parse("21:00") }
            });

            await context.SaveChangesAsync();

            context.FacilitySchedules.AddRange(new[]
            {
                new FacilitySchedule { Id = Guid.NewGuid(), 
                    FacilityId = (await context.Facilities.FirstAsync()).Id,
                    ScheduleId = (await context.Schedules.FirstAsync()).Id },
                new FacilitySchedule { Id = Guid.NewGuid(), 
                    FacilityId = (await context.Facilities.LastAsync()).Id,
                    ScheduleId = (await context.Schedules.LastAsync()).Id }
            });
            await context.SaveChangesAsync();
            return context;
        }

        [Test]
        public async Task Index_ReturnsViewWithAllFacilitySchedules()
        {
            var context = await GetInMemoryContext();
            var controller = new FacilitySchedulesController(context);

            var result = await controller.Index(null) as ViewResult;
            var model = result.Model as IEnumerable<FacilitySchedule>;

            Assert.That(model.Count(), Is.EqualTo(2));
        }

        [Test]
        public async Task Index_WithSearchTerm_ReturnsFilteredFacilitySchedules()
        {
            var context = await GetInMemoryContext();
            var controller = new FacilitySchedulesController(context);

            var result = await controller.Index("Gym") as ViewResult;
            var model = result.Model as IEnumerable<FacilitySchedule>;

            Assert.That(model.Count(), Is.EqualTo(1));
            Assert.That(model.First().Facility.Name, Is.EqualTo("Gym"));
        }

        [Test]
        public async Task Details_ReturnsNotFound_WhenIdIsNull()
        {
            var context = await GetInMemoryContext();
            var controller = new FacilitySchedulesController(context);

            var result = await controller.Details(null);

            Assert.IsInstanceOf<NotFoundResult>(result);
        }

        [Test]
        public async Task Details_ReturnsView_WhenIdIsValid()
        {
            var context = await GetInMemoryContext();
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
            var context = await GetInMemoryContext();
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
            var context = await GetInMemoryContext();
            var controller = new FacilitySchedulesController(context);
            var facilitySchedule = context.FacilitySchedules.First();
            facilitySchedule.FacilityId = context.Facilities.Last().Id;

            var result = await controller.Edit(facilitySchedule.Id, facilitySchedule);

            Assert.IsInstanceOf<RedirectToActionResult>(result);
            Assert.That((await context.FacilitySchedules.FindAsync(facilitySchedule.Id)).FacilityId,
                Is.EqualTo( (await context.Facilities.LastAsync()).Id));
        }

        [Test]
        public async Task Delete_Post_DeletesFacilitySchedule_AndRedirects()
        {
            var context = await GetInMemoryContext();
            var controller = new FacilitySchedulesController(context);
            var facilitySchedule = context.FacilitySchedules.First();

            var result = await controller.DeleteConfirmed(facilitySchedule.Id);

            Assert.IsInstanceOf<RedirectToActionResult>(result);
            Assert.That(context.FacilitySchedules.Count(), Is.EqualTo(1)); // One should be deleted
        }

        [Test]
        public async Task DeleteConfirmed_DeletesFacilitySchedule_AndRedirects()
        {
            var context = await GetInMemoryContext();
            var controller = new FacilitySchedulesController(context);
            var facilitySchedule = context.FacilitySchedules.First();

            var result = await controller.DeleteConfirmed(facilitySchedule.Id);

            Assert.IsInstanceOf<RedirectToActionResult>(result);
            Assert.That(context.FacilitySchedules.Count(), Is.EqualTo(1)); // One should be deleted
        }
    }
}
