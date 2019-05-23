using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace LIMS.Web.Host.Controllers
{
    [Consumes("application/json", "multipart/form-data")]//此处为新增
    [Route("api/[controller]")]
    [ApiController]
    public class FileController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Upload(IFormFile[] image, string fileName, Guid name)
        {

            foreach (var formFile in image)
            {
                if (formFile.Length > 0)
                {
                    string fileExt = Path.GetExtension(formFile.FileName); //文件扩展名，不含“.”
                    long fileSize = formFile.Length; //获得文件大小，以字节为单位
                    name = name == Guid.Empty ? Guid.NewGuid() : name;
                    string newName = name + fileExt; //新的文件名
                    var fileDire = "D:\\Ftp\\www\\upload\\";
                    if (!Directory.Exists(fileDire))
                    {
                        Directory.CreateDirectory(fileDire);
                    }

                    var filePath = fileDire + newName;

                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        formFile.CopyTo(stream);
                    }
                }
            }

            return new JsonResult("{'ok':123}");
        }
    }
}