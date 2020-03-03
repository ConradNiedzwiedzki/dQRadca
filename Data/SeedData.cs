using System;
using System.Linq;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

using dQRadca.Models;

namespace dQRadca.Data
{
    public static class SeedData
    {
        public static async Task Initialize(IServiceProvider serviceProvider, string testUserPassword)
        {
            using (var context = new ApplicationDbContext(serviceProvider.GetRequiredService<DbContextOptions<ApplicationDbContext>>()))
            {
                var adminId = await EnsureUser(serviceProvider, testUserPassword, Resources.ApplicationTexts.AdminEmail);
                await EnsureRole(serviceProvider, adminId, Resources.ApplicationTexts.ProductAdministratorsRole);

                var managerId = await EnsureUser(serviceProvider, testUserPassword, Resources.ApplicationTexts.ManagerEmail);
                await EnsureRole(serviceProvider, managerId, Resources.ApplicationTexts.ProductAccountantRole);

                SeedDb(context, adminId);
            }
        }
      
        private static async Task<string> EnsureUser(IServiceProvider serviceProvider, string testUserPassword, string userName)
        {
            var userManager = serviceProvider.GetService<UserManager<ApplicationUser>>();

            var user = await userManager.FindByNameAsync(userName);
            if (user != null)
            {
                return user.Id;
            }

            user = new ApplicationUser { UserName = userName };
            await userManager.CreateAsync(user, testUserPassword);

            return user.Id;
        }

        private static async Task<IdentityResult> EnsureRole(IServiceProvider serviceProvider, string userId, string role)
        {
            var roleManager = serviceProvider.GetService<RoleManager<IdentityRole>>();

            if (!await roleManager.RoleExistsAsync(role))
            {
                _ = await roleManager.CreateAsync(new IdentityRole(role));
            }

            var userManager = serviceProvider.GetService<UserManager<ApplicationUser>>();
            var user = await userManager.FindByIdAsync(userId);
            var identityResult = await userManager.AddToRoleAsync(user, role);

            return identityResult;
        }

        public static void SeedDb(ApplicationDbContext context, string adminId)
        {
            if (context.Product.Any())
            {
                return;
            }

            context.Product.AddRange(
            new Product
            {
                ProductName = "Test",
                CreatedDate = new DateTime(2019, 5, 15),
                UrlString = "http://example.pl/",
                QRCode = "",
                OwnerId = adminId
            }
            );

            context.SaveChanges();
        }
    }
}