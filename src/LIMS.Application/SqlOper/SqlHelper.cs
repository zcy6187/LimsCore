using LIMS.Assay;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Text;

namespace LIMS.SqlOper
{
    class SqlHelper
    {
        private string _connStr = "Data Source=10.68.2.2;DataBase=LIMS;User ID=SA;Password=Sa***123;Max Pool Size=512;";
        // 插入主条码
        //public void WriteDetectMainsToDb(List<DetectMainInfo> mainList)
        //{
        //    DataTable dt = new DataTable();
        //    dt.Columns.Add("specId");
        //    dt.Columns.Add("specName");
        //    // dt.Columns.Add("operatorId");
        //    dt.Columns.Add("mainScanId");
        //    dt.Columns.Add("fhId");
        //    dt.Columns.Add("chs");
        //    dt.Columns.Add("verdor");
        //    dt.Columns.Add("fhtime");
        //    dt.Columns.Add("createtime");
        //    dt.Columns.Add("modifyTime");
        //    dt.Columns.Add("isDeleted");

        //    foreach (var item in mainList)
        //    {
        //        DataRow dr = dt.NewRow();
        //        dr["specId"] = item.specId;
        //        dr["specName"] = item.specName;
        //        dr["operatorId"] = item.operatorId.ToString();
        //        dr["mainScanId"] = item.mainScanId;
        //        dr["fhId"] = item.fhId;
        //        dr["chs"] = item.chs;
        //        dr["verdor"] = item.verdor;
        //        dr["fhtime"] = item.fhtime;
        //        dr["createtime"] = item.createtime;
        //        dr["modifyTime"] = item.modifyTime;
        //        dr["isDeleted"] = "0";
        //        dt.Rows.Add(dr);
        //    }
        //    BulkInsert("assay_detectMainInfo",dt);
        //}

        public void WriteDetectMainsToDb(List<DetectMainInfo> mainList)
        {
            List<string> sqlList = new List<string>();
            string baseSql = @"insert into assay_detectMainInfo (
[specId],[specName],[operatorId],[mainScanId],[fhId],[chs],[verdor],[fhtime],[createtime],[modifyTime],[isDeleted],[hisId]) values 
('{0}','{1}','{2}','{3}','{4}','{5}','{6}','{7}','{8}','{9}','{10}','{11}')";
            mainList.ForEach((item) =>
            {
                string tmpSql = string.Format(baseSql,
                GetEmptyIfNull(item.specId),
                GetEmptyIfNull(item.specName),
                GetEmptyIfNull(item.operatorId),
                GetEmptyIfNull(item.mainScanId),
                GetEmptyIfNull(item.fhId),
                GetEmptyIfNull(item.chs),
                GetEmptyIfNull(item.verdor),
                GetEmptyIfNull(item.fhtime),
                item.createtime.ToString("yyyy-MM-dd HH:mm:ss"),
                item.modifyTime.ToString("yyyy-MM-dd HH:mm:ss"),
                "0",
                item.hisId);
                sqlList.Add(tmpSql);
            });

            BulkInsert(sqlList);
        }
        private string GetEmptyIfNull(object obj)
        {
            if (obj == null)
            {
                return string.Empty;
            }
            return obj.ToString();
        }
        // 插入平行样
        public void WriteDetectDuplicationToDb(List<DuplicationInfoItems> mainList)
        {
            List<string> sqlList = new List<string>();
            string baseSql = @"insert into assay_detectDuplicationItems ([mainId],[mainScanId],[scanId],[tplSpecId],[operId],[operName],[modifyTime],[isDeleted],[selfCode],[printTime],[hisId]) values 
('{0}','{1}','{2}','{3}','{4}','{5}','{6}','{7}','{8}','{9}','{10}')";

            mainList.ForEach((item) =>
            {
                string tmpSql = string.Format(baseSql,
                    GetEmptyIfNull(item.mainId),
                    GetEmptyIfNull(item.mainScanId),
                    GetEmptyIfNull(item.scanId),
                    GetEmptyIfNull(item.tplSpecId),
                    GetEmptyIfNull(item.operId),
                    GetEmptyIfNull(item.operName),
                    item.modifyTime.ToString("yyyy-MM-dd HH:mm:ss"),
                    "0",
                    GetEmptyIfNull(item.selfCode),
                    item.printTime.ToString("yyyy-MM-dd HH:mm:ss"),
                    item.hisId);
                sqlList.Add(tmpSql);
            });

            BulkInsert(sqlList);
        }
        public void WriteDuplicationElementsToDb(List<DuplicationElements> mainList)
        {
            List<string> sqlList = new List<string>();
            string baseSql = @"insert into assay_detectDuplicationElements (
                [duplicateId],
                [tplSpecId],
                [tplEleId],
                [tplEleName],
                [eleValue],
                [operId],
                [operName],
                [modifyTime],
                [modifyUserId],
                [modifyUserName],
                [IsDeleted],
                [desInfo]) values 
('{0}','{1}','{2}','{3}','{4}','{5}','{6}','{7}','{8}','{9}','{10}','{11}')";

            mainList.ForEach((item) =>
            {
                string tmpSql = string.Format(baseSql,
                    GetEmptyIfNull(item.duplicateId),
                    GetEmptyIfNull(item.tplSpecId),
                    GetEmptyIfNull(item.tplEleId),
                    GetEmptyIfNull(item.tplEleName),
                    GetEmptyIfNull(item.eleValue),
                    GetEmptyIfNull(item.operId),
                    GetEmptyIfNull(item.operName),
                    item.modifyTime.ToString("yyyy-MM-dd HH:mm:ss"),
                    GetEmptyIfNull(item.modifyUserId),
                    GetEmptyIfNull(item.modifyUserName),
                    "0",
                    GetEmptyIfNull(item.desInfo));
                sqlList.Add(tmpSql);
            });

            BulkInsert(sqlList);
        }

        private bool BulkInsert(List<string> sqlList)
        {
            using (System.Data.SqlClient.SqlConnection conn = new System.Data.SqlClient.SqlConnection(this._connStr))
            {
                conn.Open();
                string strCmd = string.Empty;
                for (int i = 0; i < sqlList.Count; i++)
                {
                    strCmd += sqlList[i] + ";";
                    if (i > 0 && i % 50 == 0)
                    {
                        SqlCommand command = new SqlCommand(strCmd, conn);
                        command.ExecuteNonQuery();
                        strCmd = string.Empty;
                    }
                }
                if (!string.IsNullOrEmpty(strCmd))
                {
                    SqlCommand command = new SqlCommand(strCmd, conn);
                    command.ExecuteNonQuery();
                }
                conn.Close();
            }
            return true;
        }
    }
}
