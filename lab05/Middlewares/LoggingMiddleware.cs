using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace lab05.Middlewares
{
    public class LoggingMiddleware
    {
        private readonly RequestDelegate _next;
        private static readonly string Path = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile) + @"\requestsLog.txt";

        public LoggingMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            context.Request.EnableBuffering();
            if (context.Request != null)
            {
                string path = context.Request.Path;
                string method = context.Request.Method;
                string queryString = context.Request.QueryString.ToString();
                string bodyStr = "";

                using (var reader = new StreamReader(context.Request.Body, Encoding.UTF8, true, 1024, true))
                {
                    bodyStr = await reader.ReadToEndAsync();
                    context.Request.Body.Position = 0;
                }

                string message = String.Join(" ", new[] {DateTime.Now.ToString(), method, path, queryString, bodyStr, "\n"});
                await System.IO.File.AppendAllTextAsync(Path, message);

            }
            await _next(context);
        }
    }

}
