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
    public class FacilitySchedulesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public FacilitySchedulesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: FacilitySchedules
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.FacilitySchedules.Include(f => f.Facility).Include(f => f.Schedule);
            return View(await applicationDbContext.ToListAsync());
        }

        // GET: FacilitySchedules/Details/5
        public async Task<IActionResult> Details(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var facilitySchedule = await _context.FacilitySchedules
                .Include(f => f.Facility)
                .Include(f => f.Schedule)
                .FirstOrDefaultAsync(m => m.FacilityId == id);
            if (facilitySchedule == null)
            {
                return NotFound();
            }

            return View(facilitySchedule);
        }

        // GET: FacilitySchedules/Create
        public IActionResult Create()
        {
            ViewData["FacilityId"] = new SelectList(_context.Facilities, "Id", "Name");
            ViewData["ScheduleId"] = new SelectList(
            _context.Schedules
                .Select(s => new {
                    s.Id,
                    Display = s.Open.ToString("HH:mm") + " - " + s.Close.ToString("HH:mm")
                }),
                "Id",
                "Display"
            );
            return View();
        }

        // POST: FacilitySchedules/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("FacilityId,ScheduleId")] FacilitySchedule facilitySchedule)
        {
            if (ModelState.IsValid)
            {
                _context.Add(facilitySchedule);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["FacilityId"] = new SelectList(_context.Facilities, "Id", "Address", facilitySchedule.FacilityId);
            ViewData["ScheduleId"] = new SelectList(_context.Schedules, "Id", "Id", facilitySchedule.ScheduleId);
            return View(facilitySchedule);
        }

        // GET: FacilitySchedules/Edit/5
        public async Task<IActionResult> Edit(Guid? Id)
        {
            if (Id == null)
            {
                return NotFound();
            }

            var facilitySchedule = await _context.FacilitySchedules.FindAsync(Id);
            if (facilitySchedule == null)
            {
                return NotFound();
            }
            ViewData["FacilityId"] = new SelectList(_context.Facilities, "Id", "Address", facilitySchedule.FacilityId);
            ViewData["ScheduleId"] = new SelectList(_context.Schedules, "Id", "Id", facilitySchedule.ScheduleId);
            return View(facilitySchedule);
        }

        // POST: FacilitySchedules/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid Id, [Bind("FacilityId,ScheduleId")] FacilitySchedule facilitySchedule)
        {
            if (Id != facilitySchedule.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(facilitySchedule);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!FacilityScheduleExists(facilitySchedule.FacilityId))
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
            ViewData["FacilityId"] = new SelectList(_context.Facilities, "Id", "Name", facilitySchedule.FacilityId);
            ViewData["ScheduleId"] = new SelectList(_context.Schedules, "Id", "Id", facilitySchedule.ScheduleId);
            return View(facilitySchedule);
        }

        // GET: FacilitySchedules/Delete/5
        public async Task<IActionResult> Delete(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var facilitySchedule = await _context.FacilitySchedules
                .Include(f => f.Facility)
                .Include(f => f.Schedule)
                .FirstOrDefaultAsync(m => m.FacilityId == id);
            if (facilitySchedule == null)
            {
                return NotFound();
            }

            return View(facilitySchedule);
        }

        // POST: FacilitySchedules/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            var facilitySchedule = await _context.FacilitySchedules.FindAsync(id);
            if (facilitySchedule != null)
            {
                _context.FacilitySchedules.Remove(facilitySchedule);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool FacilityScheduleExists(Guid id)
        {
            return _context.FacilitySchedules.Any(e => e.FacilityId == id);
        }
    }
}
