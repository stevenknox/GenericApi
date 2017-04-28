using GenericApi;

namespace SampleWebApiWithDTO.Models
{
    public class OrderViewModel
    {
        public int Id { get; set; }
        [MapToEntity("Product")]
        public string ProductName { get; set; }
        [MapToEntity("Product.ProductType")]
        public string ProductTypeName { get; set; }
        public int Quantity { get; set; }
    }
}
