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
        string uploadDir = "D:\\Ftp\\www\\upload\\";

        [HttpGet]
        public JsonResult Index()
        {
            return new JsonResult("{'ok':123}");
        }

        [HttpPost]
        public JsonResult PostFile([FromForm] IFormCollection formCollection)
        {
            string result = "fail";
            FormFileCollection fileCollection = (FormFileCollection)formCollection.Files;
            
            foreach (IFormFile file in fileCollection)
            {
                StreamReader reader = new StreamReader(file.OpenReadStream());
                String content = reader.ReadToEnd();
                
                String name = file.FileName;
                string ext = Path.GetExtension(name);
                String filename =DateTime.Now.ToString("yyyyMMddhhmmss")+Guid.NewGuid()+ext;
                string allPath = this.uploadDir + filename;
                using (FileStream fs = System.IO.File.Create(allPath))
                {
                    // 复制文件
                    file.CopyTo(fs);
                    // 清空缓冲区数据
                    fs.Flush();
                }
                result = filename;
            }
            dynamic dyObj = new { ret = result };
            return new JsonResult(dyObj);
        }

        public string RecieveFile()
        {
            return null;
        }
    }
}
