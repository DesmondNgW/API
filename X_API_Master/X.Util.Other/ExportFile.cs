using System.Collections;
using System.Data;
using System.IO;
using System.Linq;
using System.Web;
using NPOI.HSSF.UserModel;

namespace X.Util.Other
{
    class ExportFile
    {
        /// <summary>
        /// 导出Excel文件
        /// </summary>
        /// <param name="context"></param>
        /// <param name="dataList">数据集合</param>
        /// <param name="fields"></param>
        /// <param name="headTexts">标题数组</param>
        /// <param name="fileName">文件名称</param>
        public static void ExportExcelFile(HttpContext context, IList dataList, string[] fields, string[] headTexts, string fileName)
        {
            if (dataList == null || dataList.Count == 0 || headTexts == null || headTexts.Length == 0 || string.IsNullOrEmpty(fileName)) return;
            var result = List2DataTable(dataList, fields);
            var totalCount = dataList.Count;
            const int pageSize = 50000;
            var pageCount = (totalCount + pageSize - 1) / pageSize;
            var book = new HSSFWorkbook();
            for (var s = 0; s < pageCount; s++)
            {
                var sheet = book.CreateSheet(string.Format("第{0}页", s + 1));
                #region 标题行
                var columnRow = sheet.CreateRow(0);
                for (var c = 0; c < headTexts.Length; c++)
                {
                    columnRow.CreateCell(c).SetCellValue(headTexts[c]);
                }
                #endregion
                #region 数据行
                var rowIndex = 0;
                for (var r = (s * pageSize); r < totalCount; r++, rowIndex++)
                {
                    var dataRow = sheet.CreateRow(rowIndex + 1);
                    for (var j = 0; j < headTexts.Length; j++)
                    {
                        dataRow.CreateCell(j).SetCellValue(result.Rows[r][j] != null ? result.Rows[r][j].ToString() : string.Empty);
                    }
                    if (r == pageSize * (s + 1) - 1)
                    {
                        break;
                    }
                }
                #endregion
            }
            var ms = new MemoryStream();
            book.Write(ms);
            context.Response.AddHeader("Content-Disposition", string.Format("attachment; filename={0}.xls", HttpUtility.UrlEncode(fileName)));
            context.Response.BinaryWrite(ms.ToArray());
            ms.Close();
            ms.Dispose();
        }

        /// <summary>
        /// 实现对IList转DataTable的转换
        /// </summary>
        /// <param name="dataList">待转换的IList</param>
        /// <param name="fields"></param>
        /// <returns>转换后的DataTable</returns>
        private static DataTable List2DataTable(IList dataList, string[] fields)
        {
            var dt = new DataTable();
            var propertyInfos = dataList[0].GetType().GetProperties();
            foreach (var field in fields)
            {
                dt.Columns.Add(field, typeof(string));
            }
            foreach (var t in dataList)
            {
                var dr = dt.NewRow();
                foreach (var field2 in fields)
                {
                    var p = propertyInfos.FirstOrDefault(propertyInfo => field2 == propertyInfo.Name);
                    if (p != null) dr[field2] = p.GetValue(t, null);
                }
                dt.Rows.Add(dr);
            }
            return dt;
        }
    }
}
