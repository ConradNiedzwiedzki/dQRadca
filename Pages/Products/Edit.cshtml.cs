using System.Linq;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

using dQRadca.Authorization;
using dQRadca.Data;
using dQRadca.Models;
using static QRCoder.PayloadGenerator;
using QRCoder;
using dQRadca.Controllers;

namespace dQRadca.Pages.Products
{
    public class EditModel : DiBasePageModel
    {
        public EditModel(ApplicationDbContext context, IAuthorizationService authorizationService, UserManager<ApplicationUser> userManager, IProductController productController)
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

            var isAuthorized = await AuthorizationService.AuthorizeAsync(User, Product, ProductOperations.Update);
            if (!isAuthorized.Succeeded)
            {
                return new ChallengeResult();
            }

            return Page();
        }

        public async Task<IActionResult> OnPostAsync(int id)
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            var product = await Context.Product.FirstOrDefaultAsync(i => i.ProductId == id);

            if (product == null)
            {
                return NotFound();
            }

            var isAuthorized = await AuthorizationService.AuthorizeAsync(User, product, ProductOperations.Update);

            if (!isAuthorized.Succeeded)
            {
                return new ChallengeResult();
            }

            product.ProductName = Product.ProductName;
            product.UrlString = Product.UrlString;
            product.QRCode = GenerateQrCode(Product.UrlString);

            await Context.SaveChangesAsync();

            return RedirectToPage("./Index");
        }

        private bool ProductExists(int id)
        {
            return Context.Product.Any(e => e.ProductId == id);
        }

        private string GenerateQrCode(string urlString)
        {
            Url generator = new Url(urlString);
            string payload = generator.ToString();

            QRCodeGenerator qrGenerator = new QRCodeGenerator();
            QRCodeData qrCodeData = qrGenerator.CreateQrCode(payload, QRCodeGenerator.ECCLevel.Q);
            Base64QRCode qrCode = new Base64QRCode(qrCodeData);

            return qrCode.GetGraphic(20);
        }
    }
}
