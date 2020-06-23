using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace IdentityServerPOC.Infrastructure
{
    public class AppIdentityDbContext : IdentityDbContext<ApplicationUser>
    {

        public AppIdentityDbContext(DbContextOptions<AppIdentityDbContext> options) : base(options)
        {
           
        }
     
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            // Customize the ASP.NET Identity model and override the defaults if needed.
            // For example, you can rename the ASP.NET Identity table names and more.
            // Add your customizations after calling base.OnModelCreating(builder);

            //modelBuilder.Entity<IdentityUserRole>().HasKey(cs => new { cs.UserId, cs.RoleId });

            //modelBuilder.Entity("AspNetUserRoles", b =>
            //{
            //    b.HasOne("AspNetUsers", "ApplicationUser")
            //        .WithMany("UserRoles")
            //        .HasForeignKey("UserId");

            //    b.HasOne("AspNetRoles", "ApplicationRole")
            //        .WithMany("UserRoles")
            //        .HasForeignKey("RoleId");
            //});

            modelBuilder.Entity<IdentityRole>().HasData(new IdentityRole { Name = Infrastructure.Roles.SuperAdmin, NormalizedName = Infrastructure.Roles.SuperAdmin.ToUpper() });

        }
    }
}
