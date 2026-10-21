using DotNetFoundations.DTOs.Validation;

namespace DotNetFoundations.DTOs.Orders.Requests;

public class DeleteOrderRequest
{
    [NotEmptyGuid]
    public required Guid OrdersId { get; set; }
}
