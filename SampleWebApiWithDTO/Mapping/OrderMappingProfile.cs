using SampleWebApiWithDTO.Models;
using AutoMapper;

namespace SampleWebApiWithDTO.Mapping
{
    public class OrderMappingProfile : Profile
    {
        public OrderMappingProfile()
        {
            CreateMap<Order, OrderViewModel>();
            CreateMap<OrderInputModel, Order>();
        }
    }
}
