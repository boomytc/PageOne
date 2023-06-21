using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace db.bll
{
    public class ShoppingTrolley
    {
        //获取Session中的购物车
        public static Dictionary<string, int> getShopping()
        {
            Dictionary<string, int> shopping = rui.sessionHelper.getValue<Dictionary<string, int>>("shopping");
            if (shopping == null)
            {
                shopping = new Dictionary<string, int>();
                rui.sessionHelper.saveValue("shopping", shopping);
            }
            return shopping;
        }

        //加入购物车 - 数据库版
        public static void addCart(string customerCode, string bookCode, db.dbEntities dc)
        {
            efHelper ef = new efHelper(ref dc);

            db.bks_ShoppingTrolley entry = dc.bks_ShoppingTrolley
                .SingleOrDefault(a => a.customerCode == customerCode && a.bookCode == bookCode);
            if (entry == null)
            {
                entry = new db.bks_ShoppingTrolley();
                entry.customerCode = customerCode;
                entry.bookCode = bookCode;
                entry.quantity = 1;
                entry.rowID = ef.newGuid();
                dc.bks_ShoppingTrolley.Add(entry);
            }
            else
            {
                entry.quantity++;
            }
            dc.SaveChanges();
        }

        //数量增加
        public static void addNum(string customerCode, string bookCode, db.dbEntities dc)
        {
            efHelper ef = new efHelper(ref dc);

            db.bks_ShoppingTrolley entry = dc.bks_ShoppingTrolley
                .SingleOrDefault(a => a.customerCode == customerCode && a.bookCode == bookCode);
            entry.quantity++;
            dc.SaveChanges();
        }

        //数量减少
        public static void subNum(string customerCode, string bookCode, db.dbEntities dc)
        {
            efHelper ef = new efHelper(ref dc);

            db.bks_ShoppingTrolley entry = dc.bks_ShoppingTrolley
                .SingleOrDefault(a => a.customerCode == customerCode && a.bookCode == bookCode);
            entry.quantity--;
            if (entry.quantity == 0)
                dc.bks_ShoppingTrolley.Remove(entry);
            dc.SaveChanges();
        }

        //删除
        public static void delete(string rowID, db.dbEntities dc)
        {
            efHelper ef = new efHelper(ref dc);
            try
            {
                ef.Execute("DELETE from dbo.bks_ShoppingTrolley where rowID=@rowID ", new { rowID });
            }
            catch (Exception ex)
            {
                rui.logHelper.log(ex);
                throw ex;
            }
        }

        //通过bookCode获取quantity
        public static int getquantityByCode(string bookCode, db.dbEntities dc)
        {
            efHelper ef = new efHelper(ref dc);

            string sql = "select quantity from dbo.sv_bks_ShoppingTrolley where bookCode=@bookCode ";
            object value = ef.ExecuteScalar(sql, new { bookCode });
            return rui.typeHelper.toInt(value);
        }

        //通过bookCode获取costPrice
        public static decimal getcostPriceByCode(string bookCode, db.dbEntities dc)
        {
            efHelper ef = new efHelper(ref dc);

            string sql = "select price from dbo.bks_BookStockDetail where bookCode=@bookCode ";
            object value = ef.ExecuteScalar(sql, new { bookCode });
            return rui.typeHelper.toDecimal(value);
        }

        //通过bookCode获取Price
        public static decimal getpriceByCode(string bookCode, db.dbEntities dc)
        {
            efHelper ef = new efHelper(ref dc);

            string sql = "select price from dbo.sv_bks_ShoppingTrolley where bookCode=@bookCode ";
            object value = ef.ExecuteScalar(sql, new { bookCode });
            return rui.typeHelper.toDecimal(value);
        }

        //通过bookCode获取subPrice
        public static decimal getPriceByCode(string bookCode, db.dbEntities dc)
        {
            efHelper ef = new efHelper(ref dc);

            string sql = "select subPrice from dbo.sv_bks_ShoppingTrolley where bookCode=@bookCode ";
            object value = ef.ExecuteScalar(sql, new { bookCode });
            return rui.typeHelper.toDecimal(value);
        }

        //计算总价
        public static decimal returnprice(List<string> KeyFieldList, dbEntities dc)
        {
            decimal subPrice = 0;
            foreach (string bookCode in KeyFieldList)
            {
                subPrice += getPriceByCode(bookCode, dc);
            }
            return subPrice;
        }
    }
}
