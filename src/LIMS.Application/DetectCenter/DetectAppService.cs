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
        private readonly IRepository<DuplicationElements, int> _detectDuplicationElements;
        private readonly IRepository<DuplicationModifyElements, int> _detectDuplicationModifyElements;
        private readonly IRepository<ImportHistory, int> _importHistory;
        private string uploadDirName = "D:\\Ftp\\www\\upload\\";
        private string downloadDirName = "D:\\Ftp\\www\\Excels\\";
        private ElementGatherInfo rowCreator;

        public DetectAppService(
            IRepository<TplSpecimen, int> specRepository,
            IRepository<TplElement, int> elementRepository,
            IRepository<Attendance, int> attendanceRepostiroy,
            IRepository<AssayUser, int> assayUserRepostiroy,
            IRepository<MaterialPrintData, int> printRepostiroy,
            IRepository<DetectMainInfo, int> detectMainInfo,
            IRepository<DetectMainInfoItems, int> detectMainInfoItems,
            IRepository<ImportHistory, int> importHistory,
            IRepository<DuplicationInfoItems, int> detectDuplicationItems,
            IRepository<DuplicationElements, int> detectDuplicationElements,
            IRepository<DuplicationModifyElements, int> detectDuplicationModifyElements
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
            this._detectDuplicationElements = detectDuplicationElements;
            this._detectDuplicationModifyElements = detectDuplicationModifyElements;
            rowCreator = new ElementGatherInfo();
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

            #region 第4步 修改子id数据 并插入到数据库
            // 去除重复的子条码
            var newScanIdList = scanIdList.Distinct().ToList();
            duplicationList = duplicationList.Distinct(new ComparerDuplication()).ToList(); // 去重
            var repeateScanIds = this._detectDuplicationItems.GetAll().Where(x => newScanIdList.Contains(x.scanId) && x.tplSpecId == specId).Select(x => x.scanId).ToList();
            duplicationList.RemoveAll(x => repeateScanIds.Contains(x.scanId));
            // 获取主ID
            var curMainItems = this._detectMainInfo.GetAll().Where(x => distinctMainIdList.Contains(x.mainScanId) && x.specId == specId).ToList();
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

            #region 第5步 修改元素数据 并插入到数据库
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
                var curDuplicateItem = allDuplicateItems.Where(x => x.scanId == item.modifyUserName).First();
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

        /// <summary>
        /// 搜索平行样的值
        /// </summary>
        /// <param name="tplSpecId"></param>
        /// <param name="beginTime"></param>
        /// <param name="endTime"></param>
        /// <param name="searchId"></param>
        /// <param name="type">0-打印时间,1-录入时间</param>
        /// <returns></returns>
        public TableInfoDto SearchDuplicateItems(int tplSpecId, DateTime beginTime, DateTime endTime, string searchId, int dateType)
        {
            var query = this._detectDuplicationItems.GetAll();
            if (dateType == 0)
            {
                query = query.Where(x => x.printTime >= beginTime && x.printTime <= endTime);
            }
            if (dateType == 1)
            {
                query = query.Where(x => x.modifyTime >= beginTime && x.modifyTime <= endTime);
            }
            if (!string.IsNullOrEmpty(searchId))
            {
                query = query.Where(x => x.scanId.EndsWith(searchId));
            }
            if (tplSpecId > 0)
            {
                query = query.Where(x => x.tplSpecId == tplSpecId);
            }
            var duplicationItemList = query.ToList();
            var duplicationIdList = duplicationItemList.Select(x => x.Id);
            var eleList = this._detectDuplicationElements.GetAll().Where(x => duplicationIdList.Contains(x.duplicateId)).ToList();
            var eleLookup = eleList.ToLookup(x => x.duplicateId);
            // <平行样id，元素值集合>
            Dictionary<int, List<TmpEleInfo>> eleDict = new Dictionary<int, List<TmpEleInfo>>();
            // <元素id,元素信息>
            Dictionary<int, TmpEleTitle> eleTitleDict = new Dictionary<int, TmpEleTitle>();
            foreach (var group in eleLookup)
            {
                int tmpItemId = group.Key;
                List<TmpEleInfo> tmpEleList = new List<TmpEleInfo>();
                Dictionary<int, int> tmpEleDict = new Dictionary<int, int>(); // 元素及个数
                Dictionary<int, int> tmpOperDict = new Dictionary<int, int>(); // 元素操作者及个数
                foreach (var eleItem in group)
                {
                    TmpEleInfo tmpEle = new TmpEleInfo();
                    tmpEle.eleId = eleItem.tplEleId;
                    tmpEle.eleValue = eleItem.eleValue;
                    tmpEle.operId = eleItem.operId;
                    tmpEle.operName = eleItem.operName;

                    // 将元素值及操作人信息存储到字典中，方便对比，生成最大列
                    if (!tmpEleDict.Keys.Contains(eleItem.tplEleId))
                    {
                        tmpEleDict.Add(eleItem.tplEleId, 1);
                        if (eleItem.operId > 0)
                        {
                            tmpOperDict.Add(eleItem.tplEleId, 1);
                        }
                        else
                        {
                            tmpOperDict.Add(eleItem.tplEleId, 0);
                        }
                    }
                    else
                    {
                        tmpEleDict[eleItem.tplEleId] += 1;
                        if (eleItem.operId > 0)
                        {
                            tmpOperDict[eleItem.tplEleId] += 1;
                        }
                    }

                    tmpEleList.Add(tmpEle);
                }
                eleDict.Add(tmpItemId, tmpEleList);
                // 合并最大列
                foreach (var item in tmpEleDict)
                {
                    if (eleTitleDict.Keys.Contains(item.Key))
                    {
                        var titleInfo = eleTitleDict[item.Key];
                        if (titleInfo.eleCount < item.Value)
                        {
                            titleInfo.eleCount = item.Value;
                        }
                        if (titleInfo.operCount < tmpOperDict[item.Key])
                        {
                            titleInfo.operCount = tmpOperDict[item.Key];
                        }
                    }
                    else
                    {
                        var tmpTitleInfo = new TmpEleTitle();
                        tmpTitleInfo.eleId = item.Key;
                        tmpTitleInfo.eleCount = item.Value;
                        tmpTitleInfo.operCount = tmpOperDict[item.Key];
                        eleTitleDict.Add(item.Key, tmpTitleInfo);

                    }
                }
            }

            // 生成表头信息
            int eleColumnCount = 0;// 记录表的总列数
            var tplEleList = this._elementRepository.GetAll().Where(x => x.TplSpecId == tplSpecId).OrderBy(x => x.OrderNo).ToList();
            List<ElementTitleInfo> columnList = new List<ElementTitleInfo>();
            foreach (var item in tplEleList)
            {
                if (eleTitleDict.Keys.Contains(item.Id))
                {
                    var eleTitleInfo = eleTitleDict[item.Id];
                    var tmpColumn = new ElementTitleInfo();
                    tmpColumn.EleId = item.Id;
                    tmpColumn.EleName = item.ElementName;
                    tmpColumn.EleCount = eleTitleInfo.eleCount;
                    tmpColumn.OperCount = eleTitleInfo.operCount;
                    tmpColumn.ColumnCount = (eleTitleInfo.eleCount + eleTitleInfo.operCount);
                    eleColumnCount += tmpColumn.ColumnCount;
                    columnList.Add(tmpColumn);
                }
            }
            List<List<string>> tableList = new List<List<string>>();
            // 生成表体数据
            foreach (var item in duplicationItemList)
            {
                List<string> rowList = new List<string>();
                rowList.Add(item.scanId);
                rowList.Add(item.scanId.Substring(item.scanId.Length - 6));
                rowList.Add(item.printTime.ToString("yyyy-MM-dd"));
                rowList.Add(item.modifyTime.ToString("yyyy-MM-dd"));
                var tmpDataList = eleDict[item.Id];
                if (tmpDataList.Count > 0)
                {
                    tmpDataList = tmpDataList.OrderByDescending(x => x.operId).ToList(); // 按操作人排序
                    foreach (var columnItem in columnList) // 按Column取值
                    {
                        var tmpEleDataList = tmpDataList.Where(x => x.eleId == columnItem.EleId).ToList();
                        int rowCount = 0; // 已生成的列数
                        int operCount = columnItem.OperCount;

                        foreach (var eleItem in tmpEleDataList)
                        {
                            rowList.Add(eleItem.eleValue);
                            rowCount++;
                            if (eleItem.operId > 0 || operCount > 0) // 默认如果有操作人的，其肯定有操作人这列，operId=0的，其肯定没有操作人这列
                            {
                                rowList.Add(eleItem.operName);
                                rowCount++;
                                operCount--;
                            }
                        }
                        // 如果该元素列数不够，则用string.empty 补足
                        for (; rowCount < columnItem.ColumnCount; rowCount++)
                        {
                            rowList.Add(string.Empty);
                        }
                    }
                    tableList.Add(rowList);
                }
            }

            List<string> titleList = new List<string>();
            titleList.Add("条码号");
            titleList.Add("编号");
            titleList.Add("送样时间");
            titleList.Add("录入时间");
            foreach (var columnItem in columnList) // 按Column取值
            {
                int eleCount = columnItem.EleCount;
                int operCount = columnItem.OperCount;
                string columnName = columnItem.EleName;
                for (int eleIndex = 0; eleIndex < eleCount; eleIndex++)
                {
                    titleList.Add(columnName);
                    if (operCount > 0)
                    {
                        titleList.Add("操作人");
                        operCount--;
                    }
                }
            }

            TableInfoDto retData = new TableInfoDto();
            retData.titleList = titleList;
            retData.titleInfo = columnList;
            retData.dataList = tableList;
            return retData;
        }

        /// <summary>
        /// 获取修正值信息
        /// </summary>
        /// <param name="tplSpecId"></param>
        /// <param name="beginTime"></param>
        /// <param name="endTime"></param>
        /// <param name="searchId"></param>
        /// <param name="dateType"></param>
        /// <returns></returns>
        public ModifyTableInfoDto SearchDuplicateModificationItems(int tplSpecId, DateTime beginTime, DateTime endTime, string searchId, int dateType)
        {
            // 获取所有的平行样信息
            var query = this._detectDuplicationItems.GetAll();
            if (dateType == 0)
            {
                query = query.Where(x => x.printTime >= beginTime && x.printTime <= endTime);
            }
            if (dateType == 1)
            {
                query = query.Where(x => x.modifyTime >= beginTime && x.modifyTime <= endTime);
            }
            if (!string.IsNullOrEmpty(searchId))
            {
                query = query.Where(x => x.scanId.EndsWith(searchId));
            }
            if (tplSpecId > 0)
            {
                query = query.Where(x => x.tplSpecId == tplSpecId);
            }
            var duplicationItemList = query.ToList();
            var duplicationIdList = duplicationItemList.Select(x => x.Id);
            // 获取所有的修正值
            var eleModifyList = this._detectDuplicationModifyElements.GetAll().Where(x => duplicationIdList.Contains(x.duplicateId)).ToList();
            var eleModifyLookup = eleModifyList.ToLookup(x => x.duplicateId);
            // 获取所有的平行样的值
            var eleList = this._detectDuplicationElements.GetAll().Where(x => duplicationIdList.Contains(x.duplicateId)).ToList();
            var eleLookup = eleList.ToLookup(x => x.duplicateId);
            // <平行样id，元素值集合>--所有平行样的值
            Dictionary<int, TmpEleValues> eleDict = new Dictionary<int, TmpEleValues>();
            // <元素id,元素信息>--所有元素title的信息
            Dictionary<int, TmpEleTitle> eleTitleDict = new Dictionary<int, TmpEleTitle>();
            foreach (var group in eleLookup)
            {
                int tmpItemId = group.Key;
                List<TmpEleInfo> tmpEleList = new List<TmpEleInfo>();
                List<TmpEleInfo> tmpEleModifyList = new List<TmpEleInfo>();
                TmpEleValues eleObj = new TmpEleValues();
                Dictionary<int, int> tmpEleDict = new Dictionary<int, int>(); // 元素及个数
                Dictionary<int, int> tmpOperDict = new Dictionary<int, int>(); // 元素操作者及个数
                // 平行样值存储
                foreach (var eleItem in group)
                {
                    TmpEleInfo tmpEle = new TmpEleInfo();
                    tmpEle.eleId = eleItem.tplEleId;
                    tmpEle.eleValue = eleItem.eleValue;
                    tmpEle.operId = eleItem.operId;
                    tmpEle.operName = eleItem.operName;
                    tmpEleList.Add(tmpEle);
                }
                // 修正值存储
                if (eleModifyLookup.Contains(tmpItemId))
                {
                    var tmpValList = eleModifyLookup[tmpItemId];
                    foreach (var eleItem in group)
                    {
                        TmpEleInfo tmpEle = new TmpEleInfo();
                        tmpEle.eleId = eleItem.tplEleId;
                        tmpEle.eleValue = eleItem.eleValue;
                        tmpEle.operId = eleItem.operId;
                        tmpEle.operName = eleItem.operName;
                        tmpEleModifyList.Add(tmpEle);
                    }
                }
                eleObj.duplicationInfo = tmpEleList;
                eleObj.modifyInfo = tmpEleModifyList;
                eleDict.Add(tmpItemId, eleObj);
            }

            // 生成表头信息
            List<string> titleList = new List<string>();
            titleList.Add("条码号");
            titleList.Add("编号");
            titleList.Add("送样时间");
            titleList.Add("录入时间");
            var tplEleList = this._elementRepository.GetAll().Where(x => x.TplSpecId == tplSpecId).OrderBy(x => x.OrderNo).ToList();
            foreach (var item in tplEleList)
            {
                titleList.Add(item.ElementName);
            }
            
            // 生成表体数据
            List<List<string>> tableList = new List<List<string>>();
            List<ModifyRowInfoDto> modifyRowList = new List<ModifyRowInfoDto>();
            foreach (var item in duplicationItemList)
            {
                List<string> rowList = new List<string>();
                rowList.Add(item.scanId);
                rowList.Add(item.scanId.Substring(item.scanId.Length - 6));
                rowList.Add(item.printTime.ToString("yyyy-MM-dd"));
                rowList.Add(item.modifyTime.ToString("yyyy-MM-dd"));
                var tmpDataInfo = eleDict[item.Id];
                var dupInfo = tmpDataInfo.duplicationInfo;
                var modInfo = tmpDataInfo.modifyInfo;
                TmpRowInfo rowInfo=rowCreator.GetElementInfoFromElementInfo(tplSpecId,tplEleList,dupInfo,modInfo);
                rowList.AddRange(rowInfo.rowList);
                ModifyRowInfoDto rowDto = new ModifyRowInfoDto();
                rowDto.duplicateId = item.Id;
                rowDto.duplicationInfoStr = rowInfo.duplicationStr;
                rowDto.rowList = rowList;
                modifyRowList.Add(rowDto);
            }
            ModifyTableInfoDto tableDto = new ModifyTableInfoDto();
            tableDto.titleList = titleList;
            tableDto.rowList = modifyRowList;
            return tableDto;
        }

        private void GetElementInfoFromElementInfo(List<TplElement> tplEleList,List<TmpEleInfo> dupInfo,List<TmpEleInfo> modifyInfo)
        {
            foreach (var eleItem in tplEleList)
            {
                var tmpDupEleList = dupInfo.Where(x => x.eleId == eleItem.ElementId).ToList();

            }
        }

        public List<ModifyEditInfoDto> GetSingleModifyInfo(int dupId)
        {
            // 获取所有的平行样信息
            var dupItem = this._detectDuplicationItems.GetAll().Where(x=>x.Id==dupId).FirstOrDefault();
            if (dupItem != null)
            {
                int tplSpecId =dupItem.tplSpecId;
                var eleList=this._elementRepository.GetAll().Where(x => x.TplSpecId == tplSpecId).OrderBy(x=>x.OrderNo).ToList(); // 所有元素值
                var dupEleList=this._detectDuplicationElements.GetAll().Where(x => x.duplicateId == dupId).ToList(); // 所有平行样的值
                var dupModifyList = this._detectDuplicationModifyElements.GetAll().Where(x => x.duplicateId == dupId).ToList(); // 所有修正样的值

                return this.rowCreator.GetModifyElementInfo(tplSpecId,eleList,dupEleList,dupModifyList);
            }
            return null;
        }

        class ElementGatherInfo // 元素数据汇总的算法
        {
            // 获取单个样品所有平行样和修正值
            public TmpRowInfo GetElementInfoFromElementInfo(int tplSpecId, List<TplElement> tplEleList, List<TmpEleInfo> dupInfo, List<TmpEleInfo> modifyInfo)
            {
                TmpRowInfo ret = new TmpRowInfo();
                switch (tplSpecId)
                {
                    case 1682:
                        ret=GetElementInfoFromElementInfoZn(tplEleList,dupInfo,modifyInfo);
                        break;
                    default:
                        break;
                }
                return ret;
            }

            // 获取单条平行样的所有化验结果和修正值
            public List<ModifyEditInfoDto> GetModifyElementInfo(int tplSpecId, List<TplElement> tplEleList, List<DuplicationElements> dupInfo, List<DuplicationModifyElements> modifyInfo)
            {
                List<ModifyEditInfoDto> ret =new List<ModifyEditInfoDto>();
                switch (tplSpecId)
                {
                    case 1682:
                        ret = GetModifyElementInfoZn(tplEleList, dupInfo, modifyInfo);
                        break;
                    default:
                        break;
                }
                return ret;
            }

            // 锌精矿的算法
            private TmpRowInfo GetElementInfoFromElementInfoZn(List<TplElement> tplEleList, List<TmpEleInfo> dupInfo, List<TmpEleInfo> modifyInfo)
            {
                Dictionary<int, double> eleDupDict = new Dictionary<int, double>();
                Dictionary<int, double> eleModifyDict = new Dictionary<int, double>();
                StringBuilder eleSb = new StringBuilder();
                foreach (var eleItem in tplEleList)
                {
                    var tmpDupEleList = dupInfo.Where(x => x.eleId == eleItem.Id).ToList();
                    double tmpRetVal = 0.0;
                    int eleCount = 0;
                    string dupStr = string.Empty;
                    // 遍历平行样中值
                    foreach (var tmpDupItem in tmpDupEleList) 
                    {
                        double tmpVal = 0.0;
                        double.TryParse(tmpDupItem.eleValue, out tmpVal);
                        tmpRetVal += tmpVal;
                        if (tmpVal > 0)
                        {
                            eleCount++;
                            if (tmpDupItem.operId > 0)
                            {
                                dupStr += string.Format(" {0}({1}) |", tmpDupItem.eleValue, tmpDupItem.operName);
                            }
                            else
                            {
                                dupStr += $"{tmpDupItem.eleValue}|";
                            }
                        }
                    }
                    if (tmpDupEleList.Count > 0)
                    {
                        eleSb.Append($"[{eleItem.ElementName}: {dupStr.TrimEnd('|')}],   ");
                    }

                    // 多个时计算平均值
                    if (eleCount > 0) 
                    {
                        tmpRetVal = Math.Round((double)tmpRetVal / eleCount, 2, MidpointRounding.AwayFromZero);
                    }
                    eleDupDict.Add(eleItem.Id,tmpRetVal);

                    // 修正值只有一个
                    var modifyItem =modifyInfo.Where(x => x.eleId == eleItem.Id).FirstOrDefault(); 
                    double modifyVal = 0.0;
                    if (modifyItem != null)
                    {
                        double.TryParse(modifyItem.eleValue, out modifyVal);
                    }
                    eleModifyDict.Add(eleItem.Id,modifyVal); // 存储修正值
                }
                eleDupDict[9010] = eleDupDict[9010] - eleDupDict[9016]; // Zn=Avg(Zn)-Co的值

                string duplicationInfoStr = eleSb.ToString().TrimEnd().Trim(',');
                List<string> dupItemList = new List<string>();
                // 元素数据
                foreach (var eleItem in tplEleList)
                {
                    if (eleModifyDict[eleItem.Id] > 0)
                    {
                        dupItemList.Add(eleModifyDict[eleItem.Id].ToString());
                    }
                    else
                    {
                        dupItemList.Add(eleDupDict[eleItem.Id].ToString());
                    }
                    
                }
                TmpRowInfo tmpRow = new TmpRowInfo();
                tmpRow.rowList = dupItemList;
                tmpRow.duplicationStr = duplicationInfoStr;
                return tmpRow;
            }
            // 锌精矿的算法
            private List<ModifyEditInfoDto> GetModifyElementInfoZn(List<TplElement> tplEleList, List<DuplicationElements> dupInfo, List<DuplicationModifyElements> modifyInfo)
            {
                Dictionary<int, double> eleDupDict = new Dictionary<int, double>();
                Dictionary<int, string> eleDupStrDict = new Dictionary<int, string>();
                Dictionary<int, double> eleModifyDict = new Dictionary<int, double>();
                foreach (var eleItem in tplEleList)
                {
                    var tmpDupEleList = dupInfo.Where(x => x.tplEleId == eleItem.Id).ToList();
                    double tmpRetVal = 0.0;
                    int eleCount = 0;
                    string dupStr = string.Empty;
                    // 遍历平行样中值
                    foreach (var tmpDupItem in tmpDupEleList)
                    {
                        double tmpVal = 0.0;
                        double.TryParse(tmpDupItem.eleValue, out tmpVal);
                        tmpRetVal += tmpVal;
                        if (tmpVal > 0)
                        {
                            eleCount++;
                            if (tmpDupItem.operId > 0)
                            {
                                dupStr += $" {tmpDupItem.eleValue}({tmpDupItem.operName}) |";
                            }
                            else
                            {
                                dupStr += $"  {tmpDupItem.eleValue}|";
                            }
                        }
                    }
                    // 多个时计算平均值
                    if (eleCount > 0)
                    {
                        tmpRetVal = Math.Round((double)tmpRetVal / eleCount, 2, MidpointRounding.AwayFromZero);
                    }
                    eleDupDict.Add(eleItem.Id, tmpRetVal);

                    // 修正值只有一个
                    var modifyItem = modifyInfo.Where(x => x.tplEleId == eleItem.Id).FirstOrDefault();
                    double modifyVal = 0.0;
                    if (modifyItem != null)
                    {
                        double.TryParse(modifyItem.eleValue, out modifyVal);
                    }
                    eleModifyDict.Add(eleItem.Id, modifyVal); // 存储修正值
                }
                eleDupDict[9010] = eleDupDict[9010] - eleDupDict[9016]; // Zn=Avg(Zn)-Co的值

                List<ModifyEditInfoDto> retItemList = new List<ModifyEditInfoDto>();
                // 元素数据
                foreach (var eleItem in tplEleList)
                {
                    ModifyEditInfoDto tmpModify = new ModifyEditInfoDto();
                    tmpModify.EleId = eleItem.Id;
                    tmpModify.EleName = eleItem.ElementName;
                    tmpModify.DuplicationStr = eleDupStrDict[eleItem.Id];
                    tmpModify.EleValue = eleDupDict[eleItem.Id];
                    retItemList.Add(tmpModify);
                }
                return retItemList;
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

        class ComparerDuplication : IEqualityComparer<DuplicationInfoItems>
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

        class TmpEleInfo
        {
            public int eleId { get; set; }
            public string eleValue { get; set; }
            public int operId { get; set; }
            public string operName { get; set; }
        }

        class TmpEleTitle
        {
            public int eleId { get; set; }
            public int eleCount { get; set; }
            public int operCount { get; set; }
        }

        class TmpEleValues
        {
            public List<TmpEleInfo> duplicationInfo { get; set; }
            public List<TmpEleInfo> modifyInfo { get; set; }
        }

        class TmpRowInfo
        {
            public List<string> rowList { get; set; }
            public string duplicationStr { get; set; }
        }
    }
}
