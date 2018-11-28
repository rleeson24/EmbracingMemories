using System;
using System.ComponentModel.DataAnnotations;

namespace EmbracingMemories.Areas.QrProfiles.Models
{
	public class CreateProfileModel
	{
		// required only when the user is not creating their own profile
		[Display(Name = "First Name")]
		[Required]
		public String UserFirstName { get; set; }

		[Display(Name = "Last Name")]
		[Required]
		public String UserLastName { get; set; }

		[Display(Name = "Address Line 1")]
		[Required]
		public String UserAddressLine1 { get; set; }

		[Display(Name = "Address Line 2")]
		public String UserAddressLine2 { get; set; }

		[Display(Name = "City")]
		[Required]
		public String UserCity { get; set; }

		[Display(Name = "State")]
		[MaxLength(2)]
		[Required]
		public String UserState { get; set; }

		[Display(Name = "Postal Code")]
		[Required]
		public String UserPostalCode { get; set; }

		[Display(Name = "Country")]
		[Required]
		public String UserCountry { get; set; }

		[Display(Name = "Phone")]
		[Required]
		public String UserPhone { get; set; }

		[Display(Name = "Email Address")]
		[Required]
		public String UserEmail { get; set; }

		[Display(Name = "Email Address Confirmation")]
		[Required]
		[Compare("UserEmail", ErrorMessage = "The email address and confirmation email address do not match.")]
		public String UserEmailConfirmation { get; set; }


		[Display(Name = "First Name")]
		[Required]
		public String FirstName { get; set; }

		[Display(Name = "Middle Name")]
		public string MiddleName { get; set; }

		[Display(Name = "Last Name")]
		[Required]
		public String LastName { get; set; }

		[Required]
		public String Sex { get; set; }

		//[Required]
		//public String Obituary { get; set; }

		[Required]
		public DateTime? Birthday { get; set; }

		[Display(Name = "Date of Death")]
		[Required]
		public DateTime? DateOfDeath { get; set; }

		public String CardToken { get; set; }
	}

	//public class RequireWhenBasicUser : ValidationAttribute
	//{
	//	protected override ValidationResult IsValid(object value, ValidationContext validationContext)
	//	{
	//		var currentUser = Thread.CurrentPrincipal;
	//		if (!currentUser.IsInRole("Admin") && !currentUser.IsInRole("BusinessUser"))
	//		{
	//			return ValidationResult.Success;
	//		}
	//		var emailStr = value as String;
	//		return String.IsNullOrWhiteSpace(emailStr) ? new ValidationResult("The " + validationContext.DisplayName + " is required.") : ValidationResult.Success;
	//	}
	//}
}