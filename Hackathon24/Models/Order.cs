namespace Hackathon24.Models
{
    public class Order : ICloneable
    {
        public int Id { get; set; }
        public decimal Amount { get; set; }

        public object Clone()
        {
            return new Order
            {
                Id = this.Id,
                Amount = this.Amount
            };
        }
    }
}
