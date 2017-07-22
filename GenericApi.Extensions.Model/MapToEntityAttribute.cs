using System;

namespace GenericApi
{

    [AttributeUsage(AttributeTargets.Property)]
    public class MapToEntityAttribute : Attribute
    {
        private Type entity;
        private string entityName;

        public MapToEntityAttribute(Type entity)
        {
            this.entity = entity;
        }
        public MapToEntityAttribute(string EntityName)
        {
            this.entityName = EntityName;
        }
        public virtual Type Entity
        {
            get { return entity; }
        }
        public virtual string EntityName
        {
            get { return entityName; }
        }

    }
}
