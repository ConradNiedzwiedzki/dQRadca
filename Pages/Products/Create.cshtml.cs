using System;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

using dQRadca.Authorization;
using dQRadca.Data;
using dQRadca.Models;
using System.Drawing;
using static QRCoder.PayloadGenerator;
using QRCoder;
using dQRadca.Controllers;

namespace dQRadca.Pages.Products
{
    public class CreateModel : DiBasePageModel
    {
        private readonly UserManager<ApplicationUser> userManager;

        public CreateModel(ApplicationDbContext context, IAuthorizationService authorizationService, UserManager<ApplicationUser> userManager, IProductController productController)
            : base(context, authorizationService, userManager, productController)
        {
            this.userManager = userManager;
        }

        public IActionResult OnGet()
        {
            Product = new Product
            {
                CreatedDate = DateTime.Now
            };
            return Page();
        }

        [BindProperty]
        public Product Product { get; set; }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            var user = await userManager.GetUserAsync(User);
            if (user == null)
            {
                throw new ApplicationException(string.Format(Resources.ApplicationTexts.UnableToLoadUserWithId, userManager.GetUserId(User)));
            }

            Product.CreatedDate = DateTime.Now;
            Product.OwnerId = UserManager.GetUserId(User);
            Product.QRCode = GenerateQrCode(Product.UrlString);

            var isAuthorized = await AuthorizationService.AuthorizeAsync(User, Product, ProductOperations.Create);

            if (!isAuthorized.Succeeded)
            {
                return new ChallengeResult();
            }

            Context.Product.Add(Product);
            await Context.SaveChangesAsync();

            return RedirectToPage("./Index");
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