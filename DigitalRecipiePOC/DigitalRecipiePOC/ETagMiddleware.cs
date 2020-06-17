using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace DigitalRecipiePOC
{
    public class ETagMiddleware
    {
        private readonly RequestDelegate _next;

        public ETagMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var response = context.Response;
            var originalStream = response.Body;
            try
            {
                using (var ms = new MemoryStream())
                {
                    response.Body = ms;

                    await _next(context);

                    if (response.StatusCode == 304) { return; }

                    ms.Position = 0;
                    await ms.CopyToAsync(originalStream);
                }
            }
            catch (Exception)
            {
                context.Response.Body = originalStream;
                throw;
            }
        }
    }

    public static class ApplicationBuilderExtensions
    {
        public static void UseETagMiddleware(this IApplicationBuilder app)
        {
            app.UseMiddleware<ETagMiddleware>();
        }
    }
}
