using NUnit.Framework;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Booking.Data;
using Booking.Data.Entities;
using Booking.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Booking.Tests
{
    public class PricingsControllerTests
    {
        private ApplicationDbContext GetInMemoryContext()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            var context = new ApplicationDbContext(options);

            var facility = new Facility
            {
                Id = Guid.NewGuid(),
                Name = "Test Facility",
                Mobile = "111",
                Address = "123 Test Street"
            };

            context.Facilities.Add(facility);

            context.Pricings.AddRange(new[]
            {
                new Pricing { Id = Guid.NewGuid(), FacilityId = facility.Id, PricePerHour = 50 },
                new Pricing { Id = Guid.NewGuid(), FacilityId = facility.Id, PricePerHour = 100 }
            });

            context.SaveChanges();
            return context;
        }

        [Test]
        public async Task Index_ReturnsAllPricings()
        {
            var context = GetInMemoryContext();
            var controller = new PricingsController(context);

            var result = await controller.Index(null) as ViewResult;
            var model = result.Model as List<Pricing>;

            Assert.That(model.Count, Is.EqualTo(2));
        }

        [Test]
        public async Task Index_WithSearchTerm_FiltersResults()
        {
            var context = GetInMemoryContext();
            var controller = new PricingsController(context);

            var result = await controller.Index("50") as ViewResult;
            var model = result.Model as List<Pricing>;

            Assert.That(model.Count, Is.EqualTo(1));
            Assert.That(model.First().PricePerHour, Is.EqualTo(50));
        }

        [Test]
        public async Task Details_ReturnsPricing_WhenIdIsValid()
        {
            var context = GetInMemoryContext();
            var controller = new PricingsController(context);
            var pricing = context.Pricings.First();

            var result = await controller.Details(pricing.Id) as ViewResult;
            var model = result.Model as Pricing;

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
        public async Task Create_Post_AddsPricing_AndRedirects()
        {
            var context = GetInMemoryContext();
            var controller = new PricingsController(context);
            var facilityId = context.Facilities.First().Id;

            var pricing = new Pricing
            {
                FacilityId = facilityId,
                PricePerHour = 120
            };

            var result = await controller.Create(pricing);

            Assert.IsInstanceOf<RedirectToActionResult>(result);
            Assert.That(context.Pricings.Count(), Is.EqualTo(3));
        }

        [Test]
        public async Task Edit_Post_UpdatesPricing()
        {
            var context = GetInMemoryContext();
            var controller = new PricingsController(context);
            var pricing = context.Pricings.First();
            pricing.PricePerHour = 75;

            var result = await controller.Edit(pricing.Id, pricing);

            Assert.IsInstanceOf<RedirectToActionResult>(result);
            Assert.That((await context.Pricings.FindAsync(pricing.Id)).PricePerHour, Is.EqualTo(75));
        }

        [Test]
        public async Task DeleteConfirmed_DeletesPricing()
        {
            var context = GetInMemoryContext();
            var controller = new PricingsController(context);
            var pricing = context.Pricings.First();

            var result = await controller.DeleteConfirmed(pricing.Id);

            Assert.IsInstanceOf<RedirectToActionResult>(result);
            Assert.That(context.Pricings.Any(p => p.Id == pricing.Id), Is.False);
        }
    }
}
