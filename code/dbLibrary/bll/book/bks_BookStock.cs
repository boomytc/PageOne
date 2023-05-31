using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace db.bll
{
    public class bks_BookStock
    {
        /// <summary>
        /// 利用代码生成图书编号
        /// 编码规则：B201205020001,B201205020002
        /// </summary>
        /// <param name="dc"></param>
        /// <returns></returns>
        private static string _createCode(db.dbEntities dc)
        {
            string Code = "S" + DateTime.Now.ToString("yyyyMMdd");
            string result = (from a in dc.bks_BookStock
                             where a.stockCode.StartsWith(Code)
                             select a.stockCode).Max();
            if (result != null)
            {
                Code = rui.stringHelper.codeNext(result, 4);
            }
            else
            {
                Code = Code + "001";
            }
            return Code;
            //return idHelper.nextId().ToString();
        }

        //并发性比较高，编号没规律要求用(雪花编码)方案
        private static rui.idWorker idHelper = new rui.idWorker();

        //对字段的相关合法性进行检查
        private static void _checkInput(db.bks_BookStock entry)
        {
            rui.dataCheck.checkNotNull(entry.stockCode, "图书进货编号");
        }

        //新增
        public static string insert(db.bks_BookStock entry, db.dbEntities dc)
        {
            efHelper ef = new efHelper(ref dc);

            _checkInput(entry);
            if (rui.typeHelper.isNullOrEmpty(entry.stockCode))
                entry.stockCode = _createCode(dc);
            else if (dc.bks_BookStock.Count(a => a.stockCode == entry.stockCode) > 0)
            {
                rui.excptHelper.throwEx("编号已存在");
            }
            entry.rowID = ef.newGuid();
            //如果当前登录人就是经手人，则通过如下代码实现，如果通过下拉框选择经手人，则不需要如下的代码
            entry.userCode = db.bll.loginAdminHelper.getUserCode();
            entry.status = "初始";
            dc.bks_BookStock.Add(entry);
            dc.SaveChanges();
            return entry.rowID;
        }

        //修改
        public static void update(db.bks_BookStock entry, db.dbEntities dc)
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
                db.bks_BookStock entry = getEntryByRowID(rowID, dc);
                if (entry.status == "发布")
                    rui.excptHelper.throwEx("已发布，不允许删除");

                //删除
                ef.Execute("DELETE from dbo.bks_BookStock where rowID=@rowID ", new { rowID });
                ef.Execute("DELETE from dbo.bks_BookStockDetail where stockCode=@stockCode",
                    new { stockCode = keyCode });
            }
            catch (Exception ex)
            {
                rui.logHelper.log(ex);
                throw ex;
            }
        }

        //发布
        public static void publish(string rowID, db.dbEntities dc)
        {
            efHelper ef = new efHelper(ref dc);

            string sql = "update dbo.bks_BookStock set status='确认' where rowID=@rowID";
            ef.Execute(sql, new {rowID});
        }

        //通过rowID获取主键
        public static string getCodeByRowID(string rowID, db.dbEntities dc)
        {
            efHelper ef = new efHelper(ref dc);

            string sql = "select stockCode from dbo.bks_BookStock where rowID=@rowID ";
            object value = ef.ExecuteScalar(sql, new { rowID });
            return rui.typeHelper.toString(value);
        }

        //通过rowID获取名称
        public static string getNameByRowID(string rowID, db.dbEntities dc)
        {
            efHelper ef = new efHelper(ref dc);

            string sql = "select sysAdmin from dbo.bks_BookStock where rowID=@rowID ";
            object value = ef.ExecuteScalar(sql, new { rowID });
            return rui.typeHelper.toString(value);
        }

        //通过编号获取名称
        public static string getNameByCode(string keyCode, db.dbEntities dc)
        {
            efHelper ef = new efHelper(ref dc);

            string sql = "select sysAdmin from dbo.bks_BookStock where stockCode=@stockCode ";
            object value = ef.ExecuteScalar(sql, new { stockCode = keyCode });
            return rui.typeHelper.toString(value);
        }

        //获取实体
        public static db.bks_BookStock getEntryByRowID(string rowID, db.dbEntities dc)
        {
            efHelper ef = new efHelper(ref dc); dc = ef.dc;

            db.bks_BookStock entry = dc.bks_BookStock.Single(a => a.rowID == rowID);
            return entry;
        }

        //获取实体
        public static db.bks_BookStock getEntryByCode(string keyCode, db.dbEntities dc)
        {
            efHelper ef = new efHelper(ref dc);

            db.bks_BookStock entry = dc.bks_BookStock.SingleOrDefault(a => a.stockCode == keyCode);
            return entry;
        }


        //批量保存
        public static string batchSave(List<string> stockCodeList, List<string> detailNoList, db.dbEntities dc)
        {
            efHelper ef = new efHelper(ref dc);
            Dictionary<string, string> errorDic = new Dictionary<string, string>();
            for (int i = 0; i < stockCodeList.Count; i++)
            {
                try
                {
                    //采用Dapper的方式写代码,变量都定义为参数
                    string sql = " UPDATE dbo.bks_BookStock SET sysAdmin=@sysAdmin WHERE stockCode=@stockCode ";
                    if (ef.Execute(sql, new { sysAdmin = detailNoList[i], stockCode = stockCodeList[i] }) == 0)
                        rui.excptHelper.throwEx("数据未变更");
                }
                catch (Exception ex)
                {
                    errorDic.Add(stockCodeList[i], rui.excptHelper.getExMsg(ex));
                    rui.logHelper.log(ex);
                }
            }
            return rui.dbTools.getBatchMsg("批量保存", stockCodeList.Count, errorDic);
        }

        ///// <summary>
        ///// 绑定图书进货
        ///// </summary>
        ///// <param name="has请选择"></param>
        ///// <param name="isLoginOrg">是否当前登录组织</param>
        ///// <param name="selectedValue"></param>
        ///// <param name="dc"></param>
        ///// <returns></returns>
        //public static List<SelectListItem> bindDdl(bool has请选择 = false, string selectedValue = "")
        //{
        //    efHelper ef = new efHelper();
        //    string sql = " SELECT stockCode as code,sysAdmin as No FROM dbo.bks_BookStock order by stockCode desc ";
        //    DataTable table = ef.ExecuteDataTable(sql);
        //    List<SelectListItem> list = rui.listHelper.dataTableToDdlList(table, has请选择, selectedValue);
        //    return list;
        //}
    }
}
