using LIMS.Assay.Data.Dto;
using NPOI.HPSF;
using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using NPOI.SS.Util;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Text;

namespace LIMS.Assay.Data
{
    public class ExcelOper
    {
        public string DirPath = "D:\\Ftp\\www\\Excels\\";

        public string CreateSingleTableSearchExcel(DataSearchTableDto excelData)
        {
            var tableHead = excelData.TableHead;
            var eleList = tableHead.Elements;
            var specList = tableHead.Specimens;
            var dataList = excelData.TableData;

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

            ICellStyle titleStyle = workbook.CreateCellStyle();
            //设置单元格的样式：水平对齐居中
            titleStyle.Alignment = HorizontalAlignment.CenterSelection;
            // titleStyle.VerticalAlignment = VerticalAlignment.Center;
            //新建一个字体样式对象
            IFont font = workbook.CreateFont();
            //设置字体加粗样式
            // font.Boldweight = short.MaxValue;
            font.IsBold = true;
            //使用SetFont方法将字体样式添加到单元格样式中 
            titleStyle.SetFont(font);

            int rowIndex = 0;
            IRow row1 = sheet.CreateRow(rowIndex++);
            ICell cell00 = row1.CreateCell(0);    
            SetCell(cell00, null, "System.String", "签到时间");
            //ICell cell01 = row1.CreateCell(1);
            //SetCell(cell01, null, null, "采样时间");
            int cellIndex = 1;
            // 合并单元格
            foreach (var specItem in specList)
            {
                int curIndex = cellIndex;
                if (specItem.Count > 1)
                {
                    int newCellIndex= cellIndex + specItem.Count;
                    sheet.AddMergedRegion(new CellRangeAddress(0, 0,cellIndex,newCellIndex - 1));
                    cellIndex = newCellIndex;
                }
                else
                {
                    cellIndex++;
                }

                var tempCell = row1.CreateCell(curIndex);
                tempCell.CellStyle=titleStyle;
                SetCell(tempCell, null, "System.String", specItem.Name);
            }
            cellIndex = 1;
            //// 填充样品值
            //foreach (var item in specList)
            //{
            //    var tempCell = row1.CreateCell(cellIndex++);
            //    SetCell(tempCell, null, null, item.Name);
            //}
            IRow row2 = sheet.CreateRow(rowIndex++);
            cellIndex = 1;
            // 填充元素数据
            foreach (var eleItem in eleList)
            {
                var tempCell = row2.CreateCell(cellIndex++);
                SetCell(tempCell, null, "System.String", eleItem.Name);
            }
            // 合并签到时间和取样时间列表
            sheet.AddMergedRegion(new CellRangeAddress(0, 1, 0, 0));
            //sheet.AddMergedRegion(new CellRangeAddress(0, 1, 1, 1));
            #region 日期样式
            ICellStyle dateStyle = workbook.CreateCellStyle();
            IDataFormat format = workbook.CreateDataFormat();
            dateStyle.DataFormat = format.GetFormat("yyyy-MM-dd hh:mm");
            #endregion
            //填充数据
            foreach (var dataRow in dataList)
            {
                IRow tmpRow = sheet.CreateRow(rowIndex++);
                cellIndex = 0;
                foreach (var cellValue in dataRow)
                {
                    ICell tmpCell = tmpRow.CreateCell(cellIndex++);
                    if (cellIndex < 1)
                    {
                        SetCell(tmpCell, dateStyle, "System.DateTime", cellValue);
                    }
                    else
                    {
                        SetCell(tmpCell, null, "System.Double", cellValue);
                    }
                }
            }

            #region 设置列宽度
            sheet.SetColumnWidth(0, 18 * 256);
            sheet.SetColumnWidth(1, 18 * 256);
            for (int i = 2; i < eleList.Count; i++)
            {
                sheet.SetColumnWidth(i, 10 * 256);
            }
            #endregion

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

        public string CreateSelfCodeSearchExcel(SelfSearchTableDto excelData)
        {
            var tableHead = excelData.TableHead;
            var dataList = excelData.TableData;

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

            #region 设置样式
            ICellStyle titleStyle = workbook.CreateCellStyle();
            //设置单元格的样式：水平对齐居中
            titleStyle.Alignment = HorizontalAlignment.CenterSelection;
            // titleStyle.VerticalAlignment = VerticalAlignment.Center;
            //新建一个字体样式对象
            IFont font = workbook.CreateFont();
            //设置字体加粗样式
            // font.Boldweight = short.MaxValue;
            font.IsBold = true;
            //使用SetFont方法将字体样式添加到单元格样式中 
            titleStyle.SetFont(font);
            #endregion

            int rowIndex = 0;
            IRow row1 = sheet.CreateRow(0);
            IRow row2 = sheet.CreateRow(1);
            ICell cell00 = row1.CreateCell(0);
            SetCell(cell00, null, "System.String", "签到时间");
            int cellIndex = 1;

            int firstIndex = 1; // 第一行行首
            int lastIndex = 1; // 第一行行尾
            // 生成第一行和第二行
            foreach (var item in tableHead)
            {
                var specList=item.Specimens;
                firstIndex = cellIndex;
                // 设置样品值
                foreach (var specItem in specList)
                {
                    int curIndex = cellIndex;
                    if (specItem.Count > 1)
                    {
                        int newCellIndex = cellIndex + specItem.Count;
                        sheet.AddMergedRegion(new CellRangeAddress(1, 1, cellIndex, newCellIndex - 1));
                        cellIndex = newCellIndex;
                    }
                    else
                    {
                        cellIndex++;
                    }

                    var tempCell = row2.CreateCell(curIndex);
                    tempCell.CellStyle = titleStyle;
                    SetCell(tempCell, null, "System.String", specItem.Name);
                }
                lastIndex = cellIndex - 1;

                sheet.AddMergedRegion(new CellRangeAddress(0, 0, firstIndex, lastIndex));
                ICell firstCell=row1.CreateCell(firstIndex);
                SetCell(firstCell,null, "System.String", item.Template.Name);
            }
            // 生成元素-第3行以及列宽
            cellIndex = 1;
            IRow row3 = sheet.CreateRow(2);
            sheet.SetColumnWidth(0, 18 * 256);
            foreach (var item in tableHead)
            {
                sheet.SetColumnWidth(cellIndex, 10 * 256);
                var eleList = item.Elements;
                // 填充元素数据
                foreach (var eleItem in eleList)
                {
                    var tempCell = row3.CreateCell(cellIndex++);
                    SetCell(tempCell, null, "System.String", eleItem.Name);
                }
            }
            // 合并签到时间列表
            sheet.AddMergedRegion(new CellRangeAddress(0, 2, 0, 0));
            #region 日期样式
            ICellStyle dateStyle = workbook.CreateCellStyle();
            IDataFormat format = workbook.CreateDataFormat();
            dateStyle.DataFormat = format.GetFormat("yyyy-MM-dd hh:mm");
            #endregion
            rowIndex = 3;
            //填充数据
            foreach (var dataRow in dataList)
            {
                IRow tmpRow = sheet.CreateRow(rowIndex++);
                cellIndex = 0;
                foreach (var cellValue in dataRow)
                {
                    ICell tmpCell = tmpRow.CreateCell(cellIndex++);
                    if (cellIndex < 1)
                    {
                        SetCell(tmpCell, dateStyle, "System.DateTime", cellValue);
                    }
                    else
                    {
                        SetCell(tmpCell, null, "System.Double", cellValue);
                    }
                }
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

        public string CreateExcelAndSaveLocalSingleSheet(List<MultiTableDataInfoDto> excelData)
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

            int row = 0;
            sheet.CreateRow(row);
            row++;
            foreach (var tableDto in excelData)
            {
                if (tableDto.TableData.Count > 0)
                {
                    CreateExcelSheet(workbook, sheet, tableDto.TableData, tableDto.TableHead, tableDto.TableTitle, ref row);
                    row += 2;
                }
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

        public string CreateExcelAndSaveLocalMultiSheet(List<MultiTableDataInfoDto> excelData)
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

            foreach (var tableDto in excelData)
            {
                if (tableDto.TableData.Count > 0)
                {
                    int row = 0;
                    ISheet sheet = workbook.CreateSheet(tableDto.TableTitle);
                    sheet.CreateRow(row);
                    row++;
                    CreateExcelSheet(workbook, sheet, tableDto.TableData, tableDto.TableHead, tableDto.TableTitle, ref row);
                }
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

        private ICell GetCell(IRow tempRow, int index)
        {
            ICell cell = tempRow.GetCell(index);
            if (cell == null)
            {
                cell = tempRow.CreateCell(index);
            }
            return cell;
        }

        private ISheet CreateExcelSheet(HSSFWorkbook workbook, ISheet sheet, List<List<string>> dataList, List<string> titleList, string title, ref int beginRowNum)
        {
            #region 表头样式

            ICellStyle cHeadStyle = workbook.CreateCellStyle();
            cHeadStyle.Alignment = HorizontalAlignment.Center;
            IFont cfont = workbook.CreateFont();
            cfont.FontHeightInPoints = 15;
            cHeadStyle.SetFont(cfont);

            #endregion

            #region 日期样式
            ICellStyle dateStyle = workbook.CreateCellStyle();
            IDataFormat format = workbook.CreateDataFormat();
            dateStyle.DataFormat = format.GetFormat("yyyy-MM-dd hh:mm");
            #endregion

            int rowIndex = beginRowNum;
            IRow titleRow = sheet.CreateRow(rowIndex);
            ICell titleCell = titleRow.CreateCell(0);
            titleCell.SetCellValue(title);
            titleCell.CellStyle = cHeadStyle;

            rowIndex++;
            IRow headerRow = sheet.CreateRow(rowIndex);
            #region 生成表头
            ICell tempCell0 = GetCell(headerRow, 0);
            tempCell0.SetCellValue("签到时间");
            tempCell0.CellStyle = cHeadStyle;

            ICell tempCell1 = GetCell(headerRow, 1);
            tempCell1.SetCellValue("采样时间");
            tempCell1.CellStyle = cHeadStyle;

            ICell tempCell2 = GetCell(headerRow, 2);
            tempCell2.SetCellValue("班次");
            tempCell2.CellStyle = cHeadStyle;

            ICell tempCell3 = GetCell(headerRow, 3);
            tempCell3.SetCellValue("炉次");
            tempCell3.CellStyle = cHeadStyle;

            ICell tempCell4 = GetCell(headerRow, 4);
            tempCell4.SetCellValue("编号");
            tempCell4.CellStyle = cHeadStyle;

            ICell tempCell5 = GetCell(headerRow, 5);
            tempCell5.SetCellValue("备注");
            tempCell5.CellStyle = cHeadStyle;

            for (int i = 0; i < titleList.Count; i++)
            {
                string tempTile = titleList[i];
                ICell ttCell = GetCell(headerRow, i + 6);
                ttCell.SetCellValue(tempTile);
                ttCell.CellStyle = cHeadStyle;
            }
            #endregion

            rowIndex++;

            #region 表体
            foreach (var listItem in dataList)
            {
                IRow tempRow = sheet.CreateRow(rowIndex);
                rowIndex++;
                for (int colIndex = 0; colIndex < listItem.Count; colIndex++)
                {
                    ICell newCell = tempRow.CreateCell(colIndex);
                    if (colIndex < 2)
                    {
                        SetCell(newCell, dateStyle, "System.DateTime", listItem[colIndex]);
                    }
                    else
                    {
                        SetCell(newCell, dateStyle, "System.Double", listItem[colIndex]);
                    }
                }
            }
            #endregion

            #region 设置列宽度
            sheet.SetColumnWidth(0, 18 * 256);
            sheet.SetColumnWidth(1, 18 * 256);
            for (int i = 2; i < titleList.Count + 5; i++)
            {
                sheet.SetColumnWidth(i, 10 * 256);
            }
            #endregion
            beginRowNum = rowIndex;
            return sheet;
        }

        #region RGB颜色转NPOI颜色
        private short GetXLColour(HSSFWorkbook workbook, Color SystemColour)
        {
            short s = 0;
            HSSFPalette XlPalette = workbook.GetCustomPalette();
            NPOI.HSSF.Util.HSSFColor XlColour = XlPalette.FindColor(SystemColour.R, SystemColour.G, SystemColour.B);
            if (XlColour == null)
            {
                if (NPOI.HSSF.Record.PaletteRecord.STANDARD_PALETTE_SIZE < 255)
                {
                    XlColour = XlPalette.FindSimilarColor(SystemColour.R, SystemColour.G, SystemColour.B);
                    s = XlColour.Indexed;
                }
            }
            else
            {
                s = XlColour.Indexed;
            }
            return s;
        }
        #endregion

        #region 设置数据的值
        private void SetCell(IWorkbook workbook, ICell newCell, ICellStyle dataStyle, string drValue, bool isDateTime)
        {
            if (isDateTime)
            {
                IDataFormat format = workbook.CreateDataFormat();
                dataStyle.DataFormat = format.GetFormat("yyyy-MM-dd HH:mm");
                DateTime dtV = DateTime.Parse(drValue);
                newCell.SetCellValue(dtV);
                newCell.CellStyle = dataStyle;
                return;
            }
            double dvalue = 0.0;
            bool isDouble = double.TryParse(drValue, out dvalue);
            if (isDouble)
            {
                dataStyle.DataFormat = HSSFDataFormat.GetBuiltinFormat("0.000000");
                newCell.SetCellValue(dvalue);
            }
            else
            {
                newCell.SetCellValue(drValue);
            }
        }

        private void SetCell(ICell newCell, ICellStyle dateStyle, string dataType, string drValue)
        {

            switch (dataType)
            {
                case "System.String"://字符串类型
                    newCell.SetCellValue(drValue);
                    newCell.CellStyle = dateStyle;
                    break;
                case "System.DateTime"://日期类型
                    System.DateTime dateV;
                    if (System.DateTime.TryParse(drValue, out dateV))
                    {
                        newCell.SetCellValue(dateV);
                        newCell.CellStyle = dateStyle;
                    }
                    else
                    {
                        newCell.SetCellValue("");
                    }
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
