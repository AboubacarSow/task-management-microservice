using Microsoft.AspNetCore.Http;

namespace dispatcher_service.Tests.Helpers;

public class ContextHelper
{
    
    public static HttpContext BuildHttpContext(string? correlationId = null)
    {
        var context = new DefaultHttpContext();
        context.Response.Body = new MemoryStream();

        if (correlationId is not null)
            context.Request.Headers["X-Correlation-ID"] = correlationId;

        return context;
    }
    

   
}
