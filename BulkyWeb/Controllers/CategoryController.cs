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
        if (obj.Name == obj.DisplayOrder.ToString())
        {
            ModelState.AddModelError("name","The Display Order cannot exactly match the Category Name");
        }
        if (ModelState.IsValid)
        {
            db.Categories.Add(obj);
            db.SaveChanges();
            return RedirectToAction("Index");
        }
        return View();
    }
}