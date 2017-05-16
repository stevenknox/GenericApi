using System.ComponentModel.DataAnnotations.Schema;

namespace GenericApi
{
    public class GenericEntity: IHasGenericRepository, IEntityWithState
    {
        [NotMappedAttribute]
        public EntityState EntityState { get; set; }
    }
}