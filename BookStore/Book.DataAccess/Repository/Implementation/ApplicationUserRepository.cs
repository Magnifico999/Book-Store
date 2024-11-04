using Book.DataAccess.Data;
using Book.DataAccess.Repository.Interface;
using Book.Models;

namespace Book.DataAccess.Repository.Implementation
{
    public class ApplicationUserRepository : Repository<ApplicationUser>, IApplicationUserRepository
    {
        private readonly ApplicationDbContext _db;

        public ApplicationUserRepository(ApplicationDbContext db) : base(db)
        {
            _db = db;
        }
       
    }
}
