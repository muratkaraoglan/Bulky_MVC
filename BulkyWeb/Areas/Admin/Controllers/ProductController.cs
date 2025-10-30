using Bulky.DataAccess.Repository.IRepository;
using Bulky.Models;
using Microsoft.AspNetCore.Mvc;

namespace BulkyWeb.Areas.Admin.Controllers;

[Area("Admin")]
public class ProductController : Controller
{
    private readonly IUnitOfWork _unitOfWork;

    public ProductController(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    // GET
    public IActionResult Index()
    {
        var products = _unitOfWork.Product.GetAll().ToList();
        return View(products);
    }

    public IActionResult Create()
    {
        return View();
    }

    [HttpPost]
    public IActionResult Create(Product product)
    {
        if(!ModelState.IsValid) return View();
        _unitOfWork.Product.Add(product);
        _unitOfWork.Save();
        TempData["Success"] = "Product created successfully";
        return RedirectToAction("Index");
    }

    public IActionResult Edit(int? id)
    {
        if(id is null or 0) return NotFound();
        var productFromDb = _unitOfWork.Product.Get(p => p.Id == id);
        if(productFromDb is null) return NotFound();
        
        return View(productFromDb);
    }

    [HttpPost]
    public IActionResult Edit(Product product)
    {
        if(!ModelState.IsValid) return View();
        _unitOfWork.Product.Update(product);
        _unitOfWork.Save();
        TempData["Success"] = "Product updated successfully";
        return RedirectToAction("Index");
    }

    public IActionResult Delete(int? id)
    {
        if(id is null or 0) return NotFound();
        var productFromDb = _unitOfWork.Product.Get(p => p.Id == id);
        if(productFromDb is null) return NotFound();
        
        return View(productFromDb);
    }

    [HttpPost, ActionName("Delete")]
    public IActionResult DeletePost(int? id)
    {
        var productFromDb = _unitOfWork.Product.Get(p => p.Id == id);
        if(productFromDb is null) return NotFound();
        _unitOfWork.Product.Remove(productFromDb);
        _unitOfWork.Save();
        TempData["Success"] = "Product deleted successfully";
        return RedirectToAction("Index");
    }
}