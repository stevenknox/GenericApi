using GenericApi;

namespace SampleWebApiWithDTO.Models
{
    public class ProductViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int Cost { get; set; }

        [MapToEntity(typeof(ProductType))]
        public string ProductTypeName { get; set; }
    }
}
