using ExhibitionApp.Data;
using ExhibitionApp.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace ExhibitionApp.Controllers
{
    [Authorize(Roles = "Storekeeper")]
    public class WarehouseController : Controller
    {
        private readonly ExhibitionAppDbContext _dbContext;

        public WarehouseController(ExhibitionAppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        [HttpGet]
        public IActionResult GetAllWarehouses()
        {
            List<Warehouse> allWarehouses = _dbContext
                .Warehouses
                    .Include(w => w.Address.Street)
                    .Include(w => w.Address.Street.City)
                .ToList();

            return View(allWarehouses);
        }

        [HttpGet]
        public IActionResult Create()
        {
            var addressesFromDb = _dbContext.Addresses.Include(a => a.Street.City);

            SelectList addresses = new SelectList(addressesFromDb, "Id", "");
            ViewBag.Addresses = addresses;

            return View();
        }

        [HttpPost]
        public IActionResult Create(Warehouse warehouse)
        {
            warehouse.Address = _dbContext.Addresses.FirstOrDefault(a => a.Id == warehouse.AddressId);

            _dbContext.Warehouses.Add(warehouse);
            _dbContext.SaveChanges();

            return RedirectToAction("GetAllWarehouses");
        }

        public IActionResult Delete(long? id)
        {
            var warehouseToDelete = _dbContext.Warehouses.Find(id);
            _dbContext.Warehouses.Remove(warehouseToDelete);
            _dbContext.SaveChanges();

            return RedirectToAction("GetAllWarehouses");
        }

        [HttpGet]
        public IActionResult Edit(long? id)
        {
            var addressesFromDb = _dbContext.Addresses.Include(a => a.Street.City);
            SelectList addresses = new SelectList(addressesFromDb, "Id", "");
            ViewBag.Addresses = addresses;

            ViewBag.WarehouseId = id;

            Warehouse warehouseToUpdate = _dbContext.Warehouses.FirstOrDefault(w => w.Id == id);

            return View(warehouseToUpdate);
        }

        [HttpPost]
        public IActionResult Edit(Warehouse warehouseToUpdate)
        {
            _dbContext.Attach(warehouseToUpdate);
            _dbContext.Entry(warehouseToUpdate).State = EntityState.Modified;
            _dbContext.SaveChanges();

            return RedirectToAction("GetAllWarehouses");
        }
    }
}
