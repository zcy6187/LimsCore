using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Internal;
using Microsoft.AspNetCore.Mvc;

namespace LearnWebApi.Controllers
{
    [Route("[controller]/[action]")]
    [ApiController]
    public class FileController : ControllerBase
    {
        [HttpGet]
        public JsonResult Index()
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

        [HttpPost]
        public IActionResult PostFile([FromForm] IFormCollection formCollection)
        {
            string result = "fail";
            FormFileCollection fileCollection = (FormFileCollection)formCollection.Files;
            var fileDire = "D:\\Ftp\\www\\upload\\";
            foreach (IFormFile file in fileCollection)
            {
                StreamReader reader = new StreamReader(file.OpenReadStream());
                String content = reader.ReadToEnd();
                
                String name = file.FileName;
                string ext = Path.GetExtension(name);
                String filename =DateTime.Now.ToString("yyyyMMddhhmmss")+Guid.NewGuid()+ext;
                string allPath = fileDire + filename;
                using (FileStream fs = System.IO.File.Create(allPath))
                {
                    // 复制文件
                    file.CopyTo(fs);
                    // 清空缓冲区数据
                    fs.Flush();
                }
                result = filename;
            }
            return new JsonResult("{'ret':'"+result+"'}");
        }

        public string RecieveFile()
        {
            return null;
        }
    }
}
