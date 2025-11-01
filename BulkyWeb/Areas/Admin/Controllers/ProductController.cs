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

    // GET
    public IActionResult Index()
    {
        var products = _unitOfWork.Product
            .GetAll()
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
        if (id == null || id == 0) // Insert
        {
            return View(productVm);
        }
        else //Update
        {
            productVm.Product = _unitOfWork.Product.Get(p => p.Id == id);
            return View(productVm);
        }
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

    public IActionResult Delete(int? id)
    {
        if (id is null or 0) return NotFound();
        var productFromDb = _unitOfWork.Product.Get(p => p.Id == id);
        if (productFromDb is null) return NotFound();

        return View(productFromDb);
    }

    [HttpPost, ActionName("Delete")]
    public IActionResult DeletePost(int? id)
    {
        var productFromDb = _unitOfWork.Product.Get(p => p.Id == id);
        if (productFromDb is null) return NotFound();
        _unitOfWork.Product.Remove(productFromDb);
        _unitOfWork.Save();
        TempData["Success"] = "Product deleted successfully";
        return RedirectToAction("Index");
    }
}