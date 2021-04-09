using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Polly;
using Polly.CircuitBreaker;
using Polly.Extensions.Http;
using Polly.Timeout;
using System;
using System.Collections.Concurrent;
using System.Net.Http;

namespace Blueprint.Polly.WebApi
{
    /// <summary>
    /// Factory for producing WebApiClient fault handling policies.
    /// </summary>
    public class WebApiPolicyFactory : IWebApiPolicyFactory
	{
		private readonly ILogger<WebApiPolicyFactory> _logger;
		private readonly FaultHandlingConfiguration _config;
		private readonly IAsyncPolicy<HttpResponseMessage> _retryPolicy;
		private readonly IAsyncPolicy<HttpResponseMessage> _timeOutPolicy;
		private readonly ConcurrentDictionary<string, IAsyncPolicy<HttpResponseMessage>> _circuitBreakerPolicies = new ConcurrentDictionary<string, IAsyncPolicy<HttpResponseMessage>>();

		public WebApiPolicyFactory(ILogger<WebApiPolicyFactory> logger, IOptions<FaultHandlingConfiguration> options)
		{
			_logger = logger;
			_config = options.Value;

			_timeOutPolicy = Policy.TimeoutAsync<HttpResponseMessage>(TimeSpan.FromMilliseconds(_config.WebApiTimeoutInMs));

			// Retry policy is stateless so share same instance
			_retryPolicy = HttpPolicyExtensions
				// Only retry for transient errors (network errors, 5xx and 408 status codes)
				.HandleTransientHttpError()
				.Or<TimeoutRejectedException>()
				// Retry with exponential backoff
				.WaitAndRetryAsync(
					_config.MaxRetryAttempts,
					retryAttempt => TimeSpan.FromMilliseconds(_config.InitialRetryDelayInMs * Math.Pow(2, retryAttempt - 1)),
					async (result, sleepDuration, retryAttempt, context) =>
					{
						if (result.Result != null)
						{
							string content = result.Result.Content != null ? await result.Result.Content?.ReadAsStringAsync() : string.Empty;
							_logger.LogWarning(result.Exception, "Call returned with {statusCode} and {response} - Retry {retryCount} time after sleeping {duration} ms",
								result.Result.StatusCode, content, retryAttempt, sleepDuration.TotalMilliseconds);
						}
						else
						{
							_logger.LogWarning(result.Exception, "Call encountered an error - Retry {retryCount} time after sleeping {duration} ms",
								retryAttempt, sleepDuration.TotalMilliseconds);
						}

						context[ContextKey.RetryCount] = retryAttempt;
					})
				.WithPolicyKey("RetryPolicy-HttpResponseMessage");
		}

		public IAsyncPolicy<HttpResponseMessage> CreateNoOpPolicy()
		{
			return Policy.NoOpAsync<HttpResponseMessage>();
		}

		public IAsyncPolicy<HttpResponseMessage> CreateRetryPolicy() => _retryPolicy;

		public IAsyncPolicy<HttpResponseMessage> CreateCircuitBreakerPolicy(string name)
		{
			// Separate circuit breaker policies are possible because each independent circuit must have its own policy
			return _circuitBreakerPolicies.GetOrAdd(name, key =>
			{
				_logger.LogInformation("{providerName} circuit has been {circuitStatus}", name, "created");
				var circuitName = $"CircuitBreakerPolicy-HttpResponseMessage-{key}";

				void OnBreak(DelegateResult<HttpResponseMessage> result, CircuitState state, TimeSpan timespan, Context context)
				{
					if (state != CircuitState.HalfOpen)
					{
						_logger.LogError("{providerName} circuit is {circuitStatus} after failing {exceptionsAllowed} attempts. Duration of Break is {durationOnBreak}(s)",
							key, "broken", _config.ExceptionsAllowedBeforeBreaking, _config.DurationOnBreakInSec);
					}
					context[ContextKey.CircuitStatus] = CircuitStatus.Open;
				}

				void OnReset(Context context)
				{
					_logger.LogInformation("{providerName} circuit has been {circuitStatus}", key, "reset");
					context[ContextKey.CircuitStatus] = CircuitStatus.Closed;
				}

				return HttpPolicyExtensions
					// Only retry for transient errors (network errors, 5xx and 408 status codes)
					.HandleTransientHttpError()
					.Or<TimeoutRejectedException>()
					.CircuitBreakerAsync(
						handledEventsAllowedBeforeBreaking: _config.ExceptionsAllowedBeforeBreaking,
						durationOfBreak: TimeSpan.FromSeconds(_config.DurationOnBreakInSec),
						onBreak: OnBreak,
						onReset: OnReset,
						onHalfOpen: () => {})
					.WithPolicyKey(circuitName);
			});
		}

		public IAsyncPolicy<HttpResponseMessage> CreateTimeoutPolicy()
		{
			return _timeOutPolicy;
		}

		public IAsyncPolicy<HttpResponseMessage> CreateMainPolicy(string circuitName)
		{
			return Policy.WrapAsync(CreateCircuitBreakerPolicy(circuitName), CreateRetryPolicy());
		}
	}
}
