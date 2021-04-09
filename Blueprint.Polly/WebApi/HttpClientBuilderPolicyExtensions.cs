using Microsoft.Extensions.DependencyInjection;
using System.Net.Http;

namespace Blueprint.Polly.WebApi
{
    /// <summary>
    /// Document:
    /// Net Core: https://github.com/App-vNext/Polly/wiki/Polly-and-HttpClientFactory
    /// Net 4.5
    /// https://github.com/App-vNext/Polly-Samples/blob/master/PollyTestClient/Samples/Demo08_Wrap-Fallback-WaitAndRetry-CircuitBreaker.cs
    /// https://github.com/App-vNext/Polly-Samples/blob/master/PollyTestClient/Samples/Demo06_WaitAndRetryNestingCircuitBreaker.cs
    /// </summary>
    public static class HttpClientBuilderPolicyExtensions
    {
        public static IHttpClientBuilder AddDefaultFaultHandlingPolicies(this IHttpClientBuilder builder)
        {
            return builder
                .AddPolicyHandler((serviceProvider, request) => serviceProvider.GetService<IWebApiPolicyFactory>().CreateCircuitBreakerPolicy(builder.Name))
                .AddPolicyHandler((serviceProvider, request) =>
                {
                    var factory = serviceProvider.GetService<IWebApiPolicyFactory>();
                    // Retries should only be performed on idempotent operations
                    return request.Method == HttpMethod.Get ? factory.CreateRetryPolicy() : factory.CreateNoOpPolicy();
                })
                .AddPolicyHandler((serviceProvider, request) => serviceProvider.GetService<IWebApiPolicyFactory>().CreateTimeoutPolicy());
        }
    }
}
