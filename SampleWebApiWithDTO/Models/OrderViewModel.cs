using GenericApi;

namespace SampleWebApiWithDTO.Models
{
    public class OrderViewModel
    {
        public int Id { get; set; }
        [MapToEntity(typeof(Product))]
        public string ProductName { get; set; }
        public int Quantity { get; set; }
    }
}
