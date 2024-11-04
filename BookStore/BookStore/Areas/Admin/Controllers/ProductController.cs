using Microsoft.AspNetCore.Mvc;
using Book.DataAccess.Repository.Interface;
using Microsoft.EntityFrameworkCore;
using Book.Models;
using Microsoft.AspNetCore.Mvc.Rendering;
using Book.Models.ViewModels;
using Book.Utility;
using Microsoft.AspNetCore.Authorization;
using Book.DataAccess.DTO;
using AutoMapper;
using System.Globalization;

namespace BookStore.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = SD.Role_Admin)]
    public class ProductController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public ProductController(IUnitOfWork unitOfWork, IWebHostEnvironment webHostEnvironment, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _webHostEnvironment = webHostEnvironment;
            _mapper = mapper;
        }

        public IActionResult Index(int page = 1)
        {
            const int PageSize = 5;
            var products = _unitOfWork.Product.GetAll(includeProperties: "Category").ToList();
            if (products != null && products.Any())
            {
                var productModels = _mapper.Map<List<GetProduct>>(products);
                var paginatedProducts = productModels
                    .Skip((page - 1) * PageSize)
                    .Take(PageSize)
                    .ToList();
                var totalProductCount = productModels.Count();
                var model = new PaginatedList<GetProduct>(paginatedProducts, totalProductCount, page, PageSize);
                return View(model);
            }
            else
            {
                var emptyList = new PaginatedList<GetProduct>(new List<GetProduct>(), 0, 1, PageSize);
                return View(emptyList);
            }
        }

        public IActionResult Search(string searchString)
        {
            if (string.IsNullOrEmpty(searchString))
            {
                return RedirectToAction(nameof(Index));
            }

            var accounts = _unitOfWork.Product.GetAll(includeProperties: "Category").AsQueryable();


            if (accounts == null)
            {
                return BadRequest("It can't be empty");
            }

            searchString = searchString.ToLower();

            var searchResults = accounts
                .Include(account => account.Category)
                .Where(account =>
        (account.Title != null && account.Title.ToLower().Contains(searchString)) ||
        (account.ISBN != null && account.ISBN.ToLower().Contains(searchString)) ||
        (account.Price.ToString(CultureInfo.InvariantCulture).ToLower().Contains(searchString)) ||
        (account.Author != null && account.Author.ToLower().Contains(searchString)) ||
        (account.Category != null && account.Category.Name.ToLower().Contains(searchString)))
    .Select(account => new GetProduct
    {
        Id = account.Id,
        Title = account.Title,
        ISBN = account.ISBN,
        Price = account.Price,
        Author = account.Author,
        Category = account.Category,

    })
                .ToList();

            if (searchResults.Count() > 0)
            {
                var totalItems = accounts.Count();
                TempData["success"] = "Search successful";
                return View("Index", new PaginatedList<GetProduct>(searchResults, totalItems, 1, 1));
            }

            return View("Index", new PaginatedList<GetProduct>(new List<GetProduct>(), 0, 1, 1));
        }


        public IActionResult Upsert(int? id)
        {

            ProductVM productVM = new()
            {
                CategoryList = _unitOfWork.Category.GetAll().Select(u => new SelectListItem
                {
                    Text = u.Name,
                    Value = u.Id.ToString()
                }),
                Product = new Product()
            };
            if (id == null || id == 0)
            {
                //create
                return View(productVM);
            }
            else
            {
                //update
                productVM.Product = _unitOfWork.Product.Get(u => u.Id == id);
                return View(productVM);
            }

        }

        [HttpPost]
        public IActionResult Upsert(ProductVM productVM, IFormFile file)
        {
            if (ModelState.IsValid)
            {
                string wwwRootPath = _webHostEnvironment.WebRootPath;
                if (file != null)
                {
                    string fileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
                    string productPath = Path.Combine(wwwRootPath, @"images/product");

                    if (!string.IsNullOrEmpty(productVM.Product.ImageUrl))
                    {
                        var oldImagePath = Path.Combine(wwwRootPath, productVM.Product.ImageUrl.TrimStart('\\'));

                        if (System.IO.File.Exists(oldImagePath))
                        {
                            System.IO.File.Delete(oldImagePath);
                        }
                    }

                    using (var fileStream = new FileStream(Path.Combine(productPath, fileName), FileMode.Create))
                    {
                        file.CopyTo(fileStream);
                    }
                    productVM.Product.ImageUrl = @"\images\product\" + fileName;
                }


                if (productVM.Product.Id == 0)
                {
                    _unitOfWork.Product.Add(productVM.Product);
                    TempData["success"] = "Product created successfully";
                }

                else
                {
                    // Update operation
                    _unitOfWork.Product.Update(productVM.Product);
                    TempData["success"] = "Product updated successfully";
                }
                _unitOfWork.Save();
                return RedirectToAction("Index");
            }
            else
            {
                productVM.CategoryList = _unitOfWork.Category.GetAll().Select(u => new SelectListItem
                {
                    Text = u.Name,
                    Value = u.Id.ToString()
                });
                return View(productVM);
            }

        }

        [HttpGet]
        public async Task<IActionResult> UpdateProduct(int id)
        {
            var account = await _unitOfWork.Product.GetById(id);

            if (account == null)
            {
                return NotFound();
            }

            var categoryList = _unitOfWork.Category.GetAll()
                .Select(u => new SelectListItem
                {
                    Text = u.Name,
                    Value = u.Id.ToString()
                });

            UpdateProductViewModel updateModel = new UpdateProductViewModel
            {
                Product = new GetProduct
                {
                    Id = account.Id,
                    Title = account.Title,
                    ISBN = account.ISBN,
                    Description = account.Description,
                    Price = account.Price,
                    Price100 = account.Price100,
                    Price50 = account.Price50,
                    ListPrice = account.ListPrice,
                    Author = account.Author,
                    Category = account.Category,
                    ImageUrl = account.ImageUrl,
                },
                CategoryList = categoryList
            };

            return View(updateModel);
        }

        [HttpPost]
        public async Task<IActionResult> UpdateProduct(UpdateProductViewModel model, IFormFile imageFile)
        {
            if (!ModelState.IsValid)
            {
                model.CategoryList = _unitOfWork.Category.GetAll()
                    .Select(u => new SelectListItem
                    {
                        Text = u.Name,
                        Value = u.Id.ToString()
                    });
                return View(model);
            }

            var existingProduct = await _unitOfWork.Product.GetById(model.Product.Id);

            if (existingProduct == null)
            {
                return NotFound();
            }

            // Update other properties
            existingProduct.Title = model.Product.Title;
            existingProduct.ISBN = model.Product.ISBN;
            existingProduct.Description = model.Product.Description;
            existingProduct.Price = model.Product.Price;
            existingProduct.Price50 = model.Product.Price50;
            existingProduct.Price100 = model.Product.Price100;
            existingProduct.ListPrice = model.Product.ListPrice;
            existingProduct.Author = model.Product.Author;
            existingProduct.CategoryId = model.Product.CategoryId;
            try
            {
                // Update image if a new one is provided
                if (imageFile != null)
                {
                    string wwwRootPath = _webHostEnvironment.WebRootPath;
                    string fileName = Guid.NewGuid().ToString() + Path.GetExtension(imageFile.FileName);
                    string productPath = Path.Combine(wwwRootPath, @"images/product");

                    // Delete the old image if it exists
                    if (!string.IsNullOrEmpty(existingProduct.ImageUrl))
                    {
                        var oldImagePath = Path.Combine(wwwRootPath, existingProduct.ImageUrl.TrimStart('\\'));

                        if (System.IO.File.Exists(oldImagePath))
                        {
                            System.IO.File.Delete(oldImagePath);
                        }
                    }

                    // Save the new image
                    using (var fileStream = new FileStream(Path.Combine(productPath, fileName), FileMode.Create))
                    {
                        imageFile.CopyTo(fileStream);
                    }

                    existingProduct.ImageUrl = @"\images\product\" + fileName;
                }


                _unitOfWork.Product.Update(existingProduct);
                _unitOfWork.Save();

                TempData["success"] = "Product updated successfully";

                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                return StatusCode(500, "An error occurred while updating the product: " + ex.Message);
            }
        }

        public async Task<IActionResult> DeleteProduct(int Id)
        {

            var account = await _unitOfWork.Product.GetById(Id);

            if (account == null)
            {
                return NotFound();
            }
            GetProduct deleteModel = new()
            {
                Id = account.Id,
                Title = account.Title,
                ISBN = account.ISBN,
                Price = account.Price,
                Description = account.Description,
                Price100 = account.Price100,
                Price50 = account.Price50,
                ListPrice = account.ListPrice,
                Author = account.Author,
                Category = account.Category,
                ImageUrl = account.ImageUrl,

            };

            return View(deleteModel);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteProduct(GetProduct model)
        {

            var accountToDelete = await _unitOfWork.Product.GetById(model.Id);

            if (accountToDelete == null)
            {
                return NotFound();
            }

            try
            {
                _unitOfWork.Product.Remove(accountToDelete);
                _unitOfWork.Save();
                TempData["success"] = "Product deleted successfully";

                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                TempData["error"] = $"Error deleting product: {ex.Message}";
                return View(model);
            }
        }

    }
}
