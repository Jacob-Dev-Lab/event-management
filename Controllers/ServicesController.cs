using Microsoft.AspNetCore.Mvc;
using RentalService.Data;
using RentalService.Models;

namespace RentalService.Controllers
{
    public class ServicesController : Controller
    {
        private readonly AppDbContext _context;

        public ServicesController(AppDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
           return View(GetServices());
        }

        public IActionResult Filter(string category)
        {
            var services = GetServices();

            if (!string.IsNullOrEmpty(category))
            {
                services = services.Where(s => s.Category == category).ToList();
            }

            return PartialView("_serviceList", services);
        }

        private List<Item> GetServices()
        {
            return _context.Items.ToList();
        }
       
    }
}
