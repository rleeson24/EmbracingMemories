namespace EmbracingMemories.IdentityMigrations
{
	using System.Data.Entity.Migrations;

	internal sealed class Configuration : DbMigrationsConfiguration<EmbracingMemories.Areas.Account.Models.ApplicationDbContext>
	{
		public Configuration()
		{
			AutomaticMigrationsEnabled = false;
			MigrationsDirectory = @"IdentityMigrations";
		}

		protected override void Seed(EmbracingMemories.Areas.Account.Models.ApplicationDbContext context)
		{
			//try
			//{
			//	//  This method will be called after migrating to the latest version.

			//	//  You can use the DbSet<T>.AddOrUpdate() helper extension method 
			//	//  to avoid creating duplicate seed data. E.g.
			//	//
			//	//    context.People.AddOrUpdate(
			//	//      p => p.FullName,
			//	//      new Person { FullName = "Andrew Peters" },
			//	//      new Person { FullName = "Brice Lambson" },
			//	//      new Person { FullName = "Rowan Miller" }
			//	//    );
			//	//
			//	context.Roles.AddOrUpdate(new[] {
			//	new Microsoft.AspNet.Identity.EntityFramework.IdentityRole("BasicUser"),
			//	new Microsoft.AspNet.Identity.EntityFramework.IdentityRole("ProfileUser"),
			//	new Microsoft.AspNet.Identity.EntityFramework.IdentityRole("BusinessUser"),
			//	new Microsoft.AspNet.Identity.EntityFramework.IdentityRole("ArchiveUser"),
			//	new Microsoft.AspNet.Identity.EntityFramework.IdentityRole("Admin")
			//});
			//}
			//catch (DbEntityValidationException dbEx)
			//{
			//	foreach (var validationErrors in dbEx.EntityValidationErrors)
			//	{
			//		foreach (var validationError in validationErrors.ValidationErrors)
			//		{
			//			Debug.WriteLine("Property: {0} Error: {1}",
			//									validationError.PropertyName,
			//									validationError.ErrorMessage);
			//		}
			//	}
			//}
		}
	}
}
