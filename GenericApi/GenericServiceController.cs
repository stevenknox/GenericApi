using Microsoft.AspNetCore.Mvc;

namespace GenericApi
{
    [GenericControllerNameConvention]
    public class GenericServiceController<T, Tid, TContext> : ServiceController<T, Tid, TContext>
    {
        public GenericServiceController(IGenericService<T, Tid, TContext> service) : base(service) { }
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
        public IActionResult Find(Tid id)
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
        public IActionResult Put(Tid id, [FromBody]T input)
        {
           // var entity = _service.FindById(id);

            var result = _service.Update(input);

            return Ok(result);
        }

        [HttpDelete("{id}")]
        public IActionResult Delete(Tid id)
        {
            _service.Delete(id);

            return Ok();
        }

    }
}
