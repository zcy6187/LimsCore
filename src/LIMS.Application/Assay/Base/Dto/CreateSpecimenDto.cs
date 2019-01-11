using Abp.AutoMapper;
using System.ComponentModel.DataAnnotations;

namespace LIMS.Assay.Base.Dto
{
    [AutoMapTo(typeof(LIMS.Assay.Specimen))]
    public class CreateSpecimenDto
    {
        [Required]
        public string Name { get; set; }
        public string Lx { get;set;}
    }

}
