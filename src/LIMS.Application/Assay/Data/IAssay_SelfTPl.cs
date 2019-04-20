using LIMS.Assay.Data.Dto;
using LIMS.Dtos;
using System;
using System.Collections.Generic;
using System.Text;

namespace LIMS.Assay.Data
{
    interface IAssay_SelfTPl
    {
        HtmlDataOperRetDto WriteTplValueToTable(CreateSelfTplDto input);
        List<CreateSelfTplDto> GetTplInfoById();
        HtmlDataOperRetDto DeleteSelfTplById(int tplId);
    }
}
