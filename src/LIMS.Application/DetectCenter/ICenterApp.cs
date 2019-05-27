using LIMS.DetectCenter.Dto;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Text;

namespace LIMS.DetectCenter
{
    public interface ICenterApp
    {
        // ImportRetInfoDto UploadFile([FromForm]CreateMediaDto input);
        ImportRetInfoDto UploadFile(string fileName);
        string DownLoadExcelBySpecId(int input);
    }
}
