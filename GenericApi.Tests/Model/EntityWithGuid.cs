using System;

namespace GenericApi.Tests.Model
{
    public class EntityWithGuid: GenericEntity
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
    }
}
