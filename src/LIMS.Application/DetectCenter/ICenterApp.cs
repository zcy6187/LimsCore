using LIMS.DetectCenter.Dto;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Text;

namespace LIMS.DetectCenter
{
    public interface ICenterApp
    {
        Dtos.HtmlDataOperRetDto UploadFile([FromForm]CreateMediaDto input);
    }
}
