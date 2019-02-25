using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Abp.AutoMapper;
using Abp.Domain.Repositories;
using Abp.Domain.Uow;
using LIMS.Assay.Base.Dto;

namespace LIMS.Assay.Base
{
    public class Assay_TplAppService : LIMSAppServiceBase, IAssay_TplAppService
    {
        private readonly IRepository<TplElement, int> _tplElementRepository;
        private readonly IRepository<Template, int> _tplRepository;
        private readonly IRepository<TplSpecimen, int> _tplSpecimenRepository;
        private readonly IRepository<SysManager.OrgInfo, int> _orgRepository;

        public Assay_TplAppService(
            IRepository<TplElement, int> tplElementRepository,
            IRepository<Template, int> tplRepository,
            IRepository<TplSpecimen, int> tplSpecimenRepository,
            IRepository<SysManager.OrgInfo, int> orgRepository)
        {
            _tplElementRepository = tplElementRepository;
            _tplRepository = tplRepository;
            _tplSpecimenRepository = tplSpecimenRepository;
            _orgRepository = orgRepository;
        }

        public string AddTpl(CreateTplDto input)
        {
            int itemCount = this._tplRepository.GetAll().Where(x => !x.IsDeleted && x.TplName == input.TplName && x.OrgCode == input.OrgCode).Count();
            if (itemCount > 0)
            {
                return "该机构下，此名称已存在!";
            }

            Template newTemplate = input.MapTo<Template>();
            newTemplate.CreateTime = DateTime.Now;
            newTemplate.IsDeleted = false;
            string orgName = _orgRepository.Single(x => x.Code == input.OrgCode).AliasName;
            newTemplate.OrgName = orgName;
            newTemplate.CreatorId = Convert.ToInt32(AbpSession.UserId);
            newTemplate.LastModifyTime = DateTime.Now;

            _tplRepository.InsertAsync(newTemplate);
            return "添加成功！";
        }

        public string AddTplElement(CreateTplElementDto input)
        {
            int itemCount = this._tplElementRepository.GetAll().Where(x => !x.IsDeleted && x.TplSpecId == input.TplSpecId && x.ElementId == input.ElementId).Count();
            if (itemCount > 0)
            {
                return "该样品下，此元素已存在!";
            }

            TplElement newTplElement = input.MapTo<TplElement>();
            newTplElement.IsDeleted = false;
            newTplElement.CreateTime = DateTime.Now;
            newTplElement.LastModifyTime = DateTime.Now;
            newTplElement.CreatorId = Convert.ToInt32(AbpSession.UserId);
            newTplElement.IsUse = true;
            var maxOrderNoItem = _tplElementRepository.GetAll().Where(x => x.TplSpecId == input.TplSpecId).OrderBy(x => x.OrderNo).LastOrDefault();
            newTplElement.OrderNo = maxOrderNoItem == null ? 1 : maxOrderNoItem.OrderNo + 1;

            _tplElementRepository.InsertAsync(newTplElement);

            return "添加成功!";
        }

        public string AddTplSpecimen(CreateTplSpecimenDto input)
        {
            int itemCount = this._tplSpecimenRepository.GetAll().Where(x => !x.IsDeleted && x.TplId == input.TplId && x.SpecId == input.SpecId).Count();
            if (itemCount > 0)
            {
                return "该模板下，此样品已存在!";
            }

            TplSpecimen newSpecimen = input.MapTo<TplSpecimen>();
            newSpecimen.CreateTime = DateTime.Now;
            newSpecimen.LastModifyTime = DateTime.Now;
            newSpecimen.IsUse = true;
            newSpecimen.IsDeleted = false;
            newSpecimen.CreatorId = Convert.ToInt32(AbpSession.UserId);

            var maxOrderNoItem = _tplSpecimenRepository.GetAll().Where(x => x.TplId == input.TplId).OrderBy(x => x.OrderNum).LastOrDefault();
            newSpecimen.OrderNum = maxOrderNoItem == null ? 1 : maxOrderNoItem.OrderNum + 1;

            _tplSpecimenRepository.InsertAsync(newSpecimen);

            return "添加成功！";
        }

        public async Task DeleteTplElement(int inputId)
        {
            var deleteItem = _tplElementRepository.Single(x => x.Id == inputId);
            deleteItem.IsDeleted = true;

            await _tplElementRepository.UpdateAsync(deleteItem);

        }

        public async Task DeleteTplSpecimen(int inputId)
        {
            var deleteItem = _tplSpecimenRepository.Single(x => x.Id == inputId);
            deleteItem.IsDeleted = true;

            await _tplSpecimenRepository.UpdateAsync(deleteItem);
        }

        public async Task DelTpl(int inputId)
        {
            var deleteItem = _tplRepository.Single(x => x.Id == inputId);
            deleteItem.IsDeleted = true;

            await _tplRepository.UpdateAsync(deleteItem);
        }

        public string EditTpl(EditTplDto input)
        {
            int itemCount = this._tplRepository.GetAll().Where(x => !x.IsDeleted && x.TplName == input.TplName && x.OrgCode == input.OrgCode && x.Id != input.Id).Count();
            if (itemCount > 0)
            {
                return "该机构下，此名称已存在!";
            }

            string orgName = _orgRepository.Single(x => x.Code == input.OrgCode).AliasName;
            var editItem = this._tplRepository.Single(x => x.Id == input.Id);
            editItem.TplName = input.TplName;
            editItem.LastModifyTime = DateTime.Now;
            editItem.CreatorId = Convert.ToInt32(AbpSession.UserId);
            editItem.OrgCode = input.OrgCode;
            editItem.OrgName = orgName;

            _tplRepository.UpdateAsync(editItem);

            return "修改成功！";
        }

        public string EditTplElement(EditTplElementDto input)
        {
            int itemCount = this._tplElementRepository.GetAll()
                .Where(x => !x.IsDeleted && x.TplSpecId == input.TplSpecId && x.ElementId == input.ElementId && x.Id != input.Id).Count();
            if (itemCount > 0)
            {
                return "该样品下，此元素已存在!";
            }
            var editItem=this._tplElementRepository.Single(x => x.Id == input.Id);
            editItem.UnitId = input.UnitId;
            editItem.UnitName = input.UnitName;
            editItem.ElementName = input.ElementName;
            editItem.ElementId = input.ElementId;
            editItem.LastModifyTime = DateTime.Now;
            editItem.CreatorId = Convert.ToInt32(AbpSession.UserId);

            _tplElementRepository.UpdateAsync(editItem);

            return "修改成功！";
        }

        [UnitOfWork]
        public string EditTplSpecimen(EditTplSpecimenDto input)
        {
            int itemCount = this._tplSpecimenRepository.GetAll()
                .Where(x => !x.IsDeleted && x.TplId == input.TplId && x.SpecId == input.SpecId && x.Id!=input.Id).Count();
            if (itemCount > 0)
            {
                return "该模板下，此样品已存在!";
            }

            var editItem = this._tplSpecimenRepository.Single(x=>x.Id==input.Id);
            editItem.SpecId = input.SpecId;
            editItem.SpecName = input.SpecName;
            editItem.LastModifyTime = DateTime.Now;
            editItem.CreatorId = Convert.ToInt32(AbpSession.UserId);

            _tplSpecimenRepository.UpdateAsync(editItem);
            var editElements = _tplElementRepository.GetAll().Where(x => x.TplSpecId == editItem.Id);
            foreach (var item in editElements)
            {
                item.SpecId = editItem.SpecId;
                item.SpecName = editItem.SpecName;
                _tplElementRepository.UpdateAsync(item);
            }
            return "修改成功！";
        }

        public EditTplDto GetSingleTpl(int inputId)
        {
            var item = _tplRepository.Single(x => x.Id == inputId);
            EditTplDto editDto = item.MapTo<EditTplDto>();

            return editDto;
        }

        // 获取所有的元素
        public List<EditTplElementDto> GetTplElementsByTplSpecimenId(int inputId)
        {
            var itemList = _tplElementRepository.GetAll().Where(x => !x.IsDeleted && x.TplSpecId == inputId).OrderBy(x => x.OrderNo).ToList();
            List<EditTplElementDto> dtoList = itemList.MapTo<List<EditTplElementDto>>();

            return dtoList;
        }

        // 获取所有模板（按组织机构代码）
        public List<EditTplDto> GetTplsByOrgCode(string inputCode)
        {
            var itemList = _tplRepository.GetAll().Where(x => !x.IsDeleted && x.OrgCode.StartsWith(inputCode)).OrderByDescending(x => x.Id).ToList();
            List<EditTplDto> dtoList = itemList.MapTo<List<EditTplDto>>();

            return dtoList;
        }

        // 获取所有模板（按查询）
        public List<EditTplDto> GetTpls(SearchTplDto input)
        {
            if (!string.IsNullOrEmpty(input.TplName))
            {
                var itemList = _tplRepository.GetAll().Where(x => !x.IsDeleted && x.TplName.Contains(input.TplName)).OrderByDescending(x => x.Id).ToList();
                List<EditTplDto> dtoList = itemList.MapTo<List<EditTplDto>>();

                return dtoList;
            }
            else
            {
                return GetTplsByOrgCode(input.OrgCode);
            }
        }

        // 获取模板下的所有元素
        public List<EditTplSpecimenDto> GetTplSpecimensByTplId(int inputId)
        {
            var itemList = _tplSpecimenRepository.GetAll().Where(x => !x.IsDeleted && x.TplId == inputId).OrderBy(x => x.OrderNum).ToList();
            List<EditTplSpecimenDto> dtoList = itemList.MapTo<List<EditTplSpecimenDto>>();

            return dtoList;
        }

        [UnitOfWork]
        public string ReOrderTplSpecimen(List<ReOrderDto> inputList)
        {
            try
            {
                foreach (var item in inputList)
                {
                    var tempItem = _tplSpecimenRepository.Single(x => x.Id == item.Id);
                    tempItem.OrderNum = item.OrderNo;
                    _tplSpecimenRepository.UpdateAsync(tempItem);
                }
                return "更新成功!";
            }
            catch
            {
                return "更新失败!";
            }
        }

        [UnitOfWork]
        public string ReOrderTplElement(List<ReOrderDto> inputList)
        {
            try
            {
                foreach (var item in inputList)
                {
                    var tempItem = _tplElementRepository.Single(x => x.Id == item.Id);
                    tempItem.OrderNo = item.OrderNo;
                    _tplElementRepository.UpdateAsync(tempItem);
                }
                return "更新成功!";
            }
            catch
            {
                return "更新失败!";
            }
        }
    }
}
