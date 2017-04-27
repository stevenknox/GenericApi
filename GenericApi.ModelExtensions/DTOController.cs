using System.Reflection;
using Microsoft.AspNetCore.Mvc;

namespace GenericApi
{

    [GenericControllerNameConvention]
    public class GenericDTOController<T, TInputModel, TViewModel, Tid, TContext> : DTOController<T, TInputModel, TViewModel, Tid, TContext>
    {
        public GenericDTOController(IGenericService<T, Tid, TContext> service) : base(service) { }
    }

    [Produces("application/json")]
    [Route("api/[controller]")]
    public class DTOController<T, TInputModel, TViewModel, Tid, TContext> : GenericController<T, Tid, TContext>
    {
        private IGenericService<T, Tid, TContext> _service;

        public DTOController(IGenericService<T, Tid, TContext> service) : base(service) { }

        [HttpGet("{id}")]
        public new IActionResult Find(string id)
        {
            object _id = GetIdFromParameter(id);

            var obj = _service.FindById(_id).AsViewModel<TViewModel, T>();

            return Ok();
        }

        [HttpGet]
        public new IActionResult Get()
        {
            var data = _service.GetAll().AsViewModel<TViewModel, T>();

            return Ok(data);
        }

        [HttpPost]
        public new IActionResult Post([FromBody]object input)
        {
            var result = _service.Add((T)input);

            return Ok(result);

        }

        [HttpPut("{id}")]
        public new IActionResult Put(Tid id, [FromBody]object input)
        {
           // var entity = _service.FindById(id);

            var result = _service.Update((T)input);

            return Ok(result);
        }

    }
}
