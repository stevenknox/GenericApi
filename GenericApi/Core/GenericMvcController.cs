using System.Reflection;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;

namespace GenericApi
{

    [GenericControllerNameConvention]
    [Route("[controller]")]
    [Authorize(Policy = "SecureGenericApi")]
    public class MvcController<T, TContext> : Controller 
    {
        private IGenericRepository<T, TContext> _service;

        public MvcController(IGenericRepository<T, TContext> service)
        {
            _service = service;
        }
        [HttpGet("{id}")]

        public IActionResult Details(string id)
        {
            object _id = GetIdFromParameter(id);

            return View(_service.FindById(_id));
        }

        public IActionResult Index()
        {
            var obj = new List<object>();
            _service.GetAll().ForEach(f=> obj.Add(f));
            return View(obj);
        }

        [HttpPost]
        [SanitizeModel]
        public IActionResult Create([FromBody]object input)
        {
            if (input.GetType() == typeof(JObject))
                input = input.ToJObject().ToObject<T>();

            var result = _service.Add((T)input);

            return View(result);

        }

        [HttpPut("{id}")]
        [SanitizeModel]
        public IActionResult Update(string id, [FromBody]object input)
        {
            object _id = GetIdFromParameter(id);

            if (input.GetType() == typeof(JObject))
                input = input.ToJObject().ToObject<T>();

            var result = _service.Update((T)input);

            return View(result);
        }

        [HttpDelete("{id}")]
        public IActionResult Delete(string id)
        {
            object _id = GetIdFromParameter(id);

            _service.Delete(_id);

            return View();
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
