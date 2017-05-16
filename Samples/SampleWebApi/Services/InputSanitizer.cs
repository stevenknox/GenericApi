using GenericApi;

namespace SampleWebApi.Services
{
    public class InputSanitizer : IInputSanitizer
    {
        public string Sanitize(string input)
        {
            // you can use any library here to handle sanitization
            return input.Replace("<script></script>", "");

            //if using Gnass.XSS
            //var sanitizer = new HtmlSanitizer();
            //return sanitizer.Sanitize(input, null));
        }
    }
}
