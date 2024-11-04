using Book.DataAccess.Data;
using Book.DataAccess.Repository.Interface;
using Book.Models;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace Book.DataAccess.Repository.Implementation
{
    public class CompanyRepository : Repository<Company>, ICompanyRepository
    {
        private readonly ApplicationDbContext _db;

        public CompanyRepository(ApplicationDbContext db) : base(db) 
        {
            _db = db;
        }
        public void Update(Company obj)
        {
            _db.Companies.Update(obj);
        }
        private async Task<Company> GetAsync(Expression<Func<Company, bool>> filter)
        {
            IQueryable<Company> query = _db.Set<Company>();
            query = query.Where(filter);
            return await query.FirstOrDefaultAsync();
        }
        public async Task<Company> GetById(int id)
        {
            // Assuming your entity has an "Id" property
            return await GetAsync(entity => entity.Id == id);
        }
    }
}
