using System.ComponentModel.DataAnnotations;

namespace IS220_PROJECT.Models
{
    public class CustomerInvoice
    {
        [Key]
        public int OrderId { get; set; }
        public int? AccountId { get; set; }
        public int? CustomerId { get; set; }
        public string? CustomerName { get; set; }
        public string? Address { get; set; }
        public string? Email { get; set; }
        public string? Phone { get; set; }
        public bool Paid { get; set; }
        public DateTime? OrderDate { get; set; }
        public int OrderDetailId { get; set; }
        public int? ProductId { get; set; }
        public string? ProductName { get; set; }
        public int? Price { get; set; }
        public string? Note { get; set; }
        public int? OrderNumber { get; set; }
        public int? Quantity { get; set; }
        public int? Discount { get; set; }
        public string? Thumbnail { get; set; }
        public int? Total { get; set; }
        public DateTime? ShipDate { get; set; }
        public string? TransactStatus { get; set; }
    }
}
