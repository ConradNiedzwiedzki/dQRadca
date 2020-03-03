using System.Collections.Generic;
using System.Linq;
using dQRadca.Controllers;
using dQRadca.Data;
using dQRadca.Models;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;

namespace dQRadca.Pages.Products
{
    public class PrintModel : DiBasePageModel
    {
        public PrintModel(ApplicationDbContext context, IAuthorizationService authorizationService, UserManager<ApplicationUser> userManager, IProductController productController)
            : base(context, authorizationService, userManager, productController)
        {
        }

        public List<Product> Product { get; set; }

        public void OnGet()
        {
            var ids = ProductController.IdsList;

            List<Product> products = (from p in Context.Product select p).ToList();
            var productsToPrint = new List<Product>();

            foreach (var p in products)
            {
                if (ids.Contains(p.ProductId))
                {
                    productsToPrint.Add(p);
                }
            }

            Product = productsToPrint;
        }
    }
}
