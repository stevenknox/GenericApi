using System;

namespace GenericApi
{
    public class GenericControllerOptions
    {
        public string EntityAssemblyName { get; set; }
        public string DbContextAssemblyName { get; set; }
        public Type db { get; set; }
        public bool UseViewModels { get; set; }
        public bool UseInputModels { get; set; }

        public string GetDbAssembly()
        {
            if (!String.IsNullOrEmpty(DbContextAssemblyName))
                return DbContextAssemblyName;

            return EntityAssemblyName;
        }
    }
}