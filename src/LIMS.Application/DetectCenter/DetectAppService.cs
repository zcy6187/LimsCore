using Abp.Domain.Repositories;
using LIMS.Assay;
using LIMS.DetectCenter.Dto;
using LIMS.Dtos;
using Microsoft.AspNetCore.Mvc;
using MimeDetective.Extensions;
using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace LIMS.DetectCenter
{
    public class DetectAppService : LIMSAppServiceBase, ICenterApp
    {
        private readonly IRepository<TplSpecimen, int> _specRepository;
        private readonly IRepository<TplElement, int> _elementRepository;
        private readonly IRepository<Attendance, int> _attendanceRepostiroy;
        private readonly IRepository<AssayUser, int> _assayUserRepostiroy;
        private readonly IRepository<MaterialPrintData, int> _printRepostiroy;
        private readonly IRepository<DetectMainInfo, int> _detectMainInfo;
        private readonly IRepository<DetectMainInfoItems, int> _detectMainInfoItems;

        public DetectAppService(
            IRepository<TplSpecimen, int> specRepository,
            IRepository<TplElement, int> elementRepository,
            IRepository<Attendance, int> attendanceRepostiroy,
            IRepository<AssayUser, int> assayUserRepostiroy,
            IRepository<MaterialPrintData, int> printRepostiroy,
            IRepository<DetectMainInfo, int> detectMainInfo,
            IRepository<DetectMainInfoItems, int> detectMainInfoItems
            )
        {
            this._specRepository = specRepository;
            this._elementRepository = elementRepository;
            this._attendanceRepostiroy = attendanceRepostiroy;
            this._assayUserRepostiroy = assayUserRepostiroy;
            this._printRepostiroy = printRepostiroy;
            this._detectMainInfo = detectMainInfo;
            this._detectMainInfoItems = detectMainInfoItems;
        }

        public ImportRetInfoDto UploadFile([FromForm]CreateMediaDto input)
        {
            /*状态码、描述、值*/
            string dir = "D:\\Ftp\\www\\upload";
            var stream = input.File.OpenReadStream();
            string fileName = Guid.NewGuid().ToString();
            string ext = stream.GetFileType().Extension;
            ImportRetInfoDto retDto = new ImportRetInfoDto();
            using (var fileStream = new FileStream($"{dir}\\{fileName}.{ext}", FileMode.Create))
            {
                IWorkbook wk;
                stream.CopyTo(fileStream);
                if (ext == ".xlsx" || ext == ".xls")
                {
                    //判断excel的版本
                    if (ext == ".xlsx")
                    {
                        wk = new XSSFWorkbook(fileStream);
                    }
                    else
                    {
                        wk = new HSSFWorkbook(fileStream);
                    }
                    ISheet sheet = wk.GetSheetAt(0);
                    IRow row0 = sheet.GetRow(0);
                    ICell titleCell01 = row0.GetCell(1);
                    string specId = titleCell01.StringCellValue;
                    int specIdInt = -1;
                    int colCount = 0; // 记录列的个数

                    if (int.TryParse(specId, out specIdInt))
                    {
                        // 获取所有的样品元素
                        var eleList = this._elementRepository.GetAll().Where(x => x.SpecId == specIdInt).ToList();
                        // 获取所有元素
                        IRow row1 = sheet.GetRow(1);
                        // 存储excel的表头-元素、操作员
                        List<string> rowTitleList = new List<string>();
                        // 标题异常
                        List<string> expTitleList = new List<string>();
                        // 元素与ID对照List
                        List<TitleElementInfo> eleIdList = new List<TitleElementInfo>();
                        int curEleId = 0;
                        int curEleIndex = 0;
                        // 提取所有的元素及其与ID的对照关系
                        for (int i = 2; i < 100; i++)
                        {
                            ICell tmpCell = row1.GetCell(i);
                            if (tmpCell == null)
                            {
                                colCount = i;
                                break;
                            }
                            string tmpCellValue = tmpCell.StringCellValue;
                            if (!string.IsNullOrEmpty(tmpCellValue))
                            {
                                rowTitleList.Add(tmpCellValue);
                                tmpCellValue = tmpCellValue.Trim();
                                // 提取元素名称
                                if (tmpCellValue != "操作员")
                                {
                                    int splitIndex = tmpCellValue.IndexOf('(');
                                    string tmpEleValue = string.Empty;
                                    if (splitIndex > 0)
                                    {
                                        tmpEleValue = tmpCellValue.Substring(0, splitIndex);
                                    }
                                    var tmpEle = eleList.Where(x => x.ElementName.ToLower() == tmpEleValue.ToLower()).FirstOrDefault();
                                    if (tmpEle == null)
                                    {
                                        expTitleList.Add(string.Format("元素{0} : 不存在", tmpCellValue));
                                    }
                                    else
                                    {
                                        // 存储对应的元素内容
                                        TitleElementInfo tmpSelectDto = new TitleElementInfo();
                                        tmpSelectDto.eleId = tmpEle.Id.ToString();
                                        tmpSelectDto.cellName = tmpEleValue;
                                        tmpSelectDto.cellId = i;
                                        tmpSelectDto.cellType = 1;
                                        tmpSelectDto.cellValIndex = i;
                                        eleIdList.Add(tmpSelectDto);
                                        curEleId = tmpEle.Id;
                                        curEleIndex = i;
                                    }
                                }
                                else
                                {
                                    // 存储对应的元素内容
                                    TitleElementInfo tmpSelectDto = new TitleElementInfo();
                                    tmpSelectDto.eleId = curEleId.ToString();
                                    tmpSelectDto.cellName = tmpCellValue;
                                    tmpSelectDto.cellId = i;
                                    tmpSelectDto.cellType = 0;
                                    tmpSelectDto.cellValIndex = curEleIndex;
                                    eleIdList.Add(tmpSelectDto);
                                }
                            }

                        }

                        // 如果元素有异常信息
                        if (expTitleList.Count > 0)
                        {
                            retDto.code = 1;
                            retDto.message = "标题有错误！";
                            retDto.expList = expTitleList;
                        }
                        else
                        {
                            // 读取每一行的数据
                            int rowIndex = 2;
                            bool hasRow = true;
                            DateTime endTime = DateTime.Now;
                            DateTime beginTime = endTime.AddDays(-30);
                            // 获取该样品的所有签到信息
                            var attendanceList = this._printRepostiroy.GetAll().Where(x => x.PrintTime > beginTime && x.PrintTime < endTime && x.TplSpecId == specIdInt).ToList();

                            // 存储返回客户端的代码(异常信息)
                            List<List<string>> retList = new List<List<string>>();
                            // 存储正常的代码，写入到数据库中
                            List<List<string>> storeList = new List<List<string>>();
                            // 所有操作员的名称
                            List<string> operNameList = new List<string>();
                            while (hasRow)
                            {
                                IRow tmpValRow = sheet.GetRow(rowIndex);

                                if (tmpValRow == null)
                                {
                                    hasRow = false;
                                    break;
                                }
                                // 存储Excel行数据
                                List<string> rowValList = new List<string>();
                                // 获取每一行的数据
                                for (int colIndex = 0; colIndex < colCount; colIndex++)
                                {
                                    ICell tmpValCell = tmpValRow.GetCell(colIndex);
                                    string cellValue = string.Empty;
                                    if (tmpValCell != null)
                                    {
                                        cellValue = tmpValCell.StringCellValue;
                                    }
                                    rowValList.Add(cellValue);
                                }
                                string scanId = rowValList[1];
                                List<string> retRowList = new List<string>();
                                List<string> storeRowList = new List<string>();
                                /* 0 正常；1 请输入条码；2 条码不存在；3 条码重复 */
                                if (string.IsNullOrEmpty(scanId))
                                {
                                    retRowList.Add("1");
                                    retRowList.Add("请输入条码");
                                }
                                else
                                {
                                    var attenList = attendanceList.Where(x => x.ScanId.EndsWith(scanId)).ToList();
                                    if (attenList.Count == 0)
                                    {
                                        retRowList.Add("2");
                                        retRowList.Add("条码不存在");
                                    }
                                    if (attenList.Count > 1)
                                    {
                                        string scanIds = String.Join(',', attenList.Select(x => x.ScanId).ToArray());
                                        retRowList.Add("3");
                                        retRowList.Add("条码重复：" + scanIds);
                                    }
                                    // 正常数据时写入
                                    if (attenList.Count == 1)
                                    {
                                        retRowList.Add("0");
                                        retRowList.Add("正常数据");

                                        string scanIdVal = attenList[0].ScanId;
                                        string mainIdVal = attenList[0].MainScanId;
                                        storeRowList.Add(mainIdVal);
                                        storeRowList.Add(scanIdVal);
                                        // 将正常值写入要存储的队列
                                        foreach (var item in eleIdList)
                                        {
                                            string val = rowValList[item.cellId];
                                            if (!string.IsNullOrEmpty(val))
                                            {
                                                string tmpValue = string.Format("{0}&#&*{1}&#&*{2}&#&*{3}", item.cellType, item.eleId, item.cellValIndex, val);
                                                // 操作员元素id为元素id
                                                // 操作员
                                                if (item.cellType == 0)
                                                {

                                                    storeRowList.Add(tmpValue);
                                                    operNameList.Add(val);
                                                }
                                                else
                                                {
                                                    storeRowList.Add(tmpValue);
                                                }
                                            }

                                        }
                                        storeList.Add(storeRowList);
                                    }
                                }
                                foreach (var item in rowValList)
                                {
                                    retRowList.Add(item);
                                }
                                retList.Add(retRowList);
                            }

                            // 获取所有非法的操作员
                            var ztUserList = this._assayUserRepostiroy.GetAll().Where(x => x.OrgCode == "00010005").ToList();
                            var allOperNameList = ztUserList.Select(x => x.UserName).ToList();
                            foreach (var item in allOperNameList)
                            {
                                operNameList.Remove(item);
                            }
                            if (operNameList.Count > 0)
                            {
                                expTitleList.Add(string.Format("操作员不存在：{0}", string.Join(",", operNameList)));
                                retDto.code = 2;
                                retDto.message = "操作人员有错误";
                                retDto.expList = operNameList;
                            }

                            if (expTitleList.Count == 0)
                            {
                                List<string> titleList = new List<string>();
                                titleList.Add("状态码");
                                titleList.Add("说明");
                                titleList.AddRange(rowTitleList);
                                if (storeList.Count == retList.Count)
                                {
                                    retDto.code = 0;
                                    retDto.message = "一切正常";
                                    retDto.dataList = retList;
                                    retDto.dataTitle = titleList;
                                }
                                else
                                {
                                    retDto.code = 3;
                                    retDto.message = "存在部分错误";
                                    retDto.dataList = retList;
                                    retDto.dataTitle = titleList;
                                }
                            }
                        }
                    }
                    else
                    {
                        retDto.code = -2;
                        retDto.message = "样品编号不存在";
                    }
                }
                else
                {
                    retDto.code = -1;
                    retDto.message = "文件类型不支持";
                }
                fileStream.Close();
            }
            return retDto;
        }

        /// <summary>
        /// 将数据写入到数据库中
        /// </summary>
        /// <returns></returns>
        private bool WriteDataToTb(List<List<string>> dataList, int specId)
        {
            List<DuplicationInfoItems> duplicationList = new List<DuplicationInfoItems>();
            List<OperValInfo> operList = new List<OperValInfo>();
            List<string> mainIdList = new List<string>(); // 主Id列表
            List<string> scanIdList = new List<string>(); // 子id列表
            var userId=AbpSession.UserId ?? 0;


            // [mainIdVal,scanIdVal,值]
            //  值以"&#&*"为分隔符（cellType+eleid+值）,-1 操作员，eleId
            foreach (var rowList in dataList)
            {
                string mainId = rowList[0];
                string scanId = rowList[1];
                mainIdList.Add(mainId);
                scanIdList.Add(scanId);

                /* 1. 获取所有的元素值、操作员值、主ID
                 * 2. 检测子ID是否异常-数据表中是否已经存在，如果有异常则返回异常（暂时先不检测）
                 * 3. 生成主ID，插入数据，并获取其ID
                 * 4. 更改子id数据中的mainID
                 * 5. 更改子id数据中的
                 */

                #region 第1步
                for (int i = 2; i < rowList.Count; i++)
                {
                    string tmpStr = rowList[i];
                    string[] valArray = tmpStr.Split("&#&*", StringSplitOptions.RemoveEmptyEntries);
                    if (valArray[0] == "0") // 操作员
                    {
                        var operItem = new OperValInfo();
                        operItem.scanId = scanId;
                        operItem.eleId = valArray[1];
                        operItem.cellValIndex = valArray[2];
                        operItem.operName = valArray[3];
                        operList.Add(operItem);
                    }
                    if (valArray[0] == "1") // 元素值
                    {
                        var duplicateItem = new DuplicationInfoItems();
                        duplicateItem.mainScanId = mainId;
                        duplicateItem.scanId = scanId;
                        duplicateItem.tplSpecId = specId;
                        duplicateItem.tplEleId = int.Parse(valArray[1]);
                        duplicateItem.eleValue = valArray[3];
                        duplicateItem.mainId = int.Parse(valArray[2]); // 暂时存储，所在excel中的列,之后替换
                    }
                }
                #endregion

                #region 第3步
                // 获取所有mainItems
                // 去除重复值
                var distinctMainIdList = mainIdList.Distinct().ToList();
                var newMainIdList = new List<string>();
                newMainIdList.AddRange(distinctMainIdList);

                var mainItems = this._detectMainInfo.GetAll().Where(x => distinctMainIdList.Contains(x.mainScanId)).ToList();
                // 清除已经存在的MainId
                foreach (var item in mainItems)
                {
                    newMainIdList.Remove(item.mainScanId);
                }
                
                // 将没有主ID的数据插入，生成主ID
                newMainIdList.ForEach(s =>
                {
                    DetectMainInfo mainItem = new DetectMainInfo();
                    mainItem.operatorId = userId;
                    mainItem.specId = specId;
                    mainItem.operatorId = userId;
                    mainItem.mainScanId=
                });

                #endregion


            }

            return false;
        }

        public string DownLoadExcelBySpecId(int input)
        {
            var eleList = this._elementRepository.GetAll().Where(x => x.TplSpecId == input).ToList();
            var firstItem = eleList[0];
            string specName = firstItem.SpecName;
            string specId = firstItem.SpecId.ToString();
            List<string> titleList = new List<string>();
            titleList.Add("编号");
            titleList.Add("条码");
            foreach (var eleItem in eleList)
            {
                titleList.Add(string.Format("{0}({1})", eleItem.ElementName, eleItem.UnitName));
                titleList.Add("操作员");
            }

            string templateDir = "D:\\Ftp\\www\\Excels\\";
            string uid = (AbpSession.UserId ?? 0).ToString();
            string filename = DateTime.Now.ToString("yyyyMMddHHmmss") + uid + "t.xls";
            string filePath = templateDir + filename;
            // 第一行 样品名称、样品编号
            // 第二行 编号、条码、元素、操作员
            using (FileStream fsWrite = File.Create(filePath))
            {
                IWorkbook wk = new HSSFWorkbook();
                ISheet sheet = wk.CreateSheet(specName);

                IRow row0 = sheet.CreateRow(0);
                ICell cell00 = row0.CreateCell(0);
                cell00.SetCellValue(specName);
                ICell cell01 = row0.CreateCell(1);
                cell01.SetCellValue(specId.ToString());

                IRow row1 = sheet.CreateRow(1);
                int cellIndex = 0;
                foreach (var rowData in titleList)
                {
                    ICell tempCell = row1.CreateCell(cellIndex);
                    tempCell.SetCellValue(rowData);
                    cellIndex++;
                }

                wk.Write(fsWrite);
            }
            return filename;
        }
    }

    class TitleElementInfo
    {
        public string eleId { get; set; }
        public int cellId { get; set; }
        public int cellType { get; set; } // 0-操作员，1-元素值
        public string cellName { get; set; }
        public int cellValIndex { get; set; } // 存储元素值所在的列（用于）
    }

    class ElementValInfo
    {
        public int eleId { get; set; }
        public string eleVal { get; set; }
        public string operName { get; set; }
    }

    class OperValInfo
    {
        public string eleId { get; set; }
        public string scanId { get; set; }
        public string operName { get; set; }
        public string cellValIndex { get; set; }
    }
}
