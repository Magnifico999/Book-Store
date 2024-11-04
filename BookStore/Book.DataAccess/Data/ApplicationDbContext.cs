using Microsoft.EntityFrameworkCore;
using Book.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;

namespace Book.DataAccess.Data
{
    public class ApplicationDbContext : IdentityDbContext<IdentityUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

        public DbSet<Category> Categories { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<Company> Companies { get; set; }
        public DbSet<ApplicationUser> ApplicationUsers { get; set; }
        public DbSet<ShoppingCart> ShoppingCarts { get; set; }
        public DbSet<OrderHeader> OrderHeaders { get; set; }
        public DbSet<OrderDetail> OrderDetails  { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Category>().HasData(
                new Category
                {
                    Id = 1,
                    Name = "Action",
                    DisplayOrder = 1
                },
                new Category
                {
                    Id = 2,
                    Name = "SciFi",
                    DisplayOrder = 2
                },
                new Category
                {
                    Id = 3,
                    Name = "Adventure",
                    DisplayOrder = 3
                }
                );

			modelBuilder.Entity<Company>().HasData(
			   new Company
			   {
				   Id = 1,
				   Name = "Tech solution",
				   StreetAddress = "Cele",
                   City = "Alimosho",
                   State = "Lagos",
                   PostalCode="12333",
                   PhoneNumber="09123456789"

			   },
			   new Company
			   {
				   Id = 2,
				   Name = "Decagon",
				   StreetAddress = "Mushin",
				   City = "Alimosho",
				   State = "Lagos",
				   PostalCode = "12333",
				   PhoneNumber = "09176456789"
			   },
			   new Company
			   {
				   Id = 3,
				   Name = "Andela",
				   StreetAddress = "Oshodi",
				   City = "Alimosho",
				   State = "Lagos",
				   PostalCode = "12333",
				   PhoneNumber = "09129956789"
			   }
			   );
			modelBuilder.Entity<Product>().HasData(
                new Product
                {
                    Id = 1,
                    Title = "John Wick",
                    Description = "An interesting movie",
                    ISBN = "TH1233333333",
                    Author = "Ron Parker",
                    ListPrice = 30,
                    Price = 27,
                    Price50 = 25,
                    Price100 = 20,
                    CategoryId = 1,
                    ImageUrl = ""
                },
                 new Product
                 {
                     Id = 2,
                     Title = "Black Adam",
                     Description = "An interesting movie",
                     ISBN = "RD128888888",
                     Author = "Ron Parker",
                     ListPrice = 25,
                     Price = 23,
                     Price50 = 22,
                     Price100 = 20,
                     CategoryId = 2,
                     ImageUrl = ""
                 },
                  new Product
                  {
                      Id = 3,
                      Title = "13 Reasons ",
                      Description = "An interesting movie",
                      ISBN = "UJ2573817Y",
                      Author = "Ron Parker",
                      ListPrice = 30,
                      Price = 27,
                      Price50 = 25,
                      Price100 = 20,
                      CategoryId = 3,
                      ImageUrl = ""
                  }
                );
        }
    }
}
