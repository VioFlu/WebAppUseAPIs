using DutchTreat.Data;
using DutchTreat.Services;
using DutchTreat.ViewModels;
using Microsoft.AspNetCore.Mvc;
using System.Linq;

namespace DutchTreat.Controllers
{
    public class AppController : Controller
    {
        private readonly IMailService _mailService;
        private readonly IDutchRepository _repository;


        public AppController(IMailService mailService, IDutchRepository repository)
        {
            _mailService = mailService;
            _repository = repository;

        }
        public IActionResult Index()
        {
            //throw new InvalidOperationException("Bad things happened");
            // var results = _context.Products.ToList();
            return View();
        }
        [HttpGet("Contact")]
        public IActionResult Contact()
        {
            ViewBag.Title = "Contact Us";
            return View();
        }
        [HttpPost("contact")]
        public IActionResult Contact(ContactViewModel model)
        {
            if (ModelState.IsValid)
            {
                //send email
                _mailService.SendMessage("admin@mail.com", model.Subject, $"From: {model.Name} - {model.Email}, Message {model.Message}");
                ViewBag.UserMessage = "Mail sent";
                ModelState.Clear();
            }
            else
            {
                //shw error
            }
            return View();
        }

        public IActionResult About()
        {
            ViewBag.Title = "About Us";
            return View();
        }

        public IActionResult Shop()
        {

            //var results = _context.Products
            //    .OrderBy(p => p.Category)
            //    .ToList();
            //we can do the above With  a Linq query as follows:

            var results = _repository.GetAllProducts();

            return View(results.ToList());
        }

    }
}