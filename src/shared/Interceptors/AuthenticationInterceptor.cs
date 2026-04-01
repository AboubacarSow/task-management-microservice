using Grpc.Core;
using Grpc.Core.Interceptors;
using Microsoft.AspNetCore.Http;


namespace shared.Interceptors;

public class AuthenticationInterceptor : Interceptor
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public AuthenticationInterceptor(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public override AsyncUnaryCall<TResponse> AsyncUnaryCall<TRequest, TResponse>(
        TRequest request,
        ClientInterceptorContext<TRequest, TResponse> context,
        AsyncUnaryCallContinuation<TRequest, TResponse> continuation)
    {
        var httpContext = _httpContextAccessor.HttpContext;

        var token = httpContext?.Request.Headers["Authorization"].ToString(); ;

        var headers = context.Options.Headers ?? [];

        if (!string.IsNullOrEmpty(token))
        {
            headers.Add("Authorization", token);
        }

        var newOptions = context.Options.WithHeaders(headers);

        var newContext = new ClientInterceptorContext<TRequest, TResponse>(
            context.Method,
            context.Host,
            newOptions);

        return continuation(request, newContext);
    }
}