using System;

namespace GenericApi
{

    [AttributeUsage(AttributeTargets.Property)]
    public class MapToEntityAttribute : Attribute
    {
        private Type entity;

        public MapToEntityAttribute(Type entity)
        {
            this.entity = entity;
        }
        public virtual Type Name
        {
            get { return entity; }
        }
       
    }
}
