using System.Threading.Tasks;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

using dQRadca.Authorization;
using dQRadca.Data;
using dQRadca.Models;
using QRCoder;
using dQRadca.Controllers;

namespace dQRadca.Pages.Products
{
    public class DetailsModel : DiBasePageModel
    {
        public DetailsModel(ApplicationDbContext context, IAuthorizationService authorizationService, UserManager<ApplicationUser> userManager, IProductController productController) 
            : base(context, authorizationService, userManager, productController)
        {
        }

        public Product Product { get; set; }

        public async Task<IActionResult> OnGetAsync(int id)
        {
            Product = await Context.Product.FirstOrDefaultAsync(i => i.ProductId == id);

            if (Product == null)
            {
                return NotFound();
            }

            var isAuthorized = User.IsInRole(Resources.ApplicationTexts.ProductAccountantRole) || User.IsInRole(Resources.ApplicationTexts.ProductAdministratorsRole);

            var currentUserId = UserManager.GetUserId(User);

            return Page();
        }

        public async Task<IActionResult> OnPostAsync(int id)
        {
            var Product = await Context.Product.FirstOrDefaultAsync(i => i.ProductId == id);

            if (Product == null)
            {
                return NotFound();
            }

            Context.Product.Update(Product);
            await Context.SaveChangesAsync();

            return RedirectToPage("./Index");
        }
    }
}
