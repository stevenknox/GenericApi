using SampleWebApiWithDTO.Models;

namespace SampleWebApiWithDTO.Mapping
{
    public class OrderMappingProfile : AutoMapper.Profile
    {
        public OrderMappingProfile()
        {
            CreateMap<Order, OrderViewModel>();
            CreateMap<OrderInputModel, Order>();
        }
    }
}
