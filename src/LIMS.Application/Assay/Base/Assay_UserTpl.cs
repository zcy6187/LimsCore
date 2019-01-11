using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Abp.AutoMapper;
using Abp.Domain.Repositories;
using LIMS.Assay.Base.Dto;

namespace LIMS.Assay.Base
{
    public class Assay_UserTpl : LIMSAppServiceBase, IAssay_UserTpl
    {
        private IRepository<V_UserTpl, long> _repository;
        private IRepository<Template, int> _tplRepository;
        private IRepository<UserTpl, int> _utplRepository;

        public Assay_UserTpl(IRepository<V_UserTpl, long> repository, IRepository<Template, int> tplRepository, IRepository<UserTpl, int> uTplRepository)
        {
            this._repository = repository;
            this._tplRepository = tplRepository;
            this._utplRepository = uTplRepository;
        }

        public List<EditVUserTplDto> SearchUserTpls(string input)
        {
            List<V_UserTpl> userTplList = new List<V_UserTpl>();
            if (string.IsNullOrEmpty(input))
            {
                userTplList = this._repository.GetAllList();
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
                if (!string.IsNullOrEmpty(tplIds))
                {
                    string[] tplIdStrList = tplIds.Split(',', StringSplitOptions.RemoveEmptyEntries);
                    int[] tplIdIntList = Array.ConvertAll<string, int>(tplIdStrList, s => int.Parse(s));
                    tplList = _tplRepository.GetAll().Where(x => tplIdIntList.Contains(x.Id)).ToList();
                }

                foreach (var data in userTplList)
                {
                    EditVUserTplDto tempVUserTpl = new EditVUserTplDto();
                    if (tplList.Count() >  0 && !string.IsNullOrEmpty(data.TplIds))
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
                                TplName = tpl.TplName
                            });
                            tplNames += tpl.TplName + ",";
                        }
                        tempVUserTpl.TplNames = tplNames.TrimEnd(',');
                        tempVUserTpl.TplList = uTplList;
                    }


                    tempVUserTpl.Id = data.Id;
                    tempVUserTpl.UserName = data.UserName;
                    tempVUserTpl.Name = data.Name;
                    tempVUserTpl.Lx = data.Lx;
                    tempVUserTpl.IsDeleted = data.IsDeleted;
                    tempVUserTpl.UserTplId = data.UserTplId;
                    tempVUserTpl.TplIds = data.TplIds;

                    retList.Add(tempVUserTpl);

                }
            }
            return retList;
        }


        public Task AddUserTpl(CreateUserTplDto input)
        {
            var addUserTpl=input.MapTo<UserTpl>();
            return _utplRepository.InsertAsync(addUserTpl);

        }
        public Task EditUserTpl(EditVUserTplDto input)
        {
            var editItem=_utplRepository.Single(x => x.Id == input.UserTplId);
            editItem.TplIds = input.TplIds;

            return _utplRepository.UpdateAsync(editItem);
        }
    }
}
