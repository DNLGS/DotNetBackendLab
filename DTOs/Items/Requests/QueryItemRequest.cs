using DotNetFoundations.DTOs.Common;
using System.ComponentModel.DataAnnotations;
using DotNetFoundations.DTOs.Validation;

namespace DotNetFoundations.DTOs.Items.Requests;

public class QueryItemRequest : QueryRequest
{
    [NotEmptyGuid]
    public Guid? OrdersId { get; set; }
    [NotEmptyGuid]
    public Guid? ProductId { get; set; }
}
