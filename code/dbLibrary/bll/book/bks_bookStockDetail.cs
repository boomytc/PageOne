using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace db.bll
{
    /// <summary>
    /// 出版社管理
    /// </summary>
    public class bks_bookStockDetail
    {
        /// <summary>
        /// 生成编号
        /// </summary>
        /// <param name="dc"></param>
        /// <returns></returns>
        private static int _createDetailNo(string stockCode, db.dbEntities dc)
        {
            int value = 0;
            var result = from a in dc.bks_BookStockDetail
                         where a.stockCode == stockCode
                         orderby a.detailNo descending
                         select a.detailNo;
            if (result.Count() == 0)
                value = 1;
            else
                value = rui.typeHelper.toInt(result.First()) + 1;
            return value;
        }


        //并发性比较高，编号没规律要求用(雪花编码)方案
        private static rui.idWorker idHelper = new rui.idWorker();

        //对字段的相关合法性进行检查
        private static void _checkInput(db.bks_BookStockDetail entry)
        {
            rui.dataCheck.checkNotNull(entry.stockCode, "图书进货编号");
        }

        //新增
        public static string insert(db.bks_BookStockDetail entry, db.dbEntities dc)
        {
            efHelper ef = new efHelper(ref dc);

            _checkInput(entry);
            //if (rui.typeHelper.isNotNullOrEmpty(entry.stockCode))
            //    rui.excptHelper.throwEx("必须提供关联的入库编号");

            entry.detailNo = _createDetailNo(entry.stockCode, dc);
            entry.sellQuantity = 0;
            entry.rowID = ef.newGuid();
            dc.bks_BookStockDetail.Add(entry);
            dc.SaveChanges();
            return entry.rowID;
        }

        //修改
        public static void update(db.bks_BookStockDetail entry, db.dbEntities dc)
        {
            efHelper ef = new efHelper(ref dc);

            //检查数据合法性
            _checkInput(entry);

            efHelper.entryUpdate(entry, dc);
            dc.SaveChanges();
        }

        //删除
        public static void delete(string rowID, db.dbEntities dc)
        {
            efHelper ef = new efHelper(ref dc);
            try
            {
                string keyCode = getCodeByRowID(rowID, dc);
                
                //删除前检查
                ef.checkCanDelete("bks_BookStock", "stockCode", keyCode, "已有图书进货信息，不允许删除");

                //删除
                ef.Execute("DELETE from dbo.bks_BookStockDetail where rowID=@rowID ", new { rowID });
            }
            catch (Exception ex)
            {
                rui.logHelper.log(ex);
                throw ex;
            }
        }

        //通过rowID获取主键
        public static string getCodeByRowID(string rowID, db.dbEntities dc)
        {
            efHelper ef = new efHelper(ref dc);

            string sql = "select stockCode from dbo.bks_BookStockDetail where rowID=@rowID ";
            object value = ef.ExecuteScalar(sql, new { rowID });
            return rui.typeHelper.toString(value);
        }

        //通过rowID获取名称
        public static string getNameByRowID(string rowID, db.dbEntities dc)
        {
            efHelper ef = new efHelper(ref dc);

            string sql = "select detailNo from dbo.bks_BookStockDetail where rowID=@rowID ";
            object value = ef.ExecuteScalar(sql, new { rowID });
            return rui.typeHelper.toString(value);
        }

        //通过编号获取名称
        public static string getNameByCode(string keyCode, db.dbEntities dc)
        {
            efHelper ef = new efHelper(ref dc);

            string sql = "select detailNo from dbo.bks_BookStockDetail where stockCode=@stockCode ";
            object value = ef.ExecuteScalar(sql, new { stockCode = keyCode });
            return rui.typeHelper.toString(value);
        }

        //获取实体
        public static db.bks_BookStockDetail getEntryByRowID(string rowID, db.dbEntities dc)
        {
            efHelper ef = new efHelper(ref dc); dc = ef.dc;

            db.bks_BookStockDetail entry = dc.bks_BookStockDetail.Single(a => a.rowID == rowID);
            return entry;
        }

        //获取实体
        public static db.bks_BookStockDetail getEntryByCode(string keyCode, db.dbEntities dc)
        {
            efHelper ef = new efHelper(ref dc);

            db.bks_BookStockDetail entry = dc.bks_BookStockDetail.SingleOrDefault(a => a.stockCode == keyCode);
            return entry;
        }


        //批量保存
        public static string batchSave(List<string> rowIDList, List<string> bookCodeList, List<string> quantityList, List<string> priceList, db.dbEntities dc)
        {
            efHelper ef = new efHelper(ref dc);
            Dictionary<string, string> errorDic = new Dictionary<string, string>();
            for (int i = 0; i < rowIDList.Count; i++)
            {
                try
                {
                    //采用Dapper的方式写代码,变量都定义为参数
                    string sql = " UPDATE dbo.bks_BookStockDetail SET bookCode=@bookCode,quantity=@quantity,price=@price WHERE rowID=@rowID ";
                    if (ef.Execute(sql, new { bookCode = bookCodeList[i], quantity = quantityList[i], price = priceList[i], rowID = rowIDList[i] }) == 0)
                        rui.excptHelper.throwEx("数据未变更");
                }
                catch (Exception ex)
                {
                    errorDic.Add(rowIDList[i], rui.excptHelper.getExMsg(ex));
                    rui.logHelper.log(ex);
                }
            }
            return rui.dbTools.getBatchMsg("批量保存", bookCodeList.Count, errorDic);
        }

        /// <summary>
        /// 批量新增
        /// </summary>
        /// <param name="stockCode"></param>
        /// <param name="selectedBooks"></param>
        public static void batchInsert(string stockCode, string selectedBooks, db.dbEntities dc)
        {
            efHelper ef = new efHelper(ref dc);

            List<string> list = rui.dbTools.getList(selectedBooks);

            foreach (string bookCode in list)
            {
                db.bks_BookStockDetail entry = new db.bks_BookStockDetail();
                entry.stockCode = stockCode;
                entry.bookCode = bookCode;
                db.bll.bks_bookStockDetail.insert(entry, dc);
            }
        }
    }
}
