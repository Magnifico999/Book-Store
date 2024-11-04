using Book.Models;

namespace Book.DataAccess.Repository.Interface
{
    public interface ICategoryRepository : IRepository<Category>
    {
        void Update(Category obj);
    }
}
