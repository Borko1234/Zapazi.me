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

namespace Booking.Tests
{
    public class SchedulesControllerTests
    {
        private ApplicationDbContext GetInMemoryContext()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            var context = new ApplicationDbContext(options);

            context.Schedules.AddRange(new[]
            {
                new Schedule { Id = Guid.NewGuid(), Open = TimeOnly.Parse("08:00"), Close = TimeOnly.Parse("16:00") },
                new Schedule { Id = Guid.NewGuid(), Open = TimeOnly.Parse("10:00"), Close = TimeOnly.Parse("18:00") }
            });

            context.SaveChanges();
            return context;
        }

        [Test]
        public async Task Index_ReturnsAllSchedules()
        {
            var context = GetInMemoryContext();
            var controller = new SchedulesController(context);

            var result = await controller.Index(null) as ViewResult;
            var model = result.Model as IEnumerable<Schedule>;

            Assert.That(model.Count(), Is.EqualTo(2));
        }

        [Test]
        public async Task Index_WithSearchTerm_ReturnsFilteredSchedules()
        {
            var context = GetInMemoryContext();
            var controller = new SchedulesController(context);

            var result = await controller.Index("08") as ViewResult;
            var model = result.Model as IEnumerable<Schedule>;

            Assert.That(model.Count(), Is.EqualTo(1));
        }

        [Test]
        public async Task Details_ReturnsViewResult_WhenIdIsValid()
        {
            var context = GetInMemoryContext();
            var schedule = context.Schedules.First();
            var controller = new SchedulesController(context);

            var result = await controller.Details(schedule.Id) as ViewResult;
            var model = result.Model as Schedule;

            Assert.That(model.Id, Is.EqualTo(schedule.Id));
        }

        [Test]
        public async Task Details_ReturnsNotFound_WhenIdIsNull()
        {
            var context = GetInMemoryContext();
            var controller = new SchedulesController(context);

            var result = await controller.Details(null);

            Assert.IsInstanceOf<NotFoundResult>(result);
        }

        [Test]
        public async Task Create_Post_AddsSchedule_AndRedirects()
        {
            var context = GetInMemoryContext();
            var controller = new SchedulesController(context);
            var schedule = new Schedule
            {
                Open = TimeOnly.Parse("07:00"),
                Close = TimeOnly.Parse("15:00")
            };

            var result = await controller.Create(schedule);

            Assert.IsInstanceOf<RedirectToActionResult>(result);
            Assert.That(context.Schedules.Count(), Is.EqualTo(3));
        }

        [Test]
        public async Task Edit_Post_UpdatesSchedule()
        {
            var context = GetInMemoryContext();
            var controller = new SchedulesController(context);
            var schedule = context.Schedules.First();
            schedule.Open = TimeOnly.Parse("09:00");

            var result = await controller.Edit(schedule.Id, schedule);

            Assert.IsInstanceOf<RedirectToActionResult>(result);
            Assert.That((await context.Schedules.FindAsync(schedule.Id)).Open, Is.EqualTo(TimeOnly.Parse("09:00")));
        }

        [Test]
        public async Task DeleteConfirmed_RemovesSchedule()
        {
            var context = GetInMemoryContext();
            var controller = new SchedulesController(context);
            var schedule = context.Schedules.First();

            var result = await controller.DeleteConfirmed(schedule.Id);

            Assert.IsInstanceOf<RedirectToActionResult>(result);
            Assert.That(context.Schedules.Any(s => s.Id == schedule.Id), Is.False);
        }
    }
}
