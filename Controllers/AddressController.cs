using ExhibitionApp.Data;
using ExhibitionApp.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace ExhibitionApp.Controllers
{
    [Authorize(Roles = "Storekeeper")]
    public class AddressController : Controller
    {
        private readonly ExhibitionAppDbContext _dbContext;

        public AddressController(ExhibitionAppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        [HttpGet]
        public IActionResult GetAllAddresses()
        {
            List<Address> allAddresses = _dbContext
                .Addresses
                    .Include(a => a.Street)
                    .Include(a => a.Street.City)
                .ToList();

            return View(allAddresses);
        }

        [HttpGet]
        public IActionResult Create()
        {
            SelectList cities = new SelectList(_dbContext.Cities, "Id", "Name");
            ViewBag.Cities = cities;

            SelectList streets = new SelectList(_dbContext.Streets, "Id", "Name");
            ViewBag.Streets = streets;

            return View();
        }

        [HttpPost]
        public IActionResult Create(Address address)
        {
            address.Street = _dbContext.Streets.FirstOrDefault(street => street.Id == address.StreetId);

            _dbContext.Addresses.Add(address);
            _dbContext.SaveChanges();

            return RedirectToAction("GetAllAddresses");
        }

        [HttpGet]
        public JsonResult GetStreetsByCity(long cityId)
        {
            List<SelectListItem> streets = GetStreets(cityId);

            return Json(streets);
        }

        public IActionResult Delete(long? id)
        {
            var addressToDelete = _dbContext.Addresses.Find(id);
            _dbContext.Addresses.Remove(addressToDelete);

            _dbContext.SaveChanges();

            return RedirectToAction("GetAllAddresses");
        }

        private List<SelectListItem> GetStreets(long cityId)
        {
            var lstStreets = new List<SelectListItem>();

            List<Street> streets = _dbContext.Streets.Where(st => st.CityId == cityId).ToList();

            lstStreets = streets.Select(st => new SelectListItem()
            {
                Value = st.Id.ToString(),
                Text = st.Name
            }).ToList();

            //var defItem = new SelectListItem()
            //{
            //    Value = "",
            //    Text = "Выберите улицу"
            //};

            //lstStreets.Insert(0, defItem);

            return lstStreets;
        }
    }
}
