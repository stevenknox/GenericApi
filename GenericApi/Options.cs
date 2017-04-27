using System;

namespace GenericApi
{
    public class Options
    {
        public string EntityAssemblyName { get; set; }
        public string DbContextAssemblyName { get; set; }
        public Type db { get; set; }

        public string GetDbAssembly()
        {
            if (!String.IsNullOrEmpty(DbContextAssemblyName))
                return DbContextAssemblyName;

            return EntityAssemblyName;
        }
    }

}