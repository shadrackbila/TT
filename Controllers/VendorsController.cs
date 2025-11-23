using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FirebaseAdmin.Auth;
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
        private readonly string _apiKey;


        public VendorsController(SQLiteDbContext context, IConfiguration config)
        {
            _context = context;
            _apiKey = config["ApiSettings:ApiKey"] ?? "";

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

                return RedirectToAction("Index", "Listings");

            }
            return View(vendors);
        }

        // GET: Vendors/Edit/5
        public async Task<IActionResult> Edit()
        {
            var vendorId = HttpContext.Session.GetString("VendorID");

            // 1. If no session, redirect
            if (vendorId == null)
                return RedirectToAction("LogIn", "LogIn");

            var vendors = await _context.Vendors.FirstOrDefaultAsync(o => o.VendorID == vendorId);

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
        public async Task<IActionResult> Edit(Vendors updateVendor)

        {
            var vendorId = HttpContext.Session.GetString("VendorID");

            // 1. If no session, redirect
            if (vendorId == null)
                return RedirectToAction("LogIn", "LogIn");


            if (ModelState.IsValid)
            {
                try
                {
                    var ExistingVendor = await _context.Vendors.FirstOrDefaultAsync(o => o.VendorID == vendorId);

                    if (ExistingVendor == null)
                    {
                        return NotFound();
                    }


                    // Handle file upload
                    if (updateVendor.LogoImageFile != null && updateVendor.LogoImageFile.Length > 0)
                    {
                        using (var ms = new MemoryStream())
                        {
                            await updateVendor.LogoImageFile.CopyToAsync(ms);
                            ExistingVendor.Logo = ms.ToArray();
                        }
                    }

                    ExistingVendor.ShopOwnerName = updateVendor.ShopOwnerName;
                    ExistingVendor.Name = updateVendor.Name;
                    ExistingVendor.Biography = updateVendor.Biography;
                    ExistingVendor.Address = updateVendor.Address;


                    _context.Update(ExistingVendor);
                    await _context.SaveChangesAsync();


                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!VendorsExists(updateVendor.VendorID))
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
            return View(updateVendor);
        }

        // GET: Vendors/Delete/5
        public async Task<IActionResult> Delete()
        {
            var vendorId = HttpContext.Session.GetString("VendorID");

            // 1. If no session, redirect
            if (vendorId == null)
                return RedirectToAction("LogIn", "LogIn");

            var vendors = await _context.Vendors
                .FirstOrDefaultAsync(m => m.VendorID == vendorId);
            if (vendors == null)
            {
                return NotFound();
            }

            return View(vendors);
        }

        // POST: Vendors/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed()
        {
            var vendorId = HttpContext.Session.GetString("VendorID");
            var AccessToken = HttpContext.Session.GetString("AccessToken");



            // 1. If no session, redirect
            if (vendorId == null || AccessToken == null)
                return RedirectToAction("LogIn", "LogIn");

            var vendors = await _context.Vendors
                .FirstOrDefaultAsync(m => m.VendorID == vendorId);

            if (vendors != null)
            {
                vendors.IsDeleted = true;



                using var httpClient = new HttpClient();

                var payload = new
                {
                    idToken = AccessToken
                };

                var response = await httpClient.PostAsJsonAsync(
                    $"https://identitytoolkit.googleapis.com/v1/accounts:delete?key={_apiKey}",
                    payload
                );

                if (!response.IsSuccessStatusCode)
                {
                    // Handle failure
                }

                HttpContext.Session.Remove("VendorID");
                HttpContext.Session.Remove("AccessToken");
                await _context.SaveChangesAsync();
                return RedirectToAction("LogIn", "LogIn");


            }

            return View("Index");

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
