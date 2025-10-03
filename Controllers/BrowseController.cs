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
    public class BrowseController : Controller
    {
        private readonly SQLiteDbContext _context;

        public BrowseController(SQLiteDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {

            List<BrowseViewModel> items = new List<BrowseViewModel>();
            var vendors = await _context.Vendors.ToListAsync() ?? new List<Vendors>();
            var listings = await _context.Listings.ToListAsync() ?? new List<Listing>();


            foreach (Vendors item in vendors)
            {
                Listing list = listings.FirstOrDefault(f => f.VendorID == item.VendorID);


                if (list != null && list.QuantityAvailable > 0)
                {
                    items.Add(new BrowseViewModel
                    {
                        vendors = item,
                        listing = list
                    });
                }


            }

            return View(items);
        }


        public async Task<IActionResult> Orders()
        {
            return View();
        }


    }
}