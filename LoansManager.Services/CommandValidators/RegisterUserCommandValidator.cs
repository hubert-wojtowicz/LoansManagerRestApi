﻿using System;
using System.Text.RegularExpressions;
using FluentValidation;
using LoansManager.CommandHandlers.Commands;
using LoansManager.Services.Infrastructure.SettingsModels;
using LoansManager.Services.Resources;
using LoansManager.Services.ServicesContracts;

namespace LoansManager.Services.CommandValidators
{
    public class RegisterUserCommandValidator : AbstractValidator<RegisterUserCommand>
    {
        public RegisterUserCommandValidator(IUserService userService, ApiSettings apiSettings)
        {
            if (userService == null)
            {
                throw new ArgumentNullException($"{nameof(userService)} can not be null.");
            }

            RuleFor(x => x)
                .NotEmpty();

            RuleFor(x => x.Password)
                .NotEmpty()
                .Must(x => Regex.IsMatch(x, apiSettings.UserPasswordPattern))
                .WithMessage(RegisterUserCommandValidatorResource.PasswortInvalid);

            RuleFor(x => x.UserName)
                .NotEmpty()
                .MustAsync(userService.UserDoesNotExist)
                .WithMessage(RegisterUserCommandValidatorResource.UserExists);
        }
    }
}
