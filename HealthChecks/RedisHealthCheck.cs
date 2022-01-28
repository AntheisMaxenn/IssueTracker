using Microsoft.Extensions.Diagnostics.HealthChecks;
using StackExchange.Redis;

namespace IssueTracker.HealthChecks
{
    public class RedisHealthCheck : IHealthCheck
    {
        private readonly IConnectionMultiplexer connectionMultiplexer;

        public RedisHealthCheck(IConnectionMultiplexer connectionMultiplexer)
        {
            this.connectionMultiplexer = connectionMultiplexer;
        }

        public Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
        {
            try
            {
                var database = connectionMultiplexer.GetDatabase();
                database.StringGet("Health");
                return Task.FromResult(HealthCheckResult.Healthy());

            }
            catch(Exception e)
            {
                return Task.FromResult(HealthCheckResult.Unhealthy(e.Message));
            }
        }
    }
}
