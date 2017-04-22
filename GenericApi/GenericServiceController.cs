using Microsoft.AspNetCore.Mvc;

namespace GenericApi
{
    [GenericControllerNameConvention]
    public class GenericServiceController<T> : ServiceController<T>
    {
        public GenericServiceController(IGenericService<T> service) : base(service) { }
    }

    [Produces("application/json")]
    [Route("api/[controller]")]
    public class ServiceController<T> : Controller
    {
        private IGenericService<T> _service;

        public ServiceController(IGenericService<T> service)
        {
            _service = service;
        }
        [HttpGet("{id}")]
        public IActionResult Find(int id)
        {
            return Ok(_service.FindById(id));
        }

        [HttpGet]
        public IActionResult Get()
        {
            return Ok(_service.GetAll());
        }

        [HttpPost]
        public IActionResult Post([FromBody]T input)
        {
            var result = _service.Add(input); ;

            return Ok(result);

        }

        [HttpPut("{id}")]
        public IActionResult Put(int id, [FromBody]T input)
        {
            var entity = _service.FindById(id);

            var result = _service.Update(entity);

            return Ok(result);
        }

        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            _service.Delete(id);

            return Ok();
        }

    }
}
