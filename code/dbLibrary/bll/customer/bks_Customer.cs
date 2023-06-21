using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace db.bll
{
    public class bks_Customer
    {
        private static string _createCode(db.dbEntities dc)
        {
            string Code = "U" + DateTime.Now.ToString("yyyyMMdd");
            string result = (from a in dc.bks_Customer
                             where a.customerCode.StartsWith(Code)
                             select a.customerCode).Max();
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
        public static db.bks_Customer getEntryByCode(string keyCode, db.dbEntities dc)
        {
            efHelper ef = new efHelper(ref dc);

            db.bks_Customer entry = dc.bks_Customer.SingleOrDefault(a => a.customerCode == keyCode);
            return entry;
        }
        public static db.bks_Customer getEntryByName(string customerName, db.dbEntities dc)
        {
            efHelper ef = new efHelper(ref dc);
            db.bks_Customer entry = dc.bks_Customer.SingleOrDefault(a => a.customerName == customerName);
            return entry;
        }
        public static bool getEntryByName(string customerName)
        {
            db.dbEntities dc = new dbEntities();
            efHelper ef = new efHelper(ref dc);
            db.bks_Customer entry = dc.bks_Customer.SingleOrDefault(a => a.customerName == customerName);
            if (entry != null)
            {
                return false;
            }
            else
            {
                return true;
            }
        }
        private static void _checkInput(db.bks_Customer entry)
        {
            rui.dataCheck.checkNotNull(entry.customerName, "用户名");
            rui.dataCheck.checkNotNull(entry.password, "密码");
        }
        public static string insert(string username, string password, string email, db.dbEntities dc)
        {
            efHelper ef = new efHelper(ref dc);
            db.bks_Customer entry = new db.bks_Customer();
            if (rui.typeHelper.isNullOrEmpty(entry.customerCode))
                entry.customerCode = _createCode(dc);
            else if (dc.bks_Customer.Count(a => a.customerCode == entry.customerCode) > 0)
            {
                rui.excptHelper.throwEx("编号已存在");
            }
            entry.customerName = username;
            entry.password = password;
            entry.telephone = null;
            entry.remark = null;
            entry.email = email;
            entry.rowID = ef.newGuid();
            //检查数据合法性
            _checkInput(entry);
            //设置字段默认值         
            dc.bks_Customer.Add(entry);
            dc.SaveChanges();
            return entry.rowID;
        }
        public static db.bks_Customer getEntryByEmail(string email, db.dbEntities dc)
        {
            efHelper ef = new efHelper(ref dc);
            db.bks_Customer entry = dc.bks_Customer.SingleOrDefault(a => a.email == email);
            return entry;
        }
        public static bool IsValidEmail(string email)
        {
            db.dbEntities db = new dbEntities();
            var emailExist = db.bks_Customer.Any(u => u.email == (string)email);
            if (!emailExist)
            {
                return false;
            }
            else
            {
                return true;
            }
        }
        public static void UpdateByEmail(string email, string password, db.dbEntities dc)
        {
            efHelper ef = new efHelper(ref dc);
            db.bks_Customer entry = getEntryByEmail(email, dc);
            entry.password = password;
            dc.SaveChanges();
        }

        public static void UpdateByPerson(string name, string email, string password, string phone, db.dbEntities dc)
        {
            efHelper ef = new efHelper(ref dc);
            db.bks_Customer entry = getEntryByEmail(email, dc);
            entry.customerName = name;
            entry.password = password;
            entry.telephone = phone;
            entry.email = email;
            dc.SaveChanges();
        }
    }
}
