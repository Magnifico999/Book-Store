using Book.Models;

namespace Book.DataAccess.Repository.Interface
{
    public interface IProductRepository : IRepository<Product>
    {
        void Update(Product obj);
        Task<Product> GetById(int id);
    }
}
