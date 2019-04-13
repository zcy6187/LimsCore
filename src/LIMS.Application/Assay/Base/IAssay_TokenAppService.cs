using LIMS.Assay.Base.Dto;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace LIMS.Assay.Base
{
    public interface IAssay_TokenAppService
    {
        // 搜索令牌
        List<EditTplToken> GetTplTokensByConf(SearchTokenDto input);
        // 添加令牌
        string AddTplToken(CreateTplToken input);
        // 删除令牌
        Task DeleteTplToken(int input);
        // 检测token字符串是否已经存在
        bool CheckTplTokenName(string token);
        // 更新令牌
        string EditTplToken(EditTplToken input);
    }
}
