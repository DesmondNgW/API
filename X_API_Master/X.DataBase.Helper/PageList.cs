using System;
using System.Collections.Generic;

namespace X.DataBase.Helper
{
    public class PageList<TModel>
    {
        /// <summary>
        /// 分页结果集
        /// </summary>
        public List<TModel> List { get; set; }
        /// <summary>
        /// 分页信息
        /// </summary>
        public PageInfo PageInfo { get; set; }
        public PageList()
        {
            PageInfo = new PageInfo();
        }
        public PageList(PageInfo pageInfo)
        {
            PageInfo = pageInfo;
        }
        public PageList(int pageSize, int pageIndex)
        {
            PageInfo = new PageInfo(pageSize, pageIndex);
        }
    }

    /// <summary>
    /// 分页信息
    /// </summary>
    public class PageInfo
    {
        /// <summary>
        /// 总记录数
        /// </summary>
        public int RecordCount { get; set; }
        /// <summary>
        /// 每页大小
        /// </summary>
        public int PageSize { get; set; }
        /// <summary>
        /// 页索引
        /// </summary>
        public int PageIndex { get; set; }
        /// <summary>
        /// 总页数
        /// </summary>
        public int PageCount { get; set; }
        /// <summary>
        /// 该页在所有结果中的起始索引。比如每页10条，第1页起始索引为0，第2页起始索引为10，以此类推
        /// </summary>
        public int FirstIndex { get; set; }
        /// <summary>
        /// 排序字段
        /// </summary>
        public string SortFiled { get; set; }
        /// <summary>
        /// 排序方式  TRUE默认降序DESC
        /// </summary>
        public bool SortMethod { get; set; }
        /// <summary>
        /// 查询条件:无需where
        /// </summary>
        public string Condition { get; set; }
        /// <summary>
        /// 开始计算页数及第一条记录的索引。
        /// 请在赋值PageSize,PageIndex,RecoundCount完毕后调用此方法。
        /// </summary>
        public PageInfo() { }
        public PageInfo(int pageSize, int pageIndex)
        {
            PageSize = pageSize;
            PageIndex = pageIndex;
        }
        public void Compute()
        {
            PageCount = (int)(Math.Ceiling(RecordCount / (decimal)PageSize));
            PageIndex = Math.Min(Math.Max(1, PageIndex), PageCount);
            FirstIndex = PageSize * (PageIndex - 1);
        }
    }
}