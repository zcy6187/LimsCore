using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Abp.Domain.Repositories;
using LIMS.Assay.Data.Dto;
using LIMS.Dtos;

namespace LIMS.Assay.Data
{
    public class Assay_SelfTpl : LIMSAppServiceBase, IAssay_SelfTPl
    {
        private IRepository<SelfTpl, int> _selfTplRepostitory;
        private IRepository<Template, int> _tplRepostitory;
        private IRepository<TplSpecimen, int> _tplSpecRepostitory;

        public Assay_SelfTpl(IRepository<SelfTpl, int> selfTplRepostitory, IRepository<Template, int> tplRepostitory, IRepository<TplSpecimen, int> tplSpecRepostitory)
        {
            this._selfTplRepostitory = selfTplRepostitory;
            this._tplRepostitory = tplRepostitory;
            this._tplSpecRepostitory = tplSpecRepostitory;
        }

        // 按照用户id获取其所有的快捷方式
        public List<CreateSelfTplDto> GetTplInfoById()
        {
            long usrId = AbpSession.UserId ?? 0;

            var tpls = this._selfTplRepostitory.GetAll().Where(x => x.userId == usrId && !x.IsDeleted).ToList();

            string tplIds = string.Empty;
            string specIds = string.Empty;
            List<CreateSelfTplDto> selfTplList = new List<CreateSelfTplDto>();
            foreach (var tpl in tpls)
            {
                CreateSelfTplDto tmpSelfTpl = new CreateSelfTplDto();
                tplIds += "," + tpl.tplIds;
                specIds += "," + tpl.tplSpecIds;

                var tplStrArray = tpl.tplIds.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                //int[] tplIntArray = Array.ConvertAll<string, int>(tplStrArray, s => int.Parse(s));

                List<SelfTplDto> selfTpls = new List<SelfTplDto>();
                foreach (var tplId in tplStrArray)
                {
                    SelfTplDto tmpTpl = new SelfTplDto();
                    tmpTpl.tplId = tplId;
                    selfTpls.Add(tmpTpl);
                }

                tmpSelfTpl.id = tpl.Id;
                tmpSelfTpl.selfTpls = selfTpls;
                tmpSelfTpl.tplName = tpl.tplName;

                selfTplList.Add(tmpSelfTpl);

            }

            var tplStrIds = tplIds.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
            var tplIntIds = Array.ConvertAll<string, int>(tplStrIds, s => int.Parse(s));
            var tplSpecStrIds = specIds.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
            var tplSpecIntIds = Array.ConvertAll<string, int>(tplSpecStrIds, s => int.Parse(s));
            var allTemplate = this._tplRepostitory.GetAll().Where(x => tplIntIds.Contains(x.Id)).ToList();
            var allTplSpecimen = this._tplSpecRepostitory.GetAll().Where(x => tplSpecIntIds.Contains(x.Id)).ToList();

            foreach (var item in selfTplList)
            {
                foreach (var tplItem in item.selfTpls)
                {
                    int tplid = int.Parse(tplItem.tplId);
                    tplItem.tplName = allTemplate.Single(x => x.Id == tplid).TplName;
                    var tplSpecimens = allTplSpecimen.Where(x => x.TplId == tplid).OrderBy(x => x.OrderNum).ToList();
                    var specList = new List<KeyValDto>();
                    foreach (var specItem in tplSpecimens)
                    {
                        KeyValDto kv = new KeyValDto();
                        kv.key = specItem.Id.ToString();
                        kv.val = specItem.SpecName;
                        specList.Add(kv);
                    }
                    tplItem.specList = specList;
                }
            }

            return selfTplList;
        }

        public HtmlDataOperRetDto WriteTplValueToTable(CreateSelfTplDto input)
        {
            string tplIds = string.Empty;
            string specIds = string.Empty;
            foreach (var item in input.selfTpls)
            {
                tplIds += item.tplId + ",";
                foreach (var specItem in item.specList)
                {
                    specIds += specItem.key + ",";
                }
            }
            tplIds = tplIds.Trim(',');
            specIds = specIds.Trim(',');

            SelfTpl addSelfTpl = new SelfTpl();
            addSelfTpl.tplIds = tplIds;
            addSelfTpl.tplName = input.tplName;
            addSelfTpl.userId = AbpSession.UserId ?? 0;
            addSelfTpl.tplSpecIds = specIds;
            addSelfTpl.IsDeleted = false;
            this._selfTplRepostitory.Insert(addSelfTpl);

            return
                new HtmlDataOperRetDto()
                {
                    Code = 1,
                    Message = "操作成功"
                };
        }

        public HtmlDataOperRetDto DeleteSelfTplById(int tplId)
        {
            var item = this._selfTplRepostitory.Single(x => x.Id == tplId);
            item.IsDeleted = true;
            this._selfTplRepostitory.Update(item);
            return
                new HtmlDataOperRetDto()
                {
                    Code = 1,
                    Message = "操作成功"
                };
        }
    }
}
