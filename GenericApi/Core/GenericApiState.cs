using System.ComponentModel.DataAnnotations.Schema;
using Newtonsoft.Json;

namespace GenericApi
{
    public interface IGenericApi
    {
        GenericApiState GenericApiState { get; set; }
    }

    public enum GenericApiState
    {
        Unchanged,
        Added,
        Modified,
        Deleted,
        Detached
    }
}
