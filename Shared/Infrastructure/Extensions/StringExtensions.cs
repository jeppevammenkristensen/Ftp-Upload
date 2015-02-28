namespace Shared.Infrastructure.Extensions
{
    public static class StringExtensions
    {
        public static string Stringify(this string source)
        {
            return source.Replace("\\", "\\\\").Replace("\"","\\\"");
        }
    }
}