﻿using System;
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

        public Assay_UserTpl(IRepository<V_UserTpl, long> repository, IRepository<Template, int> tplRepository,
            IRepository<UserTpl, int> uTplRepository, IRepository<UserTplSpecimens, int> uTplSpecimenRepository)
        {
            this._repository = repository;
            this._tplRepository = tplRepository;
            this._utplRepository = uTplRepository;
            this._uTplSpecimenRepository = uTplSpecimenRepository;
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
            UpdateUserTplSpecimen(input.specimens,input.UserId);
            return _utplRepository.InsertAsync(addUserTpl);

        }
        public Task EditUserTpl(EditVUserTplDto input)
        {
            var editItem = _utplRepository.Single(x => x.Id == input.UserTplId);
            editItem.TplIds = input.TplIds;
            UpdateUserTplSpecimen(input.SpecimenList,input.Id);
            return _utplRepository.UpdateAsync(editItem);
        }
        public Task DeleteUserTplById(int inputId)
        {
            var deleteItem=this._utplRepository.Single(x=>x.Id==inputId);
            deleteItem.TplIds = string.Empty;
            return this._utplRepository.UpdateAsync(deleteItem);
        }

        // 更新用户模板样品
        private void UpdateUserTplSpecimen(List<TplSpecimenDto> specimenList,long userId)
        {
            if (specimenList.Count() > 0)
            {
                // 删除原有的
                var userRep = this._uTplSpecimenRepository.GetAll().Where(x => x.UserId == userId);
                foreach (var item in userRep)
                {
                    this._uTplSpecimenRepository.Delete(item);
                }
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
    }
}
