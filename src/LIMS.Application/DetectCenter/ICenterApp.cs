using LIMS.DetectCenter.Dto;
using LIMS.Dtos;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Text;

namespace LIMS.DetectCenter
{
    public interface ICenterApp
    {
        // ImportRetInfoDto UploadFile([FromForm]CreateMediaDto input);
        ImportRetInfoDto UploadFile(string fileName, bool isImport,int type);
        string DownLoadExcelBySpecId(int input);
        TableInfoDto SearchDuplicateItems(int tplSpecId, DateTime beginTime, DateTime endTime,string searchId, int dateTyped);
        ModifyTableInfoDto SearchDuplicateModificationItems(int tplSpecId, DateTime beginTime, DateTime endTime, string searchId, int dateType);
        List<ModifyEditInfoDto> GetSingleModifyInfo(int dupId);
        List<DuplicationEleInfoDto> GetDuplicationEleInfo(int dupId);
        List<DuplicationEleInfoDto> GetDuplicationEleInfoByEleId(int dupId, int eleId);
        HtmlDataOperRetDto AddOrUpdateDuplicationParallelInfo(int dupId, int eleId, string formObj);
        HtmlDataOperRetDto PatchUpdateModificationItems(int tplSpecId, DateTime beginTime, DateTime endTime, string searchId, int dateType);
    }
}
