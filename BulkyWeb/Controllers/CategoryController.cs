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
            ModelState.AddModelError("name", "The Display Order cannot exactly match the Category Name");
        }

        if (!ModelState.IsValid) return View();
        db.Categories.Add(obj);
        db.SaveChanges();
        TempData["success"] = "Category created successfully";
        return RedirectToAction("Index");
    }

    public IActionResult Edit(int? id)
    {
        if (id is null or 0) return NotFound();
        var categoryFromDb = db.Categories.Find(id);
        if (categoryFromDb is null) return NotFound();
        
        return View(categoryFromDb);
    }
    
    [HttpPost]
    public IActionResult Edit(Category obj)
    {
        if (!ModelState.IsValid) return View();
        db.Categories.Update(obj);
        db.SaveChanges();
        TempData["success"] = "Category edited successfully";
        return RedirectToAction("Index");
    }
    
    public IActionResult Delete(int? id)
    {
        if (id is null or 0) return NotFound();
        var categoryFromDb = db.Categories.Find(id);
        if (categoryFromDb is null) return NotFound();
        
        return View(categoryFromDb);
    }
    
    [HttpPost,ActionName("Delete")]
    public IActionResult DeletePost(int? id)
    {
        var categoryFromDb = db.Categories.Find(id);
        if (categoryFromDb is null ) return NotFound();
        db.Categories.Remove(categoryFromDb);
        db.SaveChanges();
        TempData["success"] = "Category deleted successfully";
        return RedirectToAction("Index");
    }
}