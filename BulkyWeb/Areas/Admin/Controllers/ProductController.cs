using System.Text;
using Bulky.DataAccess.Repository.IRepository;
using Bulky.Models;
using Bulky.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace BulkyWeb.Areas.Admin.Controllers;

[Area("Admin")]
public class ProductController : Controller
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IWebHostEnvironment _webHostEnvironment;

    public ProductController(IUnitOfWork unitOfWork, IWebHostEnvironment webHostEnvironment)
    {
        _unitOfWork = unitOfWork;
        _webHostEnvironment = webHostEnvironment;
    }

    public IActionResult Index()
    {
        var products = _unitOfWork.Product
            .GetAll(includeProperties: "Category")
            .ToList();
        return View(products);
    }

    public IActionResult Upsert(int? id) //Update Insert
    {
        IEnumerable<SelectListItem> categories = _unitOfWork.Category
            .GetAll().Select(c => new SelectListItem
            {
                Text = c.Name,
                Value = c.Id.ToString()
            });
        ProductVM productVm = new()
        {
            Product = new Product(),
            CategoryList = categories
        };
        if (id is null or 0) // Insert
        {
            return View(productVm);
        }

        //Update
        productVm.Product = _unitOfWork.Product.Get(p => p.Id == id);
        return View(productVm);
    }

    [HttpPost]
    public IActionResult Upsert(ProductVM productVm, IFormFile? file)
    {
        if (!ModelState.IsValid) return View();
        if (file != null)
        {
            var wwwRootPath = _webHostEnvironment.WebRootPath;
            var fileName = Guid.NewGuid() + file.FileName;
            var productPath = Path.Combine(wwwRootPath, @"images\product");

            if (!string.IsNullOrEmpty(productVm.Product.ImageUrl))
            {
                //delete the old image
                var oldImagePath = Path.Combine(wwwRootPath, productVm.Product.ImageUrl.TrimStart('\\'));
                if (System.IO.File.Exists(oldImagePath)) System.IO.File.Delete(oldImagePath);
            }

            using (var fileStream = new FileStream(Path.Combine(productPath, fileName), FileMode.Create))
            {
                file.CopyTo(fileStream);
            }

            productVm.Product.ImageUrl = @"\images\product\" + fileName;
        }

        if (productVm.Product.Id == 0)
        {
            _unitOfWork.Product.Add(productVm.Product);
        }
        else
        {
            _unitOfWork.Product.Update(productVm.Product);
        }

        _unitOfWork.Save();
        TempData["Success"] = "Product created successfully";
        return RedirectToAction("Index");
    }

    #region API CALLS

    [HttpGet]
    public IActionResult GetAll()
    {
        var products = _unitOfWork.Product
            .GetAll(includeProperties: "Category")
            .ToList();
        return Json(new { data = products });
    }

    [HttpDelete]
    public IActionResult Delete(int? id)
    {
        var product = _unitOfWork.Product.Get(p => p.Id == id);
        if (product == null)
        {
            return Json(new { success = false, message = "Error while deleting" });
        }
        var oldImagePath = Path.Combine(_webHostEnvironment.WebRootPath, product.ImageUrl.TrimStart('\\'));
        if (System.IO.File.Exists(oldImagePath)) System.IO.File.Delete(oldImagePath);
        
        _unitOfWork.Product.Remove(product);
        _unitOfWork.Save();
        return Json(new { success = true, message = "Delete Successful" });
    }

    #endregion
}