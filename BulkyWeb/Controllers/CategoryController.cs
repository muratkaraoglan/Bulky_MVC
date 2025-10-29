using Bulky.DataAccess.Data;
using Bulky.DataAccess.Repository.IRepository;
using Bulky.Models;
using Microsoft.AspNetCore.Mvc;

namespace BulkyWeb.Controllers;

public class CategoryController : Controller
{
    private readonly IUnitOfWork _unitOfWork;

    public CategoryController(IUnitOfWork db)
    {
        _unitOfWork = db;
    }

    public IActionResult Index()
    {
        var objCategories = _unitOfWork.Category.GetAll().ToList();
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
        _unitOfWork.Category.Add(obj);
        _unitOfWork.Save();
        TempData["success"] = "Category created successfully";
        return RedirectToAction("Index");
    }

    public IActionResult Edit(int? id)
    {
        if (id is null or 0) return NotFound();
        var categoryFromDb =  _unitOfWork.Category.Get(c => c.Id == id);
        if (categoryFromDb is null) return NotFound();

        return View(categoryFromDb);
    }

    [HttpPost]
    public IActionResult Edit(Category obj)
    {
        if (!ModelState.IsValid) return View();
        _unitOfWork.Category.Update(obj);
        _unitOfWork.Save();
        TempData["success"] = "Category edited successfully";
        return RedirectToAction("Index");
    }

    public IActionResult Delete(int? id)
    {
        if (id is null or 0) return NotFound();
        var categoryFromDb =  _unitOfWork.Category.Get(c => c.Id == id);
        if (categoryFromDb is null) return NotFound();

        return View(categoryFromDb);
    }

    [HttpPost, ActionName("Delete")]
    public IActionResult DeletePost(int? id)
    {
        var categoryFromDb =  _unitOfWork.Category.Get(c => c.Id == id);
        if (categoryFromDb is null) return NotFound();
        _unitOfWork.Category.Remove(categoryFromDb);
        _unitOfWork.Save();
        TempData["success"] = "Category deleted successfully";
        return RedirectToAction("Index");
    }
}