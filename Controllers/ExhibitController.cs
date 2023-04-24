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

        public IActionResult Details(long? id)
        {
            var exhibitToShow = _dbContext
                .Exhibits
                .Include(e => e.ExhibitType)
                .Include(e => e.Authors)
                .Include(e => e.Warehouse.Address.Street.City)
                .FirstOrDefault(e => e.Id == id);

            return View(exhibitToShow);
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
            SelectList authors = new SelectList(authorsFromDb, "Id", "Pseudonym");
            ViewBag.Authors = authors;

            return View();
        }

        [HttpPost]
        public IActionResult CreateExhibit(ExhibitViewModel exhibitModel)
        {
            var exhibit = exhibitModel.Exhibit;
            exhibit.Warehouse = _dbContext.Warehouses.FirstOrDefault(warehouse => warehouse.Id == exhibit.WarehouseId);
            exhibit.ExhibitType = _dbContext.ExhibitTypes.FirstOrDefault(exhibitType => exhibitType.Id == exhibit.ExhibitTypeId);
            exhibit.Authors = _dbContext.Authors.Include(a => a.Exhibits).Where(author => exhibitModel.SelectedAuthorsId.Contains(author.Id)).ToList();

            _dbContext.Exhibits.AddRange(exhibit);
            _dbContext.SaveChanges();

            return RedirectToAction("GetAllExhibits");
        }

        public IActionResult Edit(long? id)
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

            var exhibitToUpdate = _dbContext.Exhibits.Include(e => e.Authors).FirstOrDefault(e => e.Id == id);
            ViewBag.ExhibitId = id;

            ViewBag.CreationDate = exhibitToUpdate.CreationDate.ToString();
            ViewBag.ArrivalDate = exhibitToUpdate.ArrivalDate.ToString();

            var model = new ExhibitViewModel()
            {
                Exhibit = exhibitToUpdate,
                SelectedAuthorsId = exhibitToUpdate.Authors.Select(e => e.Id).ToList(),
            };

            return View(model);
        }

        [HttpPost]
        public IActionResult Edit(ExhibitViewModel exhibitModel)
        {
            var exhibitToUpdate = exhibitModel.Exhibit;

            _dbContext.Attach(exhibitToUpdate);

            exhibitToUpdate.Warehouse = _dbContext.Warehouses.FirstOrDefault(warehouse => warehouse.Id == exhibitToUpdate.WarehouseId);
            exhibitToUpdate.ExhibitType = _dbContext.ExhibitTypes.FirstOrDefault(exhibitType => exhibitType.Id == exhibitToUpdate.ExhibitTypeId);
            exhibitToUpdate.Authors = _dbContext
                .Authors
                .Include(a => a.Exhibits)
                .Where(author => exhibitModel.SelectedAuthorsId.Contains(author.Id))
                .ToList();

            _dbContext.Entry(exhibitToUpdate).State = EntityState.Modified;
            _dbContext.SaveChanges();

            ClearAuthors(exhibitModel.SelectedAuthorsId, exhibitToUpdate);
            return RedirectToAction("GetAllExhibits");
        }

        private void ClearAuthors(List<long> selectedId, Exhibit entry)
        {
            var authors = _dbContext.Authors.Include(a => a.Exhibits);

            foreach (var author in authors)
            {
                if (!selectedId.Contains(author.Id) && author.Exhibits.Any(ex => ex.Id == entry.Id))
                {
                    author.Exhibits.Remove(entry);
                }
                else if (selectedId.Contains(author.Id) && !author.Exhibits.Any(ex => ex.Id == entry.Id))
                {
                    author.Exhibits.Add(entry);
                }
            }

            _dbContext.SaveChanges();
        }

        public IActionResult Delete(long? id)
        {
            var exhibitToDelete = _dbContext.Exhibits.Find(id);
            _dbContext.Exhibits.Remove(exhibitToDelete);

            _dbContext.SaveChanges();

            return RedirectToAction("GetAllExhibits");
        }
    }
}
