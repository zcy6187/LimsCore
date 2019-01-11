using LIMS.Assay.Data.Dto;
using LIMS.Dtos;
using System;
using System.Collections.Generic;
using System.Text;

namespace LIMS.Assay.Data
{
    public interface IAssay_DataSearch
    {
        List<Dtos.HtmlSelectDto> GetTemplateHtmlSelectDtosByOrgCode(string input);
        TemplateInfoDto GetTemplateInfoByTemplateId(int input);
        TemplateInfoDto GetTemplateInfoByTemplateIdAndSpecId(int input,int[] specId);
        List<Dtos.HtmlSelectDto> GetSpecimenHtmlSelectByTemplateId(int input);
        HtmlDataOperRetDto GetFormValueBySignId(int signId);
        DataSearchTableDto GetDataInfoByTemplateIdAndSpecId(int input, int[] specId, DateTime begin, DateTime endTime);
    }
}
