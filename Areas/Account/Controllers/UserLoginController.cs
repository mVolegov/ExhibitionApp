using ExhibitionApp.Data;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Microsoft.AspNetCore.Identity;

namespace ExhibitionApp.Areas.Account.Controllers
{
    public class UserLoginController : Controller
    {
        private readonly ExhibitionAppDbContext _dbContext;

        public UserLoginController(ExhibitionAppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Index(string login, string password)
        {
            var passHash = Utils.PasswordHash.GetHash(password.ToCharArray());
            var user = _dbContext.Users.FirstOrDefault(u => u.Name == login && u.PasswordHash == passHash);

            if (user == null)
            {
                ModelState.AddModelError("", "Такого пользователя нет");
                return View();
            }

            List<Claim> claims = new List<Claim>();
            claims.Add(new Claim(ClaimTypes.Name, user.Name));
            claims.Add(new Claim("role", user.IsStorekeeper ? "Storekeeper" : "Manager"));

            ClaimsIdentity identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            ClaimsPrincipal principal = new ClaimsPrincipal(identity);

            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);

            if (user.IsStorekeeper)
            {
                return RedirectToRoute("default", new { controller = "Exhibit", action = "GetAllExhibits" });
            }
            else
            {
                return RedirectToRoute("default", new { controller = "Exhibition", action = "GetAllExhibitions" });
            }
        }

        public async Task<IActionResult> UserSignOut()
        {
            await HttpContext.SignOutAsync();

            return RedirectToAction("Index");
        }
    }
}
