using System.Text;

namespace WebFront.Middlewares
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
            await _next(context);
            try
            {
            }
            catch (Exception ex)
            {
                await context.Response.BodyWriter.WriteAsync(Encoding.UTF8.GetBytes(ex.Message));
            }
        }
    }
}
