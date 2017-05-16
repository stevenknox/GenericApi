namespace GenericApi.Tests.Services
{
    public class MockInputSanitizer : IInputSanitizer
    {
        public string Sanitize(string input)
        {
            return input.Replace("<script></script>", "");
        }
    }
}
