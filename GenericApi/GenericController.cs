using System.Reflection;
using Microsoft.AspNetCore.Mvc;

namespace GenericApi
{

    [GenericControllerNameConvention]
    public class GenericController<T, Tid, TContext> : ServiceController<T, Tid, TContext>
    {
        public GenericController(IGenericService<T, Tid, TContext> service) : base(service) { }
    }

    [Produces("application/json")]
    [Route("api/[controller]")]
    public class ServiceController<T, Tid, TContext> : Controller 
    {
        private IGenericService<T, Tid, TContext> _service;

        public ServiceController(IGenericService<T, Tid, TContext> service)
        {
            _service = service;
        }
        [HttpGet("{id}")]
        public IActionResult Find(string id)
        {
            object _id = GetIdFromParameter(id);

            return Ok(_service.FindById(_id));
        }

        [HttpGet]
        public IActionResult Get()
        {
            return Ok(_service.GetAll());
        }

        [HttpPost]
        public IActionResult Post([FromBody]object input)
        {
            var result = _service.Add((T)input);

            return Ok(result);

        }

        [HttpPut("{id}")]
        public IActionResult Put(Tid id, [FromBody]object input)
        {
           // var entity = _service.FindById(id);

            var result = _service.Update((T)input);

            return Ok(result);
        }

        [HttpDelete("{id}")]
        public IActionResult Delete(string id)
        {
            object _id = GetIdFromParameter(id);

            _service.Delete(_id);

            return Ok();
        }


        public static object GetIdFromParameter(string id)
        {
            //MVC doesnt like having Tid as the param type so we must type as string and cast
            var targetType = typeof(T).GetProperty("Id").PropertyType;

            var _id = id.ConvertTo(targetType);
            return _id;
        }

    }
}
