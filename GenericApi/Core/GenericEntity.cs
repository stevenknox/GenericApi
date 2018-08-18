using System.ComponentModel.DataAnnotations.Schema;
using Newtonsoft.Json;

namespace GenericApi
{
    public class GenericEntity : IGenericApi
    {
        [JsonIgnore]
        public GenericApiState GenericApiState { get; set; }
    }
}