using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Abp.AutoMapper;
using Abp.Domain.Repositories;
using Abp.Domain.Uow;
using LIMS.Dtos;
using LIMS.SysManager.ZtManager.Dto;

namespace LIMS.SysManager.ZtManager
{
    public class ZtCodeAppService : IZtCodeService
    {
        private readonly IRepository<SysManager.ZtCode, int> _ztRep;
        private readonly IRepository<SysManager.UserZt, int> _userZtRep;

        public ZtCodeAppService(IRepository<SysManager.ZtCode, int> ztRep)
        {
            this._ztRep = ztRep;
        }

        public HtmlDataOperRetDto AddSingleZtCode(CreateZtCodeDto input)
        {
            ZtCode addZtCode=input.MapTo<ZtCode>();
            int tempId=_ztRep.InsertAndGetId(addZtCode);
            var ret = new HtmlDataOperRetDto();
            ret.Code = tempId;
            if (tempId > 0)
            {
                ret.Message = "添加成功!";
            }
            else
            {
                ret.Message = "添加失败";
            }
            return ret;
        }

        public HtmlDataOperRetDto DeleteSingleZtCode(int inputId)
        {
            var ztItem=_ztRep.Single(x=>x.Id==inputId);
            ztItem.IsDeleted = true;
            _ztRep.Update(ztItem);

            return new HtmlDataOperRetDto()
            {
                Code = 1,
                Message = "删除成功！"
            };
        }

        public HtmlDataOperRetDto EditSingleZtCode(Dto.EditZtCodeDto input)
        {
            ZtCode inputZtCode = input.MapTo<ZtCode>();
            _ztRep.Update(inputZtCode);
            var ret = new HtmlDataOperRetDto();
            ret.Code = 1;
            ret.Message = "更新成功！";
            return ret;
        }

        public List<EditZtCodeDto> GetAllZtCode()
        {
            var ret = _ztRep.GetAll().Where(x=>!x.IsDeleted).MapTo<List<EditZtCodeDto>>();
            return ret;
        }

        // 设置用户帐套
        [UnitOfWork]
        public void SetUserZt(int userId,List<SetUserZtDto> ztList)
        {
            // 批量刪除
            var ztUserList=_userZtRep.GetAll().Where(x => x.UserId == userId);
            foreach (var userItem in ztUserList)
            {
                _userZtRep.Delete(userItem);
            }
            // 批量添加
            foreach (var ztItem in ztList)
            {
                _userZtRep.Insert(new UserZt()
                {
                    ZtCode=ztItem.ZtCode,
                    ZtId=ztItem.ZtId,
                    UserId=userId
                });
            }
        }

        public List<SetUserZtDto> GetUserZt(int userId)
        {
            List<SetUserZtDto> userZtList = new List<SetUserZtDto>();
            var ztUserList = _userZtRep.GetAll().Where(x => x.UserId == userId).ToList();
            foreach (var item in ztUserList)
            {
                userZtList.Add(new SetUserZtDto()
                {
                    ZtCode = item.ZtCode,
                    ZtId = item.ZtId
                });
            }

            return userZtList;
        }
    }
}
