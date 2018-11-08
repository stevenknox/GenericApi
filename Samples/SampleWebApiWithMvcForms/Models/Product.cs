using GenericApi;

namespace SampleWebApiWithMvcForms.Models
{
    public class Product: GenericEntity
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int Cost { get; set; }

        public int ProductTypeId { get; set; }
        public ProductType ProductType { get; set; }
    }
}
