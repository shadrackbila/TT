using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TimelyTastes.Data;
using TimelyTastes.Models;

namespace TimelyTastes.Controllers
{
    [Route("Ratings")]
    public class RatingInvitationController : Controller
    {
        private readonly SQLiteDbContext _context;

        public RatingInvitationController(SQLiteDbContext context)
        {
            _context = context;
        }

        // GET: Ratings/Rate/{token}
        [HttpGet("Rate")]
        public async Task<IActionResult> Rate(string token)
        {
            var invitation = await _context.RatingInvitations

                .FirstOrDefaultAsync(r => r.Token == token && !r.IsUsed);

            if (invitation == null)
            {
                return NotFound("Rating link not found or already used.");
            }

            if (invitation.ExpiresAt.HasValue && invitation.ExpiresAt < DateTime.UtcNow)
            {
                return BadRequest("This rating link has expired.");
            }

            // ViewBag.VendorName = invitation.Vendor.Name;
            ViewBag.Token = token;

            return View();
        }

        // POST: Ratings/Submit
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Submit(string token, RatingInvitation model)
        {


            var invitation = await _context.RatingInvitations
                .FirstOrDefaultAsync(r => r.Token == token && !r.IsUsed);

            if (invitation == null)
            {
                return NotFound("Rating link not found or already used.");
            }

            if (ModelState.IsValid)
            {
                var orders = _context.Orders
                 .Include(o => o.Vendor)
                .FirstOrDefault(o => o.ID == invitation.VendorId);

                if (orders == null)
                {
                    return NotFound("Something went wrong");
                }

                var vendor = _context.Vendors.FirstOrDefault(o => o.VendorID == orders.Vendor!.VendorID);

                if (vendor == null)
                {
                    return NotFound("Something went wrong with Identity");
                }

                // Update invitation with ratings
                invitation.FoodQualityRating = model.FoodQualityRating;
                invitation.FoodQuantityRating = model.FoodQuantityRating;
                invitation.FoodVarietyRating = model.FoodVarietyRating;
                invitation.CollectionExperienceRating = model.CollectionExperienceRating;
                invitation.Comment = model.Comment;
                invitation.IsUsed = true;
                invitation.UsedAt = DateTime.UtcNow;

                // Update vendor ratings (calculate new average)

                vendor.TotalReviews++;

                // // Recalculate averages
                vendor.FoodQuality = await CalculateNewAverage(vendor.FoodQuality, model.FoodQualityRating.Value, vendor.TotalReviews);
                vendor.FoodQuantity = await CalculateNewAverage(vendor.FoodQuantity, model.FoodQuantityRating.Value, vendor.TotalReviews);
                vendor.FoodVariety = await CalculateNewAverage(vendor.FoodVariety, model.FoodVarietyRating.Value, vendor.TotalReviews);
                vendor.CollectionExperience = await CalculateNewAverage(vendor.CollectionExperience, model.CollectionExperienceRating.Value, vendor.TotalReviews);

                // // Calculate overall rating
                vendor.Rating = (vendor.FoodQuality + vendor.FoodQuantity + vendor.FoodVariety + vendor.CollectionExperience) / 4;

                await _context.SaveChangesAsync();

                return View("ThankYou");
            }

            // ViewBag.VendorName = invitation.Vendor.Name;
            ViewBag.Token = token;
            return View("Rate", model);
        }

        private async Task<double> CalculateNewAverage(double currentAverage, double newRating, int totalReviews)
        {
            // Formula: (currentAverage * (totalReviews-1) + newRating) / totalReviews
            return ((currentAverage * (totalReviews - 1)) + newRating) / totalReviews;
        }
    }
}