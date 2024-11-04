using AutoMapper;
using Book.DataAccess.DTO;
using Book.DataAccess.Repository.Interface;
using Book.Models;
using Book.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BulkyBookWeb.Controllers
{
	[Area("Admin")]
    [Authorize(Roles = SD.Role_Admin)]
    public class CompanyController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public CompanyController(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public IActionResult Index(int page = 1)
        {
            const int PageSize = 5;
            var companies = _unitOfWork.Company.GetAll();
            if (companies != null && companies.Any())
            {
                var companyModels = _mapper.Map<List<GetCompany>>(companies);
                var paginatedCompanies = companyModels
                    .Skip((page - 1) * PageSize)
                    .Take(PageSize)
                    .ToList();
                var totalCompanyCount = companyModels.Count();
                var model = new PaginatedList<GetCompany>(paginatedCompanies, totalCompanyCount, page, PageSize);
                return View(model);
            }
            else
            {
                var emptyList = new PaginatedList<GetCompany>(new List<GetCompany>(), 0, 1, PageSize);
                return View(emptyList);
            }
        }

        public IActionResult Search(string searchString)
        {
            if (string.IsNullOrEmpty(searchString))
            {
                return RedirectToAction(nameof(Index));
            }

            var accounts = _unitOfWork.Company.GetAll();

            if (accounts == null)
            {
                return BadRequest("It can't be empty");
            }

            searchString = searchString.ToLower();

            var searchResults = accounts
                .Where(account =>
                    account.Name.ToLower().Contains(searchString) ||
                    account.StreetAddress.ToLower().Contains(searchString) ||
                    account.City.ToLower().Contains(searchString) ||
                    account.State.ToString().ToLower() == searchString ||
                    account.PhoneNumber.Contains(searchString) ||
                    account.PostalCode.Contains(searchString))
                    
                .Select(account => new GetCompany
                {
                    Id = account.Id,
                    Name = account.Name,
                    StreetAddress = account.StreetAddress,
                    State = account.State,
                    PhoneNumber = account.PhoneNumber,
                    City = account.City,
                    PostalCode = account.PostalCode,
                })
                .ToList();

            if (searchResults.Count > 0)
            {
                var totalItems = accounts.Count();
                TempData["success"] = "Search successful";
                return View("Index", new PaginatedList<GetCompany>(searchResults, totalItems, 1, 1));
            }

            return View("AccountIndex", new PaginatedList<GetCompany>(new List<GetCompany>(), 0, 1, 1));
        }

        //GET
        public IActionResult Upsert(int? id)
        {
            Company company = new();

            if (id == null || id == 0)
            {
                return View(company);
            }
            else
            {
                company = _unitOfWork.Company.Get(u => u.Id == id);
                return View(company);
            }
        }

        //POST
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Upsert(Company obj)
        {

            if (ModelState.IsValid)
            {

                if (obj.Id == 0)
                {
                    _unitOfWork.Company.Add(obj);
                    TempData["success"] = "Company created successfully";
                }
                else
                {
                    _unitOfWork.Company.Update(obj);
                    TempData["success"] = "Company updated successfully";
                }
                _unitOfWork.Save();

                return RedirectToAction("Index");
            }
            return View(obj);
        }

        public async Task<IActionResult> UpdateCompany(int id)
        {
            var account = await _unitOfWork.Company.GetById(id);

            if (account == null)
            {
                return NotFound();
            }
            GetCompany updateModel = new()
            {
                Id = account.Id,
                Name = account.Name,
                StreetAddress = account.StreetAddress,
                PhoneNumber = account.PhoneNumber,
                City = account.City,
                State = account.State,
                PostalCode = account.PostalCode,
            };

            return View(updateModel);
        }

        [HttpPost]
        public async Task<IActionResult> UpdateCompany(GetCompany model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var existingCompany = await _unitOfWork.Company.GetById(model.Id);

            if (existingCompany == null)
            {
                return NotFound();
            }
            existingCompany.Name = model.Name;
            existingCompany.StreetAddress = model.StreetAddress;
            existingCompany.PhoneNumber = model.PhoneNumber;
            existingCompany.City = model.City;
            existingCompany.State = model.State;
            existingCompany.PostalCode = model.PostalCode;

            try
            {
                _unitOfWork.Company.Update(existingCompany);
                _unitOfWork.Save();

                TempData["success"] = "Company updated successfully";

                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                return StatusCode(500, "An error occurred while updating the company: " + ex.Message);
            }
        }

        public async Task<IActionResult> DeleteCompany(int Id)
        {

            var account = await _unitOfWork.Company.GetById(Id);

            if (account == null)
            {
                return NotFound();
            }
            GetCompany deleteModel = new()
            {
                Id = account.Id,
                Name = account.Name,
                StreetAddress = account.StreetAddress,
                State = account.State,
                City = account.City,
                PostalCode = account.PostalCode,
                PhoneNumber = account.PhoneNumber,
                
            };

            return View(deleteModel);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteCompany(GetCompany model)
        {
            var accountToDelete = await _unitOfWork.Company.GetById(model.Id);

            if (accountToDelete == null)
            {
                return NotFound();
            }

            try
            {
                var account = _mapper.Map<Company>(model);
                _unitOfWork.Company.Remove(accountToDelete);
                _unitOfWork.Save();
                TempData["success"] = "Company deleted successfully";

                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                TempData["error"] = $"Error deleting company: {ex.Message}";
                return View(model);
            }
        }
    }
}
