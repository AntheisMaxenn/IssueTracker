using Microsoft.AspNetCore.Mvc.Filters;

namespace IssueTracker.Filters
{
    public class ActionFilterAttribute : Attribute, IActionFilter
    {
        private readonly ILogger<ActionFilterAttribute> logger;
        private readonly string location;
        private Guid guid;
        public ActionFilterAttribute(ILogger<ActionFilterAttribute> logger, string location = "global")
        {
            this.logger = logger;
            this.location = location;
            guid = Guid.NewGuid();
        }


        public void OnActionExecuted(ActionExecutedContext context)
        {
            //var services = context.HttpContext.RequestServices;
            //ILogger logger = services.GetRequiredService<ILogger>();

            logger.LogInformation($"Service Id: {guid}, OnActionExecuted: {location}");
        }

        public void OnActionExecuting(ActionExecutingContext context)
        {
            //var services = context.HttpContext.RequestServices;
            //ILogger logger = services.GetRequiredService<ILogger>();

            logger.LogInformation($"Service Id: {guid}, OnActionExecuting: {location}");
        }
    }
}
