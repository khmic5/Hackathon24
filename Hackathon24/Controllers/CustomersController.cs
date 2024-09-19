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
        [ScopeTagActionFilter]
        [AuthorizeUserFor("1")] // Only customers with ScopeTag "1" can access this
        public ActionResult<IEnumerable<Customer>> Get()
        {
            return Ok(customers);
        }

        [EnableQuery]
        [ScopeTagActionFilter]
        [AuthorizeUserFor("2")] // Only customers with ScopeTag "2" can access this
        public ActionResult<Customer> Get([FromRoute] int key)
        {
            var item = customers.SingleOrDefault(d => d.Id.Equals(key));

            if (item == null)
            {
                return NotFound();
            }

            return Ok(item);
        }
    }
}
