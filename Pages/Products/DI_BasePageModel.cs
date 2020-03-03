using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.RazorPages;

using dQRadca.Data;
using dQRadca.Controllers;

namespace dQRadca.Pages.Products
{
    public class DiBasePageModel : PageModel
    {
        protected ApplicationDbContext Context { get; }
        protected IAuthorizationService AuthorizationService { get; }
        protected UserManager<ApplicationUser> UserManager { get; }
        protected IProductController ProductController { get; }

        public DiBasePageModel(ApplicationDbContext context, IAuthorizationService authorizationService, UserManager<ApplicationUser> userManager, IProductController productController) : base()
        {
            Context = context;
            UserManager = userManager;
            AuthorizationService = authorizationService;
            ProductController = productController;
        } 
    }
}
