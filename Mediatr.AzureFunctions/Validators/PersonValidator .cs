using FluentValidation;
using IsolatedMediatr.Models;

namespace IsolatedMediatr.Validators
{
    public class PersonValidator : AbstractValidator<Person>
    {
        public PersonValidator()
        {
            RuleFor(x => x.Name).NotEmpty().NotNull().WithMessage("Name is required.");
            RuleFor(x => x.Email).NotEmpty().NotNull().WithMessage("Email is required.");
            RuleFor(x => x.Email).EmailAddress().WithMessage("Enter a valid Email Address.");
        }
    }
}
