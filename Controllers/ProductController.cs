using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;

namespace dQRadca.Controllers
{
    public class ProductController : Controller, IProductController
    {
        public List<int> IdsList { get; set; }

        public ProductController()
        {
        }
    }
}
