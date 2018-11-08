using GenericApi;

namespace SampleWebApiWithMvcForms.Models
{
    public class Order : GenericEntity
    {
        public int Id { get; set; }
        public int Quantity { get; set; }
        public int Total { get; set; }

        public int ProductId { get; set; }
        public Product Product { get; set; }
    }
}
