using Abp.Domain.Repositories;
using Abp.Domain.Uow;
using LIMS.Assay.Statistic.Dto;
using LIMS.SysManager;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LIMS.Assay.Statistic
{
    public class Assay_StatisticAppService : LIMSAppServiceBase, IAssay_Statistic
    {
        private IRepository<TypeIn, int> _typeInRepostitory;
        private IRepository<TypeInItem, int> _typeInItemRepostitory;
        private IRepository<OrgInfo, int> _orgRepostitory;
        private IRepository<Template, int> _tplRepostitory;
        private IRepository<TplSpecimen, int> _tplSpecRepostitory;

        public Assay_StatisticAppService(IRepository<TypeIn, int> typeInRepostitory,
        IRepository<TypeInItem, int> typeInItemRepostitory,
        IRepository<OrgInfo, int> orgRepostitory,
        IRepository<Template, int> tplRepostitory,
        IRepository<TplSpecimen, int> tplSpecRepostitory)
        {
            this._typeInItemRepostitory = typeInItemRepostitory;
            this._typeInRepostitory = typeInRepostitory;
            this._orgRepostitory = orgRepostitory;
            this._tplRepostitory = tplRepostitory;
            this._tplSpecRepostitory = tplSpecRepostitory;
        }

        // 获取单个分厂的数据
        public PlantSummaryDto GetPlantSummary(DateTime beginTime, DateTime endTime, string orgCode)
        {
            // 获取查询的机构下所有的化验模板
            var tplList = _tplRepostitory.GetAll().Where(x => x.OrgCode.StartsWith(orgCode)).ToList();
            var tplIds = tplList.Select(x => x.Id).ToList();
            // 获取模板下的所有样品
            var tplSpecList = _tplSpecRepostitory.GetAll().Where(x => tplIds.Contains(x.TplId)).ToList();
            // 获取所有的输入信息
            var typeInList = _typeInRepostitory.GetAll().Where(x => tplIds.Contains(x.TplId) && x.SignTm >= beginTime && x.SignTm <= endTime).ToList();
            var typeInIds = typeInList.Select(x => x.Id).ToList();
            var typeInItemList = this._typeInItemRepostitory.GetAll().Where(x => typeInIds.Contains(x.TypeInId));
            // 获取模板中所有的Org信息
            var orgList = tplList.GroupBy(x => x.OrgCode).Select(x => x.Key).ToList();
            var orgInfoList = _orgRepostitory.GetAll().Where(x => orgList.Contains(x.Code)).ToList();


            List<SectionSummaryDto> sectionSummaryList = new List<SectionSummaryDto>();
            // 组织信息
            foreach (var item in orgList)
            {
                List<SpecSummaryDto> specList = new List<SpecSummaryDto>();
                // 当前组织下的所有模板
                var tempTplList = tplList.Where(x => x.OrgCode == item).Select(x => x.Id).ToList();
                if (tempTplList.Count == 0)
                {
                    continue;
                }
                // 组织下的所有化验元素
                var tempTypeIn = typeInList.Where(x => tempTplList.Contains(x.TplId)).ToList();
                if (tempTypeIn.Count == 0)
                {
                    continue;
                }
                // 所有化验结果按照样品分类
                var groupSpec = tempTypeIn.GroupBy(x => new { x.SpecId }).Select(x => x.Key.SpecId);
                foreach (var specItem in groupSpec)
                {
                    // 按照样品，获取所有的化验主表Id
                    var tTypeInIds = tempTypeIn.Where(x => x.SpecId == specItem).Select(x => x.Id).ToList();
                    // 获取样品下的所有元素
                    var tTypeInItemList = typeInItemList.Where(x => tTypeInIds.Contains(x.TypeInId) && !string.IsNullOrEmpty(x.EleValue)).ToList();
                    if (tTypeInItemList.Count() == 0)
                    {
                        continue;
                    }
                    string specName = tplSpecList.Where(x => x.Id == specItem).Select(x => x.SpecName).First();
                    // 金银个数
                    int auCount = tTypeInItemList.Where(x => x.EleName == "Au").Count();
                    int agCount = tTypeInItemList.Where(x => x.EleName == "Ag").Count();
                    int auAgCount = auCount > agCount ? auCount : agCount; // 金银取最大值
                                                                          
                    int allCount = tTypeInItemList.Count(); // 全部个数
                    // 普通元素个数
                    int commonCount = allCount - auCount - agCount;
                    // 样品个数
                    int specCount = tTypeInIds.Count();

                    SpecSummaryDto tSpecSummary = new SpecSummaryDto()
                    {
                        SpecName = specName,
                        SpecCount = specCount,
                        AuAg = auAgCount,
                        EleCount = commonCount
                    };
                    specList.Add(tSpecSummary);
                }

                string orgName = orgInfoList.Where(x => x.Code == item).Select(x => x.OrgName).First();
                SectionSummaryDto ssd = new SectionSummaryDto()
                {
                    OrgCode = item,
                    OrgName = orgName,
                    SpecList = specList
                };

                sectionSummaryList.Add(ssd);
            }
            string plantName =_orgRepostitory.GetAll().Where(x => x.Code == orgCode).Select(x => x.OrgName).First();
            PlantSummaryDto p = new PlantSummaryDto()
            {
                OrgName = plantName,
                OrgCode = orgCode,
                SectionList = sectionSummaryList
            };
            return p;
        }

        // 获取任意组织下的数据(分厂一下的，比如工段)
        public List<SectionSummaryDto> GetSectionSummary(DateTime beginTime, DateTime endTime, string orgCode)
        {
            // 公司8位，工厂12位，工段>12位，获取公司的code
            if (orgCode.Length < 12)
            {
                return null;
            }
            // 获取工厂的code
            string plantCode = orgCode.Substring(0, 12);

            // 获取查询的机构下所有的化验模板
            var tplList = _tplRepostitory.GetAll().Where(x => x.OrgCode.StartsWith(orgCode)).ToList();
            var tplIds = tplList.Select(x => x.Id).ToList();
            // 获取模板下的所有样品
            var tplSpecList = _tplSpecRepostitory.GetAll().Where(x => tplIds.Contains(x.TplId)).ToList();
            // 获取所有的输入信息
            var typeInList = _typeInRepostitory.GetAll().Where(x => tplIds.Contains(x.TplId) && x.SignTm >= beginTime && x.SignTm <= endTime).ToList();
            var typeInIds = typeInList.Select(x => x.Id).ToList();
            var typeInItemList = this._typeInItemRepostitory.GetAll().Where(x => typeInIds.Contains(x.TypeInId));
            // 获取模板中所有的Org信息
            var orgList = tplList.GroupBy(x => x.OrgCode).Select(x => x.Key).ToList();
            var orgInfoList = _orgRepostitory.GetAll().Where(x =>orgList.Contains(x.Code)).ToList();


            List<SectionSummaryDto> sectionSummaryList = new List<SectionSummaryDto>();
            // 组织信息
            foreach (var item in orgList)
            {
                List<SpecSummaryDto> specList = new List<SpecSummaryDto>();
                // 当前组织下的所有模板
                var tempTplList = tplList.Where(x => x.OrgCode == item).Select(x => x.Id).ToList();
                if (tempTplList.Count > 0)
                {
                    // 组织下的所有化验元素
                    var tempTypeIn = typeInList.Where(x => tempTplList.Contains(x.TplId)).ToList();
                    if (tempTypeIn.Count > 0)
                    {
                        // 所有化验结果按照样品分类
                        var groupSpec = tempTypeIn.GroupBy(x => new { x.SpecId }).Select(x => x.Key.SpecId);
                        foreach (var specItem in groupSpec)
                        {
                            // 按照样品，获取所有的化验主表Id
                            var tTypeInIds = tempTypeIn.Where(x => x.SpecId == specItem).Select(x => x.Id).ToList();
                            // 获取样品下的所有元素
                            var tTypeInItemList = typeInItemList.Where(x => tTypeInIds.Contains(x.TypeInId) && !string.IsNullOrEmpty(x.EleValue)).ToList();
                            if (tTypeInItemList.Count() > 0)
                            {
                                string specName = tplSpecList.Where(x => x.Id == specItem).Select(x => x.SpecName).First();
                                // 金银个数
                                int auCount = tTypeInItemList.Where(x => x.EleName == "Au").Count();
                                int agCount = tTypeInItemList.Where(x => x.EleName == "Ag").Count();
                                int auAgCount = auCount > agCount ? auCount : agCount; // 金银取最大值
                                                                                       // 全部个数
                                int allCount = tTypeInItemList.Count();
                                // 普通元素个数
                                int commonCount = allCount - auCount - agCount;
                                // 样品个数
                                int specCount = tTypeInIds.Count();

                                SpecSummaryDto tSpecSummary = new SpecSummaryDto()
                                {
                                    SpecName = specName,
                                    SpecCount = specCount,
                                    AuAg = auAgCount,
                                    EleCount = commonCount
                                };
                                specList.Add(tSpecSummary);
                            }
                        }
                    }
                }

                string orgName = orgInfoList.Where(x => x.Code == item).Select(x => x.OrgName).First();
                SectionSummaryDto ssd = new SectionSummaryDto()
                {
                    OrgCode = item,
                    OrgName = orgName,
                    SpecList = specList
                };

                sectionSummaryList.Add(ssd);
            }
            return sectionSummaryList;
        }

        // 获取公司下的所有数据
        public List<PlantSummaryDto> GetCompanySummary(DateTime beginTime, DateTime endTime, string orgCode)
        {
            if (orgCode.Length != 8)
            {
                return null;
            }
            // 获取查询的机构下所有的化验模板
            var tplList = _tplRepostitory.GetAll().Where(x => x.OrgCode.StartsWith(orgCode)).ToList();
            var tplIds = tplList.Select(x => x.Id).ToList();
            var tplSpecList = new List<TplSpecimen>();
            var typeInList = new List<TypeIn>();
            using (UnitOfWorkManager.Current.DisableFilter(AbpDataFilters.SoftDelete))
            {
                // 获取模板下的所有样品
                tplSpecList = _tplSpecRepostitory.GetAll().Where(x => tplIds.Contains(x.TplId)).ToList();
                // 所有输入信息-样品
                typeInList = _typeInRepostitory.GetAll().Where(x => tplIds.Contains(x.TplId) && x.SignTm >= beginTime && x.SignTm <= endTime).ToList();
            }           
            var typeInIds = typeInList.Select(x => x.Id).ToList();
            // 所有输入信息-元素
            var typeInItemList = this._typeInItemRepostitory.GetAll().Where(x => typeInIds.Contains(x.TypeInId));
            // 获取模板中所有的Org信息
            var orgList = tplList.GroupBy(x => new { x.OrgCode }).Select(x => x.Key.OrgCode).ToList();
            var orgInfoList = _orgRepostitory.GetAll().Where(x => orgList.Contains(x.Code)).ToList();
            var compOrgList = _orgRepostitory.GetAll().Where(x => x.Code.StartsWith(orgCode)).ToList();
            // 获取所有的分厂
            var plantOrgList = compOrgList.Where(x => x.Code.Length == 12).ToList();
            List<PlantSummaryDto> plantList = new List<PlantSummaryDto>();
            foreach (var plant in plantOrgList)
            {
                List<SectionSummaryDto> sectionList = new List<SectionSummaryDto>();
                var tmpOrgList = orgList.Where(x => x.StartsWith(plant.Code)).ToList();
                // 组织信息
                foreach (var item in tmpOrgList)
                {
                    // 当前组织下的所有模板
                    var tempTplList = tplList.Where(x => x.OrgCode == item).Select(x => x.Id).ToList();
                    if (tempTplList.Count == 0)
                    {
                        continue;
                    }
                    // 组织下的所有化验元素
                    var tempTypeIn = typeInList.Where(x => tempTplList.Contains(x.TplId)).ToList();
                    if (tempTypeIn.Count == 0)
                    {
                        continue;
                    }
                    // 所有化验结果按照样品分类
                    var groupSpec = tempTypeIn.GroupBy(x => new { x.SpecId }).Select(x => x.Key.SpecId);
                    List<SpecSummaryDto> specList = new List<SpecSummaryDto>();
                    foreach (var specItem in groupSpec)
                    {
                        // 按照样品，获取所有的化验主表Id
                        var tTypeInIds = tempTypeIn.Where(x => x.SpecId == specItem).Select(x => x.Id).ToList();
                        // 获取样品下的所有元素
                        var tTypeInItemList = typeInItemList.Where(x => tTypeInIds.Contains(x.TypeInId) && !string.IsNullOrEmpty(x.EleValue)).ToList();
                        if (tTypeInItemList.Count() > 0)
                        {
                            string specName = tplSpecList.Where(x => x.Id == specItem).Select(x => x.SpecName).First();
                            // 金银个数
                            int auCount = tTypeInItemList.Where(x => x.EleName == "Au").Count();
                            int agCount= tTypeInItemList.Where(x => x.EleName == "Ag").Count();
                            int auAgCount = auCount > agCount ?auCount:agCount; // 金银取最大值
                            // 全部个数
                            int allCount = tTypeInItemList.Count();
                            // 普通元素个数
                            int commonCount = allCount - auCount-agCount;
                            // 样品个数
                            int specCount = tTypeInIds.Count();

                            SpecSummaryDto tSpecSummary = new SpecSummaryDto()
                            {
                                SpecName = specName,
                                SpecCount = specCount,
                                AuAg = auAgCount,
                                EleCount = commonCount
                            };
                            specList.Add(tSpecSummary);
                        }
                    }
                    if (specList.Count > 0)
                    {
                        string orgName = orgInfoList.Where(x => x.Code == item).Select(x => x.OrgName).First();
                        SectionSummaryDto ssd = new SectionSummaryDto()
                        {
                            OrgCode = item,
                            OrgName = orgName,
                            SpecList = specList
                        };
                        sectionList.Add(ssd);
                    }
                }
                if (sectionList.Count > 0)
                {
                    PlantSummaryDto p = new PlantSummaryDto()
                    {
                        OrgCode = plant.Code,
                        OrgName = plant.OrgName,
                        SectionList = sectionList
                    };
                    plantList.Add(p);
                }
            }

            return plantList;
        }

        public string GetExcel(DateTime beginTime, DateTime endTime, string orgCode)
        {
            ExcelStatistic exceler = new ExcelStatistic();
            if (orgCode.Length == 8)
            {
                List<PlantSummaryDto> plantInfo=GetCompanySummary(beginTime, endTime, orgCode);
                return exceler.CreateCompanyStatisticExcel(plantInfo);
            }
            if (orgCode.Length == 12)
            {
                PlantSummaryDto plantInfo=GetPlantSummary(beginTime,endTime,orgCode);
                return exceler.CreateSingleSheetByPlant(plantInfo);
            }
            List<SectionSummaryDto> sectionInfo = GetSectionSummary(beginTime,endTime,orgCode);

            return exceler.CreateSingleSheetBySectionList(sectionInfo);
        }
    }
}
