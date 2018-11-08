using SampleWebApiWithMvcForms.Models;
using AutoMapper;

namespace SampleWebApiWithMvcForms.Mapping
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
