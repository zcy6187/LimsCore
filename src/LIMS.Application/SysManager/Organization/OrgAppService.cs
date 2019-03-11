using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Abp.AutoMapper;
using Abp.Domain.Repositories;
using LIMS.SysManager.Organization.Dto;

namespace LIMS.SysManager.Organization
{
    public class OrgAppService : LIMSAppServiceBase, IOrgAppService
    {
        private readonly IRepository<OrgInfo, int> _repository;
        private readonly IRepository<UserZt, int> _userZtRepository;

        public OrgAppService(
            IRepository<OrgInfo, int> repository,
            IRepository<UserZt, int> userZtRepository)
        {
            _repository = repository;
            this._userZtRepository = userZtRepository;
        }

        
        // 获取整个组织机构
        public List<OrgDto> GetOrgInfos()
        {
            var orgInfoList = _repository.GetAll().Where(x => x.IsUse).ToList();
            List<OrgDto> orgs = orgInfoList.MapTo<List<OrgDto>>();
            return orgs;
        }

        #region 机构树生成
        // 将整个组织结构生成树
        public List<OrgTreeNodeDto> GetOrgTree()
        {
            var orgInfoList = _repository.GetAll().Where(x => x.IsUse).ToList();

            var rootOrg = orgInfoList.Where(x => x.FatherCode == "0").First();

            OrgTreeNodeDto rootNode = new OrgTreeNodeDto()
            {
                id = rootOrg.Id.ToString(),
                expanded = true,
                key = rootOrg.Code,
                title = rootOrg.AliasName,
            };

            List<OrgTreeNodeDto> orgNodes = GetTreeNodeListByParent(rootOrg, orgInfoList);
            rootNode.children = orgNodes;

            List<OrgTreeNodeDto> treeNodes = new List<OrgTreeNodeDto>();
            treeNodes.Add(rootNode);

            return treeNodes;

        }

        // 递归获取所有子结点
        private List<OrgTreeNodeDto> GetTreeNodeListByParent(OrgInfo parent, List<OrgInfo> orgList)
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
                    id = item.Id.ToString(),
                    key = item.Code,
                    title = item.AliasName,
                };

                List<OrgTreeNodeDto> childNodeList = GetTreeNodeListByParent(item, orgList);
                if (childNodeList == null)
                {

                    tempNode.expanded = false;
                    tempNode.isLeaf = true;
                }
                else
                {
                    bool isExpanded = item.Layer >= 3 ? false : true;
                    tempNode.expanded = isExpanded;
                    tempNode.isLeaf = false;
                    tempNode.children = childNodeList;
                }
                nodeList.Add(tempNode);
            }

            return nodeList;
        }

        public List<OrgTreeNodeDto> GetOrgTreeByRootCode(string code)
        {
            var orgInfoList = _repository.GetAll().Where(x => x.IsUse).ToList();

            var rootOrg = orgInfoList.Where(x => x.Code == code).First();

            OrgTreeNodeDto rootNode = new OrgTreeNodeDto()
            {
                id = rootOrg.Id.ToString(),
                expanded = true,
                key = rootOrg.Code,
                title = rootOrg.AliasName,
            };

            List<OrgTreeNodeDto> orgNodes = GetTreeNodeListByParent(rootOrg, orgInfoList);
            rootNode.children = orgNodes;

            List<OrgTreeNodeDto> treeNodes = new List<OrgTreeNodeDto>();
            treeNodes.Add(rootNode);

            return treeNodes;
        }
        #endregion

        // 根据用户的帐套获取组织机构
        public List<OrgTreeNodeDto> GetOrgTreeByZtCode()
        {
            long userId=this.AbpSession.UserId??0;
            var ztCodeArray=this._userZtRepository.GetAll().Where(x => x.UserId == userId).Select(x => x.ZtCode).ToArray();

            var orgInfoList = _repository.GetAll().Where(x => x.IsUse).ToList();

            var rootOrgList = orgInfoList.Where(x => ztCodeArray.Contains(x.Code)).ToList();

            List<OrgTreeNodeDto> treeNodes = new List<OrgTreeNodeDto>();
            foreach (var rootOrg in rootOrgList)
            {
                OrgTreeNodeDto rootNode = new OrgTreeNodeDto()
                {
                    id = rootOrg.Id.ToString(),
                    expanded = true,
                    key = rootOrg.Code,
                    title = rootOrg.AliasName,
                };

                List<OrgTreeNodeDto> orgNodes = GetTreeNodeListByParent(rootOrg, orgInfoList);
                rootNode.children = orgNodes;
                treeNodes.Add(rootNode);
            }
            return treeNodes;
        }

        // 获取单个组织结构信息
        public EditOrgDto GetSingleOrgInfo(int inputId)
        {
            var orgInfo = _repository.Get(inputId);
            OrgDto tempOrg = orgInfo.MapTo<OrgDto>();
            EditOrgDto org = tempOrg.MapTo<EditOrgDto>();

            return org;
        }

        public async Task AddOrgInfo(CreateOrgDto input)
        {
            var parentOrgInfo = _repository.Get(input.ParentId);

            OrgInfo tempOrg = new OrgInfo();
            tempOrg.CreateTime = DateTime.Now;
            tempOrg.AliasName = input.AliasName;
            tempOrg.FullName = parentOrgInfo.FullName + input.OrgName;
            tempOrg.OrgName = input.OrgName;
            tempOrg.IsUse = true;
            tempOrg.Layer = parentOrgInfo.Layer + 1;
            tempOrg.FatherCode = parentOrgInfo.Code;

            // 获取本层级最大code
            var maxOrgInfo = _repository.GetAll().Where(x => x.FatherCode == parentOrgInfo.Code).OrderByDescending(x => x.Code).FirstOrDefault();
            // 获取本层级最大orderNo
            var maxOrderNo = _repository.GetAll().Where(x => x.FatherCode == parentOrgInfo.Code).OrderByDescending(x => x.OrderNo).FirstOrDefault();
            if (maxOrgInfo != null)
            {
                tempOrg.Code = GetMaxCode(maxOrgInfo.Code);
            }
            else
            {
                tempOrg.Code = parentOrgInfo.Code + "0001";
            }
            // 最大排序号
            if (maxOrderNo != null)
            {
                tempOrg.OrderNo = maxOrderNo.OrderNo + 1;
            }
            else
            {
                tempOrg.OrderNo = 1;
            }

            await _repository.InsertAsync(tempOrg);
        }

        // 获取尾部4个字符串
        private string GetMaxCode(string code)
        {
            string tailCode = code.Substring(code.Length - 4);
            string headCode = code.Substring(0, code.Length - 4);
            tailCode.TrimStart('0');
            int maxOrder = int.Parse(tailCode) + 1;
            string newCode = "000" + maxOrder.ToString();
            newCode = headCode + newCode.Substring(newCode.Length - 4);
            return newCode;
        }

        // 删除组织
        public async Task DeleteOrgInfo(int inputId)
        {
            var item=_repository.Get(inputId);
            item.IsUse = false;
            await _repository.UpdateAsync(item);
        }

        // 编辑信息
        public async Task EditOrgInfo(EditOrgDto input)
        {
            var item = _repository.Get(input.Id);
            item.OrgName = input.OrgName;
            item.AliasName = input.AliasName;
            string fullName=item.FullName;
            fullName.Replace(item.OrgName,input.OrgName);
            item.FullName = fullName;

            await _repository.UpdateAsync(item);

        }
    }
}
