using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Threading;

namespace EmbracingMemories.Areas.Account.Models
{
	// Models used as parameters to AccountController actions.

	public class AddExternalLoginBindingModel
	{
		[Required]
		[Display(Name = "External access token")]
		public string ExternalAccessToken { get; set; }
	}

	public class ChangePasswordBindingModel
	{
		[Required]
		[DataType(DataType.Password)]
		[Display(Name = "Current password")]
		public string OldPassword { get; set; }

		[Required]
		[StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 6)]
		[DataType(DataType.Password)]
		[Display(Name = "New password")]
		public string NewPassword { get; set; }

		[DataType(DataType.Password)]
		[Display(Name = "Confirm new password")]
		[Compare("NewPassword", ErrorMessage = "The new password and confirmation password do not match.")]
		public string ConfirmPassword { get; set; }
	}

	public class LoginBindingModel
	{
		[Required]
		[Display(Name = "Email")]
		public string Email { get; set; }

		[Required]
		[DataType(DataType.Password)]
		[Display(Name = "Password")]
		public string Password { get; set; }

		public Boolean RememberMe { get; set; }
	}

	public class RegisterBindingModel
	{
		[Required]
		[Display(Name = "Email")]
		public string Email { get; set; }

		[RequiredWhenNotAdmin]
		//[StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 6)]
		[DataType(DataType.Password)]
		[Display(Name = "Password")]
		public string Password { get; set; }

		[RequiredWhenNotAdmin]
		[DataType(DataType.Password)]
		[Display(Name = "Confirm password")]
		[Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
		public string ConfirmPassword { get; set; }

		[Display(Name = "First Name")]
		[Required]
		public String FirstName { get; set; }

		[Display(Name = "Last Name")]
		[Required]
		public String LastName { get; set; }

		[Display(Name = "Company")]
		public String Company { get; set; }

		[Display(Name = "Address Line 1")]
		[Required]
		public String AddressLine1 { get; set; }

		[Display(Name = "Address Line 2")]
		public String AddressLine2 { get; set; }

		[Display(Name = "City")]
		[Required]
		public String City { get; set; }

		[Display(Name = "State")]
		[MaxLength(2)]
		[Required]
		public String State { get; set; }

		[Display(Name = "Postal Code")]
		[Required]
		public String PostalCode { get; set; }

		[Display(Name = "Country")]
		[MaxLength(2)]
		[Required]
		public String Country { get; set; }

		[Display(Name = "Phone")]
		[Required]
		public String Phone { get; set; }

		[CustomValidation(typeof(RegisterBindingModel), "ValidateRoleCode", ErrorMessage = "The role code is not valid")]
		public String RoleCode { get; set; }

		public static Dictionary<Role, String> ValidRoleCodes = new Dictionary<Role, String>
		{
			{ Role.Admin, "admin" },//@"qe$n$1dQ/33cH|hO=3'l" },
			{ Role.BusinessUser, "business" },//@"0^!CHH\RKV\#AWrTz+sX" },
			{ Role.ArchiveUser, "archive" },//@"n+BBK|%=J2#Hd2Yjymqd" }
			{ Role.BasicUser, "" },
		};

		public static ValidationResult ValidateRoleCode(string code, ValidationContext validationContext)
		{
			var currentUser = Thread.CurrentPrincipal;
			if (!String.IsNullOrWhiteSpace(code) && (currentUser == null || !currentUser.IsInRole("Admin")))
			{
				return new ValidationResult("Role code is not valid", new List<string> { "RoleCode" });
			}
			if (!ValidRoleCodes.ContainsValue(code ?? String.Empty))
				return new ValidationResult("Role code is not valid", new List<string> { "RoleCode" });
			return ValidationResult.Success;
		}

		public enum Role
		{
			Admin,
			BusinessUser,
			BasicUser,
			ArchiveUser
		}
	}

	public class RequiredWhenNotAdmin : ValidationAttribute
	{
		protected override ValidationResult IsValid(object value, ValidationContext validationContext)
		{
			var currentUser = Thread.CurrentPrincipal;
			if (currentUser.IsInRole("Admin"))
			{
				return ValidationResult.Success;
			}
			var emailStr = value as String;
			return String.IsNullOrWhiteSpace(emailStr) ? new ValidationResult("The " + validationContext.DisplayName + " is required.") : ValidationResult.Success;
		}
	}

	public class RegisterExternalBindingModel
	{
		[Required]
		[Display(Name = "Email")]
		public string Email { get; set; }
	}

	public class RemoveLoginBindingModel
	{
		[Required]
		[Display(Name = "Login provider")]
		public string LoginProvider { get; set; }

		[Required]
		[Display(Name = "Provider key")]
		public string ProviderKey { get; set; }
	}

	public class SetPasswordBindingModel
	{
		[Required]
		public String Code { get; set; }
		[Required]
		[DataType(DataType.EmailAddress)]
		public String EmailAddress { get; set; }

		[Required]
		[StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 6)]
		[DataType(DataType.Password)]
		[Display(Name = "New password")]
		public string NewPassword { get; set; }

		[DataType(DataType.Password)]
		[Display(Name = "Confirm new password")]
		[Compare("NewPassword", ErrorMessage = "The new password and confirmation password do not match.")]
		public string ConfirmPassword { get; set; }
	}
}
