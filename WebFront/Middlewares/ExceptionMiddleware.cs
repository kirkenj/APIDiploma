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
                var str = $"<!DOCTYPE html><meta charset=\"utf-8\">\r\n<p>Произошла ошибка</p>\r\n<p>{ex.Message}</p>\r\n<p> <input type=\"button\" onclick=\"history.back();\" value=\"Назад\" /></p>\r\n<a class=\"navbar-brand\" href=\"/Home\">На главную</a>\r\n";
                await context.Response.BodyWriter.WriteAsync(Encoding.UTF8.GetBytes(str));
            }
        }
    }
}
