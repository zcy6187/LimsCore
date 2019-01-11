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
    public class Assay_UnitAppService : IAssay_UnitAppService
    {
        private readonly IRepository<Unit, int> _repository;

        public Assay_UnitAppService(
            IRepository<Unit, int> repository)
        {
            _repository = repository;
        }


        public async Task Add(CreateUnitDto input)
        {
            var entity = input.MapTo<Unit>();
            await _repository.InsertAsync(entity);
        }

        public async Task Delete(int inputId)
        {
            var item = _repository.Single(x => x.Id == inputId);
            item.IsDeleted = true;
            await _repository.UpdateAsync(item);
        }

        public async Task<PagedResultDto<UnitDto>> GetPages(PagedResultRequestDto pageQueryDto)
        {
            var query = _repository.GetAll().Where(x => !x.IsDeleted);
            var count = await query.CountAsync();

            var items = query.OrderByDescending(x => x.Id)
                .Skip(pageQueryDto.SkipCount)
                .Take(pageQueryDto.MaxResultCount)
                .ToList();

            var itemListDtos = items.MapTo<List<UnitDto>>();

            return new PagedResultDto<UnitDto>(
                count,
                itemListDtos
                );
        }

        public async Task Update(UnitDto input)
        {
            var item = input.MapTo<Unit>();

            await _repository.UpdateAsync(item);
        }

        public List<Dtos.HtmlSelectDto> GetHtmlSelectUnits()
        {
            var list = _repository.GetAll().Where(x => !x.IsDeleted).Select(x => new Dtos.HtmlSelectDto()
            {
                Key = x.Id.ToString(),
                Value = x.Name
            }).ToList();

            return list;
        }
    }
}
