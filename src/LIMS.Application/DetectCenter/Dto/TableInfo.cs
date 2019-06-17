using System;
using System.Collections.Generic;
using System.Text;

namespace LIMS.DetectCenter.Dto
{
    public class TableInfoDto
    {
        public List<string> titleList { get; set; }
        public List<ElementTitleInfo> titleInfo { get; set; }
        public List<List<string>> dataList { get; set; } 
    }

    public class ElementTitleInfo
    {
        public int EleId { get; set; }
        public string EleName { get; set; }
        public int EleCount { get; set; }
        public int OperCount { get; set; }
        public int ColumnCount { get; set; }
    }

    public class ModifyTableInfoDto
    {
        public List<string> titleList { get; set; }
        public List<ModifyRowInfoDto> rowList { get; set; }
    }

    public class ModifyRowInfoDto
    {
        public int duplicateId { get; set; }
        public List<string> rowList { get; set; } // 修正样的数据
        public string duplicationInfoStr { get; set; } // 将平行样存储为单个字符串
    }

    public class ModifyItemInfoDto
    {
        public List<string> eleId { get; set; }
        public List<string> eleName { get; set; }
    }

    public class ModifyEditInfoDto
    {
        public int EleId { get; set; }
        public string EleName { get; set; }
        public string DuplicationStr { get; set; }
        public string EleValue { get; set; }
    }

    public class DuplicationEleInfoDto
    {
        public int DupId { get; set; }
        public int EleId { get; set; }
        public string EleName { get; set; }
        public string EleValue { get; set; }
        public int OperId { get; set; }
    }
}
