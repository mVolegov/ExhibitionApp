using ExhibitionApp.Data;
using ExhibitionApp.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace ExhibitionApp.Controllers
{
    public class ExhibitController : Controller
    {
        private readonly ExhibitionAppDbContext _dbContext;

        public ExhibitController(ExhibitionAppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        [HttpGet]
        public IActionResult GetAllExhibits()
        {
            List<Exhibit> allExhibits = _dbContext
                .Exhibits
                .Include(e => e.Authors)
                .ToList();

            return View(allExhibits);
        }

        [HttpGet]
        public IActionResult CreateExhibit()
        {
            SelectList exhibitTypes = new SelectList(_dbContext.ExhibitTypes, "Id", "TypeName");
            ViewBag.ExhibitTypes = exhibitTypes;

            var warehousesFromDb = _dbContext
                .Warehouses
                .Include(w => w.Address.Street.City);
            SelectList warehouses = new SelectList(warehousesFromDb, "Id", "Address");
            ViewBag.Warehouses = warehouses;

            var authorsFromDb = _dbContext.Authors;
            MultiSelectList authors = new MultiSelectList(authorsFromDb, "Id", "Pseudonym");
            ViewBag.Authors = authors;

            return View();
        }

        [HttpPost]
        public IActionResult CreateExhibit(Exhibit exhibit)
        {
            exhibit.Warehouse = _dbContext.Warehouses.FirstOrDefault(warehouse => warehouse.Id == exhibit.WarehouseId);
            exhibit.ExhibitType = _dbContext.ExhibitTypes.FirstOrDefault(exhibitType => exhibitType.Id == exhibit.ExhibitTypeId);
            exhibit.Authors = _dbContext.Authors.Where(author => exhibit.AuthorsId.Contains(author.Id)).ToList();

            _dbContext.Exhibits.AddRange(exhibit);
            _dbContext.SaveChanges();

            return RedirectToAction("GetAllExhibits");
        }
    }
}
