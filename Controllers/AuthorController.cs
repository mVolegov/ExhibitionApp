using ExhibitionApp.Data;
using ExhibitionApp.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace ExhibitionApp.Controllers
{
    [Authorize(Roles = "Storekeeper")]
    public class AuthorController : Controller
    {
        private readonly ExhibitionAppDbContext _dbContext;

        public AuthorController(ExhibitionAppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        [HttpGet]
        public IActionResult GetAllAuthors()
        {
            List<Author> allAuthors = _dbContext
                .Authors
                    .Include(a => a.Country)
                    .Include(a => a.Sex)
                .ToList();

            return View(allAuthors);
        }

        [HttpGet]
        public IActionResult Create()
        {
            var gendersFromDb = _dbContext.Sexes;
            SelectList genders = new SelectList(gendersFromDb, "Id", "Name");
            ViewBag.Genders = genders;

            var countriesFromDb = _dbContext.Countries;
            SelectList countries = new SelectList(countriesFromDb, "Id", "ShortName");
            ViewBag.Countries = countries;

            return View();
        }

        [HttpPost]
        public IActionResult Create(Author author)
        {
            author.Sex = _dbContext.Sexes.FirstOrDefault(s => s.Id == author.SexId);
            author.Country = _dbContext.Countries.FirstOrDefault(c => c.Id == author.CountryId);

            _dbContext.Authors.Add(author);
            _dbContext.SaveChanges();

            return RedirectToAction("GetAllAuthors");
        }

        [HttpGet]
        public IActionResult Edit(long? id)
        {
            var gendersFromDb = _dbContext.Sexes;
            SelectList genders = new SelectList(gendersFromDb, "Id", "Name");
            ViewBag.Genders = genders;

            var countriesFromDb = _dbContext.Countries;
            SelectList countries = new SelectList(countriesFromDb, "Id", "ShortName");
            ViewBag.Countries = countries;

            ViewBag.AuthorId = id;

            Author authorToUpdate = _dbContext.Authors.FirstOrDefault(a => a.Id == id);
            ViewBag.BirthDay = authorToUpdate.Birthday.ToString();

            return View(authorToUpdate);
        }

        [HttpPost]
        public IActionResult Edit(Author authorToUpdate)
        {
            _dbContext.Attach(authorToUpdate);
            _dbContext.Entry(authorToUpdate).State = EntityState.Modified;
            _dbContext.SaveChanges();

            return RedirectToAction("GetAllAuthors");
        }

        public IActionResult Delete(long? id)
        {
            var authorToDelete = _dbContext.Authors.Find(id);
            _dbContext.Authors.Remove(authorToDelete);

            _dbContext.SaveChanges();

            return RedirectToAction("GetAllAuthors");
        }

        
    }
}
