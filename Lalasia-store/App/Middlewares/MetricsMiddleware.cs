using System.Diagnostics;
using Prometheus;

namespace Lalasia_store.App.Middlewares;

public class MetricsMiddleware
{
    private readonly RequestDelegate _next;
    
    private static readonly Counter TotalRequestsCount =
        Metrics.CreateCounter("total_requests_count", "Total number of requests");
    private static readonly Histogram RequestDuration = Metrics.CreateHistogram("request_duration",
        "Histogram for the duration of http requests");
    private static readonly Counter TotalRequestsErrorsCount =
        Metrics.CreateCounter("total_requests_errors_count", "Total number of requests with errors");

    public MetricsMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        TotalRequestsCount.Inc();

        var stopwatch = Stopwatch.StartNew();

        try
        {
            await _next(context);
        }
        catch
        {
            TotalRequestsErrorsCount.Inc();
            throw;
        }
        finally
        {
            stopwatch.Stop();
            RequestDuration.Observe(stopwatch.Elapsed.TotalSeconds);
        }
    }
}