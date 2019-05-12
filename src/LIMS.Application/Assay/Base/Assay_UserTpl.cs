using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Abp.AutoMapper;
using Abp.Domain.Repositories;
using LIMS.Assay.Base.Dto;
using LIMS.SysManager;

namespace LIMS.Assay.Base
{
    public class Assay_UserTpl : LIMSAppServiceBase, IAssay_UserTpl
    {
        private IRepository<V_UserTpl, long> _repository;
        private IRepository<Template, int> _tplRepository;
        private IRepository<UserTpl, int> _utplRepository;
        private IRepository<UserTplSpecimens, int> _uTplSpecimenRepository;
        private IRepository<UserOrg, int> _uUserOrgRepository;
        private IRepository<UserOrgTpl, int> _uUserOrgTplRepository;
        private IRepository<OrgInfo, int> _orgRepository;

        public Assay_UserTpl(IRepository<V_UserTpl, long> repository, IRepository<Template, int> tplRepository,
            IRepository<UserTpl, int> uTplRepository,
            IRepository<UserTplSpecimens, int> uTplSpecimenRepository,
            IRepository<UserOrg, int> uUserOrgRepository,
            IRepository<UserOrgTpl, int> uUserOrgTplRepository,
            IRepository<OrgInfo, int> orgRepository)
        {
            this._repository = repository;
            this._tplRepository = tplRepository;
            this._utplRepository = uTplRepository;
            this._uTplSpecimenRepository = uTplSpecimenRepository;
            this._uUserOrgRepository = uUserOrgRepository;
            this._uUserOrgTplRepository = uUserOrgTplRepository;
            this._orgRepository = orgRepository;
        }

        public List<EditVUserTplDto> SearchUserTpls(string input)
        {
            List<V_UserTpl> userTplList = new List<V_UserTpl>();
            if (string.IsNullOrEmpty(input))
            {
                userTplList = this._repository.GetAll().ToList();
            }
            else
            {
                userTplList = this._repository.GetAll().Where(x => x.UserName.Contains(input) || x.Name.Contains(input)).ToList();
            }

            List<EditVUserTplDto> retList = new List<EditVUserTplDto>();

            if (userTplList.Count() > 0)
            {
                string tplIds = string.Empty;
                foreach (var item in userTplList)
                {
                    if (!string.IsNullOrEmpty(item.TplIds))
                    {
                        tplIds += "," + item.TplIds;
                    }
                }
                List<Template> tplList = new List<Template>();
                // 获取所有用户的化验模板
                if (!string.IsNullOrEmpty(tplIds))
                {
                    string[] tplIdStrList = tplIds.Split(',', StringSplitOptions.RemoveEmptyEntries);
                    int[] tplIdIntList = Array.ConvertAll<string, int>(tplIdStrList, s => int.Parse(s));
                    tplList = _tplRepository.GetAll().Where(x => tplIdIntList.Contains(x.Id)).ToList();
                }

                // 获取用户模板样品表中的数据
                var userIdArray = userTplList.Select(x => x.Id).ToArray();
                var userSpecimenList = _uTplSpecimenRepository.GetAll().Where(x => userIdArray.Contains(x.UserId));

                foreach (var data in userTplList)
                {
                    EditVUserTplDto tempVUserTpl = new EditVUserTplDto();
                    // 生成单个用户的化验模板
                    if (tplList.Count() > 0 && !string.IsNullOrEmpty(data.TplIds))
                    {
                        string[] tempStrList = data.TplIds.Split(',', StringSplitOptions.RemoveEmptyEntries);
                        int[] tempIntList = Array.ConvertAll<string, int>(tempStrList, s => int.Parse(s));
                        var tempTplList = tplList.Where(x => tempIntList.Contains(x.Id)).ToList();
                        string tplNames = string.Empty;
                        List<TplDto> uTplList = new List<TplDto>();
                        foreach (var tpl in tempTplList)
                        {
                            uTplList.Add(new TplDto()
                            {
                                Id = tpl.Id,
                                TplName = tpl.TplName + "-" + tpl.OrgName
                            });
                            tplNames += tpl.TplName + ",";
                        }
                        tempVUserTpl.TplNames = tplNames.TrimEnd(',');
                        tempVUserTpl.TplList = uTplList;
                    }
                    // 生成单个用户的样品信息
                    var specimenList = userSpecimenList.Where(x => x.UserId == data.Id && !x.IsDeleted).ToList();
                    List<TplSpecimenDto> specList = new List<TplSpecimenDto>();
                    if (specimenList.Count > 0)
                    {
                        foreach (var spec in specimenList)
                        {
                            string[] specArray = spec.SpecimenIds.Split(",", StringSplitOptions.RemoveEmptyEntries);
                            List<int> specIdArray = Array.ConvertAll<string, int>(specArray, x => int.Parse(x)).ToList();

                            TplSpecimenDto tempSpec = new TplSpecimenDto()
                            {
                                TplId = spec.TplId,
                                SpecimenIds = specIdArray
                            };
                            specList.Add(tempSpec);
                        }
                    }

                    //其它信息
                    tempVUserTpl.Id = data.Id;
                    tempVUserTpl.UserName = data.UserName;
                    tempVUserTpl.Name = data.Name;
                    tempVUserTpl.Lx = data.Lx;
                    tempVUserTpl.IsDeleted = data.IsDeleted;
                    tempVUserTpl.UserTplId = data.UserTplId;
                    tempVUserTpl.TplIds = data.TplIds;
                    tempVUserTpl.SpecimenList = specList;

                    retList.Add(tempVUserTpl);
                }
            }
            return retList;
        }


        public Task AddUserTpl(CreateUserTplDto input)
        {
            var addUserTpl = input.MapTo<UserTpl>();
            var editItem = _utplRepository.GetAll().Where(x => x.UserId == input.UserId).FirstOrDefault();
            // 找到即更新
            if (editItem != null)
            {
                editItem.TplIds = input.TplIds;
                // 插入样品权限
                UpdateUserTplSpecimen(input.specimens, input.UserId);
                return _utplRepository.UpdateAsync(editItem);
            }
            else
            {
                // 插入样品权限
                UpdateUserTplSpecimen(input.specimens, input.UserId);
                return _utplRepository.InsertAsync(addUserTpl);
            }

        }
        public Task EditUserTpl(EditVUserTplDto input)
        {
            var editItem = _utplRepository.Single(x => x.Id == input.UserTplId);
            editItem.TplIds = input.TplIds;
            // 插入样品权限
            UpdateUserTplSpecimen(input.SpecimenList, input.Id);
            return _utplRepository.UpdateAsync(editItem);
        }
        public Task DeleteUserTplById(int inputId)
        {
            var deleteItem = this._utplRepository.Single(x => x.Id == inputId);
            deleteItem.TplIds = string.Empty;
            return this._utplRepository.UpdateAsync(deleteItem);
        }

        // 获取帐套权限
        public List<string> GetUserOrgIds()
        {
            var userId = AbpSession.UserId ?? 0;

            // 查询 UserOrg
            var userInfo = this._uUserOrgRepository.GetAll().Where(x => x.UserId == userId).SingleOrDefault();
            // 返回空的字符串
            if (userInfo == null)
            {
                List<string> orgList = new List<string>();
                return orgList;
            }
            else
            {
                string orgIds = userInfo.OrgIds;
                string[] orgArray = orgIds.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                return orgArray.ToList();
            }
        }
        // 获取所有化验模板权限
        public List<string> GetUserTplIds()
        {
            var userId = AbpSession.UserId ?? 0;

            // 查询 UserOrg
            var tplInfo = this._utplRepository.GetAll().Where(x => x.UserId == userId).SingleOrDefault();
            // 返回空的字符串
            if (tplInfo == null)
            {
                List<string> tplList = new List<string>();
                return tplList;
            }
            else
            {
                string tplIds = tplInfo.TplIds;
                string[] tplArray = tplIds.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                return tplArray.ToList();
            }
        }
        // 获取用户样品权限
        public List<string> GetUserTplSpecIds(int tplId)
        {
            var userId = AbpSession.UserId ?? 0;

            // 查询 UserOrg
            var specInfo = this._uTplSpecimenRepository.GetAll().Where(x => x.UserId == userId && x.TplId == tplId).SingleOrDefault();
            // 返回空的字符串
            if (specInfo == null)
            {
                List<string> specList = new List<string>();
                return specList;
            }
            else
            {
                string specIds = specInfo.SpecimenIds;
                string[] specArray = specIds.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                return specArray.ToList();
            }
        }

        // 更新整个用户权限
        public Dtos.HtmlDataOperRetDto PostAddOrUpdateUserOrg(UserDataDto input)
        {
            long thisUserId = AbpSession.UserId ?? 0;
            string orgIds = input.OrgIds;
            string tplIds = input.TplIds;
            List<TplSpecDto> specList = input.TplSpecList;

            // 修改UserOrg
            if (!string.IsNullOrEmpty(orgIds))
            {
                var userOrg = _uUserOrgRepository.GetAll().Where(x => x.UserId == thisUserId).SingleOrDefault();
                if (userOrg == null)
                {
                    UserOrg tmpUserOrg = new UserOrg();
                    tmpUserOrg.OrgIds = orgIds;
                    tmpUserOrg.UserId = thisUserId;
                    tmpUserOrg.IsDeleted = false;
                    _uUserOrgRepository.Insert(tmpUserOrg);
                }
                else
                {
                    userOrg.OrgIds = orgIds;
                    _uUserOrgRepository.Update(userOrg);
                }
            }

            // 修改UserTpl
            if (!string.IsNullOrEmpty(tplIds))
            {
                var userTpl = _utplRepository.GetAll().Where(x => x.UserId == thisUserId).SingleOrDefault();
                if (userTpl == null)
                {
                    UserTpl tmpUserTpl = new UserTpl();
                    tmpUserTpl.UserId = thisUserId;
                    tmpUserTpl.TplIds = tplIds;
                    tmpUserTpl.IsDeleted = false;
                    _utplRepository.Insert(tmpUserTpl);
                }
                else
                {
                    userTpl.TplIds = tplIds;
                    _utplRepository.Update(userTpl);
                }
            }


            // 修改UserTplSpec
            if (specList != null && specList.Count > 0)
            {
                var tplSpecList = _uTplSpecimenRepository.GetAll().Where(x => x.UserId == thisUserId).ToList();
                foreach (var specItem in specList)
                {
                    var tmpUserTplSpec = tplSpecList.Where(x => x.TplId == int.Parse(specItem.TplId)).SingleOrDefault();
                    if (tmpUserTplSpec != null)
                    {
                        tmpUserTplSpec.SpecimenIds = specItem.SpecIds;
                        _uTplSpecimenRepository.Update(tmpUserTplSpec);
                    }
                    else
                    {
                        UserTplSpecimens insertSpec = new UserTplSpecimens();
                        insertSpec.UserId = thisUserId;
                        insertSpec.TplId = int.Parse(specItem.TplId);
                        insertSpec.SpecimenIds = specItem.SpecIds;
                        insertSpec.IsDeleted = false;
                        _uTplSpecimenRepository.Insert(insertSpec);
                    }
                }
            }

            return new Dtos.HtmlDataOperRetDto()
            {
                Code = 1,
                Message = "操作成功!"
            };
        }

        // 更新或者保存样品
        public Dtos.HtmlDataOperRetDto PostAddOrUpdateSingleTplSpec(TplSpecDto specItem)
        {
            long userId = AbpSession.UserId ?? 0;
            var tplSpecList = _uTplSpecimenRepository.GetAll().Where(x => x.UserId == userId).ToList();

            var tmpUserTplSpec = tplSpecList.Where(x => x.TplId == int.Parse(specItem.TplId)).SingleOrDefault();
            if (tmpUserTplSpec != null)
            {
                tmpUserTplSpec.SpecimenIds = specItem.SpecIds;
                _uTplSpecimenRepository.Update(tmpUserTplSpec);
            }
            else
            {
                UserTplSpecimens insertSpec = new UserTplSpecimens();
                insertSpec.UserId = userId;
                insertSpec.TplId = int.Parse(specItem.TplId);
                insertSpec.SpecimenIds = specItem.SpecIds;
                insertSpec.IsDeleted = false;
                _uTplSpecimenRepository.Insert(insertSpec);
            }

            return new Dtos.HtmlDataOperRetDto()
            {
                Code = 1,
                Message = "操作成功!"
            };
        }

        // 更新用户模板样品
        private void UpdateUserTplSpecimen(List<TplSpecimenDto> specimenList, long userId)
        {
            // 删除原有的样品信息
            var userRep = this._uTplSpecimenRepository.GetAll().Where(x => x.UserId == userId);
            foreach (var item in userRep)
            {
                this._uTplSpecimenRepository.Delete(item);
            }
            // 如果specimenList.Count()为空，则证明没有样品权限，不处理即可
            if (specimenList.Count() > 0)
            {
                // 添加新的
                foreach (var item in specimenList)
                {
                    UserTplSpecimens utpl = new UserTplSpecimens();
                    utpl.UserId = userId;
                    utpl.TplId = item.TplId;
                    utpl.SpecimenIds = string.Join(",", item.SpecimenIds);
                    utpl.IsDeleted = false;
                    this._uTplSpecimenRepository.Insert(utpl);
                }
            }

        }

        // 删除所有
        private void DeleteTplSpecimenByUserId(long userId)
        {
            // 删除原有的
            var userRep = this._uTplSpecimenRepository.GetAll().Where(x => x.UserId == userId);
            foreach (var item in userRep)
            {
                this._uTplSpecimenRepository.Delete(item);
            }

        }

        // 更新模板元素ids
        public Dtos.HtmlDataOperRetDto PostAddOrUpdateTplSpecByTplId(int tplId, string specIds)
        {
            long uid = this.AbpSession.UserId ?? 0;
            var item = this._uTplSpecimenRepository.GetAll().Where(x => x.UserId == uid && x.TplId == tplId).SingleOrDefault();
            if (item == null)
            {
                if (specIds != "-1")
                {
                    UserTplSpecimens tmpItem = new UserTplSpecimens();
                    tmpItem.UserId = uid;
                    tmpItem.TplId = tplId;
                    tmpItem.SpecimenIds = specIds;
                    tmpItem.IsDeleted = false;
                    this._uTplSpecimenRepository.Insert(tmpItem);
                }
            }
            else
            {
                if (specIds != "-1")
                {
                    item.SpecimenIds = specIds;
                    this._uTplSpecimenRepository.Update(item);
                }
                else
                {
                    this._uTplSpecimenRepository.Delete(item);
                }
            }

            return new Dtos.HtmlDataOperRetDto()
            {
                Code = 1,
                Message = "操作成功!"
            };
        }

        // 更改账户信息
        public Dtos.HtmlDataOperRetDto PostAddOrUpdateUserOrgs(string orgIds)
        {
            long uid = this.AbpSession.UserId ?? 0;
            var item = this._uUserOrgRepository.GetAll().Where(x => x.UserId == uid).SingleOrDefault();
            if (item == null)
            {
                UserOrg tmpItem = new UserOrg();
                tmpItem.UserId = uid;
                tmpItem.OrgIds = orgIds;
                tmpItem.IsDeleted = false;
                this._uUserOrgRepository.Insert(tmpItem);
            }
            else
            {
                item.OrgIds = orgIds;
                this._uUserOrgRepository.Update(item);
            }

            return new Dtos.HtmlDataOperRetDto()
            {
                Code = 1,
                Message = "操作成功!"
            };
        }

        // 更新OrgTpls
        public Dtos.HtmlDataOperRetDto PostAddOrUpdateOrgTpls(string orgId, string tplIds)
        {
            long uid = this.AbpSession.UserId ?? 0;
            var items = this._uUserOrgTplRepository.GetAll().Where(x => x.UserId == uid).ToList();
            if (items == null || items.Count == 0)
            {
                if (tplIds != "-1")
                {
                    AddNewOrgTpls(uid, orgId, tplIds);
                }
            }
            else
            {
                var findTmp = items.Where(x => x.OrgId == orgId).SingleOrDefault();
                if (findTmp == null)
                {
                    if (tplIds != "-1")
                    {
                        AddNewOrgTpls(uid, orgId, tplIds);
                    }

                }
                else
                {
                    if (tplIds != "-1") // -1表示全选
                    {
                        findTmp.TplIds = tplIds;
                        this._uUserOrgTplRepository.Update(findTmp);
                    }
                    else
                    {
                        this._uUserOrgTplRepository.Delete(findTmp);
                    }
                }

            }

            return new Dtos.HtmlDataOperRetDto()
            {
                Code = 1,
                Message = "操作成功!"
            };
        }

        private void AddNewOrgTpls(long uid, string orgId, string tplIds)
        {
            UserOrgTpl tmpItem = new UserOrgTpl();
            tmpItem.UserId = uid;
            tmpItem.OrgId = orgId;
            tmpItem.TplIds = tplIds;
            tmpItem.IsDeleted = false;
            this._uUserOrgTplRepository.Insert(tmpItem);
        }

        // 按组织ID获取化验模板
        public List<string> GetUserTplIdsByOrgId(int orgId)
        {
            var userId = AbpSession.UserId ?? 0;

            string orgCode = this._orgRepository.Single(x => x.Id == orgId).Code;

            // 查询 UserOrg
            var tplInfo = this._uUserOrgTplRepository.GetAll().Where(x => x.UserId == userId && x.OrgId == orgCode).SingleOrDefault();
            // 返回空的字符串
            if (tplInfo == null)
            {
                List<string> tplList = new List<string>();
                return tplList;
            }
            else
            {
                string tplIds = tplInfo.TplIds;
                string[] tplArray = tplIds.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                return tplArray.ToList();
            }
        }
        // 按组织Code获取化验模板
        public List<string> GetUserTplIdsByOrgCode(string orgCode)
        {
            var userId = AbpSession.UserId ?? 0;

            // 查询 UserOrg
            var tplInfo = this._uUserOrgTplRepository.GetAll().Where(x => x.UserId == userId && x.OrgId == orgCode).SingleOrDefault();
            // 返回空的字符串
            if (tplInfo == null)
            {
                List<string> tplList = new List<string>();
                return tplList;
            }
            else
            {
                string tplIds = tplInfo.TplIds;
                string[] tplArray = tplIds.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                return tplArray.ToList();
            }
        }
    }
}
