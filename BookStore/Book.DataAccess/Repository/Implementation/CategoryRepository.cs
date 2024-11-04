using Book.DataAccess.Data;
using Book.DataAccess.Repository.Interface;
using Book.Models;

namespace Book.DataAccess.Repository.Implementation
{
    public class CategoryRepository : Repository<Category>, ICategoryRepository
    {
        private readonly ApplicationDbContext _db;

        public CategoryRepository(ApplicationDbContext db) : base(db)
        {
            _db = db;
        }
       
        public void Update(Category obj)
        {
            _db.Categories.Update(obj);
        }
    }
}
