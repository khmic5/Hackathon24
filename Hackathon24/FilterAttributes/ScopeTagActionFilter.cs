using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.OData.Query.Wrapper;
using Microsoft.AspNetCore.OData.Results;
using Hackathon24.Models;
using System.Collections.Generic;

namespace Hackathon24.CustomFilters
{
    public class ScopeTagActionFilter : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            // Code to execute before the action executes
            base.OnActionExecuting(context);
        }

        public override void OnActionExecuted(ActionExecutedContext context)
        {
            // Code to execute after the action executes
            //  Filter Scope Tags for each entity
            if (context.Result is Microsoft.AspNetCore.Mvc.ObjectResult objectResult)
            {
                if (objectResult.Value is Models.Customer customer)
                {
                    if (customer.ScopeTags.Count > 1)
                    {
                        customer.ScopeTags.Remove("2");
                    }
                }
                else if (objectResult.Value is IEnumerable<Models.Customer> customers)
                {
                    foreach (var cust in customers)
                    {
                        if (cust.ScopeTags.Count > 1)
                        {
                            cust.ScopeTags.Remove("2");
                        }
                    }
                }
            }

            base.OnActionExecuted(context);
        }
    }
}
