using FastEndpoints;
using FluentValidation;
using TaskManager.Application.DTOs;

namespace TaskManager.Application.Validators;

public class CreateTaskValidator : Validator<CreateTaskRequest>
{
    public CreateTaskValidator()
    {
        RuleFor(x => x.Title)
            .NotEmpty().WithMessage("El título no puede estar vacío.")
            .MaximumLength(200).WithMessage("El título no puede superar 200 caracteres.");

        RuleFor(x => x.Description)
            .MaximumLength(500).WithMessage("La descripción no puede superar 500 caracteres.");
    }
}