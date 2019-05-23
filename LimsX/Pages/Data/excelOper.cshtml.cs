using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LimsX.Dal;
using LimsX.Dtos;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Newtonsoft.Json;

namespace LimsX.Pages.Data
{
    public class excelOperModel : PageModel
    {
        public string OrgNodes { get; set; }

        public void OnGet()
        {
            //string userId = GetUserId();
            //List<OrgTreeNodeDto> orgTreeNode= CommonHelper.GetOrgTreeByUserId(userId);
            //string orgStr=JsonConvert.SerializeObject(orgTreeNode);
            //this.OrgNodes = orgStr;
        }


        public IActionResult OnPost() {
            string userId = GetUserId();
            return new JsonResult("123"+userId);

        }

        public IActionResult OnPostOrgTree()
        {
            string userId = GetUserId();
            List<OrgTreeNodeDto> orgTreeNode = CommonHelper.GetOrgTreeByUserId(userId);
            string orgStr = JsonConvert.SerializeObject(orgTreeNode);
            return new JsonResult(orgTreeNode);
        }

        private string GetUserId()
        {
            var userInfo = this.HttpContext.User.Claims.Where(x => x.Type == "UserId").FirstOrDefault();
            return userInfo.Value;
        }

    }
}