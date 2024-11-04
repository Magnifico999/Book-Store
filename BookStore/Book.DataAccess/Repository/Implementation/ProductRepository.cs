using Book.DataAccess.Data;
using Book.DataAccess.Repository.Interface;
using Book.Models;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace Book.DataAccess.Repository.Implementation
{
    public class ProductRepository : Repository<Product>, IProductRepository
    {
        private readonly ApplicationDbContext _db;

        public ProductRepository(ApplicationDbContext db) : base(db)
        {
            _db = db;
        }

        public void Update(Product obj)
        {
            var objFromDb = _db.Products.FirstOrDefault(u => u.Id == obj.Id);
            if (objFromDb != null)
            {
                objFromDb.Title = obj.Title;
                objFromDb.ISBN = obj.ISBN;
                objFromDb.Price = obj.Price;
                objFromDb.Price50 = obj.Price50;
                objFromDb.ListPrice = obj.ListPrice;
                objFromDb.Price100 = obj.Price100;
                objFromDb.Description = obj.Description;
                objFromDb.CategoryId = obj.CategoryId;
                objFromDb.Author = obj.Author;
                if (obj.ImageUrl != null)
                {
                    objFromDb.ImageUrl = obj.ImageUrl;
                }
            }
        }
        private async Task<Product> GetAsync(Expression<Func<Product, bool>> filter)
        {
            IQueryable<Product> query = _db.Set<Product>();
            query = query.Where(filter);
            return await query.FirstOrDefaultAsync();
        }
        public async Task<Product> GetById(int id)
        {
            // Assuming your entity has an "Id" property
            return await GetAsync(entity => entity.Id == id);
        }
    }
}
