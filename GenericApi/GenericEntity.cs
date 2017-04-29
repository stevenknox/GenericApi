using System.ComponentModel.DataAnnotations.Schema;

namespace GenericApi
{
    public class GenericEntity: IHasGenericService
    {
        [NotMappedAttribute]
        public EntityState EntityState { get; set; }
    }
}