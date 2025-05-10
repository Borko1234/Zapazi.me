using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Booking.Data;
using Booking.Data.Entities;
using Booking.Data.Identity.Users;
using Microsoft.AspNetCore.Identity;

namespace Booking.Controllers
{
    public class FacilitiesController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<BookingUser> _userManager;

        public FacilitiesController(ApplicationDbContext context, UserManager<BookingUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: Facilities
        public async Task<IActionResult> Index(string searchTerm)
        {
            // Retrieve all facilities
            var facilities = from f in _context.Facilities
                             select f;

            // Filter facilities if a search term is provided
            if (!string.IsNullOrEmpty(searchTerm))
            {
                facilities = facilities.Where(f => f.Name.Contains(searchTerm));
            }

            // Return the filtered list to the view
            return View(await facilities.ToListAsync());
        }

        // GET: Facilities/Details by ID
        [HttpGet]
        public async Task<IActionResult> Details(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var facility = await _context.Facilities
                .FirstOrDefaultAsync(f => f.Id == id);

            if (facility == null)
            {
                return NotFound();
            }

            return View(facility);
        }

        // GET: Facilities/Details by Search Term
        [HttpGet("Facilities/SearchDetails")]
        public async Task<IActionResult> DetailsBySearch(string searchTerm)
        {
            if (string.IsNullOrEmpty(searchTerm))
            {
                return NotFound();
            }

            var facility = await _context.Facilities
                .FirstOrDefaultAsync(f => f.Name == searchTerm);

            if (facility == null)
            {
                return NotFound();
            }

            return View("Details", facility); // Reuse the Details view
        }

        // GET: Facilities/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Facilities/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Name,Mobile,Address,Id")] Facility facility)
        {
            ModelState.Remove("OwnerId");

            if (ModelState.IsValid)
            {
                facility.Id = Guid.NewGuid();
                var user = await _userManager.GetUserAsync(User);
                facility.OwnerId = user!.Id; // Set the owner to the current user

                _context.Add(facility);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(facility);
        }

        // GET: Facilities/Edit/5
        public async Task<IActionResult> Edit(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var facility = await _context.Facilities.FindAsync(id);
            if (facility == null)
            {
                return NotFound();
            }
            return View(facility);
        }

        // POST: Facilities/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, [Bind("Name,Mobile,Address,Id")] Facility facility)
        {
            if (id != facility.Id)
            {
                return NotFound();
            }

            ModelState.Remove("OwnerId");

            if (ModelState.IsValid)
            {
                try
                {
                    var user = await _userManager.GetUserAsync(User);
                    facility.OwnerId = user.Id;

                    _context.Update(facility);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!FacilityExists(facility.Id))
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
            return View(facility);
        }

        // GET: Facilities/Delete/5
        public async Task<IActionResult> Delete(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var facility = await _context.Facilities
                .FirstOrDefaultAsync(m => m.Id == id);
            if (facility == null)
            {
                return NotFound();
            }

            return View(facility);
        }

        // POST: Facilities/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(Guid id)
        {
            var facility = await _context.Facilities.FindAsync(id);
            if (facility != null)
            {
                _context.Facilities.Remove(facility);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }

        private bool FacilityExists(Guid id)
        {
            return _context.Facilities.Any(e => e.Id == id);
        }
    }
}