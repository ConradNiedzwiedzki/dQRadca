using System.Threading.Tasks;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

using dQRadca.Authorization;
using dQRadca.Data;
using dQRadca.Models;
using dQRadca.Controllers;

namespace dQRadca.Pages.Products
{
    public class DeleteModel : DiBasePageModel
    {
        public DeleteModel(ApplicationDbContext context, IAuthorizationService authorizationService, UserManager<ApplicationUser> userManager, IProductController productController)
            : base(context, authorizationService, userManager, productController)
        {
        }

        [BindProperty]
        public Product Product { get; set; }

        public async Task<IActionResult> OnGetAsync(int id)
        {
            Product = await Context.Product.FirstOrDefaultAsync(i => i.ProductId == id);

            if (Product == null)
            {
                return NotFound();
            }

            var isAuthorized = await AuthorizationService.AuthorizeAsync(User, Product, ProductOperations.Delete);

            if (!isAuthorized.Succeeded)
            {
                return new ChallengeResult();
            }

            return Page();
        }

        public async Task<IActionResult> OnPostAsync(int id)
        {
            Product = await Context.Product.FindAsync(id);

            var product = await Context.Product.AsNoTracking().FirstOrDefaultAsync(i => i.ProductId == id);

            if (product == null)
            {
                return NotFound();
            }

            var isAuthorized = await AuthorizationService.AuthorizeAsync(User, product, ProductOperations.Delete);

            if (!isAuthorized.Succeeded)
            {
                return new ChallengeResult();
            }

            Context.Product.Remove(Product);
            await Context.SaveChangesAsync();

            return RedirectToPage("./Index");
        }
    }
}
