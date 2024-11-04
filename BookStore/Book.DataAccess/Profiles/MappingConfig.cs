using AutoMapper;
using Book.Models;

namespace Book.DataAccess.Profiles
{
    public class MappingConfig : Profile
    {
        public MappingConfig()
        {
            CreateMap<Company, GetCompany>();
            CreateMap<GetCompany, Company>();
            CreateMap<GetProduct, Product>();
            CreateMap<Product, GetProduct>();
            CreateMap<OrderHeader, GetOrderHeader>();
            CreateMap<GetOrderHeader, OrderHeader>();
        }
    }
}
