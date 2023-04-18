using System.Text;

namespace Diploma.Models.Middlewares
{
    public class ExceptionMiddleware
    {
        private readonly RequestDelegate _next;

        public ExceptionMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                context.Response.Body.Write(Encoding.UTF8.GetBytes("ArgumentException:\n" + ex.Message));
            }
        }
    }
}
