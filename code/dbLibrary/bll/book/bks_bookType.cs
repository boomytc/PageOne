using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace db.bll
{
    public class bks_bookType
    {
        //查询图书类别
        public static DataTable getTable(db.dbEntities dc)
        {
            efHelper ef = new efHelper(ref dc);
            string sql = " SELECT rowID,bookTypeCode,bookTypeName FROM dbo.bks_BookType ";
            DataTable table = ef.ExecuteDataTable(sql);
            return table;
        }


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
                             where a.bookTypeCode.StartsWith(Code)
                             select a.bookTypeCode).Max();
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
        private static void _checkInput(db.bks_BookType entry)
        {
            rui.dataCheck.checkNotNull(entry.bookTypeCode, "图书类型编号");
            rui.dataCheck.checkNotNull(entry.bookTypeName, "图书类型名称");

        }

        //新增
        public static string insert(db.bks_BookType entry, db.dbEntities dc)
        {
            efHelper ef = new efHelper(ref dc);

            if (rui.typeHelper.isNullOrEmpty(entry.bookTypeCode))
                entry.bookTypeCode = _createCode(dc);
            else if (dc.bks_BookType.Count(a => a.bookTypeCode == entry.bookTypeCode) > 0)
            {
                rui.excptHelper.throwEx("编号已存在");
            }

            //检查数据合法性
            _checkInput(entry);

            //设置字段默认值
            entry.showOrder = null;
            entry.remark = null;
            entry.rowID = ef.newGuid();
            dc.bks_BookType.Add(entry);
            dc.SaveChanges();
            return entry.rowID;
        }

        //修改
        public static void update(db.bks_BookType entry, db.dbEntities dc)
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
                ef.checkCanDelete("bks_Book", "bookTypeCode", keyCode, "已有图书，不允许删除");

                //删除
                ef.Execute("DELETE from dbo.bks_BookType where rowID=@rowID ", new { rowID });
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

            string sql = "select bookTypeCode from dbo.bks_BookType where rowID=@rowID ";
            object value = ef.ExecuteScalar(sql, new { rowID });
            return rui.typeHelper.toString(value);
        }

        //通过rowID获取名称
        public static string getNameByRowID(string rowID, db.dbEntities dc)
        {
            efHelper ef = new efHelper(ref dc);

            string sql = "select bookTypeName from dbo.bks_BookType where rowID=@rowID ";
            object value = ef.ExecuteScalar(sql, new { rowID });
            return rui.typeHelper.toString(value);
        }

        //通过编号获取名称
        public static string getNameByCode(string keyCode, db.dbEntities dc)
        {
            efHelper ef = new efHelper(ref dc);

            string sql = "select bookTypeName from dbo.bks_BookType where bookTypeCode=@bookTypeCode ";
            object value = ef.ExecuteScalar(sql, new { bookTypeCode = keyCode });
            return rui.typeHelper.toString(value);
        }

        //获取实体
        public static db.bks_BookType getEntryByRowID(string rowID, db.dbEntities dc)
        {
            efHelper ef = new efHelper(ref dc); dc = ef.dc;

            db.bks_BookType entry = dc.bks_BookType.Single(a => a.rowID == rowID);
            return entry;
        }

        //获取实体
        public static db.bks_BookType getEntryByCode(string keyCode, db.dbEntities dc)
        {
            efHelper ef = new efHelper(ref dc);

            db.bks_BookType entry = dc.bks_BookType.SingleOrDefault(a => a.bookTypeCode == keyCode);
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
        
        public static List<SelectListItem> bindDdl(bool has请选择 = false, string selectedValue = "")
        {
            efHelper ef = new efHelper();
            string sql = " SELECT bookTypeCode as code,bookTypeName as name FROM dbo.bks_bookType order by bookTypeCode desc ";
            DataTable table = ef.ExecuteDataTable(sql);
            List<SelectListItem> list =rui.listHelper.dataTableToDdlList(table,has请选择,selectedValue);
            return list;
        }
    }
            
}
