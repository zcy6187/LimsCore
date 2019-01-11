using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Abp.Application.Services.Dto;
using Abp.AutoMapper;
using Abp.Domain.Repositories;
using LIMS.Assay.Base.Dto;
using Microsoft.EntityFrameworkCore;

namespace LIMS.Assay.Base
{
    public class Assay_SpecimenAppService : LIMSAppServiceBase, IAssay_SpecimenAppService
    {
        private readonly IRepository<Specimen, int> _specimenRepository;

        public Assay_SpecimenAppService(
            IRepository<Specimen, int> specimenRepository)
        {
            _specimenRepository = specimenRepository;
        }


        public string Add(CreateSpecimenDto input)
        {
            var findCount = _specimenRepository.GetAll().Where(x => !x.IsDeleted && x.Name == input.Name).Count();
            if (findCount > 0)
            {
                return "该样品已存在！";
            }
            var entity = input.MapTo<Specimen>();
            _specimenRepository.Insert(entity);
            return "添加成功！";
        }

        public async Task Delete(int inputId)
        {
            var item = _specimenRepository.Single(x => x.Id == inputId);
            item.IsDeleted = true;
            await _specimenRepository.UpdateAsync(item);
        }

        public async Task<PagedResultDto<SpecimenDto>> GetPages(PagedResultRequestDto pageQueryDto,string searchName)
        {
            var query = _specimenRepository.GetAll().Where(x => !x.IsDeleted);
            if (!string.IsNullOrEmpty(searchName))
            {
                query = _specimenRepository.GetAll().Where(x => !x.IsDeleted && x.Name.Contains(searchName));
            }
            var count = await query.CountAsync();

            var items = query.OrderByDescending(x => x.Id)
                .Skip(pageQueryDto.SkipCount)
                .Take(pageQueryDto.MaxResultCount)
                .ToList();

            var itemListDtos= items.MapTo<List<SpecimenDto>>();

            return new PagedResultDto<SpecimenDto>(
                count,
                itemListDtos
                );
        }

        public string Update(SpecimenDto input)
        {
            var findCount=_specimenRepository.GetAll().Where(x => !x.IsDeleted && x.Name == input.Name).Count();
            if (findCount > 0)
            {
                return "该样品已存在！";
            }

            var item = input.MapTo<Specimen>();

             _specimenRepository.Update(item);

            return "修改成功！";
        }

        public List<Dtos.HtmlSelectDto> GetHtmlSelectSpecimens()
        {
            var list = _specimenRepository.GetAll().Where(x => !x.IsDeleted).Select(x => new Dtos.HtmlSelectDto()
            {
                Key = x.Id.ToString(),
                Value = x.Name
            }).ToList();

            return list;
        }
    }
}
