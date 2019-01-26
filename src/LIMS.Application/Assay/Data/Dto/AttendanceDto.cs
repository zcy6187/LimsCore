using System;
using System.Collections.Generic;
using System.Text;

namespace LIMS.Assay.Data.Dto
{
    public class AttendanceDto
    {
        public int Id { get; set; }
        public string orgCode { get; set; }
        public string orgName { get; set; }
        public int tplId { get; set; }
        public string tplName { get; set; }
        public int tplSpecId { get; set; }
        public string tplSpecName { get; set; }
        public string tplElementNames { get; set; }
        public string man_banci { get; set; }
        public string man_luci { get; set; }
        public string signTime { get; set; }
        public DateTime signDate { get; set; }
        public string samplingdate { get; set; }
        public string Lx { get; set; }
        public string eleNames { get; set; }
        public string Flag { get; set; }
        public string samplingTime { get; set; }
        public string selfCode { get; set; }
        public string description { get; set; }
        public string scanId { get; set; }
    }
}
