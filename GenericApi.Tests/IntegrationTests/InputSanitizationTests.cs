using GenericApi.Tests.Framework;
using Newtonsoft.Json;
using SampleWebApiWithDTO;
using SampleWebApiWithDTO.Models;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Xunit;

namespace GenericApi.Tests
{
    public class InputSanitizationTests : IClassFixture<TestFixture<Startup>>
    {
        private readonly HttpClient _client;

        public InputSanitizationTests(TestFixture<Startup> fixture)
        {
            _client = fixture.Client;
        }

        
        [Fact]
        public async Task GenericController_Post_Should_SanitizeInput()
        {
            string unsanitized = "<script></script>Oranges";
            string sanitized = "Oranges";

            Product input = new Product { Name = unsanitized, ProductTypeId = 1 };
         
            var response = await _client.PostAsJsonAsync("/api/product", input);

            string content = await response.Content.ReadAsStringAsync();

            Product sut = JsonConvert.DeserializeObject<Product>(content);

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.Equal(sanitized, sut.Name);

        }

        
    }
}
