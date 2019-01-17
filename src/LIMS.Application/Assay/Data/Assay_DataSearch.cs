﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Abp.Domain.Repositories;
using LIMS.Assay.Data.Dto;
using LIMS.Dtos;
using Newtonsoft.Json.Linq;

namespace LIMS.Assay.Data
{
    public class Assay_DataSearch : LIMSAppServiceBase, IAssay_DataSearch
    {
        private IRepository<Template, int> _tplRepostitory;
        private IRepository<TplSpecimen, int> _tplSpecRepostitory;
        private IRepository<TplElement, int> _tplEleRepostitory;
        private IRepository<TypeInItem, int> _typeInItemRepository;
        private IRepository<TypeIn, int> _typeInRepository;

        public Assay_DataSearch(IRepository<Template, int> tplRepostitory,
            IRepository<TplSpecimen, int> tplSpecRepostitory,
            IRepository<TplElement, int> tplEleRepostitory,
            IRepository<TypeInItem, int> typeInItemRepository,
            IRepository<TypeIn, int> typeInRepository
            )
        {
            this._tplRepostitory = tplRepostitory;
            this._tplEleRepostitory = tplEleRepostitory;
            this._tplSpecRepostitory = tplSpecRepostitory;

            this._typeInItemRepository = typeInItemRepository;
            this._typeInRepository = typeInRepository;
        }

        public List<HtmlSelectDto> GetTemplateHtmlSelectDtosByOrgCode(string input)
        {
            var retList = _tplRepostitory.GetAll().Where(x => x.OrgCode.StartsWith(input)).Select(x => new Dtos.HtmlSelectDto()
            {
                Key = x.Id.ToString(),
                Value = x.TplName.ToString()
            }).ToList();

            return retList;
        }

        public TemplateInfoDto GetTemplateInfoByTemplateId(int input)
        {
            var item = _tplRepostitory.Single(x => x.Id == input);
            if (item != null)
            {
                BaseInfoDto tplBaseInfo = new BaseInfoDto()
                {
                    Id = item.Id,
                    Name = item.TplName,
                    Count = 1
                };
                var specList = _tplSpecRepostitory.GetAll().Where(x => x.TplId == item.Id && x.IsDeleted == false).OrderBy(x => x.OrderNum).ToList();
                var specIdList = specList.Select(x => x.Id).ToArray();
                var eleList = _tplEleRepostitory.GetAll().Where(x => specIdList.Contains(x.TplSpecId) && x.IsDeleted == false)
                    .OrderBy(x=>x.TplSpecId).ToList();

                List<BaseInfoDto> specInfoList = new List<BaseInfoDto>();
                List<BaseInfoDto> eleInfoList = new List<BaseInfoDto>();

                foreach (var specItem in specList)
                {
                    var tempEleList = eleList.Where(x => x.TplSpecId == specItem.Id).Select(x => new BaseInfoDto()
                    {
                        Id = x.Id,
                        Name = x.ElementName+ (String.IsNullOrEmpty(x.UnitName)?string.Empty:"("+x.UnitName+")"),
                        Count = 1,
                        OrderNum=x.OrderNo
                    }).OrderBy(x=>x.OrderNum).ToList();
                    BaseInfoDto tempSpecInfo = new BaseInfoDto()
                    {
                        Id = specItem.Id,
                        Name = specItem.SpecName,
                        Count = tempEleList.Count() == 0 ? 1 : tempEleList.Count()
                    };
                    specInfoList.Add(tempSpecInfo);
                    eleInfoList.InsertRange(eleInfoList.Count(),tempEleList);
                }

                TemplateInfoDto tplInfoDto = new TemplateInfoDto()
                {
                    Template = tplBaseInfo,
                    Specimens = specInfoList,
                    Elements = eleInfoList,
                };
                return tplInfoDto;

            }
            return null;
        }
        public TemplateInfoDto GetTemplateInfoByTemplateIdAndSpecId(int input, int[] specId)
        {
            var item = _tplRepostitory.Single(x => x.Id == input);
            if (item != null)
            {
                BaseInfoDto tplBaseInfo = new BaseInfoDto()
                {
                    Id = item.Id,
                    Name = item.TplName,
                    Count = 1
                };
                List<TplSpecimen> specList = new List<TplSpecimen>();
                if (specId.Contains(-1) || specId.Length == 0)
                {
                    specList = _tplSpecRepostitory.GetAll().Where(x => x.TplId == item.Id && x.IsDeleted == false).OrderBy(x => x.OrderNum).ToList();
                }
                else
                {
                    specList = _tplSpecRepostitory.GetAll().Where(x => x.TplId == item.Id && x.IsDeleted == false && specId.Contains(x.Id)).OrderBy(x => x.OrderNum).ToList();
                }
                    
                var specIdList = specList.Select(x => x.Id).ToArray();
                var eleList = _tplEleRepostitory.GetAll().Where(x => specIdList.Contains(x.TplSpecId) && x.IsDeleted == false)
                    .OrderBy(x => x.TplSpecId).ToList();

                List<BaseInfoDto> specInfoList = new List<BaseInfoDto>();
                List<BaseInfoDto> eleInfoList = new List<BaseInfoDto>();

                foreach (var specItem in specList)
                {
                    var tempEleList = eleList.Where(x => x.TplSpecId == specItem.Id).Select(x => new BaseInfoDto()
                    {
                        Id = x.Id,
                        Name = x.ElementName + (String.IsNullOrEmpty(x.UnitName) ? string.Empty : "(" + x.UnitName + ")"),
                        Count = 1,
                        OrderNum = x.OrderNo
                    }).OrderBy(x => x.OrderNum).ToList();
                    BaseInfoDto tempSpecInfo = new BaseInfoDto()
                    {
                        Id = specItem.Id,
                        Name = specItem.SpecName,
                        Count = tempEleList.Count() == 0 ? 1 : tempEleList.Count()
                    };
                    specInfoList.Add(tempSpecInfo);
                    eleInfoList.InsertRange(eleInfoList.Count(), tempEleList);
                }

                TemplateInfoDto tplInfoDto = new TemplateInfoDto()
                {
                    Template = tplBaseInfo,
                    Specimens = specInfoList,
                    Elements = eleInfoList,
                };
                return tplInfoDto;

            }
            return null;
        }

        public List<Dtos.HtmlSelectDto> GetSpecimenHtmlSelectByTemplateId(int input)
        {
            var retList = this._tplSpecRepostitory.GetAll().Where(x => x.IsDeleted == false && x.TplId == input).Select(x => new Dtos.HtmlSelectDto()
            {
                Key=x.Id.ToString(),
                Value=x.SpecName.ToString()
            }).ToList();

            retList.Insert(0, new HtmlSelectDto()
            {
                Key="-1",
                Value="全部样品"
            });

            return retList;
        }

        // 根据签到Id获取数据
        public HtmlDataOperRetDto GetFormValueBySignId(int signId)
        {
            var typeItem=_typeInRepository.FirstOrDefault(x => !x.IsDeleted && x.SignId == signId.ToString());
            if (typeItem != null)
            {
                var typeItemList=_typeInItemRepository.GetAll().Where(x => x.TypeInId == typeItem.Id).ToList();
                if (typeItemList.Count() > 0)
                {
                    JObject specObj = new JObject();
                    specObj.Add("signId",signId.ToString());
                    specObj.Add("typeSpecId", typeItem.Id.ToString());
                    foreach (var item in typeItemList)
                    {
                        JObject eleObj = new JObject();
                        eleObj.Add("eleValue",item.EleValue);
                        eleObj.Add("operId", item.OperatorId);
                        eleObj.Add("typeEleId", item.Id);
                        eleObj.Add("eleName", item.EleName);
                        specObj.Add("ele" + item.ElementId.ToString(), eleObj);
                    }
                    JObject formObj = new JObject();
                    formObj.Add("spe" + typeItem.SpecId, specObj);
                    string formStr = formObj.ToString();
                    return new HtmlDataOperRetDto()
                    {
                        Code = 1,
                        Message = formStr
                    };
                }
                else
                {
                    return new HtmlDataOperRetDto()
                    {
                        Code = 0,
                        Message = "找不到数据"
                    };
                }
            }
            return new HtmlDataOperRetDto()
            {
                Code = 0,
                Message = "找不到数据"
            };
        }

        // 按照指定序列的顺序排序
        private int PatIndex(int[] array, int value)
        {
            int index = -1;
            for (int i = 0; i < array.Length; i++)
            {
                if (value == array[i])
                {
                    index = i;
                    break;
                }
            }
            return int.MaxValue;
        }


        public DataSearchTableDto GetDataInfoByTemplateIdAndSpecId(int input, int[] specId,DateTime begin,DateTime endTime)
        {
            var typeInList=_typeInRepository.GetAll()
                .Where(x => x.TplId == input && specId.Contains(x.SpecId) && string.Compare(x.SamplingDate, begin.ToString("yyyy-MM-dd")) >= 0 && string.Compare(x.SamplingTime, endTime.ToString("yyyy-MM-dd")) <= 0)
                .OrderByDescending(x=>x.SamplingDate)
                .ToList();

            List<List<string>> strList = new List<List<string>>();
            TemplateInfoDto schemaInfo = this.GetTemplateInfoByTemplateIdAndSpecId(input, specId);

            if (typeInList.Count() > 0)
            {
                var idArray=typeInList.Select(x => x.Id);

                var typeGrpList = typeInList.GroupBy(x => new { x.SamplingDate, x.SamplingTime })
                .ToList();

                var typeItemList = _typeInItemRepository.GetAll()
                    .Where(x => !x.IsDeleted && idArray.Contains(x.TypeInId))
                    .ToList();

                // 分组整理数据
                foreach (var item in typeGrpList)
                {
                    var tempTypeIn=typeInList.Where(x => x.SamplingDate == item.Key.SamplingDate && x.SamplingTime==item.Key.SamplingTime).ToList();
                    if (tempTypeIn.Count() > 0)
                    {
                        List<string> tempRow = GetSingleRow(tempTypeIn,typeItemList,schemaInfo,item.Key.SamplingDate,item.Key.SamplingTime);
                        strList.Add(tempRow);
                    }
                }
            }

            return
                new DataSearchTableDto()
                {
                    TableHead = schemaInfo,
                    TableData = strList
                };
    
        }

        // 生成单行数据
        public List<string> GetSingleRow(List<TypeIn> typeInList,List<TypeInItem> typeItemList,TemplateInfoDto schema,string samplingDate,string samplingTime)
        {
            var tempTypeInArray = typeInList.Select(x => x.Id);
            var tempTypeItemList= typeItemList.Where(x => tempTypeInArray.Contains(x.TypeInId)).ToList();
            int eleIndex = 0;
            List<string> strList = new List<string>();
            strList.Add(samplingDate);
            strList.Add(samplingTime);
            foreach (var specItem in schema.Specimens)
            {
                for (int i = 0; i < specItem.Count; i++)
                {
                    int tempEleId=schema.Elements[i + eleIndex].Id;
                    TypeInItem tempItem=tempTypeItemList.Where(x => x.SpecimenId == specItem.Id && x.ElementId == tempEleId).FirstOrDefault();
                    if (tempItem != null)
                    {
                        strList.Add(tempItem.EleValue);
                    }
                    else
                    {
                        strList.Add(string.Empty);
                    }
                }
                eleIndex += specItem.Count;
            }
            return strList;
        }
    }
}
