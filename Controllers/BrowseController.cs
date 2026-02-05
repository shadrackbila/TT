using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using TimelyTastes.Data;
using TimelyTastes.Helper;
using TimelyTastes.Models;
using TimelyTastes.Services;

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


            string sourceLatitude = TempData["userLatitude"]?.ToString() ?? "-29.12107"; // Approx. Bloemfontein city latitude :contentReference[oaicite:0]{index=0}
            string sourceLongitude = TempData["userLongitude"]?.ToString() ?? "26.21400"; // Approx. Bloemfontein city longitude :contentReference[oaicite:1]{index=1}

            string destinationLatitude = "-29.12260"; // Approx. CUT Bloemfontein campus latitude :contentReference[oaicite:2]{index=2}
            string destinationLongitude = "26.21310"; // Approx. CUT Bloemfontein campus longitude :contentReference[oaicite:3]{index=3}


            string modes = "foot,car";
            string units = "metric";

            IRadarDistanceApi radarDistanceApi = new RadarDistanceApi();
            string distanceResponse = await radarDistanceApi.GetDistanceAsync(sourceLatitude, sourceLongitude, destinationLatitude, destinationLongitude, modes, units);

            var json = JsonDocument.Parse(distanceResponse);

            double carDistanceMeters =
                json.RootElement
                    .GetProperty("routes")
                    .GetProperty("foot")
                    .GetProperty("distance")
                    .GetProperty("value")
                    .GetDouble();

            Console.WriteLine($"Distance Response from Radar API: {carDistanceMeters} meters.");


            var items = await (from v in _context.Vendors
                               join l in _context.Listings
                               on v.VendorID equals l.VendorID
                               where l.QuantityAvailable > 0 && l.HideListing == false && v.IsDeleted == false && carDistanceMeters <= 1000
                               select new Browse
                               {
                                   vendors = v,
                                   listing = l
                               }).AsNoTracking().ToListAsync();

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
                                      where l.QuantityAvailable > 0 && l.Id == id && l.HideListing == false
                                      select new Browse
                                      {
                                          vendors = v,
                                          listing = l
                                      }).AsNoTracking().FirstOrDefaultAsync();

            if (orderDetails == null)
            {
                return NotFound();
            }


            return View(orderDetails);
        }

        [HttpPost]
        public JsonResult UserLocation([FromBody] UserLocation data)
        {
            Console.WriteLine($"Received Coordinates");

            string latitude = data.Latitude.ToString();
            string longitude = data.Longitude.ToString();

            TempData["userLatitude"] = latitude;
            TempData["userLongitude"] = longitude;

            // Preserve TempData for the next request
            TempData.Keep("userLatitude");
            TempData.Keep("userLongitude");

            // Return a JSON response indicating success
            return Json(new { success = true, message = "User location received successfully." });

        }


    }
}