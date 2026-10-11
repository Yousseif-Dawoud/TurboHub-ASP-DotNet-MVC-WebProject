
namespace TurboHub.Models;

public class Order : BaseEntity
{

    [MaxLength(20)]
    public string? OrderNumber { get; set; }

    // The default value is Pending because when an order is created, it is usually in a pending 
    public OrderStatus Status { get; set; } = OrderStatus.Pending;

    public PaymentMethod PaymentMethod { get; set; } = PaymentMethod.CashOnDelivery;

    [MaxLength(200)]
    public string? DeliveryAddress { get; set; }

    [MaxLength(100)]
    public string? FullName { get; set; }

    [MaxLength(20)]
    public string? Phone { get; set; }

    public string? Notes { get; set; }

    public decimal SubTotal { get; set; }
    public decimal VatAmount { get; set; }
    public decimal TotalAmount { get; set; }

    // FK — nullable because guest orders have no user
    public string? UserId { get; set; }
    public ApplicationUser? User { get; set; }

    // Navigation
    public ICollection<OrderItem> Items { get; set; } = new List<OrderItem>();
}