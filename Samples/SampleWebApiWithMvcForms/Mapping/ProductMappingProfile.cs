using SampleWebApiWithMvcForms.Models;
using AutoMapper;

namespace SampleWebApiWithMvcForms.Mapping
{
    public class ProductMappingProfile: Profile
    {
        public ProductMappingProfile()
        {
            CreateMap<Product, ProductViewModel>();
        }
    }
}
