using System.Collections.Generic;

namespace GenericApi.Tests.Model
{
    public class Blog: GenericEntity
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public List<Post> Posts { get; set; }

        public Blog()
        {
            Posts = new List<Post>();
        }
    }
}
