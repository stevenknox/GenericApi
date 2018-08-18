using GenericApi;
using Newtonsoft.Json;

namespace SampleWebApi.Models
{
    public class Product: IGenericApi
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int Cost { get; set; }
        [JsonIgnore]
        public GenericApiState GenericApiState { get; set; }
    }
}
