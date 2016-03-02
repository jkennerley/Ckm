using Ckm.Models;
using Ckm.Services;
using Ckm.ViewModels;
using Microsoft.AspNet.Authorization;
using Microsoft.AspNet.Mvc;
using System.Linq;

namespace Ckm.Controllers.Web
{
    public class AppController : Controller
    {
        private readonly IMailService mailService;

        private readonly ICkmRepo repo;

        public AppController(IMailService mailService, ICkmRepo repo)
        {
            this.mailService = mailService;

            //this.context = context;
            this.repo = repo;
        }

        public IActionResult Index()
        {
            return View(null);
        }

        [Authorize]
        public IActionResult Trips()
        {
            //var trips =
            //    repo
            //    .GetAllTrips()
            //    .OrderBy(x => x.Name)
            //    .ToList();
            return View();
        }


        public IActionResult About()
        {
            return View();
        }

        public IActionResult Help()
        {
            return View();
        }

        public IActionResult Contact()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Contact(ContactViewModel m)
        {
            if (ModelState.IsValid)
            {
                var email = Startup.Configuration["AppSettings:SiteEmailAddress"];

                if (string.IsNullOrWhiteSpace(email))
                {
                    ModelState.AddModelError("", "could not send email, config problem");
                }

                if (this.mailService.SendMail(email, m.Email, "", m.Message))
                {
                    ModelState.Clear();
                }

                ViewBag.Message = "mail sent, thx";
            }

            return View();
        }

        public IActionResult ErrorJester()
        {
            throw new System.Exception("App/ErrorJester");
            return View(null);
        }
    }
}
