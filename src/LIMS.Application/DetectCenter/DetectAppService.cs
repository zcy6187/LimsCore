using Abp.Domain.Repositories;
using Abp.Domain.Uow;
using LIMS.Assay;
using LIMS.DetectCenter.Dto;
using LIMS.Dtos;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MimeDetective.Extensions;
using NPOI.HPSF;
using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

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
        private readonly IRepository<DuplicationInfoItems, int> _detectDuplicationItems;
        private readonly IRepository<ImportHistory, int> _importHistory;
        private string uploadDirName = "D:\\Ftp\\www\\upload\\";
        private string downloadDirName = "D:\\Ftp\\www\\Excels\\";

        public DetectAppService(
            IRepository<TplSpecimen, int> specRepository,
            IRepository<TplElement, int> elementRepository,
            IRepository<Attendance, int> attendanceRepostiroy,
            IRepository<AssayUser, int> assayUserRepostiroy,
            IRepository<MaterialPrintData, int> printRepostiroy,
            IRepository<DetectMainInfo, int> detectMainInfo,
            IRepository<DetectMainInfoItems, int> detectMainInfoItems,
            IRepository<ImportHistory, int> importHistory,
            IRepository<DuplicationInfoItems, int> detectDuplicationItems
            )
        {
            this._specRepository = specRepository;
            this._elementRepository = elementRepository;
            this._attendanceRepostiroy = attendanceRepostiroy;
            this._assayUserRepostiroy = assayUserRepostiroy;
            this._printRepostiroy = printRepostiroy;
            this._detectMainInfo = detectMainInfo;
            this._detectMainInfoItems = detectMainInfoItems;
            this._importHistory = importHistory;
            this._detectDuplicationItems = detectDuplicationItems;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="fileName"></param>
        /// <param name="isImport">true强制导入，false全部正确后再导入</param>
        /// <returns></returns>
        public ImportRetInfoDto UploadFile(string fileName, bool isImport)
        {
            /*状态码、描述、值*/
            string localFileName = Guid.NewGuid().ToString();
            string ext = Path.GetExtension(fileName);
            ImportRetInfoDto retDto = new ImportRetInfoDto();
            // 格式异常
            List<string> expTitleList = new List<string>();
            // 存储返回客户端的数据(标题)
            List<string> titleList = new List<string>();
            // 存储返回客户端的数据(异常信息)
            List<List<string>> retList = new List<List<string>>();
            // 存储正常的代码，写入到数据库中
            List<List<string>> storeList = new List<List<string>>();
            int curTplSpecId = 0;
            string curSpecName = string.Empty;

            var curUserId = AbpSession.UserId ?? 0;

            using (var fileStream = new FileStream($"{this.uploadDirName}\\{fileName}", FileMode.Open, FileAccess.Read))
            {
                IWorkbook wk;
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
                    ICell titleCell00 = row0.GetCell(0);
                    ICell titleCell01 = row0.GetCell(1);
                    string specId = GetCellStringValue(titleCell01);
                    curSpecName = GetCellStringValue(titleCell00);
                    int specIdInt = -1;
                    int colCount = 0; // 记录列的个数
                    if (int.TryParse(specId, out specIdInt))
                    {
                        curTplSpecId = specIdInt;
                        // 获取所有的样品元素
                        var eleList = this._elementRepository.GetAll().Where(x => x.TplSpecId == specIdInt).ToList();
                        // 获取所有元素
                        IRow row1 = sheet.GetRow(1);
                        // 存储excel的表头-元素、操作员
                        List<string> rowTitleList = new List<string>();
                        // 元素与ID对照List
                        List<TitleElementInfo> eleIdList = new List<TitleElementInfo>();
                        int curEleId = 0;
                        int curEleIndex = 0;

                        // 非元素列
                        for (int j = 0; j <= 1; j++)
                        {
                            ICell tempCell = row1.GetCell(j);
                            string tempCellValue = GetCellStringValue(tempCell);
                            rowTitleList.Add(tempCellValue);
                        }

                        // 提取所有的元素及其与ID的对照关系
                        for (int i = 2; i < 100; i++)
                        {
                            ICell tmpCell = row1.GetCell(i);
                            if (tmpCell == null)
                            {
                                colCount = i;
                                break;
                            }
                            string tmpCellValue = GetCellStringValue(tmpCell);
                            if (!string.IsNullOrEmpty(tmpCellValue))
                            {
                                rowTitleList.Add(tmpCellValue);
                                tmpCellValue = tmpCellValue.Trim();
                                // 提取元素名称
                                if (tmpCellValue != "操作员")
                                {
                                    int splitIndex = tmpCellValue.IndexOf('(');
                                    string tmpEleValue = tmpCellValue;
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
                            retDto.message = "元素无法匹配！";
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
                            // 所有操作员的名称
                            List<string> operNameList = new List<string>();
                            int nullRowCount = 0; //记录连续空行的数目
                            while (hasRow)
                            {
                                IRow tmpValRow = sheet.GetRow(rowIndex);
                                // 连续3行为null，则表示已经没有数据
                                if (tmpValRow == null)
                                {
                                    rowIndex++;
                                    nullRowCount++;
                                    if (nullRowCount > 3)
                                    {
                                        hasRow = false;
                                        break;
                                    }
                                    else
                                    {
                                        hasRow = true;
                                        continue;
                                    }

                                }
                                nullRowCount = 0;
                                // 存储Excel行数据
                                List<string> rowValList = new List<string>();
                                // 获取每一行的数据
                                for (int colIndex = 0; colIndex < colCount; colIndex++)
                                {
                                    ICell tmpValCell = tmpValRow.GetCell(colIndex);
                                    string cellValue = string.Empty;
                                    if (tmpValCell != null)
                                    {
                                        cellValue = GetCellStringValue(tmpValCell);
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
                                        retRowList.Add(attenList[0].ScanId);

                                        // 解析数据，插入到正常序列中
                                        string scanIdVal = attenList[0].ScanId;
                                        string mainIdVal = attenList[0].MainScanId;
                                        string selfCode = attenList[0].SelfCode;
                                        string printTime = attenList[0].PrintTime.ToString("yyyy-MM-dd HH:mm:ss");
                                        storeRowList.Add(mainIdVal);
                                        storeRowList.Add(scanIdVal);
                                        storeRowList.Add(selfCode);
                                        storeRowList.Add(printTime);
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
                                                else // 元素值
                                                {
                                                    storeRowList.Add(tmpValue);
                                                }
                                            }
                                        }
                                        storeList.Add(storeRowList);
                                    }
                                }
                                retRowList.AddRange(rowValList);
                                retList.Add(retRowList);
                                rowIndex++;
                            }

                            // 获取所有非法的操作员
                            var ztUserList = this._assayUserRepostiroy.GetAll().Where(x => x.OrgCode == "00010005").ToList();
                            var allOperNameList = ztUserList.Select(x => x.UserName).ToList();
                            operNameList = operNameList.Distinct().ToList();
                            foreach (var item in allOperNameList)
                            {
                                operNameList.Remove(item);
                            }
                            if (operNameList.Count > 0)
                            {
                                expTitleList.Add("操作人员不存在： " + string.Join(',', operNameList));
                            }

                            // 生成表头
                            titleList.Add("状态码");
                            titleList.Add("说明");
                            titleList.AddRange(rowTitleList);

                            // 生成表体
                            if (retList.Count > 0)
                            {
                                retList = retList.OrderByDescending(x => x[0], new ComparerNumberStr()).ToList();
                                string excelFileName = WriteDataListToLocalExcel(titleList, retList);
                                retDto.uploadFileName = excelFileName;
                            }

                            if (expTitleList.Count == 0)
                            {
                                if (storeList.Count == retList.Count) // 一切正常
                                {
                                    retDto.code = 0;
                                    retDto.message = "导入成功";
                                }
                                else
                                {
                                    retDto.code = 3;
                                    retDto.message = "存在部分错误";
                                }
                            }
                        }
                    }
                    else
                    {
                        expTitleList.Add("该样品编号不存在：" + specId);
                    }
                }
                else
                {
                    expTitleList.Add(ext + " 文件类型不支持");
                }

                if (string.IsNullOrEmpty(retDto.message))
                {
                    retDto.code = -1;
                    retDto.message = "部分格式错误";
                }
                retDto.expList = expTitleList;
                retDto.dataList = retList;
                retDto.dataTitle = titleList;

                fileStream.Close();
            }

            // 记录当前导入的信息
            ImportHistory history = new ImportHistory();
            history.createtime = DateTime.Now;
            history.operatorId = curUserId;
            history.tplSpecId = curTplSpecId;
            history.ret = retDto.message;
            history.retFileName = retDto.uploadFileName;
            int importHisId = this._importHistory.InsertAndGetId(history);

            // 将数据写入到数据库
            if (curTplSpecId > 0)
            {
                if (isImport || retList.Count == storeList.Count)  // 强制导入，或者全部正确时，将数据写入数据库
                {
                    WriteDataToTb(storeList, curTplSpecId, importHisId);
                }

            }
            return retDto;
        }

        private string GetCellStringValue(ICell tmpCell)
        {
            tmpCell.SetCellType(CellType.String);
            return tmpCell.StringCellValue;
        }

        // 将异常数据写入到数据库中
        private string WriteDataListToLocalExcel(List<string> titleList, List<List<string>> dataList)
        {
            HSSFWorkbook workbook = new HSSFWorkbook();
            ISheet sheet = workbook.CreateSheet();
            #region 右击文件 属性信息
            {
                DocumentSummaryInformation dsi = PropertySetFactory.CreateDocumentSummaryInformation();
                dsi.Company = "豫光";
                workbook.DocumentSummaryInformation = dsi;

                SummaryInformation si = PropertySetFactory.CreateSummaryInformation();
                si.Author = "张承宇"; //填加xls文件作者信息
                si.ApplicationName = "LIMS"; //填加xls文件创建程序信息
                si.LastAuthor = "张承宇"; //填加xls文件最后保存者信息
                si.Comments = "张承宇"; //填加xls文件作者信息
                si.Title = "化验数据"; //填加xls文件标题信息
                si.Subject = "化验数据";//填加文件主题信息
                si.CreateDateTime = System.DateTime.Now;
                workbook.SummaryInformation = si;
            }
            #endregion

            int rowIndex = 0;
            IRow titleRow = sheet.CreateRow(rowIndex);
            int colIndex = 0;
            foreach (var item in titleList)
            {
                ICell tmpCell = titleRow.CreateCell(colIndex);
                tmpCell.SetCellValue(item);
                colIndex++;
            }
            rowIndex++;
            foreach (var rowList in dataList)
            {
                IRow contentRow = sheet.CreateRow(rowIndex);
                colIndex = 0;
                foreach (var item in rowList)
                {
                    ICell tmpCell = contentRow.CreateCell(colIndex);
                    tmpCell.SetCellValue(item);
                    colIndex++;
                }
                rowIndex++;
            }

            string fileName = string.Format("{0}{1}.xls", DateTime.Now.ToString("yyyyMMddHHmm"), Guid.NewGuid().ToString());
            string filePath = this.downloadDirName + fileName;
            using (var fs = new FileStream(filePath, FileMode.CreateNew, FileAccess.Write))
            {
                workbook.Write(fs);
            }

            return fileName;
        }

        /// <summary>
        /// 将数据写入到数据库中
        /// </summary>
        /// <returns></returns>
        private bool WriteDataToTb(List<List<string>> dataList, int specId, int hisId)
        {
            List<DuplicationInfoItems> duplicationList = new List<DuplicationInfoItems>();
            List<DuplicationElements> duplicationEleList = new List<DuplicationElements>();
            List<OperValInfo> operList = new List<OperValInfo>();
            List<string> mainIdList = new List<string>(); // 主Id列表
            List<string> scanIdList = new List<string>(); // 子id列表
            var userId = AbpSession.UserId ?? 0;
            var userList = this._assayUserRepostiroy.GetAll().OrderByDescending(x => x.Id);


            /* 1. 获取所有的元素值、操作员值、主ID
             * 2. 检测子ID是否异常-数据表中是否已经存在，如果有异常则返回异常（暂时先不检测）
             * 3. 生成主条码信息，插入数据，并获取其ID
             * 4. 更改子id数据中的mainID和用户信息，并插入到数据库中
             */

            #region 第1步
            // [mainIdVal,scanIdVal,selfCode,printTime,值]
            //  值以"&#&*"为分隔符（cellType+eleid+值）,-1 操作员，eleId
            foreach (var rowList in dataList)
            {
                string mainId = rowList[0];
                string scanId = rowList[1];
                string selfCode = rowList[2];
                string printTime = rowList[3];
                mainIdList.Add(mainId);
                scanIdList.Add(scanId);


                for (int i = 4; i < rowList.Count; i++)
                {
                    string tmpStr = rowList[i];
                    string[] valArray = tmpStr.Split("&#&*", StringSplitOptions.RemoveEmptyEntries);
                    if (valArray[0] == "0") // 操作员
                    {
                        var operItem = new OperValInfo();
                        operItem.scanId = scanId;
                        operItem.eleId = int.Parse(valArray[1]);
                        operItem.cellValIndex = int.Parse(valArray[2]);
                        operItem.operName = valArray[3];
                        var userItem = userList.Where(x => x.UserName == valArray[3]).FirstOrDefault();
                        if (userItem != null)
                        {
                            operItem.operId = userItem.Id;
                        }
                        else
                        {
                            operItem.operId = -1;
                        }
                        operList.Add(operItem);
                    }
                    if (valArray[0] == "1") // 元素值
                    {
                        // 子ID
                        var duplicateItem = new DuplicationInfoItems();
                        duplicateItem.mainScanId = mainId;
                        duplicateItem.scanId = scanId;
                        duplicateItem.selfCode = selfCode;
                        duplicateItem.mainId = int.Parse(valArray[2]); // 暂时存储，所在excel中的列,之后替换
                        duplicateItem.modifyTime = DateTime.Now;
                        duplicateItem.printTime = DateTime.Parse(printTime);
                        duplicateItem.hisId = hisId;
                        duplicateItem.operId = userId;
                        duplicateItem.tplSpecId = specId;
                        duplicationList.Add(duplicateItem);
                        // 元素
                        var duplicateEle = new DuplicationElements();
                        duplicateEle.tplSpecId = specId;
                        duplicateEle.tplEleId = int.Parse(valArray[1]);
                        duplicateEle.eleValue = valArray[3];
                        duplicateEle.desInfo = mainId; // // 暂时存储，主ID
                        duplicateEle.modifyTime = DateTime.Now;
                        duplicateEle.modifyUserId = userId;
                        duplicateEle.modifyUserName = scanId; // 暂时存储，子ID
                        duplicateEle.duplicateId = int.Parse(valArray[2]); // 暂时存储，所在excel中的列,之后替换
                        duplicationEleList.Add(duplicateEle);
                    }
                }
            }
            #endregion

            SqlOper.SqlHelper myDal = new SqlOper.SqlHelper();
            #region 第3步  主条码插入数据库
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

            //获取主ID信息
            var printMain = this._printRepostiroy.GetAll().Where(x => newMainIdList.Contains(x.MainScanId)).ToList();
            List<DetectMainInfo> detectMainList = new List<DetectMainInfo>();
            // 将没有主ID的数据插入，生成主ID
            newMainIdList.ForEach(s =>
            {
                var printItem = printMain.FirstOrDefault(x => x.MainScanId == s);
                if (printItem != null)
                {
                    DetectMainInfo mainItem = new DetectMainInfo();
                    mainItem.operatorId = userId;
                    mainItem.specId = specId;
                    mainItem.mainScanId = s;
                    mainItem.chs = printItem.chs;
                    mainItem.fhtime = printItem.fhtime;
                    mainItem.verdor = printItem.vendor;
                    mainItem.IsDeleted = false;
                    mainItem.createtime = DateTime.Now;
                    mainItem.modifyTime = DateTime.Now;
                    mainItem.fhId = -1;
                    mainItem.hisId = hisId;
                    detectMainList.Add(mainItem);
                }
            });
            // 主条码插入到数据库中
            myDal.WriteDetectMainsToDb(detectMainList);
            #endregion

            #region 第4步 修改子id数据
            // 去除重复的子条码
            var newScanIdList = scanIdList.Distinct().ToList();
            duplicationList=duplicationList.Distinct(new ComparerDuplication()).ToList(); // 去重
            var repeateScanIds = this._detectDuplicationItems.GetAll().Where(x => newScanIdList.Contains(x.scanId) && x.tplSpecId==specId).Select(x=>x.scanId).ToList();
            duplicationList.RemoveAll(x=>repeateScanIds.Contains( x.scanId));
            // 获取主ID
            var curMainItems = this._detectMainInfo.GetAll().Where(x => distinctMainIdList.Contains(x.mainScanId) && x.specId==specId).ToList();
            // 遍历主ID，将主ID的信息更新到子ID信息中
            curMainItems.ForEach(item =>
            {
                duplicationList.Where(x => x.mainScanId == item.mainScanId).ToList().ForEach(subItem =>
                {
                    subItem.mainId = item.Id;
                });
            });
            //// 遍历子ID，补充信息
            //duplicationList.ForEach(item =>
            //{
            //    var mainItem = curMainItems.Where(x => x.mainScanId == item.mainScanId).First();
            //    item.mainId = mainItem.Id;
            //});

            // 将子条码写入到数据库中
            myDal.WriteDetectDuplicationToDb(duplicationList);
            #endregion

            #region 第5步 修改元素数据
            // 从数据库中获取所有的平行样
            var allDuplicateItems = this._detectDuplicationItems.GetAll().Where(x => newScanIdList.Contains(x.scanId) && x.tplSpecId == specId).ToList();
            // 填充元素其它信息
            duplicationEleList.ForEach(item =>
            {
                var operItem = operList.Where(x => x.eleId == item.tplEleId && x.cellValIndex == item.duplicateId && x.scanId == item.modifyUserName).FirstOrDefault();
                if (operItem != null)
                {
                    item.operId = operItem.operId;
                    item.operName = operItem.operName;
                }
                var curDuplicateItem=allDuplicateItems.Where(x => x.scanId ==item.modifyUserName).First();
                item.duplicateId = curDuplicateItem.Id;
            });
            myDal.WriteDuplicationElementsToDb(duplicationEleList);
            #endregion

            return true;
        }

        public string DownLoadExcelBySpecId(int input)
        {
            var eleList = this._elementRepository.GetAll().Where(x => x.TplSpecId == input).ToList();
            var firstItem = eleList[0];
            string specName = firstItem.SpecName;
            string specId = firstItem.TplSpecId.ToString();
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
        public int eleId { get; set; }
        public string scanId { get; set; }
        public int operId { get; set; }
        public string operName { get; set; }
        public int cellValIndex { get; set; }
    }

    class ComparerNumberStr : IComparer<String>
    {
        public int Compare(String x, String y)
        {
            return int.Parse(x) - int.Parse(y);
        }
    }

    class ComparerDuplication: IEqualityComparer<DuplicationInfoItems>
    {
        bool IEqualityComparer<DuplicationInfoItems>.Equals(DuplicationInfoItems x, DuplicationInfoItems y)
        {
            return x.scanId == y.scanId;
        }

        int IEqualityComparer<DuplicationInfoItems>.GetHashCode(DuplicationInfoItems obj)
        {
            return obj.Id;
        }
    }
}
