namespace Hackathon24.Models
{
    public class Customer : ICloneable
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public List<string> ScopeTags { get; set; }
        public List<Order> Orders { get; set; }

        public object Clone()
        {
            return new Customer
            {
                Id = this.Id,
                Name = this.Name,
                ScopeTags = new List<string>(this.ScopeTags),
                Orders = new List<Order>(this.Orders.Select(o => (Order)o.Clone()))
            };
        }
    }
}
