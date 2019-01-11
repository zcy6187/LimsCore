using Abp.AutoMapper;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace LIMS.Assay.Base.Dto
{
    [AutoMapTo(typeof(Unit))]
    public class CreateUnitDto
    {
        [Required]
        public string Name { get; set; }
        public string Description { get; set; }
    }
}
