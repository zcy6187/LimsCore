using Abp.Application.Services;
using Abp.Application.Services.Dto;
using Abp.Authorization;
using Abp.AutoMapper;
using Abp.Domain.Repositories;
using LIMS.Assay.Authorization;
using LIMS.Assay.Base.Dto;
using LIMS.Authorization;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LIMS.Assay.Base
{
    public class Assay_ElementAppService :LIMSAppServiceBase, IAssay_ElementAppService
    {
        private readonly IRepository<Element, int> _elementRepository;

        public Assay_ElementAppService(
            IRepository<Element, int> elementRepository)
        {
            _elementRepository = elementRepository;
        }

        public string AddElement(CreateElementDto input)
        {
            var item = _elementRepository.GetAll().Where(x => x.IsDeleted == false && x.Name == input.Name).Count();
            if (item >0)
            {
                return "该名称已经存在";
            }

            var entity=input.MapTo<Element>();
            _elementRepository.InsertAsync(entity);
            return "添加成功！";
        }

        public async Task DeleteElement(int inputId)
        {
            var item=_elementRepository.Single(x=>x.Id==inputId);
            item.IsDeleted = true;
           await  _elementRepository.UpdateAsync(item);
        }

        public async Task<PagedResultDto<ElementDto>> GetElements(PagedResultRequestDto pageQueryDto, string searchName)
        {
            var query=_elementRepository.GetAll().Where(x=>!x.IsDeleted);
            if (!string.IsNullOrEmpty(searchName))
            {
                query = _elementRepository.GetAll().Where(x => !x.IsDeleted && x.Name.Contains(searchName.Trim()));
            }
            var elementCount=await query.CountAsync();

            var elements = query.OrderByDescending(x => x.Id)
                .Skip(pageQueryDto.SkipCount)
                .Take(pageQueryDto.MaxResultCount)
                .ToList();

            var elementListDto = elements.MapTo<List<ElementDto>>();

            return new PagedResultDto<ElementDto>(
                elementCount,
                elementListDto
                );
        }

        public CreateElementDto GetFirst()
        {
            var query = _elementRepository.GetAll();
            var firstEle =query.First();
            var eleTpl=firstEle.MapTo<CreateElementDto>();
            return eleTpl;
        }

        public string UpdateElement(ElementDto input)
        {
            var item=_elementRepository.GetAll().Where(x => x.IsDeleted == false && x.Name == input.Name).Count();
            if (item>0)
            {
                return "该元素已经存在";
            }

            var entity = input.MapTo<Element>();

             _elementRepository.Update(entity);

            return "修改成功！";
        }

        public List<Dtos.HtmlSelectDto> GetHtmlSelectElements()
        {
            var list = _elementRepository.GetAll().Where(x => !x.IsDeleted).Select(x=>new Dtos.HtmlSelectDto()
            {
                Key=x.Id.ToString(),
                Value=x.Name
            }).ToList();

            return list;
        }


    }
}
