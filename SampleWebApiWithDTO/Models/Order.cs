using GenericApi;

namespace SampleWebApiWithDTO.Models
{
    public class Order: GenericEntity
    {
        public int Id { get; set; }
        public int ProductId { get; set; }
        public int Quantity { get; set; }
        public int Total { get; set; }
    }
}
