using Abp.Application.Services;
using Abp.Application.Services.Dto;
using LIMS.Assay.Base.Dto;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace LIMS.Assay.Base
{
    public interface IAssay_UnitAppService : IApplicationService
    {

        Task<PagedResultDto<UnitDto>> GetPages(PagedResultRequestDto pageQueryDto);
        List<Dtos.HtmlSelectDto> GetHtmlSelectUnits();
        Task Update(UnitDto input);
        Task Delete(int inputId);
        Task Add(CreateUnitDto input);
    }
}
