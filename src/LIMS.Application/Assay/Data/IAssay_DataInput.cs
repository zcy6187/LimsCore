using LIMS.Assay.Data.Dto;
using System;
using System.Collections.Generic;
using System.Text;

namespace LIMS.Assay.Data
{
    public interface IAssay_DataInput
    {
        TemplateSchemaInputDto GetTemplateSchemaInputDtoByTplId(int tplId, int[] specId);
        Dtos.HtmlDataOperRetDto WriteValueToTable(CreateDataInputDto input);
        TemplateSchemaInputDto GetTemplateSchemaInputDtoBySignId(int signId);
    }
}
