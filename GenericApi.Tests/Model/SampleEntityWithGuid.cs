using System;

namespace GenericApi.Tests.Model
{
    public class SampleEntityWithGuid: GenericEntity
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
    }
}
