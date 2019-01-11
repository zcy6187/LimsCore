using Abp.AutoMapper;
using System.ComponentModel.DataAnnotations;

namespace LIMS.Assay.Base.Dto
{
    [AutoMapTo(typeof(LIMS.Assay.Element))]
    public class CreateElementDto
    {
        [Required]
        public string Name { get; set; }
        public string Code { get; set; }
        public string Description { get; set; }
    }
}
