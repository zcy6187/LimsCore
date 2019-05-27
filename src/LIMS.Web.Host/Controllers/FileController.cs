using System;
using System.IO;
using Abp.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace LIMS.Web.Host.Controllers
{
    public class FileController : AbpController
    {
        private readonly IHostingEnvironment _hostingEnvironment;

        public FileController(IHostingEnvironment hostingEnvironment)
        {
            this._hostingEnvironment = hostingEnvironment;
        }

        public IActionResult Test()
        {
            return new JsonResult("{'ok':123}");
        }

        [HttpPost]
        public IActionResult Upload(IFormFile[] file, string fileName, Guid name)
        {

            foreach (var formFile in file)
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