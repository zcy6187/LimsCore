using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LimsX.Dtos
{
    public class OrgTreeNodeDto
    {
        public string id { get; set; }
        public string name { get; set; }
        public bool open { get; set; }
        public List<OrgTreeNodeDto> children { get; set; }
        public bool isChecked { get; set; }
        
    }

    public class OrgTreeDto
    {
        public List<OrgTreeNodeDto> nodes { get; set; }
    }
}
