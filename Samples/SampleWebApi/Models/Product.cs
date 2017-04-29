using GenericApi;

namespace SampleWebApi.Models
{
    public class Product: GenericEntity
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int Cost { get; set; }
    }
}
