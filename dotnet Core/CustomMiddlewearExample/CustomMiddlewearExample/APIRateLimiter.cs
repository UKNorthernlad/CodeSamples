using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;

namespace CustomMiddlewearExample
{
    // You may need to install the Microsoft.AspNetCore.Http.Abstractions package into your project
    public class APIRateLimiter
    {
        private readonly RequestDelegate _next;

        public APIRateLimiter(RequestDelegate next)
        {
            _next = next;
        }

        public Task Invoke(HttpContext httpContext)
        {

            //httpContext.Response.Headers.Add("X-Xss-Protection", "1");
            //httpContext.Response.Headers.Add("X-Frame-Options", "SAMEORIGIN");
            //httpContext.Response.Headers.Add("X-Content-Type-Options", "nosniff");

            return _next(httpContext);
        }
    }

    // Extension method used to add the middleware to the HTTP request pipeline.
    public static class APIRateLimiterExtensions
    {
        public static IApplicationBuilder UseAPIRateLimiter(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<APIRateLimiter>();
        }
    }
}
