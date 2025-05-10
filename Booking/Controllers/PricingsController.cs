using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Booking.Data;
using Booking.Data.Entities;

namespace Booking.Controllers
{
    public class PricingsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public PricingsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Pricings
        public async Task<IActionResult> Index(string searchTerm)
        {
            var pricings = from p in _context.Pricings
                           .Include(p => p.Facility)
                           select p;

            if (!string.IsNullOrEmpty(searchTerm))
            {
                pricings = pricings.Where(p =>
                    p.Facility.Name.Contains(searchTerm) ||
                    p.PricePerHour.ToString().Contains(searchTerm));
            }

            return View(await pricings.ToListAsync());
        }

        // GET: Pricings/Details/5
        public async Task<IActionResult> Details(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var pricing = await _context.Pricings
                .Include(p => p.Facility)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (pricing == null)
            {
                return NotFound();
            }

            return View(pricing);
        }

        // GET: Pricings/Create
        [HttpGet]
        public IActionResult Create()
        {
            ViewBag.Facilities = _context.Facilities
                .Select(f => new SelectListItem
                {
                    Value = f.Id.ToString(),
                    Text = f.Name
                }).ToList();
            return View();
        }

        // POST: Pricings/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("FacilityId,PricePerHour")] Pricing pricing)
        {
            if (ModelState.IsValid)
            {
                pricing.Id = Guid.NewGuid();
                _context.Add(pricing);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewBag.Facilities = _context.Facilities
                .Select(f => new SelectListItem
                {
                    Value = f.Id.ToString(),
                    Text = f.Name
                }).ToList();
            return View(pricing);
        }

        // GET: Pricings/Edit/5
        public async Task<IActionResult> Edit(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var pricing = await _context.Pricings.FindAsync(id);
            if (pricing == null)
            {
                return NotFound();
            }
            ViewData["FacilityId"] = new SelectList(_context.Facilities, "Id", "Name", pricing.FacilityId);
            return View(pricing);
        }

        // POST: Pricings/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, [Bind("Id,FacilityId,PricePerHour")] Pricing pricing)
        {
            if (id != pricing.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(pricing);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!PricingExists(pricing.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewBag.FacilityId = new SelectList(_context.Facilities, "Id", "Name", pricing.FacilityId);
            return View(pricing);
        }

        // GET: Pricings/Delete/5
        public async Task<IActionResult> Delete(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var pricing = await _context.Pricings
                .Include(p => p.Facility)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (pricing == null)
            {
                return NotFound();
            }

            return View(pricing);
        }

        // POST: Pricings/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            var pricing = await _context.Pricings.FindAsync(id);
            if (pricing != null)
            {
                _context.Pricings.Remove(pricing);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool PricingExists(Guid id)
        {
            return _context.Pricings.Any(e => e.Id == id);
        }
    }
}
