using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace LearnWebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ExcelController : ControllerBase
    {
        private string downLoadDir = "D:\\Ftp\\www\\Excels\\";

        public FileResult GetExcel(string fileName)
        {
            string filePath = downLoadDir+fileName;

            using (FileStream fs = new FileStream(filePath, FileMode.Open, FileAccess.Read))
            {
                int fsLength = (int)fs.Length;
                byte[] bytes = new byte[fsLength];
                fs.Read(bytes, 0, fsLength);
                return File(bytes, "application/vnd.ms-excel", fileName);
            }
        }
    }
}