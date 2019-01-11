using LIMS.Assay.Base.Dto;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace LIMS.Assay.Base
{
    public interface IAssay_User
    {
        // Task<PagedResultDto<UnitDto>> GetPages(PagedResultRequestDto pageQueryDto);
        List<Dtos.HtmlSelectDto> GetHtmlSelectAssayUsers();
        string Update(EditAssayUserDto input);
        Task Delete(int inputId);
        string Add(CreateAssayUserDto input);
        List<EditAssayUserDto> GetAssayOpers(string searchTxt);
    }
}
