﻿using ExhibitionApp.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ExhibitionApp.Controllers
{
    public class PosterController : Controller
    {
        private readonly ExhibitionAppDbContext _dbContext;

        public PosterController(ExhibitionAppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public IActionResult UpcomingExhibitions()
        {
            var upcomingExhibitions = _dbContext
                .Exhibitions
                .Include(e => e.Address.Street.City)
                //.Where(e => e.HostingDate.CompareTo(DateTime.Now.ToUniversalTime()) > 0)
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

        public IActionResult Stub()
        {
            return View();
        }
    }
}
