using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace LimsX.Pages
{
    public class IndexModel : PageModel
    {
        public string UserName { get; set; }
        public void OnGet()
        {
            this.UserName = this.HttpContext.User.Identity.Name;
        }
    }
}
