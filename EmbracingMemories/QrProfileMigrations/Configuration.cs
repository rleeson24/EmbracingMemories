namespace EmbracingMemories.QrProfileMigrations
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Migrations;
    using System.Linq;

    internal sealed class Configuration : DbMigrationsConfiguration<EmbracingMemories.Models.QrContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = false;
            MigrationsDirectory = @"QrProfileMigrations";
        }

        protected override void Seed( EmbracingMemories.Models.QrContext context )
        {
            //  This method will be called after migrating to the latest version.

            //  You can use the DbSet<T>.AddOrUpdate() helper extension method 
            //  to avoid creating duplicate seed data. E.g.
            //
            //    context.People.AddOrUpdate(
            //      p => p.FullName,
            //      new Person { FullName = "Andrew Peters" },
            //      new Person { FullName = "Brice Lambson" },
            //      new Person { FullName = "Rowan Miller" }
            //    );
            //
        }
    }
}
//namespace EmbracingMemories.QrProfileMigrations
//{
//    using System;
//    using System.Data.Entity;
//    using System.Data.Entity.Migrations;
//    using System.Linq;

//    internal sealed class Configuration : DbMigrationsConfiguration<EmbracingMemories.Models.QrContext>
//    {
//        public Configuration()
//        {
//            AutomaticMigrationsEnabled = false;
//            MigrationsDirectory = @"QrProfileMigrations";
//        }

//		protected override void Seed( EmbracingMemories.Models.QrContext context )
//		{
//			//if (System.Diagnostics.Debugger.IsAttached == false)
//			//    System.Diagnostics.Debugger.Launch();
//			//try
//			//{
//			for( var i = 0; i < 1000; i++ )
//			{
//				context.ReservedQrCodes.AddOrUpdate( new Areas.QrProfiles.Models.ReservedQrCode() { Key = i, Id = Guid.NewGuid(), Used = false } );
//			}
//            context.QrProfiles.AddOrUpdate(x => x.Id,
//                new Areas.QrProfiles.Models.QrProfile()
//                {
//                    Id = new Guid("ca5803da-4a83-4d68-8293-6bc9a0d23cc8"),
//                    UserId = "8b1b3917-670a-437a-bab0-b509e840bd7f",
//                    FirstName = "Robert",
//                    MiddleName = String.Empty,
//                    LastName = "Leeson",
//                    Sex = "M",
//                    Birthday = new DateTime(1955, 10, 1),
//                    DateOfDeath = new DateTime(1981, 10, 6),
//                    LifeHistory = String.Empty,
//                    Obituary = String.Empty,
//                    Links = {
//                        new Areas.QrProfiles.Models.QrLink() { Id = 0, Label = "link 1", Url = new Uri("http://www.google.com").AbsoluteUri }
//                    },
//                    Moments = { new Areas.QrProfiles.Models.MemorableMoment { OccurredOn = new DateTime(2015, 01, 01), Text = "This moment was the coolest!" } }
//                },
//                new Areas.QrProfiles.Models.QrProfile()
//                {
//                    Id = new Guid("07b5016b-a539-44d8-b89c-411f8d8bd605"),
//                    UserId = "8b1b3917-670a-437a-bab0-b509e840bd7g",
//                    FirstName = "Jane",
//                    MiddleName = String.Empty,
//                    LastName = "Leeson",
//                    Sex = "F",
//                    Birthday = new DateTime(1956, 10, 2),
//                    DateOfDeath = new DateTime(1981, 10, 7),
//                    LifeHistory = String.Empty,
//                    Obituary = String.Empty,
//                    Links = {
//                        new Areas.QrProfiles.Models.QrLink() { Id = 1, Label = "link 2", Url = new Uri("http://www.arcbest.com").AbsoluteUri }
//                    }
//                },
//                new Areas.QrProfiles.Models.QrProfile() { Id = new Guid("99937c5b-b119-497c-b452-3b349fb8d959"), UserId = "8b1b3917-670a-437a-bab0-b509e840bd7f", FirstName = "Jimmy", MiddleName = String.Empty, LastName = "Leeson", Sex = "M", Birthday = new DateTime(1957, 10, 3), DateOfDeath = new DateTime(1981, 10, 8), LifeHistory = String.Empty, Obituary = String.Empty }
//            );
//            context.SaveChanges();
//            //}
//            //catch ( Exception ex )
//            //{

//            //}
//        }
//    }
//}
