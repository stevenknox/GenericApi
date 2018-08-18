using System;
using GenericApi;
using Newtonsoft.Json;

namespace SampleWebApi.Models
{
    public class Order: IGenericApi
    {
        [GenericApiKey]
        public Guid OrderId { get; set; }
        public int ProductId { get; set; }
        public int Quantity { get; set; }
        public int Total { get; set; }
        [JsonIgnore]
        public GenericApiState GenericApiState { get ; set; }
    }
}
