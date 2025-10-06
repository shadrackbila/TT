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
            var items = await (from v in _context.Vendors
                               join l in _context.Listings
                               on v.VendorID equals l.VendorID
                               where l.QuantityAvailable > 0
                               select new Browse
                               {
                                   vendors = v,
                                   listing = l
                               }).ToListAsync();

            return View(items);
        }



        public async Task<IActionResult> Summary(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var orderDetails = await (from v in _context.Vendors
                                      join l in _context.Listings
                                      on v.VendorID equals l.VendorID
                                      where l.QuantityAvailable > 0 && l.Id == id
                                      select new Browse
                                      {
                                          vendors = v,
                                          listing = l
                                      }).FirstOrDefaultAsync();

            if (orderDetails == null)
            {
                return NotFound();
            }


            return View(orderDetails);
        }


    }
}