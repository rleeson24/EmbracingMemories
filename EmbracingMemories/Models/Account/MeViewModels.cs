namespace EmbracingMemories.Models.Account
{
	// Models returned by MeController actions.
	public class MeViewModel
	{
		public UserProfile Me { get; set; }
		public UserProfile[] ChildUsers { get; internal set; }
	}


	public class UserProfile
	{
		public string Email { get; set; }
		public string FirstName { get; set; }
		public string LastName { get; set; }
		public string AddressLine1 { get; set; }
		public string AddressLine2 { get; set; }
		public string City { get; set; }
		public string State { get; set; }
		public string PostalCode { get; set; }
		public string PhoneNumber { get; set; }
		public string ShirtSize { get; set; }

		public string EmailConfirmed { get; set; }

		public bool TwoFactorEnabled { get; set; }

		public string Id { get; set; }

		public string UserName { get; set; }
	}


}