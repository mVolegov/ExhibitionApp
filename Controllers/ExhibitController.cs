using ExhibitionApp.Data;
using ExhibitionApp.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

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
            List<Exhibit> allExhibits = _dbContext.Exhibits.ToList();

            return View(allExhibits);
        }

        [HttpGet]
        public IActionResult CreateExhibit()
        {
            SelectList exhibitTypes = new SelectList(_dbContext.ExhibitTypes, "Id", "TypeName");
            ViewBag.ExhibitTypes = exhibitTypes;

            SelectList warehouses = new SelectList(_dbContext.Warehouses, "Id", "Address");
            ViewBag.Warehouses = warehouses;

            //ViewBag.ExhibitTypes = new SelectList(_dbContext.ExhibitTypes.ToList());
            //ViewBag.WarehousesAddresses = new SelectList(_dbContext.Warehouses.ToList());

            //ViewBag.ExhibitTypes = _dbContext.ExhibitTypes.ToList();

            //ViewData["ExhibitTypes"] = new SelectList(_dbContext.ExhibitTypes.ToList(), "Id", "TypeName", 9701100000);
            //ViewData["WarehousesAddresses"] = new SelectList(_dbContext.Warehouses.ToList(), "Id", "Address", 1);

            return View();
        }

        [HttpPost]
        public IActionResult CreateExhibit(Exhibit exhibit)
        {
            exhibit.Warehouse = _dbContext.Warehouses.FirstOrDefault(warehouse => warehouse.Id == exhibit.WarehouseId);
            exhibit.ExhibitType = _dbContext.ExhibitTypes.FirstOrDefault(exhibitType => exhibitType.Id == exhibit.ExhibitTypeId);

            _dbContext.Exhibits.AddRange(exhibit);
            _dbContext.SaveChanges();

            return RedirectToAction("GetAllExhibits");
        }
    }
}
