using Abp.Application.Services;
using Abp.Application.Services.Dto;
using LIMS.Assay.Base.Dto;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace LIMS.Assay.Base
{
    public interface IAssay_ElementAppService : IApplicationService
    {
        Task<PagedResultDto<ElementDto>>   GetElements(PagedResultRequestDto pageQueryDto,string searchName);
        List<Dtos.HtmlSelectDto> GetHtmlSelectElements();
        CreateElementDto GetFirst();
        string UpdateElement(ElementDto input);
        Task DeleteElement(int inputId);
        string AddElement(CreateElementDto input);
    }
}
