namespace TurboHub.Models;

// This file contains all the enums used in the application for better organization and maintainability.
public enum OrderStatus
{
    Pending,
    Processing,
    Shipped,
    Delivered,
    Cancelled
}

public enum PaymentMethod
{
    CashOnDelivery,
    OnlinePayment
}

public enum SpecialRequestStatus
{
    New,
    UnderReview,
    Found,
    NotFound,
    Cancelled
}

public enum ProductCondition
{
    New,
    Refurbished
}