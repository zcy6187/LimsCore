using Dapper.Contrib.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LimsX.Core
{
    [Table("LimsUser")]
    public class LimsUser
    {
        public int Id { get; set; }
        public long UserId { get; set; }
        public string UserName { get; set; }
        public string TrueName { get; set; }
        public string Password { get; set; }
    }

    [Table("sys_OrgInfo")]
    public class OrgInfo
    {
        public int Id { get; set; }
        public string Code { get; set; }
        public string FatherCode { get; set; }
        public int OrderNo { get; set; }
        public int Layer { get; set; }
        public string Lx { get; set; }
        public string OrgName { get; set; }
        public string AliasName { get; set; }
        public string FullName { get; set; }
        public bool IsUse { get; set; }
        public DateTime CreateTime { get; set; }
        public string OldId { get; set; }
    }

    [Table("sys_userOrg")]
    public class UserOrg
    {
        public int Id { get; set; }
        public long UserId { get; set; }
        // 用户的化验模板ID
        public string OrgIds { get; set; }
        public bool IsDeleted { get; set; }
    }
}
