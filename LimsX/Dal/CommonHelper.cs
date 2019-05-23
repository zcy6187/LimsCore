using Dapper;
using LimsX.Core;
using LimsX.Dtos;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;

namespace LimsX.Dal
{
    public class CommonHelper
    {
        public static string connStr = "Data Source=10.68.2.2;DataBase=LIMS;User ID=SA;Password=Sa***123;Max Pool Size=512;";

        // 登陆信息
        public static LimsUser CheckLogin(LoginUserDto ui)
        {
            using (var conn = new SqlConnection(connStr))
            {
                var item=conn.Query<LimsUser>("select * from LimsUser where UserName=@UserName and Password=@Userpassword",ui).FirstOrDefault();
                return item;
            }
        }

        #region 组织信息
        public static List<OrgTreeNodeDto> GetOrgTreeByUserId(string uId)
        {
            using (var conn = new SqlConnection(connStr))
            {
                var item = conn.Query<UserOrg>("select * from sys_userOrg where userId=@uId and IsDeleted=0",new { uId=uId }).FirstOrDefault();

                if (item != null)
                {
                    string[] orgCodeArray=item.OrgIds.Split(new char[] { ',' },StringSplitOptions.RemoveEmptyEntries);
                    if (orgCodeArray.Length <1)
                    {
                        return null;
                    }

                    List<string> orgPathList = new List<string>();
                    foreach (var orgCodeItem in orgCodeArray)
                    {
                        AddAllPath(orgCodeItem,orgPathList);
                    }
                    IEnumerable<string> userOrgCodes=orgCodeArray.Union(orgPathList.ToArray());
                    
                    var orgList = conn.Query<OrgInfo>("select * from sys_orgInfo").ToList();
                    var userOrgList=orgList.Where(x => userOrgCodes.Contains(x.Code)).ToList();
                    List<OrgTreeNodeDto> treeNodes=GetOrgTreeByOrgList(userOrgList);

                    return treeNodes;
                }
                return null;
            }
        }

        // 按orgCode生成其过往路径
        private static void AddAllPath(string orgCode, List<string> pathList)
        {
            if (orgCode.Length > 8)
            {
                string tmpStr = orgCode.Substring(0, orgCode.Length - 4);
                pathList.Add(tmpStr);
                AddAllPath(tmpStr, pathList);
            }
        }

        // 将所有组织生成树
        private static List<OrgTreeNodeDto> GetOrgTreeByOrgList(List<OrgInfo> orgList)
        {
            var rootLayer = orgList.Min(x => x.Layer);

            var rootOrg = orgList.Where(x => x.Layer == rootLayer).OrderBy(x => x.Code).ToList();
            List<OrgTreeNodeDto> treeNodes = new List<OrgTreeNodeDto>();
            foreach (var item in rootOrg)
            {
                OrgTreeNodeDto rootNode = new OrgTreeNodeDto()
                {
                    id = item.Code.ToString(),
                    name = item.AliasName,
                };
                if (item.Code.Length < 13)
                {
                    rootNode.open = true;
                    rootNode.isChecked = true;
                }
                else
                {
                    rootNode.open = false;
                    rootNode.isChecked = false;
                }
                List<OrgTreeNodeDto> orgNodes = GetTreeNodeListByParent(item, orgList);
                rootNode.children = orgNodes;
                treeNodes.Add(rootNode);
            }
            return treeNodes;
        }

        private static List<OrgTreeNodeDto> GetTreeNodeListByParent(OrgInfo parent, List<OrgInfo> orgList)
        {
            var childrenList = orgList.Where(x => x.FatherCode == parent.Code).ToList();

            if (childrenList.Count() == 0)
            {
                return null;
            }

            List<OrgTreeNodeDto> nodeList = new List<OrgTreeNodeDto>();
            foreach (var item in childrenList)
            {
                OrgTreeNodeDto tempNode = new OrgTreeNodeDto()
                {
                    id = item.Code.ToString(),
                    name = item.AliasName
                };
                if (item.Code.Length < 13)
                {
                    tempNode.open = true;
                    tempNode.isChecked = true;
                }
                else
                {
                    tempNode.open = false;
                    tempNode.isChecked = false;
                }
                List<OrgTreeNodeDto> childNodeList = GetTreeNodeListByParent(item, orgList);
                tempNode.children = childNodeList;
                nodeList.Add(tempNode);
            }

            return nodeList;
        }
        #endregion
    }
}
