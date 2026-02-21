using FluentValidation;
using TicketSystem.API.DTOs;

public class RegisterRequestValidator : AbstractValidator<RegisterRequest>
{
    public RegisterRequestValidator()
    {
        RuleFor(x => x.Password)
            .NotEmpty()
            .MinimumLength(6)
            .WithMessage("La contraseña debe tener al menos 6 caracteres");
    }
}