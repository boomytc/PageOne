using rui;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace db.bll
{
    public class bks_CustomerAddress
    {
        private static string _createCode(db.dbEntities dc)
        {
            string Code = "A";
            string result = (from a in dc.bks_CustomerAddress
                             where a.addressCode.StartsWith(Code)
                             select a.addressCode).Max();
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
        private static void _checkInput(db.bks_CustomerAddress entry)
        {
            rui.dataCheck.checkNotNull(entry.addressCode, "地址编号");
            rui.dataCheck.checkNotNull(entry.addressName, "地址名称");
        }

        //获取实体
        public static db.bks_CustomerAddress getEntryByRowID(string rowID, db.dbEntities dc)
        {
            efHelper ef = new efHelper(ref dc); dc = ef.dc;

            db.bks_CustomerAddress entry = dc.bks_CustomerAddress.Single(a => a.rowID == rowID);
            return entry;
        }

        //新增
        public static string insert(db.bks_CustomerAddress entry, db.dbEntities dc)
        {
            efHelper ef = new efHelper(ref dc);

            if (rui.typeHelper.isNullOrEmpty(entry.addressCode))
                entry.addressCode = _createCode(dc);
            else if (dc.bks_CustomerAddress.Count(a => a.addressCode == entry.addressCode) > 0)
            {
                rui.excptHelper.throwEx("编号已存在");
            }

            //检查数据合法性
            _checkInput(entry);

            //设置字段默认值
            entry.isDefault = false;
            entry.rowID = ef.newGuid();
            entry.customerCode = rui.sessionHelper.getValue("username");
            dc.bks_CustomerAddress.Add(entry);
            dc.SaveChanges();
            return entry.rowID;
        }

        //修改
        public static void update(db.bks_CustomerAddress entry, db.dbEntities dc)
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
                //删除
                ef.Execute("DELETE from dbo.bks_CustomerAddress where rowID=@rowID ", new { rowID });
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

            string sql = "select addressCode from dbo.bks_CustomerAddress where rowID=@rowID ";
            object value = ef.ExecuteScalar(sql, new { rowID });
            return rui.typeHelper.toString(value);
        }


        /// <summary>
        /// 绑定地址
        /// </summary>
        /// <param name="has请选择"></param>
        /// <param name="isLoginOrg">是否当前登录组织</param>
        /// <param name="selectedValue"></param>
        /// <param name="dc"></param>
        /// <returns></returns>

        public static List<SelectListItem> bindDdl(bool has请选择 = false, string selectedValue = "")
        {
            string customerCode = rui.sessionHelper.getValue("username");
            efHelper ef = new efHelper();
            string sql = " SELECT addressCode as code,addressName as name FROM dbo.bks_CustomerAddress WHERE customerCode = @customerCode order by customerCode desc ";

            DataTable table = ef.ExecuteDataTable(sql,new { customerCode });
            List<SelectListItem> list = rui.listHelper.dataTableToDdlList(table, has请选择, selectedValue);
            return list;
        }
    }
}
