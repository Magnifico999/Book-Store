using Book.Models;

namespace Book.DataAccess.Repository.Interface
{
    public interface IShoppingCartRepository : IRepository<ShoppingCart>
    {
        void Update(ShoppingCart obj);
    }
    
}
