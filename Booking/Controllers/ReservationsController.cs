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
    public class ReservationsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<BookingUser> _userManager;

        public ReservationsController(ApplicationDbContext context,
            UserManager<BookingUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: Reservations
        public async Task<IActionResult> Index(string searchTerm)
        {
            var reservations = _context.Reservations
                .Include(r => r.Facility)
                .Include(r => r.User)
                .AsQueryable();

            if (!string.IsNullOrEmpty(searchTerm))
            {
                reservations = reservations.Where(r =>
                    r.Facility.Name.Contains(searchTerm) ||
                    r.User.UserName.Contains(searchTerm));
            }

            return View(await reservations.ToListAsync());
        }

        // GET: Reservations/Details/5
        public async Task<IActionResult> Details(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var reservation = await _context.Reservations
                .Include(r => r.Facility)
                .Include(r => r.User)
                .FirstOrDefaultAsync(r => r.Id == id);
            if (reservation == null)
            {
                return NotFound();
            }

            return View(reservation);
        }

        // GET: Reservations/Create
        public IActionResult Create(Guid? facilityId)
        {
            ViewData["FacilityId"] = new SelectList(_context.Facilities, "Id", "Name", facilityId);
            ViewData["UserId"] = new SelectList(_context.Users, "Id", "UserName");
            var reservation = new Reservation();
            if (facilityId.HasValue)
                reservation.FacilityId = facilityId.Value;
            return View(reservation);
        }

        // POST: Reservations/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("FacilityId,Date,Duration,Description")] Reservation reservation)
        {
            ModelState.Remove("UserId");

            if (ModelState.IsValid)
            {
                reservation.Id = Guid.NewGuid();
                var user = await _userManager.GetUserAsync(User);
                reservation.UserId = user.Id;
                if (reservation.Date == DateTime.MinValue)
                {
                    reservation.Date = DateTime.Now;
                }

                _context.Add(reservation);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["FacilityId"] = new SelectList(_context.Facilities, "Id", "Name", reservation.FacilityId);
            return View(reservation);
        }

        // GET: Reservations/Edit/5
        public async Task<IActionResult> Edit(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var reservation = await _context.Reservations.FindAsync(id);
            if (reservation == null)
            {
                return NotFound();
            }
            ViewData["FacilityId"] = new SelectList(_context.Facilities, "Id", "Name", reservation.FacilityId);
            ViewData["UserId"] = new SelectList(_context.Users, "Id", "UserName", reservation.UserId);
            return View(reservation);
        }

        // POST: Reservations/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, [Bind("Id,FacilityId,Date,Duration,Description")] Reservation reservation)
        {
            ModelState.Remove("UserId");

            if (id != reservation.Id)
            {
                return NotFound();
            }

        
            if (ModelState.IsValid)
            {
                try
                {
                    var user = await _userManager.GetUserAsync(User);
                    reservation.UserId = user.Id;

                    _context.Update(reservation);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!_context.Reservations.Any(e => e.Id == reservation.Id))
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
            ViewData["FacilityId"] = new SelectList(_context.Facilities, "Id", "Name", reservation.FacilityId);
            return View(reservation);
        }

        // GET: Reservations/Delete/5
        public async Task<IActionResult> Delete(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var reservation = await _context.Reservations
                .Include(r => r.Facility)
                .Include(r => r.User)
                .FirstOrDefaultAsync(r => r.Id == id);
            if (reservation == null)
            {
                return NotFound();
            }

            return View(reservation);
        }

        // POST: Reservations/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            var reservation = await _context.Reservations.FindAsync(id);
            if (reservation != null)
            {
                _context.Reservations.Remove(reservation);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ReservationExists(Guid id)
        {
            return _context.Reservations.Any(e => e.Id == id);
        }
    }
}
