using System.ComponentModel.DataAnnotations.Schema;

namespace GenericApi
{
    public class GenericEntity: IHasGenericService
    {
        [NotMapped]
        public EntityState EntityState { get; set; }
    }
}