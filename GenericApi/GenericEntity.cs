using System.ComponentModel.DataAnnotations.Schema;

namespace GenericApi
{
    public class GenericEntity: IHasGenericRepository
    {
        [NotMappedAttribute]
        public EntityState EntityState { get; set; }
    }
}