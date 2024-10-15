using System.Text.Json;

namespace IdentityTapsiDoc.Identity.EndPoints.V1.Extensions
{
    public static class HttpContextExtension
    {

        public static async Task<string> GetPhoneNumberFromBodyAsync(this HttpContext httpContext)
        {
            httpContext.Request.EnableBuffering();

            using var reader = new StreamReader(httpContext.Request.Body, leaveOpen: true);
            var body = await reader.ReadToEndAsync();

            httpContext.Request.Body.Position = 0;

            using var jsonDoc = JsonDocument.Parse(body);
            if (jsonDoc.RootElement.TryGetProperty("phoneNumber", out JsonElement phoneNumberElement))
            {
                return phoneNumberElement.GetString();
            }

            return null;
        }
    }
}
