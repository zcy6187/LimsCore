using Abp.Application.Services;
using Abp.Application.Services.Dto;
using LIMS.Assay.Base.Dto;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace LIMS.Assay.Base
{
    public interface IAssay_SpecimenAppService:IApplicationService
    {
        Task<PagedResultDto<SpecimenDto>> GetPages(PagedResultRequestDto pageQueryDto,string searchName);
        List<Dtos.HtmlSelectDto> GetHtmlSelectSpecimens();
        string Update(SpecimenDto input);
        Task Delete(int inputId);
        string Add(CreateSpecimenDto input);
    }
}
