using LIMS.Assay.Data.Dto;
using LIMS.Dtos;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace LIMS.Assay.Data
{
    public interface IAssay_DataSearch
    {
        List<Dtos.HtmlSelectDto> GetTemplateHtmlSelectDtosByOrgCode(string input);
        List<Dtos.HtmlSelectDto> GetTemplateHtmlSelectDtosByOrgCodeAndTplQx(string input);
        TemplateInfoDto GetTemplateInfoByTemplateId(int input);
        TemplateInfoDto GetTemplateInfoByTemplateIdAndSpecId(int input,int[] specId);
        List<Dtos.HtmlSelectDto> GetSpecimenHtmlSelectByTemplateId(int input,bool flag);
        HtmlDataOperRetDto GetFormValueBySignId(int signId);
        DataSearchTableDto GetDataInfoByTemplateIdAndSpecId(int input, int[] specId, DateTime begin, DateTime endTime);

        List<Dtos.HtmlSelectDto> GetUserTemplatesByUserId();
        List<MultiTableDataInfoDto> GetMultiTableDataInfoBySpecId(int input, int[] specId, DateTime begin, DateTime endTime);

        string GetExcelNameBySpecIdSinleSheet(int input, int[] specId, DateTime begin, DateTime endTime);
        string GetExcelNameBySpecIdMultiSheet(int input, int[] specId, DateTime begin, DateTime endTime);
        string GetExcelNameByTemplateIdAndSpecId(int input, int[] specId, DateTime begin, DateTime endTime);
        string GetExcelNameBySelfCode(int selfTplId, DateTime begin, DateTime endTime);

        SelfSearchTableDto GetDataInfoBySelfCode(int selfTplId, DateTime begin, DateTime endTime);
    }
}
