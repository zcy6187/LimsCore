using Dapper;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tools.Blls
{
    public class Etl
    {
        static string ConnStr = "Data Source=10.68.2.2;DataBase=LIMS;User ID=SA;Password=Sa***123;Max Pool Size=512;";
        public static void WriteTplsToOrgTplAndOrg()
        {
            using (var conn = new SqlConnection(ConnStr))
            {
                var userTpl = conn.Query<UserTpl>("select * from sys_UserTpl where isDeleted=0").ToList();
                var userOrg = conn.Query<UserOrg>("select * from sys_userorg where isDeleted=0").ToList();
                var userOrgTpl = conn.Query<UserOrgTpl>("select * from sys_userOrgTpl where isDeleted=0").ToList();

                var orgList = conn.Query<OrgInfo>("select * from sys_OrgInfo").ToList();
                var tplList = conn.Query<Template>("select * from assay_Template where isDeleted=0").ToList();
                var userZtList = conn.Query<UserZt>("select * from sys_userZt").ToList();

                // 记录所有已出现的ID
                var userIdList = new List<long>();

                List<UserOrg> userOrgList = new List<UserOrg>();
                List<UserOrgTpl> userOrgTplList = new List<UserOrgTpl>();

                foreach (var item in userTpl)
                {
                    var tempUid = item.UserId;
                    string tplIds = item.TplIds;
                    long uid = item.UserId;
                    userIdList.Add(uid);

                    // 获取用户的所有Template
                    string[] tplIdArray = tplIds.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                    int[] tplIdIntArray = Array.ConvertAll<string, int>(tplIdArray, s => int.Parse(s));
                    var tmpTplList = tplList.Where(x => tplIdIntArray.Contains(x.Id)).ToList();

                    // 从Template中，提取出组织信息
                    var orgCodeList = tmpTplList.GroupBy(x => x.OrgCode).Select(x => x.Key).ToArray();
                    foreach (var orgItem in orgCodeList)
                    {
                        var userTplArray = tmpTplList.Where(x => x.OrgCode == orgItem).Select(x => x.Id).ToArray();
                        string userTplIds = string.Join(",", userTplArray);

                        UserOrgTpl tmp = new UserOrgTpl();
                        tmp.OrgId = orgItem;
                        tmp.TplIds = userTplIds;
                        tmp.UserId = uid;
                        tmp.IsDeleted = false;

                        userOrgTplList.Add(tmp);
                    }

                    string orgIds = string.Join(",", orgCodeList);
                    UserOrg tmpOrg = new UserOrg();
                    tmpOrg.OrgIds = orgIds;
                    tmpOrg.UserId = uid;
                    tmpOrg.IsDeleted = false;
                    userOrgList.Add(tmpOrg);
                }

                // 查询所有没有模板权限的账号，然后遍历，按照UserZt为其生成UserOrg
                var unCoverUserZt = userZtList.Where(x => !userIdList.Contains(x.UserId)).ToList();
                var unCoverUserIds=unCoverUserZt.GroupBy(x => x.UserId).Select(x=>x.Key).ToList();
                foreach (var uId in unCoverUserIds)
                {
                    var unUserOrgList=unCoverUserZt.Where(x => uId == x.UserId).Select(x=>x.ZtCode).ToList();
                    string usOrgStr = string.Empty;
                    foreach (var ztItem in unUserOrgList)
                    {
                        var tempOrgCodeList=orgList.Where(x => x.Code.StartsWith(ztItem)).Select(x => x.Code).ToArray();
                        var tempOrgStr = string.Join(",",tempOrgCodeList);
                        usOrgStr += tempOrgStr + ",";
                    }
                    usOrgStr = usOrgStr.TrimEnd(new char[] { ','});
                    UserOrg tmpUOrg = new UserOrg();
                    tmpUOrg.IsDeleted = false;
                    tmpUOrg.OrgIds = usOrgStr;
                    tmpUOrg.UserId = uId;
                    userOrgList.Add(tmpUOrg);
                }

                foreach (var item in userOrgList)
                {
                    conn.Insert<UserOrg>(item);
                }

                foreach (var item in userOrgTplList)
                {
                    conn.Insert<UserOrgTpl>(item);
                }
            }
        }
    }
}
