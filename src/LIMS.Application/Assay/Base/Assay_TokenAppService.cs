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
    public class Assay_TokenAppService : LIMSAppServiceBase, IAssay_TokenAppService
    {
        private IRepository<Token, int> _repository;
        private IRepository<Template, int> _tplRepository;
        private IRepository<SysManager.OrgInfo, int> _orgRepository;
        public Assay_TokenAppService(IRepository<Token, int> repository,IRepository<Template,int> tplRepository,
            IRepository<SysManager.OrgInfo, int> orgRepository)
        {
            this._repository = repository;
            this._tplRepository = tplRepository;
            this._orgRepository = orgRepository;
        }

        public string AddTplToken(CreateTplToken input)
        {
            Token token=input.MapTo<Assay.Token>();
            int numCount=this._repository.Count(x => x.CmdToken == input.CmdToken);
            if (numCount > 0)
            {
                return "该令牌已存在!";
            }
            string orgName=_orgRepository.Single(x => x.Code == input.OrgCode).AliasName;
            token.OrgName = orgName;
            this._repository.Insert(token);
            return "添加成功!";
        }

        public bool CheckTplTokenName(string token)
        {
            int numCount = this._repository.Count(x => x.CmdToken == token);
            if (numCount > 0)
            {
                return false;
            }
            return true;
        }

        public Task DeleteTplToken(EditTplToken input)
        {
            Token tt=this._repository.Single(x=>x.Id==input.Id);
            tt.IsDeleted = true;
            return this._repository.UpdateAsync(tt);
        }

        public List<EditTplToken> GetTplTokensByConf(SearchTokenDto input)
        {
            List<Token> tokenList;
            if (!string.IsNullOrEmpty(input.Token))
            {
                tokenList = this._repository.GetAll().Where(x => x.CmdToken.Contains(input.Token)).ToList();
            }
            else
            {
                tokenList = this._repository.GetAll().Where(x => x.OrgCode.Contains(input.OrgCode)).ToList();
            }
            

            if (tokenList.Count() > 0)
            {
                // 获取令牌对应的所有模板
                string tplIds = string.Empty;
                foreach (var item in tokenList)
                {
                    tplIds += "," + item.TplIds;
                }
                string[] tplIdStrList = tplIds.Split(',',StringSplitOptions.RemoveEmptyEntries);
                int[] tplIdIntList = Array.ConvertAll<string, int>(tplIdStrList, s => int.Parse(s));
                var tplList=_tplRepository.GetAll().Where(x => tplIdIntList.Contains(x.Id)).ToList();

                List<EditTplToken> retList = new List<EditTplToken>();
                //将令牌的模板ID，转换成模板名称
                foreach (var item in tokenList)
                {
                    string[] tempStrList=item.TplIds.Split(',', StringSplitOptions.RemoveEmptyEntries);
                    int[] tempIntList= Array.ConvertAll<string, int>(tempStrList, s => int.Parse(s));
                    var tempTplList=tplList.Where(x => tempIntList.Contains(x.Id)).ToList();

                    EditTplToken tempToken = new EditTplToken();
                    
                    tempToken.TplIds = item.TplIds;
                    tempToken.Id = item.Id;
                    tempToken.OrgCode = item.OrgCode;
                    tempToken.OrgName = item.OrgName;
                    tempToken.CmdToken = item.CmdToken;
                    tempToken.PhoneNumber = item.PhoneNumber;
                    tempToken.Contracter = item.Contracter;

                    List<TokenTplDto> tokenTplList = new List<TokenTplDto>();
                    string tplNames = string.Empty;
                    foreach (var data in tempTplList)
                    {
                        tokenTplList.Add(new TokenTplDto()
                        {
                            Id=data.Id,
                            TplName=data.TplName
                        });
                        tplNames += data.TplName+",";
                    }
                    tempToken.TokenTplList = tokenTplList;
                    tempToken.TplNames = tplNames.TrimEnd(',');

                    retList.Add(tempToken);
                }

                return retList;

            }
            return null;
        }

        public string EditTplToken(EditTplToken input)
        {
            Token token = this._repository.Single(x=>x.Id==input.Id);
            int numCount = this._repository.Count(x => x.CmdToken == input.CmdToken && x.Id!=input.Id);
            if (numCount > 0)
            {
                return "该令牌已存在!";
            }
            string orgName = _orgRepository.Single(x => x.Code == input.OrgCode).AliasName;
            token.OrgCode = input.OrgCode;
            token.OrgName = orgName;
            token.TplIds = input.TplIds;
            token.CmdToken = input.CmdToken;
            token.Contracter = input.Contracter;
            token.PhoneNumber = input.PhoneNumber;
            this._repository.Update(token);
            return "修改成功!";

        }
    }
}
