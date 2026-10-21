using System.ComponentModel.DataAnnotations;

namespace DotNetFoundations.DTOs.Validation;

[AttributeUsage(AttributeTargets.Property | AttributeTargets.Parameter)]
public sealed class NotEmptyGuidAttribute : ValidationAttribute
{
    public NotEmptyGuidAttribute() : base("O identificador deve ser diferente de Guid.Empty.") { }

    public override bool IsValid(object? value) => value is null || value is Guid id && id != Guid.Empty;
}
