using ExhibitionApp.Data;
using ExhibitionApp.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace ExhibitionApp.Controllers
{
    [Authorize(Roles = "Manager")]
    public class ExhibitionController : Controller
    {
        private readonly ExhibitionAppDbContext _dbContext;

        public ExhibitionController(ExhibitionAppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public IActionResult GetAllExhibitions()
        {
            var allExhibitions = _dbContext
                .Exhibitions
                .Include(e => e.Address.Street.City)
                .ToList();

            return View(allExhibitions);
        }

        public IActionResult GetAllUpcomingExhibitions()
        {
            var upcomingExhibitions = _dbContext
                .Exhibitions
                .Include(e => e.Address.Street.City)
                .Where(e => e.HostingDate.CompareTo(DateTime.Now.ToUniversalTime()) > 0)
                .ToList();

            return View(upcomingExhibitions);
        }

        public IActionResult Details(long? id)
        {
            var exhibitionToShow = _dbContext
                .Exhibitions
                .Include(e => e.Address.Street.City)
                .Include(e => e.Exhibits)
                .FirstOrDefault(e => e.Id == id);

            return View(exhibitionToShow);
        }

        [HttpGet]
        public IActionResult Create()
        {
            var exhibitsFromDb = _dbContext.Exhibits;
            var exhibits = new SelectList(exhibitsFromDb, "Id", "Name");
            ViewBag.Exhibits = exhibits;

            var addressesFromDb = _dbContext.Addresses.Include(a => a.Street.City);
            var addresses = new SelectList(addressesFromDb, "Id", "");
            ViewBag.Addresses = addresses;

            return View();
        }

        [HttpPost]
        public IActionResult Create(ExhibitionViewModel exhibitionViewModel)
        {
            var exhibition = exhibitionViewModel.Exhibition;
            exhibition.Address = _dbContext.Addresses.FirstOrDefault(a => a.Id == exhibition.AddressId);
            exhibition.Exhibits = _dbContext
                .Exhibits
                .Include(e => e.Exhibitions)
                .Where(exhibit => exhibitionViewModel.SelectedExhibitsId.Contains(exhibit.Id))
                .ToList();
            exhibition.HostingDate = exhibition.HostingDate.ToUniversalTime();
            exhibition.ExpirationDate = exhibition.ExpirationDate.ToUniversalTime();

            _dbContext.AddRange(exhibition);
            _dbContext.SaveChanges();

            return RedirectToAction("GetAllExhibitions");
        }

        [HttpGet]
        public IActionResult Edit(long? id)
        {
            var addressesFromDb = _dbContext.Addresses.Include(a => a.Street.City);
            var addresses = new SelectList(addressesFromDb, "Id", "");
            ViewBag.Addresses = addresses;

            var exhibitsFromDb = _dbContext.Exhibits;
            var exhibits = new SelectList(exhibitsFromDb, "Id", "Name");
            ViewBag.Exhibits = exhibits;

            ViewBag.ExhibitionId = id;

            var exhibitionToUpdate = _dbContext.Exhibitions.Include(e => e.Exhibits).FirstOrDefault(e => e.Id == id);

            var model = new ExhibitionViewModel()
            {
                Exhibition = exhibitionToUpdate,
                SelectedExhibitsId = exhibitionToUpdate.Exhibits.Select(e => e.Id).ToList()
            };

            return View(model);
        }

        [HttpPost]
        public IActionResult Edit(ExhibitionViewModel exhibitionViewModel)
        {
            var exhibitionToUpdate = exhibitionViewModel.Exhibition;

            _dbContext.Attach(exhibitionToUpdate);

            exhibitionToUpdate.Address = _dbContext.Addresses.FirstOrDefault(a => a.Id == exhibitionToUpdate.AddressId);
            exhibitionToUpdate.Exhibits = _dbContext
                .Exhibits
                .Include(e => e.Exhibitions)
                .Where(exhibit => exhibitionViewModel.SelectedExhibitsId.Contains(exhibit.Id))
                .ToList();

            _dbContext.Entry(exhibitionToUpdate).State = EntityState.Modified;
            _dbContext.SaveChanges();

            ClearExhibits(exhibitionViewModel.SelectedExhibitsId, exhibitionToUpdate);

            return RedirectToAction("GetAllExhibitions");
        }

        public IActionResult Delete(long? id)
        {
            var exhibitionToDelete = _dbContext.Exhibitions.Find(id);
            _dbContext.Exhibitions.Remove(exhibitionToDelete);

            _dbContext.SaveChanges();

            return RedirectToAction("GetAllExhibitions");
        }

        private void ClearExhibits(List<long> selectedId, Exhibition entry)
        {
            var exhibits = _dbContext.Exhibits.Include(e => e.Exhibitions);

            foreach (var exhibit in exhibits)
            {
                if (!selectedId.Contains(exhibit.Id) && exhibit.Exhibitions.Any(e => e.Id == entry.Id))
                {
                    exhibit.Exhibitions.Remove(entry);
                }
                else if (selectedId.Contains(exhibit.Id) && !exhibit.Exhibitions.Any(e => e.Id == entry.Id))
                {
                    exhibit.Exhibitions.Add(entry);
                }
            }

            _dbContext.SaveChanges();
        }
    }
}
