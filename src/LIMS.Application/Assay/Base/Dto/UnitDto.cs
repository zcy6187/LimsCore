using Abp.Application.Services.Dto;
using Abp.AutoMapper;
using System.ComponentModel.DataAnnotations;

namespace LIMS.Assay.Base.Dto
{
    [AutoMapTo(typeof(Unit))]
    public class UnitDto:EntityDto
    {
        [Required]
        public string Name { get; set; }
        public string Description { get; set; }
    }
}
