using System;

namespace GenericApi
{
     [AttributeUsage(AttributeTargets.Property)]
    public class GenericApiKeyAttribute : Attribute
    {
        public GenericApiKeyAttribute()
        {

        }
    }
}