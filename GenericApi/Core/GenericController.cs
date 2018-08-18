using System.Reflection;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Newtonsoft.Json.Linq;
using System;
using System.Linq;

namespace GenericApi
{

    [GenericControllerNameConvention]
    public class GenericController<T, TContext> : ServiceController<T, TContext>
    {
        public GenericController(IGenericRepository<T, TContext> service) : base(service) { }
    }

    [Route("api/[controller]s")] //very crude example, have an option to set plural and have dep of humanizer if they want to do this
    public class GenericControllerPluralized<T, TContext> : GenericController<T, TContext>
    {
        public GenericControllerPluralized(IGenericRepository<T, TContext> service) : base(service) { }
    }

    [Produces("application/json")]
    [Route("api/[controller]")]
    [Authorize(Policy = "SecureGenericApi")]
    public class ServiceController<T, TContext> : Controller 
    {
        private IGenericRepository<T, TContext> _service;

        public ServiceController(IGenericRepository<T, TContext> service)
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
            Type targetType =  typeof(T).GetGenericApiPrimaryKey().PropertyType;

            var _id = id.ConvertTo(targetType);
            return _id;
        }

    }
}
