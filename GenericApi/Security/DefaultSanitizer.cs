namespace GenericApi
{
    public class DefaultSanitizer : IInputSanitizer
    {
        public string Sanitize(string input)
        {
            return input;
        }
    }
}
