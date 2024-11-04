using Book.Models;

namespace Book.DataAccess.Repository.Interface
{
    public interface ICompanyRepository : IRepository<Company>
    {
        void Update(Company obj);
        Task<Company> GetById(int id);
    }
}
