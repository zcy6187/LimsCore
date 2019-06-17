using Abp.AutoMapper;
using Abp.Domain.Repositories;
using LIMS.Assay.Base.Dto;
using LIMS.SysManager;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LIMS.Assay.Base
{
    public class Assay_UserAppService : LIMSAppServiceBase, IAssay_User
    {
        private readonly IRepository<AssayUser, int> _repository;
        private readonly IRepository<OrgInfo, int> _orgRep;
        private readonly IRepository<UserZt, int> _userZtRep;

        public Assay_UserAppService(
            IRepository<AssayUser, int> repository,
            IRepository<OrgInfo, int> orgRep,
            IRepository<UserZt,int> userZtRep)
        {
            this._repository = repository;
            this._orgRep = orgRep;
            this._userZtRep = userZtRep;
        }

        private string GetAliasNameByOrgCode(string inputCode)
        {
            return _orgRep.GetAll().Where(x => x.Code == inputCode).FirstOrDefault().AliasName;
        }

        // 获取帐套的名称
        private string GetZtMc(string ztCode)
        {
            string str = string.Empty;
            switch (ztCode)
            {
                case "00010001":
                    str = "锌业";
                    break;
                case "00010002":
                    str = "股份";
                    break;
                case "00010003":
                    str = "玉川";
                    break;
                default:
                    str = "未知";
                    break;
            }
            return str;
        }

        public string Add(CreateAssayUserDto input)
        {

            int itemCount = _repository.GetAll().Where(x => x.UserName == input.UserName && !x.IsDeleted && x.OrgCode == input.OrgCode).Count();
            if (itemCount > 0)
            {
                return "该名称已存在！";
            }

            AssayUser tmpUser = new AssayUser();
            tmpUser.UserName = input.UserName;
            tmpUser.OrgCode = input.OrgCode;
            tmpUser.OrgName = input.OrgName;
            _repository.InsertAsync(tmpUser);

            return "添加成功!";
        }

        public async Task Delete(int inputId)
        {
            var item = _repository.Single(x => x.Id == inputId);
            item.IsDeleted = true;
            await _repository.UpdateAsync(item);
        }

        public string Update(EditAssayUserDto input)
        {
            int itemCount = _repository.GetAll().Where(x => x.UserName == input.UserName && !x.IsDeleted && x.OrgCode ==input.OrgCode  && x.Id!=input.Id).Count();
            if (itemCount > 0)
            {
                return "该名称已存在！";
            }

            var item = input.MapTo<AssayUser>();

            _repository.UpdateAsync(item);

            return "修改成功!";
        }

        public List<Dtos.HtmlSelectDto> GetHtmlSelectAssayUsers()
        {
            long loginId = AbpSession.UserId ?? 0;
            var userZtCode = this._userZtRep.GetAll().Where(x => x.UserId == loginId).Select(x=>x.ZtCode).ToArray();

            var ztlist = _repository.GetAll().Where(x => !x.IsDeleted && userZtCode.Contains(x.OrgCode)).OrderByDescending(x => x.Id).ToList();
            var list=ztlist.Select(x => new Dtos.HtmlSelectDto()
            {
                Key = x.Id.ToString(),
                Value = x.UserName
            }).ToList();

            return list;
        }

        public List<Dtos.HtmlSelectDto> GetHtmlSelectAssayUsersByOrgCode(string orgCode)
        {
            var ztlist = _repository.GetAll().Where(x => !x.IsDeleted && x.OrgCode==orgCode).OrderByDescending(x => x.Id).ToList();
            var list = ztlist.Select(x => new Dtos.HtmlSelectDto()
            {
                Key = x.Id.ToString(),
                Value = x.UserName
            }).ToList();

            return list;
        }

        public List<EditAssayUserDto> GetAssayOpers(string searchTxt)
        {         
            long loginId = AbpSession.UserId ?? 0;
            var userZt = this._userZtRep.GetAll().Where(x => x.UserId == loginId).Select(x=>x.ZtCode).ToArray();
            var rep = _repository.GetAll().Where(x => !x.IsDeleted && userZt.Contains(x.OrgCode)).ToList();
            if (!string.IsNullOrEmpty(searchTxt))
            {
                rep = rep
                    .Where(x => x.UserName.Contains(searchTxt)).ToList();
            }
            List<EditAssayUserDto> repList = rep.MapTo<List<EditAssayUserDto>>();

            return repList;
        }
    }
}
