using System.Reflection.Emit;

namespace Hackathon24.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Hackathon24.Models;
    using Hackathon24.CustomFilters;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.OData.Query;
    using Microsoft.AspNetCore.OData.Routing.Controllers;
    using Hackathon24.FilterAttributes;
    using Hackathon24.Helpers;

    public class CustomersController : ODataController
    {
        private static Random random = new Random();
        private static List<Customer> customers = new List<Customer>(
            Enumerable.Range(1, 3).Select(idx => new Customer
            {
                Id = idx,
                Name = $"Customer {idx}",
                ScopeTags = new List<string> { "1", "2", "3", "4" },
                Orders = new List<Order>(
                    Enumerable.Range(1, 2).Select(dx => new Order
                    {
                        Id = (idx - 1) * 2 + dx,
                        Amount = random.Next(1, 9) * 10
                    }))
            }));

        [EnableQuery]
        [AuthorizeUserFor("1;2")] // Only customers with ScopeTag "1" can access this
        //  [ScopeTagActionFilter(typeof(Customer), "ScopeTags")]
        public ActionResult<IEnumerable<Customer>> Get()
        {
            var customersToReturn = customers.Select(c => (Customer)c.Clone()).ToList();

            //  Process and fitler scope tags for each customer object
            foreach (var customer in customersToReturn)
            {
                var intersectingScopeTags = ScopeTagHelpers.GetIntersectingScopeTags(customer.ScopeTags, this.HttpContext);
                customer.ScopeTags = intersectingScopeTags;
            }

            return Ok(customersToReturn);
        }

        [EnableQuery]
        [AuthorizeUserFor("3;4")] // Only customers with ScopeTag "2" can access this
        //  [ScopeTagActionFilter(typeof(Customer), "ScopeTags")]
        public ActionResult<Customer> Get([FromRoute] int key)
        {
            var item = customers.SingleOrDefault(d => d.Id.Equals(key));

            if (item == null)
            {
                return NotFound();
            }

            //  Process and fitler scope tags
            var itemToReturn = (Customer)item.Clone();
            var intersectingScopeTags = ScopeTagHelpers.GetIntersectingScopeTags(itemToReturn.ScopeTags, this.HttpContext);
            itemToReturn.ScopeTags = intersectingScopeTags;

            return Ok(itemToReturn);
        }
    }
}
