﻿using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Text;

namespace LIMS.DetectCenter.Dto
{
    public class CreateMediaDto
    {
        public IFormFile File { get; set; }
    }
}
