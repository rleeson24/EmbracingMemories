using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data.Entity;
using System.Security.Claims;
using System.Threading.Tasks;

namespace EmbracingMemories.Models.Account
{
	// You can add profile data for the user by adding more properties to your ApplicationUser class, please visit https://go.microsoft.com/fwlink/?LinkID=317594 to learn more.
	public class ApplicationUser : IdentityUser
	{
		[Display( Name = "First Name" )]
		public string FirstName { get; set; }

		[Display( Name = "Last Name" )]
		public string LastName { get; set; }

		[Display( Name = "Line 1" )]
		public string AddressLine1 { get; set; }

		[Display( Name = "Line 2" )]
		public string AddressLine2 { get; set; }

		[Display( Name = "City" )]
		public string City { get; set; }

		[Display( Name = "State" )]
		public string State { get; set; }

		[Display( Name = "Postal Code" )]
		public string PostalCode { get; set; }

		[Display( Name = "Phone Number" )]
		public override string PhoneNumber { get; set; }

		[Display( Name = "Shirt Size" )]
		public string ShirtSize { get; set; }

		public ICollection<ApplicationUser> ChildUsers { get; set; }

		public async Task<ClaimsIdentity> GenerateUserIdentityAsync( UserManager<ApplicationUser> manager )
		{
			// Note the authenticationType must match the one defined in CookieAuthenticationOptions.AuthenticationType
			var userIdentity = await manager.CreateIdentityAsync( this, DefaultAuthenticationTypes.ApplicationCookie );
			// Add custom user claims here
			return userIdentity;
		}
	}

	public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
	{
		public ApplicationDbContext()
			: base( "DefaultConnection", throwIfV1Schema: false )
		{
		}

		public static ApplicationDbContext Create()
		{
			return new ApplicationDbContext();
		}

		protected override void OnModelCreating( DbModelBuilder modelBuilder )
		{
			base.OnModelCreating( modelBuilder );
			modelBuilder.Entity<ApplicationUser>()
				.HasMany( p => p.ChildUsers )
				.WithMany()
				.Map( m =>
				 {
					 m.MapLeftKey( "Father_Id" );
					 m.MapRightKey( "Son_Id" );
					 m.ToTable( "father_son_relation" );
				 } );
		}
	}
}