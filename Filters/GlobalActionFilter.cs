using Microsoft.AspNetCore.Mvc.Filters;

namespace IssueTracker.Filters
{
    public class GlobalActionFilter : IActionFilter
    {
        private readonly ILogger<GlobalActionFilter> logger;


        public GlobalActionFilter(ILogger<GlobalActionFilter> logger)
        {
            this.logger = logger;
        }

        public void OnActionExecuted(ActionExecutedContext context)
        {
            // Outbound
            logger.LogInformation("Sample Action Filer: OnActionExecuted");
        }

        public void OnActionExecuting(ActionExecutingContext context)
        {
            // Inbound
            logger.LogInformation("Sample Action Filer: OnActionExecuting");
        }
    }
}
