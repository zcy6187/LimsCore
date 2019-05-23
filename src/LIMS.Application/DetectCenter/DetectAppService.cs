using LIMS.DetectCenter.Dto;
using Microsoft.AspNetCore.Mvc;
using MimeDetective.Extensions;
using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace LIMS.DetectCenter
{
    public class DetectAppService: LIMSAppServiceBase, ICenterApp
    {
        public Dtos.HtmlDataOperRetDto UploadFile([FromForm]CreateMediaDto input)
        {
            string dir = "D:\\Ftp\\www\\upload";
            var stream = input.File.OpenReadStream();
            string fileName = Guid.NewGuid().ToString();
            string ext= stream.GetFileType().Extension;
            using (var fileStream = new FileStream($"{dir}\\{fileName}.{ext}", FileMode.Create))
            {
                IWorkbook wk;
                stream.CopyTo(fileStream);
                if (ext == ".xlsx" || ext == ".xls")
                {
                    //判断excel的版本
                    if (ext == ".xlsx")
                    {
                        wk = new XSSFWorkbook(fileStream);
                    }
                    else
                    {
                        wk = new HSSFWorkbook(fileStream);
                    }
                    ISheet sheet = wk.GetSheetAt(0);

                    IRow row0=sheet.GetRow(0);
                    ICell titleCell0=row0.GetCell(1);

                }
                fileStream.Close();
            }
            return new Dtos.HtmlDataOperRetDto()
            {
                Code = 1,
                Message = "操作成功！"
            };
        }
    }
}
