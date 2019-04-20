using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Abp.AutoMapper;
using Abp.Domain.Repositories;
using Abp.Domain.Uow;
using LIMS.Assay.Data.Dto;
using Newtonsoft.Json.Linq;

namespace LIMS.Assay.Data
{
    public class Assay_DataInput : LIMSAppServiceBase, IAssay_DataInput
    {
        private IRepository<Template, int> _tplRep;
        private IRepository<TplSpecimen, int> _tplSpecRep;
        private IRepository<TplElement, int> _tplEleRep;
        private IRepository<Attendance, int> _attendanceRep;
        private IRepository<TypeIn, int> _typeInRep;
        private IRepository<TypeInItem, int> _typeInItemRep;

        public Assay_DataInput(IRepository<Template, int> tplRep,
            IRepository<TplSpecimen, int> tplSpecRep,
            IRepository<TplElement, int> tplEleRep,
            IRepository<Attendance, int> attendanceRep,
            IRepository<TypeIn, int> typeInRep,
            IRepository<TypeInItem, int> typeInItemRep
            )
        {
            this._tplEleRep = tplEleRep;
            this._tplRep = tplRep;
            this._tplSpecRep = tplSpecRep;

            this._attendanceRep = attendanceRep;
            this._typeInRep = typeInRep;
            this._typeInItemRep = typeInItemRep;
        }

        public TemplateSchemaInputDto GetTemplateSchemaInputDtoBySignId(int signId)
        {
            var attendance = _attendanceRep.GetAll().Where(x => x.Id == signId).FirstOrDefault();
            if (attendance == null)
            {
                return null;
            }
            else
            {
                int tplId = attendance.TplId;
                int specId = attendance.TplSpecId;
                int[] specArray = new int[] { specId };
                return GetTemplateSchemaInputDtoByTplId(tplId, specArray);
            }

        }

        // 获取输入框架
        public TemplateSchemaInputDto GetTemplateSchemaInputDtoByTplId(int tplId, int[] specId)
        {
            var template = _tplRep.GetAll().Where(x => x.Id == tplId).Single();
            List<TplSpecimen> tplSpecimenList = new List<TplSpecimen>();
            if (specId.Count() == 0 || specId.Contains(-1))
            {
                tplSpecimenList = _tplSpecRep.GetAll().Where(x => x.TplId == tplId)
                    .OrderBy(x => x.TplId).ThenByDescending(x => x.OrderNum).ToList();
            }
            else
            {
                tplSpecimenList = _tplSpecRep.GetAll().Where(x => specId.Contains(x.Id))
                     .OrderBy(x => x.TplId).ThenByDescending(x => x.OrderNum).ToList();
            }
            var specIdList = tplSpecimenList.Select(x => x.Id).ToList();
            List<TplElement> tplEleList = this._tplEleRep.GetAll().Where(x => specIdList.Contains(x.TplSpecId))
                .OrderBy(x => x.TplSpecId).ThenBy(x => x.OrderNo).ToList();

            List<SpecInputDto> tempSpecInputList = new List<SpecInputDto>();

            foreach (var item in tplSpecimenList)
            {
                var tempEleList = tplEleList.Where(x => x.TplSpecId == item.Id).ToList();
                List<ElementInputDto> tempEleInputList = new List<ElementInputDto>();
                foreach (var ele in tempEleList)
                {
                    ElementInputDto tempEleInput = new ElementInputDto();
                    tempEleInput.EleId = ele.Id;
                    tempEleInput.SpecId = item.Id;
                    tempEleInput.TplId = tplId;
                    tempEleInput.UnitName = ele.UnitName;
                    tempEleInput.EleName = ele.ElementName;
                    tempEleInput.FormName = "ele" + ele.Id.ToString();
                    tempEleInput.IsVisible = true;
                    tempEleInputList.Add(tempEleInput);
                }

                SpecInputDto tempSpecInput = new SpecInputDto();
                tempSpecInput.SpecId = item.Id;
                tempSpecInput.SpecName = item.SpecName;
                tempSpecInput.EleList = tempEleInputList;
                tempSpecInput.FormName = "spe" + item.Id.ToString();
                tempSpecInputList.Add(tempSpecInput);
            }

            TemplateSchemaInputDto retInput = new TemplateSchemaInputDto();
            retInput.TplId = tplId;
            retInput.TplName = template.TplName;
            retInput.FormName = "for" + tplId.ToString();
            retInput.SpecList = tempSpecInputList;

            return retInput;

        }


        #region 数据录入
        public Dtos.HtmlDataOperRetDto WriteValueToTable(CreateDataInputDto input)
        {
            if (string.IsNullOrEmpty(input.FormValue))
            {
                return new Dtos.HtmlDataOperRetDto()
                {
                    Code = -1,
                    Message = "数据为空，无法添加",
                };
            }
            int tplId = input.TplId;
            DateTime samplingDate = DateTime.Parse(input.SamplingDate);
            string samplingTime = input.SamplingTime;
            List<SpecDataDto> specList = ReadValueFromJsonStr(input.FormValue);

            // 遍历，存数据于库
            foreach (var item in specList)
            {
                // OldId为空，则添加
                if (string.IsNullOrEmpty(item.OldId))
                {
                    AddSpecDataToTable(item, tplId, samplingDate, samplingTime);
                }
                else  // 更新
                {
                    UpdateOrAddSpecData(item, tplId, int.Parse(item.OldId), samplingDate, samplingTime);
                }
            }

            return new Dtos.HtmlDataOperRetDto()
            {
                Code = 1,
                Message = "添加成功！",
            };


        }

        #region 添加样品
        private void AddSpecDataToTable(SpecDataDto item, int tplId, DateTime samplingDate, string samplingTime)
        {
            // 如果没有元素数据直接返回
            List<EleDataDto> eleDataList = item.EleDataList;
            if (eleDataList.Count == 0)
            {
                return;
            }

            Attendance findAttendance = null;

            // 签到Id和typeIn的Id都为空，则需要去签到表中检测签Id
            if (string.IsNullOrEmpty(item.SignId))
            {
                DateTime beginTime = DateTime.Parse(string.Format("{0} {1}:00", samplingDate.ToString("yyyy-MM-dd"), samplingTime));
                DateTime endTime = DateTime.Parse(string.Format("{0} {1}:59", samplingDate.ToString("yyyy-MM-dd"), samplingTime));

                int tempSpecId = item.SpecId;
                // 查找签到信息
                var tempAttendance = _attendanceRep.GetAll().Where(x =>
                  x.SignTime >= beginTime && x.SignTime <= endTime
                  && !x.IsDeleted && x.TplId == tplId && x.TplSpecId == tempSpecId)
                .FirstOrDefault();
                findAttendance = tempAttendance;
            }
            else
            {
                findAttendance = _attendanceRep.Single(x => x.Id == int.Parse(item.SignId));
            }

            // 获取化验元素信息 
            eleDataList = eleDataList.OrderBy(x => x.EleId).ToList();
            string elementIds = string.Join(",", eleDataList.Select(x => x.EleId));
            string elementNames = string.Join(",", eleDataList.Select(x => x.EleName));

            // 创建签到数据模型，并填充基础信息
            CreateTypeInDto typeIn = new CreateTypeInDto();
            TypeIn tempTypeIn = null;
            // 记录数据插入的Id
            int tempTypeInId = -1;
            // 如果签到信息存在，则去TypeIn表中查找是否有录入记录
            tempTypeIn = _typeInRep.GetAll().Where(x => x.SignId == typeIn.SignId && x.IsDeleted == false).FirstOrDefault();
            // 如果typeIn表中没有记录，则根据签到信息回填，或者根据已有信息创建新的TypeIn
            if (tempTypeIn == null)
            {
                typeIn.IsParallel = false; // 平行样
                string attendanceEleIds = string.Empty;
                typeIn.TplId = tplId;
                typeIn.SpecId = item.SpecId;
                typeIn.CreateTime = DateTime.Now;
                typeIn.CreatorId = (int)AbpSession.UserId;
                typeIn.IsDeleted = false;
                typeIn.EleIds = elementIds;
                typeIn.EleNames = elementNames;
                
                // 如果有签到信息，则将签到信息回填
                if (findAttendance != null)
                {
                    typeIn.SignId = findAttendance.Id.ToString();
                    attendanceEleIds = findAttendance.ElementIds;
                    typeIn.IsParallel = string.IsNullOrEmpty(findAttendance.MainScanId) ? false : true;
                    typeIn.MainId = findAttendance.MainScanId;
                    typeIn.SamplingDate = findAttendance.SamplingDate.ToString("yyyy-MM-dd");
                    typeIn.SamplingTime = findAttendance.SamplingTime;
                    typeIn.SamplingTm = DateTime.Parse(findAttendance.SamplingDate.ToString("yyyy-MM-dd ") + findAttendance.SamplingTime);
                    typeIn.SignTm = findAttendance.SignTime;
                }
                else // 没有签到信息，则根据已有信息补充
                {
                    string samplDate = samplingDate.ToString("yyyy-MM-dd");
                    typeIn.SamplingDate = samplDate;
                    typeIn.SamplingTime = samplingTime;

                    typeIn.SamplingTm = DateTime.Parse($"{samplingDate} {samplingTime}");
                    typeIn.SignTm = DateTime.Now;
                    tempTypeIn.CreateTime = DateTime.Now;  
                }
                // 插入数据，获取当前插入ID
                tempTypeInId = _typeInRep.InsertAndGetId(tempTypeIn);
            }
            else // 有信息则，只更新元素信息
            {
                tempTypeIn.EleIds = elementIds;
                tempTypeIn.EleNames = elementNames;
                _typeInRep.Update(tempTypeIn);
                tempTypeInId = tempTypeIn.Id;
            }

            // 更新签到表信息(0-未录入，1-部分录入，2-全录入)
            if (findAttendance != null)
            {
                if (findAttendance.TplElementIds == elementIds)
                {
                    findAttendance.Flag = 2;
                }
                else
                {
                    findAttendance.Flag = 1;
                }

                _attendanceRep.UpdateAsync(findAttendance);
            }

            // 生成TypeInItem信息
            AddElementDataToTable(item, tempTypeInId, tplId);
        }

        // 添加元素
        [UnitOfWork]
        private void AddElementDataToTable(SpecDataDto specDto, int typeInId, int tplId)
        {
            foreach (var item in specDto.EleDataList)
            {
                TypeInItem tempEleDto = new TypeInItem();
                tempEleDto.CreateTime = DateTime.Now;
                tempEleDto.TplId = tplId;
                tempEleDto.TypeInId = typeInId;
                tempEleDto.OperatorId = item.OperatorId;
                tempEleDto.SpecimenId = item.SpecId;
                tempEleDto.ElementId = item.EleId;
                tempEleDto.EleValue = item.EleValue;
                tempEleDto.IsDeleted = false;
                tempEleDto.EleName = item.EleName;
                _typeInItemRep.Insert(tempEleDto);
            }
        }

        #endregion

        #region 更新或者添加元素
        [UnitOfWork]
        private void UpdateOrAddSpecData(SpecDataDto item, int tplId, int oldTypeInId, DateTime samplingDate, string samplingTime)
        {
            // 如果没有元素数据直接返回
            List<EleDataDto> eleDataList = item.EleDataList;
            if (eleDataList.Count == 0)
            {
                return;
            }

            // 查找到唯一的元素
            TypeIn findTypeIn = _typeInRep.FirstOrDefault(x => x.Id == oldTypeInId);
            Attendance findAttendance = null;
            if (!string.IsNullOrEmpty(findTypeIn.SignId))
            {
                int tempAttenId = int.Parse(findTypeIn.SignId);
                findAttendance = _attendanceRep.Single(x => x.Id == tempAttenId);
            }
            else
            {
                findTypeIn.SamplingDate = samplingDate.ToString("yyyy-MM-dd");
                findTypeIn.SamplingTime = samplingTime;
            }


            // 获取化验元素信息
            eleDataList = eleDataList.OrderBy(x => x.EleId).ToList();
            string elementIds = string.Join(",", eleDataList.Select(x => x.EleId));
            string elementNames = string.Join(",", eleDataList.Select(x => x.EleName));
            string newElementIds = findTypeIn.EleIds + "," + elementIds;
            string newElementNames = findTypeIn.EleNames + "," + elementNames;
            // 将字符串 转变为 数值数组，并去重
            var idArray = Array.ConvertAll<string, int>(newElementIds.Split(',', StringSplitOptions.RemoveEmptyEntries).Distinct().ToArray(), x => int.Parse(x));
            // 从小到大排序
            Array.Sort(idArray);
            newElementIds = string.Join(',', idArray);
            // 元素名称去掉重复项
            newElementNames = string.Join(',',
                newElementNames.Split(",", StringSplitOptions.RemoveEmptyEntries).Distinct());

            findTypeIn.EleIds = newElementIds;
            findTypeIn.EleNames = newElementNames;

            // 更新TypeIn
            _typeInRep.UpdateAsync(findTypeIn);
            // 更新或者添加TypeInItem
            UpdateOrAddElement(item, findTypeIn.Id, tplId);

            // 更新签到表信息
            if (findAttendance != null)
            {
                findAttendance.TplElementIds = newElementIds;
                if (findAttendance.TplElementIds == newElementIds)
                {
                    findAttendance.Flag = 2;
                }
                else
                {
                    findAttendance.Flag = 1;
                }
                _attendanceRep.UpdateAsync(findAttendance);
            }

        }

        /*
         * 开发时注意：重复点击，比如连续点击2次，第1次为添加元素，第2次应该为更新元素，但是由于前台点击了2次，第2次传递的数据和第1次一样，依旧没有OldId
         * 此时应该更新，但代码却以添加为操作，致使数据会重复
         */
        // 更新或添加元素
        private void UpdateOrAddElement(SpecDataDto specDto, int typeInId, int tplId)
        {
            foreach (var item in specDto.EleDataList)
            {
                if (string.IsNullOrEmpty(item.OldId))  // 添加
                {
                    var tempEleItem = _typeInItemRep.FirstOrDefault(x => x.TplId == tplId && x.TypeInId == typeInId && x.ElementId == item.EleId && x.SpecimenId == item.SpecId && !x.IsDeleted);
                    if (tempEleItem == null)
                    {
                        TypeInItem tempEleDto = new TypeInItem();
                        tempEleDto.CreateTime = DateTime.Now;
                        tempEleDto.TplId = tplId;
                        tempEleDto.TypeInId = typeInId;
                        tempEleDto.OperatorId = item.OperatorId;
                        tempEleDto.SpecimenId = item.SpecId;
                        tempEleDto.ElementId = item.EleId;
                        tempEleDto.EleValue = item.EleValue;
                        tempEleDto.IsDeleted = false;
                        tempEleDto.EleName = item.EleName;
                        _typeInItemRep.InsertAsync(tempEleDto);
                    }
                    else
                    {
                        tempEleItem.EleValue = item.EleValue;
                        tempEleItem.OperatorId = item.OperatorId;
                        _typeInItemRep.UpdateAsync(tempEleItem);
                    }

                }
                else //更新
                {
                    UpdateSingleElement(item);
                }
            }
        }

        private void UpdateSingleElement(EleDataDto item)
        {
            var tempFindItem = _typeInItemRep.Single(x => x.Id == int.Parse(item.OldId));
            tempFindItem.EleValue = item.EleValue;
            tempFindItem.OperatorId = item.OperatorId;
            _typeInItemRep.UpdateAsync(tempFindItem);
        }

        #endregion

        // 从json字符串中转换出需要的数据
        private List<SpecDataDto> ReadValueFromJsonStr(string jsonStr)
        {
            JToken specToken = JToken.Parse(jsonStr);
            List<SpecDataDto> specDataList = new List<SpecDataDto>();
            foreach (var specItem in specToken)
            {
                SpecDataDto specData = new SpecDataDto();
                List<EleDataDto> eleList = new List<EleDataDto>();

                var tempSpec = (Newtonsoft.Json.Linq.JProperty)specItem;
                int specId = int.Parse(tempSpec.Name.Substring(3));
                var eleToken = JToken.Parse(tempSpec.Value.ToString());
                int i = 0;
                string signId = string.Empty;
                string typeId = string.Empty;
                foreach (var eleItem in eleToken)
                {
                    var tempEle = (JProperty)eleItem;
                    if (i == 0) //签到Id
                    {
                        signId = tempEle.Value.ToString();
                    }
                    else if (i == 1) // 原表Id-typeIn表id
                    {
                        typeId = tempEle.Value.ToString();
                    }
                    else if (i >= 2) // 元素
                    {
                        dynamic eleData = JToken.Parse(tempEle.Value.ToString());
                        EleDataDto eleObj = new EleDataDto();


                        // 如果元素值不为空 或者 元素的原Id不为空，则将数据置入数组
                        // 当元素原Id不为空时，代表，数据可能是更新，不管将元素是否有值，因为可能需要将有值的元素更新为无值
                        if (!string.IsNullOrEmpty((string)eleData.eleValue) || !string.IsNullOrEmpty((string)eleData.typeEleId))
                        {
                            int eleId = int.Parse(tempEle.Name.Substring(3));
                            eleObj.SpecId = specId;
                            eleObj.EleId = eleId;

                            eleObj.OldId = eleData.TypeEleId;
                            eleObj.EleValue = eleData.eleValue;
                            eleObj.OperatorId = eleData.operId;
                            eleObj.EleName = eleData.eleName;

                            eleList.Add(eleObj);
                        }
                    }
                    i++;
                }
                specData.OldId = typeId;
                specData.SpecId = specId;
                specData.SignId = signId;
                specData.EleDataList = eleList;
                specDataList.Add(specData);
            }

            return specDataList;
        }

        #endregion
    }
}
