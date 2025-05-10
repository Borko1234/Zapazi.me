using NUnit.Framework;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Booking.Controllers;
using Booking.Data;
using Booking.Data.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

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

            var facility1 = new Facility { Id = Guid.NewGuid(), Name = "Gym", Address = "Main", Mobile = "123" };
            var schedule1 = new Schedule { Id = Guid.NewGuid(), Open = TimeOnly.Parse("08:00"), Close = TimeOnly.Parse("16:00") };
            var fs1 = new FacilitySchedule { Id = Guid.NewGuid(), FacilityId = facility1.Id, ScheduleId = schedule1.Id };

            context.Facilities.Add(facility1);
            context.Schedules.Add(schedule1);
            context.FacilitySchedules.Add(fs1);
            context.SaveChanges();

            return context;
        }

        [Test]
        public async Task Index_ReturnsAllFacilitySchedules()
        {
            var context = GetInMemoryContext();
            var controller = new FacilitySchedulesController(context);

            var result = await controller.Index(null) as ViewResult;
            var model = result.Model as IEnumerable<FacilitySchedule>;

            Assert.That(model.Count(), Is.EqualTo(1));
        }

        [Test]
        public async Task Index_WithSearch_ReturnsFilteredResult()
        {
            var context = GetInMemoryContext();
            var controller = new FacilitySchedulesController(context);

            var result = await controller.Index("Gym") as ViewResult;
            var model = result.Model as IEnumerable<FacilitySchedule>;

            Assert.That(model.Count(), Is.EqualTo(1));
        }

        [Test]
        public async Task Details_ReturnsFacilitySchedule_WhenFound()
        {
            var context = GetInMemoryContext();
            var fs = context.FacilitySchedules.First();
            var controller = new FacilitySchedulesController(context);

            var result = await controller.Details(fs.Id) as ViewResult;
            var model = result.Model as FacilitySchedule;

            Assert.That(model.Id, Is.EqualTo(fs.Id));
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
        public async Task Create_Post_AddsFacilitySchedule()
        {
            var context = GetInMemoryContext();
            var controller = new FacilitySchedulesController(context);

            var facility = context.Facilities.First();
            var schedule = context.Schedules.First();
            var fs = new FacilitySchedule
            {
                FacilityId = facility.Id,
                ScheduleId = schedule.Id
            };

            var result = await controller.Create(fs);

            Assert.IsInstanceOf<RedirectToActionResult>(result);
            Assert.That(context.FacilitySchedules.Count(), Is.EqualTo(2));
        }

        [Test]
        public async Task Edit_Post_UpdatesFacilitySchedule()
        {
            var context = GetInMemoryContext();
            var controller = new FacilitySchedulesController(context);
            var fs = context.FacilitySchedules.First();
            var newSchedule = new Schedule { Id = Guid.NewGuid(), Open = TimeOnly.Parse("10:00"), Close = TimeOnly.Parse("18:00") };
            context.Schedules.Add(newSchedule);
            context.SaveChanges();

            fs.ScheduleId = newSchedule.Id;
            var result = await controller.Edit(fs.Id, fs);

            Assert.IsInstanceOf<RedirectToActionResult>(result);
            Assert.That((await context.FacilitySchedules.FindAsync(fs.Id)).ScheduleId, Is.EqualTo(newSchedule.Id));
        }

        [Test]
        public async Task DeleteConfirmed_RemovesFacilitySchedule()
        {
            var context = GetInMemoryContext();
            var controller = new FacilitySchedulesController(context);
            var fs = context.FacilitySchedules.First();

            var result = await controller.DeleteConfirmed(fs.Id);

            Assert.IsInstanceOf<RedirectToActionResult>(result);
            Assert.That(context.FacilitySchedules.Any(f => f.Id == fs.Id), Is.False);
        }
    }
}
