using FluentValidation;

namespace Ahmed_mart.Dtos.v1.UserDtos.UserDtoValidator
{
    public class UpdateUserDtoValidator : AbstractValidator<UpdateUserDto>
    {
        public UpdateUserDtoValidator()
        {
            RuleFor(x => x.FirstName).NotEmpty();
            RuleFor(x => x.LastName).NotEmpty();
            RuleFor(x => x.Email).NotEmpty().EmailAddress();
            RuleFor(x => x.MobileNumber).NotEmpty();
            RuleFor(x => x.Password).Length(8, 12);
            RuleFor(x => x.Password)
                .Matches("^(?=.*[A-Z])(?=.*[a-z])(?=.*[-+_!@#$%^&*.,?])(?=.*[0-9]).+$")
                .Matches("^(?=.*[A-Z])").WithMessage("'Password' must contain at least one uppercase letter.")
                .Matches("(?=.*[a-z])").WithMessage("'Password' must contain at least one lowercase letter.")
                .Matches("(?=.*[-+_!@#$%^&*.,?])").WithMessage("'Password' must contain at least one special character.")
                .Matches("(?=.*[0-9])").WithMessage("'Password' must contain at least one number.");
        }
    }
}
