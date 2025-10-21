using BulkyWeb.Data;
using BulkyWeb.Models;
using Microsoft.AspNetCore.Mvc;

namespace BulkyWeb.Controllers;

public class CategoryController(ApplicationDbContext db) : Controller
{
    public IActionResult Index()
    {
        var objCategories = db.Categories.ToList();
        return View(objCategories);
    }

    public IActionResult Create()
    {
        return View();
    }

    [HttpPost]
    public IActionResult Create(Category obj)
    {
        if (ModelState.IsValid)
        {
            db.Categories.Add(obj);
            db.SaveChanges();
            return RedirectToAction("Index");
        }
        return View();
    }
}