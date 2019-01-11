using Abp.Application.Services;
using LIMS.SysManager.ZtManager.Dto;
using System;
using System.Collections.Generic;
using System.Text;

namespace LIMS.SysManager.ZtManager
{
    public interface IZtCodeService : IApplicationService
    {
        List<Dto.EditZtCodeDto> GetAllZtCode();
        Dtos.HtmlDataOperRetDto EditSingleZtCode(Dto.EditZtCodeDto input);
        Dtos.HtmlDataOperRetDto DeleteSingleZtCode(int inputId);
        Dtos.HtmlDataOperRetDto AddSingleZtCode(Dto.CreateZtCodeDto input);

        // 添加用户帐套信息
        void SetUserZt(int userId, List<SetUserZtDto> ztList);
        // 获取帐套信息
        List<SetUserZtDto> GetUserZt(int userId);
    }
}
