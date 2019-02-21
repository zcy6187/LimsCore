using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using LimsWebApi.Model;
using Microsoft.AspNetCore.Mvc;

namespace LimsWebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ExcelController : ControllerBase
    {
        public ActionResult<string> GetTester()
        {
            return "123";
        }

       
    }
}
