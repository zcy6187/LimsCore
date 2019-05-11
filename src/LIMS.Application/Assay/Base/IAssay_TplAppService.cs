using LIMS.Assay.Base.Dto;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace LIMS.Assay.Base
{
    public interface IAssay_TplAppService
    {
        string AddTpl(CreateTplDto input);
        Task DelTpl(int inputId);
        List<EditTplDto> GetTplsByOrgCode(string inputCode);
        List<EditTplDto> GetTpls(SearchTplDto input);
        EditTplDto GetSingleTpl(int inputId);
        string EditTpl(EditTplDto input);

        string AddTplElement(CreateTplElementDto input);
        Task DeleteTplElement(int inputId);
        string EditTplElement(EditTplElementDto input);
        List<EditTplElementDto> GetTplElementsByTplSpecimenId(int inputId);
        string ReOrderTplElement(List<ReOrderDto> inputList);

        string AddTplSpecimen(CreateTplSpecimenDto input);
        Task DeleteTplSpecimen(int inputId);
        string EditTplSpecimen(EditTplSpecimenDto input);
        List<EditTplSpecimenDto> GetTplSpecimensByTplId(int inputId);
        string ReOrderTplSpecimen(List<ReOrderDto> input);
        List<EditTplDto> GetTplsByOrgId(int orgId);
    }
}
