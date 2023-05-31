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
    public class bks_press
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
                             where a.pressCode.StartsWith(Code)
                             select a.pressCode).Max();
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
        private static void _checkInput(db.bks_Press entry)
        {
            rui.dataCheck.checkNotNull(entry.pressCode, "图书编号");
            rui.dataCheck.checkNotNull(entry.pressName, "图书名称");
        }

        //新增
        public static string insert(db.bks_Press entry, db.dbEntities dc)
        {
            efHelper ef = new efHelper(ref dc);

            if (rui.typeHelper.isNullOrEmpty(entry.pressCode))
                entry.pressCode = _createCode(dc);
            else if (dc.bks_Press.Count(a => a.pressCode == entry.pressCode) > 0)
            {
                rui.excptHelper.throwEx("编号已存在");
            }

            //检查数据合法性
            _checkInput(entry);

            //设置字段默认值
            entry.showOrder = null;
            entry.remark = null;

            entry.rowID = ef.newGuid();
            dc.bks_Press.Add(entry);
            dc.SaveChanges();
            return entry.rowID;
        }

        //修改
        public static void update(db.bks_Press entry, db.dbEntities dc)
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
                ef.checkCanDelete("bks_Book", "pressCode", keyCode, "已有图书，不允许删除");

                //删除
                ef.Execute("DELETE from dbo.bks_Press where rowID=@rowID ", new { rowID });
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

            string sql = "select pressCode from dbo.bks_Press where rowID=@rowID ";
            object value = ef.ExecuteScalar(sql, new { rowID });
            return rui.typeHelper.toString(value);
        }

        //通过rowID获取名称
        public static string getNameByRowID(string rowID, db.dbEntities dc)
        {
            efHelper ef = new efHelper(ref dc);

            string sql = "select pressName from dbo.bks_Press where rowID=@rowID ";
            object value = ef.ExecuteScalar(sql, new { rowID });
            return rui.typeHelper.toString(value);
        }

        //通过编号获取名称
        public static string getNameByCode(string keyCode, db.dbEntities dc)
        {
            efHelper ef = new efHelper(ref dc);

            string sql = "select pressName from dbo.bks_Press where pressCode=@pressCode ";
            object value = ef.ExecuteScalar(sql, new { pressCode = keyCode });
            return rui.typeHelper.toString(value);
        }

        //获取实体
        public static db.bks_Press getEntryByRowID(string rowID, db.dbEntities dc)
        {
            efHelper ef = new efHelper(ref dc); dc = ef.dc;

            db.bks_Press entry = dc.bks_Press.Single(a => a.rowID == rowID);
            return entry;
        }

        //获取实体
        public static db.bks_Press getEntryByCode(string keyCode, db.dbEntities dc)
        {
            efHelper ef = new efHelper(ref dc);

            db.bks_Press entry = dc.bks_Press.SingleOrDefault(a => a.pressCode == keyCode);
            return entry;
        }

        //批量保存excel
        public static void SaveData(db.dbEntities dc)
        {
            efHelper ef = new efHelper(ref dc);
            try
            {
                //查询数据库中已经存在的数据
                DataTable tableExist = ef.ExecuteDataTable("select * from bks_Press");
                //获取要导入的数据
                DataTable table = rui.sessionHelper.getValue<DataTable>("importTable");
                if (table == null || table.Rows.Count == 0)
                    rui.excptHelper.throwEx("文件内容为空！");
                ef.beginTran();
                for (int i = 0; i < table.Rows.Count; i++)
                {
                    //获取Excel导入的每一行数据
                    string pressCode = rui.typeHelper.toString(table.Rows[i]["出版社编号"]);
                    string pressName = rui.typeHelper.toString(table.Rows[i]["出版社名称"]);

                    //1、如果主键不存在,则插入一条数据
                    if (tableExist.Select("pressCode='" + pressCode + "'").Length == 0)
                    {
                        db.bks_Press entry = new db.bks_Press();
                        entry.pressCode = pressCode;
                        entry.pressName = pressName;
                        entry.rowID = ef.newGuid();
                        dc.bks_Press.Add(entry);
                    }
                }
                dc.SaveChanges();
                ef.commit();
            }
            catch (Exception ex)
            {
                ef.rollBack();
                rui.logHelper.log("导入出错", ex, true);
                throw ex;
            }
        }

        //批量保存
        public static string batchSave(List<string> pressCodeList, List<string> pressNameList, db.dbEntities dc)
        {
            efHelper ef = new efHelper(ref dc);
            Dictionary<string, string> errorDic = new Dictionary<string, string>();
            for (int i = 0; i < pressCodeList.Count; i++)
            {
                try
                {
                    //采用Dapper的方式写代码,变量都定义为参数
                    string sql = " UPDATE dbo.bks_Press SET pressName=@pressName WHERE pressCode=@pressCode ";
                    if (ef.Execute(sql, new { pressName = pressNameList[i], pressCode = pressCodeList[i] }) == 0)
                        rui.excptHelper.throwEx("数据未变更");
                }
                catch (Exception ex)
                {
                    errorDic.Add(pressCodeList[i], rui.excptHelper.getExMsg(ex));
                    rui.logHelper.log(ex);
                }
            }
            return rui.dbTools.getBatchMsg("批量保存", pressCodeList.Count, errorDic);
        }

        /// <summary>
        /// 绑定图书出版社
        /// </summary>
        /// <param name="has请选择"></param>
        /// <param name="isLoginOrg">是否当前登录组织</param>
        /// <param name="selectedValue"></param>
        /// <param name="dc"></param>
        /// <returns></returns>
        public static List<SelectListItem> bindDdl(bool has请选择 = false, string selectedValue = "")
        {
            efHelper ef = new efHelper();
            string sql = " SELECT pressCode as code,pressName as name FROM dbo.bks_Press order by pressCode desc ";
            DataTable table = ef.ExecuteDataTable(sql);
            List<SelectListItem> list = rui.listHelper.dataTableToDdlList(table, has请选择, selectedValue);
            return list;
        }

    }
}
