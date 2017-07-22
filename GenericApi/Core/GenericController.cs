using System.Reflection;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Newtonsoft.Json.Linq;

namespace GenericApi
{

    [GenericControllerNameConvention]
    public class GenericController<T, Tid, TContext> : ServiceController<T, Tid, TContext>
    {
        public GenericController(IGenericRepository<T, Tid, TContext> service) : base(service) { }
    }

    [Produces("application/json")]
    [Route("api/[controller]")]
    [Authorize(Policy = "SecureGenericApi")]
    public class ServiceController<T, Tid, TContext> : Controller 
    {
        private IGenericRepository<T, Tid, TContext> _service;

        public ServiceController(IGenericRepository<T, Tid, TContext> service)
        {
            _service = service;
        }

        [HttpGet("{id}")]
        public virtual IActionResult Find(string id)
        {
            object _id = GetIdFromParameter(id);

            return Ok(_service.FindById(_id));
        }

        [HttpGet]
        public virtual IActionResult Get()
        {
            return Ok(_service.GetAll());
        }

        [HttpPost]
        [SanitizeModel]
        public virtual IActionResult Post([FromBody]object input)
        {
            if (input.GetType() == typeof(JObject))
                input = input.ToJObject().ToObject<T>();

            var result = _service.Add((T)input);

            return Ok(result);

        }

        [HttpPut("{id}")]
        [SanitizeModel]
        public virtual IActionResult Put(string id, [FromBody]object input)
        {
            object _id = GetIdFromParameter(id);

            if (input.GetType() == typeof(JObject))
                input = input.ToJObject().ToObject<T>();

            var result = _service.Update((T)input);

            return Ok(result);
        }

        [HttpDelete("{id}")]
        public virtual IActionResult Delete(string id)
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
