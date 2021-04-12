using Microsoft.AspNetCore.Builder;

namespace Blueprint.CustomExceptionHandlerMiddleware.Project
{
    public static class CustomExceptionHandlerMiddlewareExtension
    {
        public static IApplicationBuilder UseCustomExceptionHandlerMiddleware(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<CustomExceptionHandlerMiddleware>();
        }
    }
}
