using Microsoft.AspNetCore.Http;
using System.Text;

namespace PriceFlowSecurity
{
    public static class HttpContextExtensions
    {
        public static int GetUserId(this HttpContext context)
        {
            if (context == null || context.Session == null)
                throw new Exception("Session is not available.");

            if (context.Session.TryGetValue("UserId", out var bytes))
                throw new Exception("User not authenticated.");
            
            var value = Encoding.UTF8.GetString(bytes);

            return int.Parse(value);
        }
    }
}
