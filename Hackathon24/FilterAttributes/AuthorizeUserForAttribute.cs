using Hackathon24.Controllers;
using Hackathon24.FilterAttributes;
using Hackathon24.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.AspNetCore.OData.Routing.Controllers;
using System;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

namespace Hackathon24.FilterAttributes
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = false)]
    public sealed class AuthorizeUserForAttribute : Attribute, IAsyncAuthorizationFilter
    {
        private readonly string requiredScopeTag;

        /// <summary>
        /// Initializes a new instance of the <see cref="AuthorizeUserForAttribute"/> class.
        /// </summary>
        /// <param name="requiredScopeTag">The required scope tag to access the resource.</param>
        public AuthorizeUserForAttribute(string requiredScopeTag)
        {
            this.requiredScopeTag = requiredScopeTag;
        }

        public async Task OnAuthorizationAsync(AuthorizationFilterContext context)
        {
            var customersField = typeof(Hackathon24.Controllers.CustomersController)
                .GetField("customers", BindingFlags.NonPublic | BindingFlags.Static);

            var customers = (List<Customer>)customersField.GetValue(null);

            // Get the customer ID from the route data (from the URL)
            if (!context.RouteData.Values.TryGetValue("key", out var keyValue))
            {
                context.Result = new BadRequestObjectResult("Customer ID not found in route.");
                return;
            }

            if (!int.TryParse(keyValue.ToString(), out int customerId))
            {
                context.Result = new BadRequestObjectResult("Invalid customer ID.");
                return;
            }

            // Find the customer in the static list
            var customer = customers.SingleOrDefault(c => c.Id == customerId);

            if (customer == null)
            {
                context.Result = new UnauthorizedResult();
                return;
            }

            // Check if the customer has the required scope tag
            if (!customer.ScopeTags.Contains(this.requiredScopeTag))
            {
                context.Result = new ForbidResult();
                return;
            }

            // Customer is authorized, proceed with the request
            await Task.CompletedTask;
        }
    }
}
