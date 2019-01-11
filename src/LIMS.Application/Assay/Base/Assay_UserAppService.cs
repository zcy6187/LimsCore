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

        public string Add(CreateAssayUserDto input)
        {
            string orgCode = "00010002";
            string orgName = "股份";

            long loginId=AbpSession.UserId??0;
            var userZt=this._userZtRep.GetAll().Where(x => x.UserId == loginId).FirstOrDefault();
            if (userZt != null)
            {
                orgCode = userZt.ZtCode;
                orgName = userZt.ZtCode;
            }

            int itemCount = _repository.GetAll().Where(x => x.UserName == input.UserName && !x.IsDeleted && x.OrgCode == orgCode).Count();
            if (itemCount > 0)
            {
                return "该名称已存在！";
            }

            var entity = input.MapTo<AssayUser>();

            entity.OrgCode = orgCode;
            entity.OrgName = orgName;
            _repository.InsertAsync(entity);

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
            string orgCode = "00010002";

            long loginId = AbpSession.UserId ?? 0;
            var userZt = this._userZtRep.GetAll().Where(x => x.UserId == loginId).FirstOrDefault();
            if (userZt != null)
            {
                orgCode = userZt.ZtCode;
            }

            int itemCount = _repository.GetAll().Where(x => x.UserName == input.UserName && !x.IsDeleted && x.OrgCode == orgCode && x.Id!=input.Id).Count();
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
            var userZtCode = this._userZtRep.GetAll().Where(x => x.UserId == loginId).Select(x=>x.ZtCode);

            var list = _repository.GetAll().Where(x => !x.IsDeleted && userZtCode.Contains(x.OrgCode)).Select(x => new Dtos.HtmlSelectDto()
            {
                Key = x.Id.ToString(),
                Value = x.UserName
            }).ToList();

            return list;
        }

        public List<EditAssayUserDto> GetAssayOpers(string searchTxt)
        {
            var rep = _repository.GetAll().Where(x => !x.IsDeleted).ToList();
            if (!string.IsNullOrEmpty(searchTxt))
            {
                rep = _repository.GetAll()
                    .Where(x => !x.IsDeleted && x.UserName.Contains(searchTxt)).ToList();
            }
            List<EditAssayUserDto> repList = rep.MapTo<List<EditAssayUserDto>>();

            return repList;
        }
    }
}
