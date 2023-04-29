using ExhibitionApp.Data;
using ExhibitionApp.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ExhibitionApp.Controllers
{
    [Authorize(Roles = "Storekeeper")]
    public class ReferenceTableController : Controller
    {
        private readonly ExhibitionAppDbContext _dbContext;

        public ReferenceTableController(ExhibitionAppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public IActionResult GetAllExtraTables()
        {
            return View();
        }

        [HttpGet]
        public IActionResult GetAllExhibitTypes()
        {
            List<ExhibitType> allExhibitTypes = _dbContext.ExhibitTypes.ToList();

            return View(allExhibitTypes);
        }

        [HttpGet]
        public IActionResult GetAllCities()
        {
            List<City> allCities = _dbContext.Cities.ToList();

            return View(allCities);
        }

        [HttpGet]
        public IActionResult GetAllStreets()
        {
            //List<Street> allStreets = _dbContext.Streets.ToList();
            var allStreets = _dbContext.Streets.Include(s => s.City).ToList();

            return View(allStreets);
        }

        public IActionResult GetAllCountries()
        {
            var allCountries = _dbContext.Countries.ToList();

            return View(allCountries);
        }

        [HttpGet]
        public IActionResult GetAllSexes()
        {
            List<Sex> allSexes = _dbContext.Sexes.ToList();

            return View(allSexes);
        }
    }
}
