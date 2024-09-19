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
        private readonly string requiredScopeTags;

        /// <summary>
        /// Initializes a new instance of the <see cref="AuthorizeUserForAttribute"/> class.
        /// </summary>
        /// <param name="requiredScopeTag">The required scope tag to access the resource.</param>
        public AuthorizeUserForAttribute(string requiredScopeTags)
        {
            //  Join the required scope tags into a single string
            this.requiredScopeTags = requiredScopeTags;
        }

        public async Task OnAuthorizationAsync(AuthorizationFilterContext context)
        {
            //  Add the configured scope tags to the "ScopeTags" claim of the DemoUser Identity
            if (context.HttpContext.User != null && context.HttpContext.User.Identities != null)
            {
                // linq to get DemoUser idenity from User.Identities
                var demoUserIdentity = context.HttpContext.User.Identities.FirstOrDefault(i => i.Name == "DemoUser");
                if (demoUserIdentity != null)
                {
                    demoUserIdentity.AddClaim(new System.Security.Claims.Claim("ScopeTags", this.requiredScopeTags));
                }
            }
            /*
            // Use reflection to access the private static 'customers' field in CustomersController
            var customersField = typeof(CustomersController)
                .GetField("customers", BindingFlags.NonPublic | BindingFlags.Static);

            var customers = (List<Customer>)customersField.GetValue(null);

            // Check if a specific customer ID is provided (for requests like GET /odata/customers/{id})
            if (context.RouteData.Values.TryGetValue("key", out var keyValue))
            {
                // If a customer ID is found in the route
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
            }
            else
            {
                // If no customer ID is provided (like in GET /odata/customers)
                // Check if any customer in the static list matches the required scope tag
                if (!customers.Any(c => c.ScopeTags.Contains(this.requiredScopeTag)))
                {
                    context.Result = new ForbidResult();
                    return;
                }
            }
            */
            // Customer is authorized, proceed with the request
            await Task.CompletedTask;
        }
    }
}
