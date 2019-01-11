using System;
using System.Collections.Generic;
using System.Text;

namespace LIMS.SysManager.Organization.Dto
{
    public class OrgTreeNodeDto
    {
        public string title { get; set; }
        public string key { get; set; }
        public bool expanded { get; set; }
        public string id { get; set; }
        public bool isLeaf { get; set; }
        public List<OrgTreeNodeDto> children { get; set; }
    }

    public class OrgTreeDto
    {
        public List<OrgTreeNodeDto> nodes { get; set; }
    }
}
