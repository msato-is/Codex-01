using Microsoft.AspNetCore.Mvc;
using System.Reflection;

namespace IntraPortal.Controllers;

public class HomeController : Controller
{
    public IActionResult Index()
    {
        ViewBag.Version = Assembly.GetExecutingAssembly().GetName().Version?.ToString() ?? "1.0.0";
        return View();
    }
}

