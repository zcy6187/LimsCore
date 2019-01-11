using Microsoft.AspNetCore.Antiforgery;
using LIMS.Controllers;

namespace LIMS.Web.Host.Controllers
{
    public class AntiForgeryController : LIMSControllerBase
    {
        private readonly IAntiforgery _antiforgery;

        public AntiForgeryController(IAntiforgery antiforgery)
        {
            _antiforgery = antiforgery;
        }

        public void GetToken()
        {
            _antiforgery.SetCookieTokenAndHeader(HttpContext);
        }
    }
}
