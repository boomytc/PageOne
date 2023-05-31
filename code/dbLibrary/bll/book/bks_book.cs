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
    /// 图书管理
    /// </summary>
    public class bks_Book
    {
        /// <summary>
        /// 利用代码生成图书编号
        /// 编码规则：B201205020001,B201205020002
        /// </summary>
        /// <param name="dc"></param>
        /// <returns></returns>
        private static string _createCode(db.dbEntities dc)
        {
            string Code = "B" + DateTime.Now.ToString("yyyyMMdd");
            string result = (from a in dc.bks_Book
                             where a.bookCode.StartsWith(Code)
                             select a.bookCode).Max();
            if (result != null)
            {
                Code = rui.stringHelper.codeNext(result, 4);
            }
            else
            {
                Code = Code + "0001";
            }
            return Code;
            //return idHelper.nextId().ToString();
        }

        //并发性比较高，编号没规律要求用(雪花编码)方案
        private static rui.idWorker idHelper = new rui.idWorker();

        //对字段的相关合法性进行检查
        private static void _checkInput(db.bks_Book entry)
        {
            rui.dataCheck.checkNotNull(entry.bookCode, "图书编号");
            rui.dataCheck.checkNotNull(entry.bookName, "图书名称");
            rui.dataCheck.checkNotNull(entry.price, "价格");
            rui.dataCheck.checkNotNull(entry.bookTypeCode, "图书类型");
        }

        //新增
        public static string insert(db.bks_Book entry, db.dbEntities dc)
        {
            efHelper ef = new efHelper(ref dc);

            if (rui.typeHelper.isNullOrEmpty(entry.bookCode))
                entry.bookCode = _createCode(dc);
            else if (dc.bks_Book.Count(a => a.bookCode == entry.bookCode) > 0)
            {
                rui.excptHelper.throwEx("编号已存在");
            }

            //检查数据合法性
            _checkInput(entry);

            //保存封面图
            List<string> surfacePicList = rui.webDiskHelper.saveUploadFiles("/upload/bks_book", "surfacePicCtl");
            if (surfacePicList.Count > 0)
                entry.surfacePic = surfacePicList[0];

            //设置字段默认值
            entry.isSell = "否";
            entry.stockSum = 0;
            entry.sellSum = 0;
            entry.rowID = ef.newGuid();
            dc.bks_Book.Add(entry);
            dc.SaveChanges();
            return entry.rowID;
        }

        //修改
        public static void update(db.bks_Book entry, db.dbEntities dc)
        {
            efHelper ef = new efHelper(ref dc);

            //检查数据合法性
            _checkInput(entry);

            efHelper.entryUpdate(entry, dc);

            //保存封面图
            List<string> surfacePicList = rui.webDiskHelper.saveUploadFiles("/upload/bks_book", "surfacePicCtl");
            if (surfacePicList.Count > 0)
                entry.surfacePic = surfacePicList[0];

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
                ef.checkCanDelete("bks_BookStockDetail", "bookCode", keyCode, "已进货，不允许删除");
                ef.checkCanDelete("bks_ShoppingTrolley", "bookCode", keyCode, "已被用户加入购物车，不允许删除");
                ef.checkCanDelete("bks_OrderDetail", "bookCode", keyCode, "已被客户采购，不允许删除");

                //删除
                ef.Execute("DELETE from dbo.bks_Book where rowID=@rowID ", new { rowID });
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

            string sql = "select bookCode from dbo.bks_Book where rowID=@rowID ";
            object value = ef.ExecuteScalar(sql, new { rowID });
            return rui.typeHelper.toString(value);
        }

        //通过rowID获取名称
        public static string getNameByRowID(string rowID, db.dbEntities dc)
        {
            efHelper ef = new efHelper(ref dc);

            string sql = "select bookName from dbo.bks_Book where rowID=@rowID ";
            object value = ef.ExecuteScalar(sql, new { rowID });
            return rui.typeHelper.toString(value);
        }

        //通过编号获取名称
        public static string getNameByCode(string keyCode, db.dbEntities dc)
        {
            efHelper ef = new efHelper(ref dc);

            string sql = "select bookName from dbo.bks_Book where bookCode=@bookCode ";
            object value = ef.ExecuteScalar(sql, new { bookCode = keyCode });
            return rui.typeHelper.toString(value);
        }

        //获取实体
        public static db.bks_Book getEntryByRowID(string rowID, db.dbEntities dc)
        {
            efHelper ef = new efHelper(ref dc); dc = ef.dc;

            db.bks_Book entry = dc.bks_Book.Single(a => a.rowID == rowID);
            return entry;
        }

        //获取实体
        public static db.bks_Book getEntryByCode(string keyCode, db.dbEntities dc)
        {
            efHelper ef = new efHelper(ref dc);

            db.bks_Book entry = dc.bks_Book.SingleOrDefault(a => a.bookCode == keyCode);
            return entry;
        }

        /// <summary>
        /// 绑定图书类型
        /// </summary>
        /// <param name="has请选择"></param>
        /// <param name="isLoginOrg">是否当前登录组织</param>
        /// <param name="selectedValue"></param>
        /// <param name="dc"></param>
        /// <returns></returns>

        public static List<SelectListItem> bindDdl(string bookTypeCode,string pressCode,bool has请选择 = false, string selectedValue = "")
        {
            efHelper ef = new efHelper();
            string sql = " SELECT bookCode as code,bookName as name FROM dbo.bks_Book where 1=1 ";
            if(rui.typeHelper.isNotNullOrEmpty(bookTypeCode))
            {
                sql += " and bookTypeCode=@bookTypeCode";
            }
            if (rui.typeHelper.isNotNullOrEmpty(pressCode))
            {
                sql += " and pressCode=@pressCode";
            }
            DataTable table = ef.ExecuteDataTable(sql, new { bookTypeCode ,pressCode});
            List<SelectListItem> list = rui.listHelper.dataTableToDdlList(table, has请选择, selectedValue);
            return list;
        }

        //批量保存
        public static string batchSave(List<string> bookCodeList, List<string> priceList, db.dbEntities dc)
        {
            efHelper ef = new efHelper(ref dc);
            Dictionary<string, string> errorDic = new Dictionary<string, string>();
            for (int i = 0; i < bookCodeList.Count; i++)
            {
                try
                {
                    //采用Dapper的方式写代码,变量都定义为参数
                    string sql = " UPDATE dbo.bks_Book SET price=@price WHERE bookCode=@bookCode ";
                    if (ef.Execute(sql, new { price = priceList[i], bookCode = bookCodeList[i] }) == 0)
                        rui.excptHelper.throwEx("数据未变更");
                }
                catch (Exception ex)
                {
                    errorDic.Add(bookCodeList[i], rui.excptHelper.getExMsg(ex));
                    rui.logHelper.log(ex);
                }
            }
            return rui.dbTools.getBatchMsg("批量保存", bookCodeList.Count, errorDic);
        }

        /// <summary>
        /// 批量上架
        /// </summary>
        /// <param name="keyFieldValues"></param>
        /// <param name="dc"></param>
        /// <returns></returns>
        public static string batchSell(string keyFieldValues, db.dbEntities dc)
        {
            efHelper ef = new efHelper(ref dc);

            List<string> KeyFieldList = rui.dbTools.getList(keyFieldValues);
            Dictionary<string, string> errorDic = new Dictionary<string, string>();

            //查询批量操作中已显示的记录，用于提示
            string sqlCheck = " SELECT bookCode,isSell FROM dbo.bks_Book WHERE bookCode IN @bookCode AND isSell='是' ";
            DataTable table = ef.ExecuteDataTable(sqlCheck, new { bookCode = KeyFieldList });

            foreach (string bookCode in KeyFieldList)
            {
                try
                {
                    DataRow[] rows = table.Select("bookCode='" + bookCode + "'");
                    rui.dbTools.checkRowFieldValue(rows, "isSell", "是", "已上架");

                    string sql = " UPDATE bks_Book SET isSell='是' WHERE bookCode=@bookCode ";
                    if (ef.Execute(sql, new { bookCode }) == 0)
                        rui.excptHelper.throwEx("数据未变更");
                }
                catch (Exception ex)
                {
                    errorDic.Add(bookCode, rui.excptHelper.getExMsg(ex));
                    rui.logHelper.log(ex);
                }
            }
            return rui.dbTools.getBatchMsg("批量上架", KeyFieldList.Count, errorDic);
        }


        /// <summary>
        /// 批量下架
        /// </summary>
        /// <param name="keyFieldValues"></param>
        /// <param name="dc"></param>
        /// <returns></returns>
        public static string batchNoSell(string keyFieldValues, db.dbEntities dc)
        {
            efHelper ef = new efHelper(ref dc);

            List<string> KeyFieldList = rui.dbTools.getList(keyFieldValues);
            Dictionary<string, string> errorDic = new Dictionary<string, string>();

            //查询批量操作中已显示的记录，用于提示
            string sqlCheck = " SELECT bookCode,isSell FROM dbo.bks_Book WHERE bookCode IN @bookCode AND isSell='否' ";
            DataTable table = ef.ExecuteDataTable(sqlCheck, new { bookCode = KeyFieldList });

            foreach (string bookCode in KeyFieldList)
            {
                try
                {
                    //检查遍历的数据是否已下架
                    DataRow[] rows = table.Select("bookCode='" + bookCode + "'");
                    rui.dbTools.checkRowFieldValue(rows, "isSell", "否", "已下架");

                    string sql = " UPDATE bks_Book SET isSell='否' WHERE bookCode=@bookCode ";
                    if (ef.Execute(sql, new { bookCode }) == 0)
                        rui.excptHelper.throwEx("数据未变更");
                }
                catch (Exception ex)
                {
                    errorDic.Add(bookCode, rui.excptHelper.getExMsg(ex));
                    rui.logHelper.log(ex);
                }
            }
            return rui.dbTools.getBatchMsg("批量下架", KeyFieldList.Count, errorDic);
        }

        /// <summary>
        /// 批量变更图书类别
        /// </summary>
        /// <param name="keyFieldValues"></param>
        /// <param name="dc"></param>
        /// <returns></returns>
        public static string batchChangeBookType(string keyFieldValues, string bookTypeCode, db.dbEntities dc)
        {
            efHelper ef = new efHelper(ref dc);

            List<string> KeyFieldList = rui.dbTools.getList(keyFieldValues);
            Dictionary<string, string> errorDic = new Dictionary<string, string>();

            foreach (string bookCode in KeyFieldList)
            {
                try
                {
                    string sql = " UPDATE bks_Book SET bookTypeCode=@bookTypeCode WHERE bookCode=@bookCode ";
                    if (ef.Execute(sql, new { bookCode, bookTypeCode }) == 0)
                        rui.excptHelper.throwEx("数据未变更");
                }
                catch (Exception ex)
                {
                    errorDic.Add(bookCode, rui.excptHelper.getExMsg(ex));
                    rui.logHelper.log(ex);
                }
            }
            return rui.dbTools.getBatchMsg("批量变更图书类型", KeyFieldList.Count, errorDic);
        }

        /// <summary>
        /// 商品单个下架
        /// </summary>
        /// <param name="bookCode"></param>
        /// <param name="dc"></param>
        public static void NoSell(string bookCode,db.dbEntities dc)
        {
            efHelper ef = new efHelper(ref dc);

            string sql = "UPDATE dbo.bks_Book SET isSell='否' WHERE bookCode=@bookCode";
            ef.Execute(sql,new{bookCode});
        }

        /// <summary>
        /// 商品单个上架
        /// </summary>
        /// <param name="bookCode"></param>
        /// <param name="dc"></param>
        public static void IsSell(string bookCode, db.dbEntities dc)
        {
            efHelper ef = new efHelper(ref dc);

            string sql = "UPDATE dbo.bks_Book SET isSell='是' WHERE bookCode=@bookCode";
            ef.Execute(sql, new { bookCode });
        }

        //返回出版社下拉框绑定项目
        public static List<SelectListItem> bindDdl(bool has请选择 = false, string selectedValue = "", string bookTypeCode = "", string pressCode = "")
        {
            efHelper ef = new efHelper();
            string sql = " SELECT bookCode AS code,bookName AS name FROM dbo.bks_Book where 1=1 ";
            if (rui.typeHelper.isNotNullOrEmpty(bookTypeCode))
                sql += " and bookTypeCode='" + bookTypeCode + "' ";
            if (rui.typeHelper.isNotNullOrEmpty(pressCode))
                sql += " and pressCode='" + pressCode + "' ";
            sql += " ORDER BY bookCode ASC ";
            DataTable table = ef.ExecuteDataTable(sql, new { bookTypeCode,pressCode});
            List<SelectListItem> list = rui.listHelper.dataTableToDdlList(table, has请选择, selectedValue);
            return list;
        }

        //获取图书类型联动的图书列表Json数据
        public static string getJsonBookDdl(string bookTypeCode, string pressCode)
        {
            return rui.jsonResult.SelectListToJsonStr(bindDdl(false, "", bookTypeCode,pressCode));
        }

    }
}
