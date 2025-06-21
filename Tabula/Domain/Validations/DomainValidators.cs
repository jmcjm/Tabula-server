using ErrorOr;

namespace Domain.Validations;

internal static class DomainValidators
{
    internal static ErrorOr<Success> NameValidator(string name, byte maxLength)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            return Error.Validation("NameValidation", "Name cannot be empty.");
        }

        if (name.Length > maxLength)
        {
            return Error.Validation("NameValidation",$"Max length of name is {maxLength}.");
        }

        return Result.Success;
    }
}