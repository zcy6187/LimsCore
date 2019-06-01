using System;
using System.Collections.Generic;
using System.Text;

namespace LIMS.DetectCenter.Dto
{
    public class ImportRetInfoDto
    {
        public int code { get; set; } // -2 样品编号不存在，-1 文件类型错误； 0-一切正常；1-标题有错误；2-操作人员有错误；3-各种错误
        public string message { get; set; }
        public List<string> expList { get; set; }
        public List<List<string>> dataList { get; set; }
        public List<string> dataTitle { get; set; }
        public string uploadFileName { get; set; }
    }
}
