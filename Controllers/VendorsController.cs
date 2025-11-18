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
            var vendorId = HttpContext.Session.GetString("VendorID");

            // 1. If no session, redirect
            if (vendorId == null)
                return RedirectToAction("LogIn", "LogIn");

            var vendor = await _context.Vendors.FirstOrDefaultAsync(o => o.VendorID == vendorId);

            if (vendor == null)
            {
                return NotFound();
            }


            return View("Details", vendor);
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
                var vendorId = HttpContext.Session.GetString("VendorID");

                // 1. If no session, redirect
                if (vendorId == null)
                    return RedirectToAction("LogIn", "LogIn");



                // Handle file upload
                if (vendors.LogoImageFile != null && vendors.LogoImageFile.Length > 0)
                {
                    using (var ms = new MemoryStream())
                    {
                        await vendors.LogoImageFile.CopyToAsync(ms);
                        vendors.Logo = ms.ToArray();
                    }
                }

                // 2. If session exists but vendor does not exist in DB, redirect

                if (!_context.Vendors.Any(v => v.VendorID == vendorId))
                {
                    var vendor = new Vendors
                    {
                        VendorID = vendorId,
                        Name = vendors.Name,
                        Biography = vendors.Biography,
                        Address = vendors.Address,
                        ShopOwnerName = vendors.ShopOwnerName,
                        FoodQuality = vendors.FoodQuality,
                        FoodQuantity = vendors.FoodQuantity,
                        FoodVariety = vendors.FoodVariety,
                        CollectionExperience = vendors.CollectionExperience,
                        Rating = vendors.Rating,
                        SavedMeals = vendors.SavedMeals,
                        TotalReviews = vendors.TotalReviews,
                        Logo = vendors.Logo
                    };


                    _context.Vendors.Add(vendor);
                    await _context.SaveChangesAsync();
                    return RedirectToAction("Index", "Listings");


                }



                // if (existingVendor != null)
                // {
                //     // UPDATE — copy all data EXCEPT VendorID
                //     existingVendor.Name = vendors.Name;
                //     existingVendor.Biography = vendors.Biography;
                //     existingVendor.Address = vendors.Address;
                //     existingVendor.ShopOwnerName = vendors.ShopOwnerName;
                //     existingVendor.FoodQuality = vendors.FoodQuality;
                //     existingVendor.FoodQuantity = vendors.FoodQuantity;
                //     existingVendor.FoodVariety = vendors.FoodVariety;
                //     existingVendor.CollectionExperience = vendors.CollectionExperience;
                //     existingVendor.Rating = vendors.Rating;
                //     existingVendor.SavedMeals = vendors.SavedMeals;
                //     existingVendor.TotalReviews = vendors.TotalReviews;

                //     // Update logo only if new file uploaded
                //     if (vendors.Logo.Length > 0)
                //         existingVendor.Logo = vendors.Logo;

                //     _context.Update(existingVendor);
                // }
                // else
                // {
                //     return RedirectToAction("Register", "SignUp");

                // }


                return RedirectToAction("Index", "Listings");

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

        public async Task<IActionResult> GetImage(string id)
        {
            var vendors = await _context.Vendors.FirstOrDefaultAsync(o => o.VendorID == id);
            if (vendors == null || vendors.Logo == null)
                return NotFound();

            return File(vendors.Logo, "image/jpeg");
        }
    }
}
