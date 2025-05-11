using Booking.Controllers;
using Booking.Data.Entities;
using Booking.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Booking.Tests
{
    [TestFixture]
    public class SchedulesControllerTests
    {
        private ApplicationDbContext GetInMemoryContext()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            var context = new ApplicationDbContext(options);
            context.Schedules.AddRange(
                new Schedule { Id = Guid.NewGuid(), Open = TimeOnly.Parse("08:00"), Close = TimeOnly.Parse("18:00") },
                new Schedule { Id = Guid.NewGuid(), Open = TimeOnly.Parse("09:00"), Close = TimeOnly.Parse("17:00") }
            );
            context.SaveChanges();

            return context;
        }

        [Test]
        public async Task Index_ReturnsAllSchedules()
        {
            var context = GetInMemoryContext();
            var controller = new SchedulesController(context);

            var result = await controller.Index(null) as ViewResult;
            var model = result?.Model as List<Schedule>;

            Assert.IsNotNull(model);
            Assert.That(model.Count, Is.EqualTo(2));
        }

        [Test]
        public async Task Index_WithSearchTerm_FiltersSchedules()
        {
            var context = GetInMemoryContext();
            var controller = new SchedulesController(context);

            var result = await controller.Index("8") as ViewResult;
            var model = result?.Model as List<Schedule>;

            Assert.That(model.Count, Is.EqualTo(1));
        }

        [Test]
        public async Task Details_NullId_ReturnsNotFound()
        {
            var context = GetInMemoryContext();
            var controller = new SchedulesController(context);

            var result = await controller.Details(null);

            Assert.IsInstanceOf<NotFoundResult>(result);
        }

        [Test]
        public async Task Details_InvalidId_ReturnsNotFound()
        {
            var context = GetInMemoryContext();
            var controller = new SchedulesController(context);

            var result = await controller.Details(Guid.NewGuid());

            Assert.IsInstanceOf<NotFoundResult>(result);
        }

        [Test]
        public async Task Details_ValidId_ReturnsView()
        {
            var context = GetInMemoryContext();
            var schedule = context.Schedules.First();
            var controller = new SchedulesController(context);

            var result = await controller.Details(schedule.Id) as ViewResult;

            Assert.IsNotNull(result);
            Assert.IsInstanceOf<Schedule>(result.Model);
        }

        [Test]
        public void Create_Get_ReturnsView()
        {
            var context = GetInMemoryContext();
            var controller = new SchedulesController(context);

            var result = controller.Create() as ViewResult;

            Assert.IsNotNull(result);
        }

        [Test]
        public async Task Create_Post_AddsSchedule()
        {
            var context = GetInMemoryContext();
            var controller = new SchedulesController(context);
            var newSchedule = new Schedule { Open = TimeOnly.Parse("10:00"), Close = TimeOnly.Parse("19:00") };

            var result = await controller.Create(newSchedule) as RedirectToActionResult;

            Assert.That(context.Schedules.Count(), Is.EqualTo(3));
            Assert.That(result.ActionName, Is.EqualTo("Index"));
        }

        [Test]
        public async Task Edit_Get_InvalidId_ReturnsNotFound()
        {
            var context = GetInMemoryContext();
            var controller = new SchedulesController(context);

            var result = await controller.Edit(Guid.NewGuid());

            Assert.IsInstanceOf<NotFoundResult>(result);
        }

        [Test]
        public async Task Edit_Get_ValidId_ReturnsView()
        {
            var context = GetInMemoryContext();
            var schedule = context.Schedules.First();
            var controller = new SchedulesController(context);

            var result = await controller.Edit(schedule.Id) as ViewResult;

            Assert.IsNotNull(result);
            Assert.IsInstanceOf<Schedule>(result.Model);
        }

        [Test]
        public async Task Edit_Post_UpdatesSchedule()
        {
            var context = GetInMemoryContext();
            var schedule = context.Schedules.First();
            var controller = new SchedulesController(context);

            schedule.Close = TimeOnly.Parse("20:00");

            var result = await controller.Edit(schedule.Id, schedule) as RedirectToActionResult;

            Assert.That(context.Schedules.Find(schedule.Id)?.Close, Is.EqualTo(TimeOnly.Parse("20:00")));
            Assert.That(result.ActionName, Is.EqualTo("Index"));
        }

        [Test]
        public async Task Delete_Get_InvalidId_ReturnsNotFound()
        {
            var context = GetInMemoryContext();
            var controller = new SchedulesController(context);

            var result = await controller.Delete(Guid.NewGuid());

            Assert.IsInstanceOf<NotFoundResult>(result);
        }

        [Test]
        public async Task Delete_Get_ValidId_ReturnsView()
        {
            var context = GetInMemoryContext();
            var schedule = context.Schedules.First();
            var controller = new SchedulesController(context);

            var result = await controller.Delete(schedule.Id) as ViewResult;

            Assert.IsNotNull(result);
            Assert.IsInstanceOf<Schedule>(result.Model);
        }

        [Test]
        public async Task DeleteConfirmed_DeletesSchedule()
        {
            var context = GetInMemoryContext();
            var schedule = context.Schedules.First();
            var controller = new SchedulesController(context);

            var result = await controller.DeleteConfirmed(schedule.Id) as RedirectToActionResult;

            Assert.That(context.Schedules.Any(s => s.Id == schedule.Id), Is.False);
            Assert.That(result.ActionName, Is.EqualTo("Index"));
        }
    }
}
