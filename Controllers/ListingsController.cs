using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using TimelyTastes.Data;
using TimelyTastes.Models;

namespace TimelyTastes.Controllers
{
    public class ListingsController : Controller
    {
        private readonly SQLiteDbContext _context;

        public ListingsController(SQLiteDbContext context)
        {
            _context = context;
        }

        // GET: Listings
        public async Task<IActionResult> Index()
        {
            return View(await _context.Listings.ToListAsync());
        }

        // GET: Listings/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var listing = await _context.Listings
                .FirstOrDefaultAsync(m => m.Id == id);
            if (listing == null)
            {
                return NotFound();
            }

            return View(listing);
        }

        // GET: Listings/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Listings/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,VendorID,Name,Description,DiscountPrice,OriginalPrice,QuantityAvailable,AvailableFrom,AvailableUntil,ImageFile")] Listing listing)
        {
            if (ModelState.IsValid)
            {
                // Handle file upload
                if (listing.ImageFile != null && listing.ImageFile.Length > 0)
                {
                    using (var ms = new MemoryStream())
                    {
                        await listing.ImageFile.CopyToAsync(ms);
                        listing.ImageData = ms.ToArray();
                    }
                }

                _context.Add(listing);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(listing);
        }


        // GET: Listings/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var listing = await _context.Listings.FindAsync(id);
            if (listing == null)
            {
                return NotFound();
            }
            return View(listing);
        }

        // POST: Listings/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,VendorID,Name,Description,DiscountPrice,OriginalPrice,QuantityAvailable,AvailableFrom,AvailableUntil,ImageFile")] Listing listing)
        {
            if (id != listing.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    // Handle file upload
                    if (listing.ImageFile != null && listing.ImageFile.Length > 0)
                    {
                        using (var ms = new MemoryStream())
                        {
                            await listing.ImageFile.CopyToAsync(ms);
                            listing.ImageData = ms.ToArray();
                        }
                    }
                    else
                    {
                        // Preserve existing image if no new file is uploaded
                        var existingListing = await _context.Listings.AsNoTracking().FirstOrDefaultAsync(l => l.Id == id);
                        if (existingListing != null)
                        {
                            listing.ImageData = existingListing.ImageData;
                        }
                    }

                    _context.Update(listing);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ListingExists(listing.Id))
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
            return View(listing);
        }


        // GET: Listings/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var listing = await _context.Listings
                .FirstOrDefaultAsync(m => m.Id == id);
            if (listing == null)
            {
                return NotFound();
            }

            return View(listing);
        }

        // POST: Listings/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var listing = await _context.Listings.FindAsync(id);
            if (listing != null)
            {
                _context.Listings.Remove(listing);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ListingExists(int id)
        {
            return _context.Listings.Any(e => e.Id == id);
        }

        public async Task<IActionResult> GetImage(int id)
        {
            var listing = await _context.Listings.FindAsync(id);
            if (listing == null || listing.ImageData == null)
                return NotFound();

            return File(listing.ImageData, "image/jpeg");
        }

    }
}
