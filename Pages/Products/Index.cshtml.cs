using System.Linq;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

using dQRadca.Data;
using dQRadca.Models;
using System;
using Microsoft.AspNetCore.Mvc;
using dQRadca.Controllers;

namespace dQRadca.Pages.Products
{
    public class IndexModel : DiBasePageModel
    {
        public IndexModel(ApplicationDbContext context, IAuthorizationService authorizationService, UserManager<ApplicationUser> userManager, IProductController productController)
            : base(context, authorizationService, userManager, productController)
        {
        }

        public string NameSort { get; set; }
        public string DateSort { get; set; }
        public string CurrentFilter { get; set; }
        public string CurrentSort { get; set; }
        public bool SelectAll { get; set; } = false;
        public PaginatedList<Product> Product { get; set; }

        public async Task OnGetAsync(string sortOrder, string currentFilter, string searchString, int? pageIndex, bool? selectAll)
        {
            if(selectAll != null)
            {
                var allProducts = (from p in Context.Product select p).ToList();

                foreach (var p in allProducts)
                {
                    p.IsSelected = selectAll.Value;
                }

                SelectAll = selectAll.Value;

                await Context.SaveChangesAsync();
            }

            CurrentSort = sortOrder;
            NameSort = String.IsNullOrEmpty(sortOrder) ? "name_desc" : "";
            DateSort = sortOrder == "Date" ? "date_desc" : "Date";

            if (searchString != null)
            {
                pageIndex = 1;
            }
            else
            {
                searchString = currentFilter;
            }

            CurrentFilter = searchString;

            IQueryable<Product> products = from p in Context.Product select p;

            var isAuthorized = User.IsInRole(Resources.ApplicationTexts.ProductAdministratorsRole);

            var currentUserId = UserManager.GetUserId(User);

            if (!String.IsNullOrEmpty(searchString))
            {
                products = products.Where(p => p.ProductName.Contains(searchString));
            }

            if (!isAuthorized)
            {
                products = products.Where(p => p.OwnerId == currentUserId);
            }

            switch (sortOrder)
            {
                case "name_desc":
                    products = products.OrderByDescending(s => s.ProductName);
                    break;
                case "Date":
                    products = products.OrderBy(s => s.CreatedDate);
                    break;
                case "date_desc":
                    products = products.OrderByDescending(s => s.CreatedDate);
                    break;
                default:
                    products = products.OrderBy(s => s.ProductName);
                    break;
            }

            int pageSize = 150;
            Product = await PaginatedList<Product>.CreateAsync(products.AsNoTracking(), pageIndex ?? 1, pageSize);
        }

        public async Task<IActionResult> OnPostAsync(int[] selectedProducts)
        {
            if(selectedProducts.Count() != 0)
            {
                ProductController.IdsList = selectedProducts.ToList();

                var products = (from p in Context.Product select p).ToList();

                foreach (var p in products)
                {
                    p.IsSelected = false;
                }

                await Context.SaveChangesAsync();

                return RedirectToPage("./Print");
            }

            return RedirectToPage("./Index");
        }
    }
}