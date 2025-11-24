using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using TimelyTastes.Data;
using TimelyTastes.Interfaces;
using TimelyTastes.Models;

namespace TimelyTastes.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private readonly SQLiteDbContext _context;
    private readonly IDBInitializer _seedDatabase;

    public HomeController(ILogger<HomeController> logger, SQLiteDbContext context,
IDBInitializer seedDatabase)
    {
        _logger = logger;
        _context = context;
        _seedDatabase = seedDatabase;
    }

    public IActionResult Index()
    {
        return View();

    }

    public IActionResult SeedDatabase()
    {
        _seedDatabase.Initialize(_context);
        ViewBag.SeedDbFeedback = "Database created and Student Table populated with Data. Check Database folder.";
        return View("SeedDatabase");
    }

    public IActionResult Privacy()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View();
    }
}
