using System;
using System.Linq;
using System.Threading.Tasks;
using Booking.Controllers;
using Booking.Data;
using Booking.Data.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;

namespace Booking.Tests
{
    [TestFixture]
    public class PricingsControllerTests
    {
        private ApplicationDbContext GetInMemoryContext()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            var context = new ApplicationDbContext(options);

            var facility1 = new Facility { Id = Guid.NewGuid(), Name = "Gym", Address = "123", Mobile = "111", OwnerId = "test-user-id" };
            var facility2 = new Facility { Id = Guid.NewGuid(), Name = "Pool", Address = "456", Mobile = "222", OwnerId = "test-user-id" };

            context.Facilities.AddRange(facility1, facility2);
            context.Pricings.AddRange(
                new Pricing { Id = Guid.NewGuid(), FacilityId = facility1.Id, PricePerHour = 20 },
                new Pricing { Id = Guid.NewGuid(), FacilityId = facility2.Id, PricePerHour = 30 }
            );

            context.SaveChanges();
            return context;
        }

        [Test]
        public async Task Index_ReturnsAllPricings()
        {
            var context = GetInMemoryContext();
            var controller = new PricingsController(context);

            var result = await controller.Index(null) as ViewResult;
            var model = result?.Model as List<Pricing>;

            Assert.IsNotNull(model);
            Assert.That(model.Count, Is.EqualTo(2));
        }

        [Test]
        public async Task Index_WithSearchTerm_FiltersResults()
        {
            var context = GetInMemoryContext();
            var controller = new PricingsController(context);

            var result = await controller.Index("Gym") as ViewResult;
            var model = result?.Model as List<Pricing>;

            Assert.IsNotNull(model);
            Assert.That(model.Count, Is.EqualTo(1));
            Assert.That(model.First().Facility.Name, Is.EqualTo("Gym"));
        }

        [Test]
        public async Task Details_ReturnsPricing_WhenIdIsValid()
        {
            var context = GetInMemoryContext();
            var pricing = context.Pricings.Include(p => p.Facility).First();
            var controller = new PricingsController(context);

            var result = await controller.Details(pricing.Id) as ViewResult;
            var model = result?.Model as Pricing;

            Assert.IsNotNull(model);
            Assert.That(model.Id, Is.EqualTo(pricing.Id));
        }

        [Test]
        public async Task Details_ReturnsNotFound_WhenIdIsInvalid()
        {
            var context = GetInMemoryContext();
            var controller = new PricingsController(context);

            var result = await controller.Details(Guid.NewGuid());

            Assert.IsInstanceOf<NotFoundResult>(result);
        }

        [Test]
        public void Create_Get_ReturnsView()
        {
            var context = GetInMemoryContext();
            var controller = new PricingsController(context);

            var result = controller.Create() as ViewResult;

            Assert.IsNotNull(result);
            Assert.IsTrue(result.ViewData.ContainsKey("Facilities"));
        }

        [Test]
        public async Task Create_Post_AddsPricingAndRedirects()
        {
            var context = GetInMemoryContext();
            var facility = context.Facilities.First();
            var controller = new PricingsController(context);

            var pricing = new Pricing { FacilityId = facility.Id, PricePerHour = 50 };
            var result = await controller.Create(pricing);

            Assert.IsInstanceOf<RedirectToActionResult>(result);
            Assert.That(context.Pricings.Count(), Is.EqualTo(3));
        }

        [Test]
        public async Task Edit_Get_ReturnsView_WhenIdIsValid()
        {
            var context = GetInMemoryContext();
            var pricing = context.Pricings.First();
            var controller = new PricingsController(context);

            var result = await controller.Edit(pricing.Id) as ViewResult;
            var model = result?.Model as Pricing;

            Assert.IsNotNull(model);
            Assert.That(model.Id, Is.EqualTo(pricing.Id));
        }

        [Test]
        public async Task Edit_Post_UpdatesPricing_AndRedirects()
        {
            var context = GetInMemoryContext();
            var pricing = context.Pricings.First();
            pricing.PricePerHour = 100;
            var controller = new PricingsController(context);

            var result = await controller.Edit(pricing.Id, pricing);

            Assert.IsInstanceOf<RedirectToActionResult>(result);
            Assert.That(context.Pricings.Find(pricing.Id).PricePerHour, Is.EqualTo(100));
        }

        [Test]
        public async Task DeleteConfirmed_RemovesPricing_AndRedirects()
        {
            var context = GetInMemoryContext();
            var pricing = context.Pricings.First();
            var controller = new PricingsController(context);

            var result = await controller.DeleteConfirmed(pricing.Id);

            Assert.IsInstanceOf<RedirectToActionResult>(result);
            Assert.That(context.Pricings.Any(p => p.Id == pricing.Id), Is.False);
        }
    }
}
