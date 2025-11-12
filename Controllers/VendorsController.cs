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
    public class VendorsController : Controller
    {
        private readonly SQLiteDbContext _context;

        public VendorsController(SQLiteDbContext context)
        {
            _context = context;
        }

        // GET: Vendors
        public async Task<IActionResult> Index()
        {
            return View(await _context.Vendors.ToListAsync());
        }

        // GET: Vendors/Details/5
        public async Task<IActionResult> Details(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var vendors = await _context.Vendors
                .FirstOrDefaultAsync(m => m.VendorID == id);
            if (vendors == null)
            {
                return NotFound();
            }

            return View(vendors);
        }

        // GET: Vendors/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Vendors/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("VendorID,LogoImageFile,Name,Biography,Address,ShopOwnerName,FoodQuality,FoodQuantity,FoodVariety,CollectionExperience,Rating,SavedMeals,TotalReviews")] Vendors vendors)
        {
            if (ModelState.IsValid)
            {
                // Handle file upload
                if (vendors.LogoImageFile != null && vendors.LogoImageFile.Length > 0)
                {
                    using (var ms = new MemoryStream())
                    {
                        await vendors.LogoImageFile.CopyToAsync(ms);
                        vendors.Logo = ms.ToArray();
                    }
                }

                _context.Add(vendors);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(vendors);
        }

        // GET: Vendors/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var vendors = await _context.Vendors.FindAsync(id);
            if (vendors == null)
            {
                return NotFound();
            }
            return View(vendors);
        }

        // POST: Vendors/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, [Bind("VendorID,LogoImageFile,Name,Biography,Address,ShopOwnerName")] Vendors vendors)

        {
            if (id != vendors.VendorID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    // Handle file upload
                    if (vendors.LogoImageFile != null && vendors.LogoImageFile.Length > 0)
                    {
                        using (var ms = new MemoryStream())
                        {
                            await vendors.LogoImageFile.CopyToAsync(ms);
                            vendors.Logo = ms.ToArray();
                        }
                    }
                    else
                    {
                        // Preserve existing image if no new file is uploaded and hidden account stats
                        var existingListing = await _context.Vendors.AsNoTracking().FirstOrDefaultAsync(l => l.VendorID == id);
                        if (existingListing != null)
                        {
                            vendors.Logo = existingListing.Logo;

                        }
                    }

                    _context.Update(vendors);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!VendorsExists(vendors.VendorID))
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
            return View(vendors);
        }

        // GET: Vendors/Delete/5
        public async Task<IActionResult> Delete(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var vendors = await _context.Vendors
                .FirstOrDefaultAsync(m => m.VendorID == id);
            if (vendors == null)
            {
                return NotFound();
            }

            return View(vendors);
        }

        // POST: Vendors/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var vendors = await _context.Vendors.FindAsync(id);
            if (vendors != null)
            {
                _context.Vendors.Remove(vendors);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool VendorsExists(string id)
        {
            return _context.Vendors.Any(e => e.VendorID == id);
        }

        public async Task<IActionResult> GetImage(int id)
        {
            var vendors = await _context.Vendors.FindAsync(id);
            if (vendors == null || vendors.Logo == null)
                return NotFound();

            return File(vendors.Logo, "image/jpeg");
        }
    }
}
