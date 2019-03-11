using LIMS.Assay.Data.Dto;
using LIMS.Assay.Statistic.Dto;
using NPOI.HPSF;
using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace LIMS.Assay.Statistic
{
    public class ExcelStatistic
    {
        public string DirPath = "D:\\Ftp\\www\\Excels\\";

        #region 统计信息
        // 整个公司
        public string CreateCompanyStatisticExcel(List<PlantSummaryDto> excelData)
        {
            HSSFWorkbook workbook = new HSSFWorkbook();
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

            foreach (var plant in excelData)
            {
                ISheet sheet = workbook.CreateSheet(plant.OrgName);
                WriteSheet(sheet, plant);
            }

            string fileName = DateTime.Now.ToString("yyyyMMddhhmmss") + ".xls";
            string filePath = DirPath + fileName;
            try
            {
                using (FileStream fs = new FileStream(filePath, FileMode.CreateNew, FileAccess.Write))
                {
                    workbook.Write(fs);
                }
                return fileName;
            }
            catch
            {
                return null;
            }
        }
        // 单个分厂
        public string CreateSingleSheetByPlant(PlantSummaryDto excelData)
        {
            HSSFWorkbook workbook = new HSSFWorkbook();
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

            ISheet sheet = workbook.CreateSheet(excelData.OrgName);
            WriteSheet(sheet, excelData);

            string fileName = DateTime.Now.ToString("yyyyMMddhhmmss") + ".xls";
            string filePath = DirPath + fileName;
            try
            {
                using (FileStream fs = new FileStream(filePath, FileMode.CreateNew, FileAccess.Write))
                {
                    workbook.Write(fs);
                }
                return fileName;
            }
            catch
            {
                return null;
            }
        }

        public string CreateSingleSheetBySectionList(List<SectionSummaryDto> excelData)
        {
            HSSFWorkbook workbook = new HSSFWorkbook();
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

            ISheet sheet = workbook.CreateSheet("sheet1");
            int rowIndex = 0;
            foreach (var item in excelData)
            {
                WriteSection(sheet, item, ref rowIndex);
            }

            string fileName = DateTime.Now.ToString("yyyyMMddhhmmss") + ".xls";
            string filePath = DirPath + fileName;
            try
            {
                using (FileStream fs = new FileStream(filePath, FileMode.CreateNew, FileAccess.Write))
                {
                    workbook.Write(fs);
                }
                return fileName;
            }
            catch
            {
                return null;
            }
        }

        private void WriteSheet(ISheet sheet, PlantSummaryDto sheetData)
        {
            int rowIndex = 0;
            foreach (var item in sheetData.SectionList)
            {
                WriteSection(sheet, item, ref rowIndex);
            }
        }

        private void WriteSection(ISheet sheet, SectionSummaryDto excelData, ref int rowIndex)
        {
            sheet.CreateRow(rowIndex);
            rowIndex++;
            // 写标题
            string[] heads = new string[] { "序号", "品名", "样品个数", "化验元素", "火试金" };
            IRow tRow = sheet.CreateRow(rowIndex);
            for (int i = 0; i < 5; i++)
            {
                ICell tCell = tRow.CreateCell(i);
                SetCell(tCell, typeof(string).FullName, heads[i]);
            }
            rowIndex++;

            // 写出全部数据
            var sectionList = excelData.SpecList;
            // 第一行数据
            IRow firstRow = sheet.CreateRow(rowIndex);
            ICell firstRowCell0 = firstRow.CreateCell(0);
            SetCell(firstRowCell0, typeof(string).FullName, excelData.OrgName);
            ICell firstRowCell1 = firstRow.CreateCell(1);
            SetCell(firstRowCell1, typeof(string).FullName, sectionList[0].SpecName);
            ICell firstRowCell2 = firstRow.CreateCell(2);
            SetCell(firstRowCell2, typeof(int).FullName, sectionList[0].SpecCount.ToString());
            ICell firstRowCell3 = firstRow.CreateCell(3);
            SetCell(firstRowCell3, typeof(int).FullName, sectionList[0].EleCount.ToString());
            ICell firstRowCell4 = firstRow.CreateCell(4);
            SetCell(firstRowCell4, typeof(int).FullName, sectionList[0].AuAg.ToString());
            rowIndex++;
            for (int i = 1; i < sectionList.Count; i++)
            {
                IRow tempRow = sheet.CreateRow(rowIndex);
                ICell tempRowCell0 = tempRow.CreateCell(0);
                ICell tempRowCell1 = tempRow.CreateCell(1);
                SetCell(tempRowCell1, typeof(string).FullName, sectionList[i].SpecName);
                ICell tempRowCell2 = tempRow.CreateCell(2);
                SetCell(tempRowCell2, typeof(int).FullName, sectionList[i].SpecCount.ToString());
                ICell tempRowCell3 = tempRow.CreateCell(3);
                SetCell(tempRowCell3, typeof(int).FullName, sectionList[i].EleCount.ToString());
                ICell tempRowCell4 = tempRow.CreateCell(4);
                SetCell(tempRowCell4, typeof(int).FullName, sectionList[i].AuAg.ToString());
                rowIndex++;
            }

            rowIndex += 2;

        }
        #endregion

        #region 设置数据的值

        private void SetCell(ICell newCell, string dataType, string drValue)
        {

            switch (dataType)
            {
                case "System.String"://字符串类型
                    newCell.SetCellValue(drValue);
                    break;
                case "System.Boolean"://布尔型
                    bool boolV = false;
                    bool.TryParse(drValue, out boolV);
                    newCell.SetCellValue(boolV);
                    break;
                case "System.Int16"://整型
                case "System.Int32":
                case "System.Int64":
                case "System.Byte":
                    int intV = 0;
                    int.TryParse(drValue, out intV);
                    newCell.SetCellValue(intV);
                    break;
                case "System.Decimal"://浮点型
                case "System.Double":
                    double doubV = 0;
                    if (double.TryParse(drValue, out doubV))
                    {
                        newCell.SetCellValue(doubV);
                    }
                    else
                    {
                        newCell.SetCellValue(drValue);
                    }

                    break;
                case "System.DBNull"://空值处理
                    newCell.SetCellValue("");
                    break;
                default:
                    newCell.SetCellValue("");
                    break;
            }
        }

        #endregion
    }
}
