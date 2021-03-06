﻿using Microsoft.AspNetCore.Mvc;
using System.Linq.Expressions;
using System;
using System.Linq;

namespace GenericApi
{

    [GenericControllerNameConvention]
    [Produces("application/json")]
    [Route("api/[controller]")]
    public class DTOController<T, TInputModel, TViewModel, TContext> : GenericController<T, TContext>
    {
        private IGenericRepository<T, TContext> _service;

        public DTOController(IGenericRepository<T, TContext> service) : base(service) {
            _service = service;
        }

        [HttpGet("{id}")]
        public override IActionResult Find(string id)
        {
            object _id = GetIdFromParameter(id);
            var includes = GetIncludesForViewModel();

            var obj = _service.FindById(_id, includes).AsViewModel<TViewModel, T>();

            return Ok(obj);
        }

        [HttpGet]
        public override IActionResult Get()
        {
            var includes = GetIncludesForViewModel();

            var data = _service.GetAll(includes).AsViewModel<TViewModel, T>();

            return Ok(data);
        }

        [HttpPost]
        [SanitizeModel]
        public override IActionResult Post([FromBody]object input)
        {
            
            var model = input.ToJObject()
                .ToObject<TInputModel>()
                .AsModel<TInputModel, T>();

            var result = _service.Add(model);

            return Ok(result);

        }

        [HttpPut("{id}")]
        [SanitizeModel]
        public override IActionResult Put(string id, [FromBody]object input)
        {
            object _id = GetIdFromParameter(id);

            var entity = _service.FindById(_id);

            var updatedModel = input.ToJObject()
               .ToObject<TInputModel>()
               .AsModel(entity);

            var result = _service.Update(updatedModel);

            return Ok(result);
        }

        
        private static Expression<Func<T, object>> GetIncludesForViewModel()
        {
            var references = Helpers.MappedProperties<TViewModel>().ToList();
            
            if (references.Count() > 0)
            {
                var includes = references.First().Entity != null ? references.First().Entity.MapIncludes<T>() : references.First().EntityName.MapIncludes<T>();
                return includes;
            }
            return null;
        }

    }
}
