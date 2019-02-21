using NPOI.HPSF;
using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;

namespace Lims.Extension
{
    public class ExcelOper
    {
        public string DirPath = "D:\\Ftp\\www\\Excels\\";
        public string CreateExcelAndSaveLocal(List<List<string>> dataList, List<string> titleList)
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
            CreateExcelSheet(workbook, sheet, dataList, titleList);
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

        private ISheet CreateExcelSheet(HSSFWorkbook workbook, ISheet sheet, List<List<string>> dataList, List<string> titleList)
        {
            //#region 表头样式

            //ICellStyle headStyle = workbook.CreateCellStyle();
            //headStyle.FillBackgroundColor = GetXLColour(workbook, Color.FromArgb(68, 114, 196));
            //#region 字体
            //IFont fontStyle = workbook.CreateFont();
            //fontStyle.Color = GetXLColour(workbook, Color.Black);
            //fontStyle.FontHeightInPoints = 20;
            //headStyle.SetFont(fontStyle);
            //#endregion

            //#region 对齐方式
            //headStyle.Alignment = HorizontalAlignment.Center;
            //#endregion

            //#endregion

            int rowIndex = 0;
            rowIndex++;
            IRow headerRow = sheet.CreateRow(rowIndex);

            #region 生成表头
            for (int i = 0; i < titleList.Count; i++)
            {
                //string tempTile = titleList[i];
                //SetCell(workbook, headerRow.CreateCell(i), headStyle, tempTile, false);
                headerRow.CreateCell(i).SetCellValue(titleList[i]);
            }
            #endregion


            #region 内容样式

            //ICellStyle contentStyle = workbook.CreateCellStyle();
            //contentStyle.FillBackgroundColor = GetXLColour(workbook, Color.White);
            //#region 字体
            //IFont contentFontStyle = workbook.CreateFont();
            //contentFontStyle.Color = GetXLColour(workbook, Color.Black);
            //contentFontStyle.FontHeight = 18;
            //contentStyle.SetFont(contentFontStyle);
            //#endregion

            //#region 对齐方式
            //contentStyle.Alignment = HorizontalAlignment.Center;
            //#endregion

            #endregion

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
                        SetCell(workbook,newCell, null, listItem[colIndex], true);
                    }
                    else
                    {
                        SetCell(workbook,newCell, null, listItem[colIndex], false);
                    }
                }
            }
            #endregion

            #region 设置列宽度
            //sheet.SetColumnWidth(0, 16 * 256);
            //sheet.SetColumnWidth(1, 16 * 256);
            #endregion
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
        private void SetCell(IWorkbook workbook,ICell newCell, ICellStyle dataStyle, string drValue, bool isDateTime)
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
                // newCell.CellStyle = dataStyle;
            }
            else
            {
                newCell.SetCellValue(drValue);
            }
        }

        private void SetCell(ICell newCell, ICellStyle dateStyle, Type dataType, string drValue)
        {
            
            switch (dataType.ToString())
            {
                case "System.String"://字符串类型
                    newCell.SetCellValue(drValue);
                    break;
                case "System.DateTime"://日期类型
                    System.DateTime dateV;
                    if (System.DateTime.TryParse(drValue, out dateV))
                    {
                        newCell.SetCellValue(dateV);
                    }
                    else
                    {
                        newCell.SetCellValue("");
                    }
                    newCell.CellStyle = dateStyle;//格式化显示
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
                    double.TryParse(drValue, out doubV);
                    newCell.SetCellValue(doubV);
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