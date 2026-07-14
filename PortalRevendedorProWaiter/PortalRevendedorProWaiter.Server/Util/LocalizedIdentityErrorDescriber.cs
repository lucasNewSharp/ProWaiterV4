using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Localization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PortalRevendedorProWaiter.Server.Util
{
    public class LocalizedIdentityErrorDescriber : IdentityErrorDescriber
    {
        private readonly IStringLocalizer<LocalizedIdentityErrorDescriber> _localizer;

        public LocalizedIdentityErrorDescriber(IStringLocalizer<LocalizedIdentityErrorDescriber> localizer)
        {
            _localizer = localizer;
        }

        public override IdentityError ConcurrencyFailure()
        {
            return InstanciarIdentityError(nameof(ConcurrencyFailure)) ?? base.ConcurrencyFailure();
        }

        public override IdentityError DefaultError()
        {
            return InstanciarIdentityError(nameof(DefaultError)) ?? base.DefaultError();
        }

        public override IdentityError DuplicateEmail(string email)
        {
            return InstanciarIdentityError(nameof(DuplicateEmail), email) ?? base.DuplicateEmail(email);
        }

        public override IdentityError DuplicateRoleName(string role)
        {
            return InstanciarIdentityError(nameof(DuplicateRoleName), role) ?? base.DuplicateRoleName(role);
        }

        public override IdentityError DuplicateUserName(string userName)
        {
            return InstanciarIdentityError(nameof(DuplicateUserName), userName) ?? base.DuplicateUserName(userName);
        }

        public override IdentityError InvalidEmail(string email)
        {
            return InstanciarIdentityError(nameof(InvalidEmail), email) ?? base.InvalidEmail(email);
        }

        public override IdentityError InvalidRoleName(string role)
        {
            return InstanciarIdentityError(nameof(InvalidRoleName), role) ?? base.InvalidRoleName(role);
        }

        public override IdentityError InvalidToken()
        {
            return InstanciarIdentityError(nameof(InvalidToken)) ?? base.InvalidToken();
        }

        public override IdentityError InvalidUserName(string userName)
        {
            return InstanciarIdentityError(nameof(InvalidUserName), userName) ?? base.InvalidToken();
        }

        public override IdentityError LoginAlreadyAssociated()
        {
            return InstanciarIdentityError(nameof(LoginAlreadyAssociated)) ?? base.LoginAlreadyAssociated();
        }

        public override IdentityError PasswordMismatch()
        {
            return InstanciarIdentityError(nameof(PasswordMismatch)) ?? base.PasswordMismatch();
        }

        public override IdentityError PasswordRequiresDigit()
        {
            return InstanciarIdentityError(nameof(PasswordRequiresDigit)) ?? base.PasswordRequiresDigit();
        }

        public override IdentityError PasswordRequiresLower()
        {
            return InstanciarIdentityError(nameof(PasswordRequiresLower)) ?? base.PasswordRequiresLower();
        }

        public override IdentityError PasswordRequiresNonAlphanumeric()
        {
            return InstanciarIdentityError(nameof(PasswordRequiresNonAlphanumeric)) ?? base.PasswordRequiresNonAlphanumeric();
        }

        public override IdentityError PasswordRequiresUniqueChars(int uniqueChars)
        {
            return InstanciarIdentityError(nameof(PasswordRequiresUniqueChars), uniqueChars.ToString()) ?? base.PasswordRequiresUniqueChars(uniqueChars);
        }

        public override IdentityError PasswordRequiresUpper()
        {
            return InstanciarIdentityError(nameof(PasswordRequiresUpper)) ?? base.PasswordRequiresUpper();
        }

        public override IdentityError PasswordTooShort(int length)
        {
            return InstanciarIdentityError(nameof(PasswordTooShort)) ?? base.PasswordTooShort(length);
        }

        public override IdentityError RecoveryCodeRedemptionFailed()
        {
            return InstanciarIdentityError(nameof(RecoveryCodeRedemptionFailed)) ?? base.RecoveryCodeRedemptionFailed();
        }

        public override IdentityError UserAlreadyHasPassword()
        {
            return InstanciarIdentityError(nameof(UserAlreadyHasPassword)) ?? base.UserAlreadyHasPassword();
        }

        public override IdentityError UserAlreadyInRole(string role)
        {
            return InstanciarIdentityError(nameof(UserAlreadyInRole)) ?? base.UserAlreadyInRole(role);
        }

        public override IdentityError UserLockoutNotEnabled()
        {
            return InstanciarIdentityError(nameof(UserLockoutNotEnabled)) ?? base.UserLockoutNotEnabled();
        }

        public override IdentityError UserNotInRole(string role)
        {
            return InstanciarIdentityError(nameof(UserNotInRole)) ?? base.UserNotInRole(role);
        }

        private IdentityError InstanciarIdentityError(string code)
        {
            return InstanciarIdentityError(code, string.Empty);
        }

        private IdentityError InstanciarIdentityError(string code, string valorStr)
        {
            var ie = new IdentityError
            {
                Code = code,
                Description = string.Format(_localizer[code], valorStr)
            };

            if (ie.Description.Equals(code))
                return null;
            return ie;
        }
    }
}