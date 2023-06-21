using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace db.bll
{
    public class bks_OrderInfo
    {
        public static string _createCode(db.dbEntities dc)
        {
            string Code = "Od" + DateTime.Now.ToString("yyyyMMdd");
            string result = (from a in dc.bks_OrderInfo
                             where a.orderCode.StartsWith(Code)
                             select a.orderCode).Max();
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
        private static void _checkInput(db.bks_OrderInfo entry)
        {
            rui.dataCheck.checkNotNull(entry.orderCode, "订单编号");
            rui.dataCheck.checkNotNull(entry.customerCode, "用户编号");
            rui.dataCheck.checkNotNull(entry.addressCode, "地址编号");
            rui.dataCheck.checkNotNull(entry.orderDate, "订单日期");
            rui.dataCheck.checkNotNull(entry.totalPrice, "下单总金额");
            rui.dataCheck.checkNotNull(entry.status, "订单状态");

        }

        //获取实体
        public static db.bks_OrderInfo getEntryByRowID(string rowID, db.dbEntities dc)
        {
            efHelper ef = new efHelper(ref dc); dc = ef.dc;

            db.bks_OrderInfo entry = dc.bks_OrderInfo.Single(a => a.rowID == rowID);
            return entry;
        }

        //生成订单
        public static string generate(string keyFieldValues, string addressCode, string customerCode, DateTime orderDate, decimal totalPrice, db.dbEntities dc)
        {
            efHelper ef = new efHelper(ref dc);

            List<string> KeyFieldList = rui.dbTools.getList(keyFieldValues);
            Dictionary<string, string> errorDic = new Dictionary<string, string>();
            string rowID = ef.newGuid();
            string orderCode = rui.sessionHelper.getValue("orderCode");
            string status = "待付款";

            foreach (string bookCode in KeyFieldList)
            {
                try
                {
                    string selectSql = "SELECT COUNT(*) FROM bks_OrderInfo WHERE orderCode = @orderCode";
                    object result = ef.ExecuteScalar(selectSql, new { orderCode });
                    int count = rui.typeHelper.toInt(result);

                    int quantity = bll.ShoppingTrolley.getquantityByCode(bookCode, dc);

                    if (count == 0)
                    {
                        string insertSql = "INSERT INTO bks_OrderInfo(rowID,addressCode, customerCode, orderCode, orderDate, status, totalPrice) VALUES (@rowID,@addressCode, @customerCode, @orderCode, @orderDate, @status, @totalPrice)";
                        if (ef.Execute(insertSql, new { rowID, addressCode, customerCode, orderCode, orderDate, status, totalPrice, bookCode, quantity }) == 0)
                        {
                            rui.excptHelper.throwEx("数据未插入");
                        }
                    }
                    else
                    {
                        string updateSql = "UPDATE bks_OrderInfo SET  totalPrice = @totalPrice WHERE orderCode = @orderCode";
                        if (ef.Execute(updateSql, new { totalPrice,orderCode }) == 0)
                        {
                            rui.excptHelper.throwEx("数据未变更");
                        }
                    }

                }
                catch (Exception ex)
                {
                    errorDic.Add(bookCode, rui.excptHelper.getExMsg(ex));
                    rui.logHelper.log(ex);
                }
            }
            return rui.dbTools.getBatchMsg("批量下单", KeyFieldList.Count, errorDic);
        }
        //生成订单明细
        public static string generateDetail(string keyFieldValues, string customerCode, db.dbEntities dc)
        {
            efHelper ef = new efHelper(ref dc);

            List<string> KeyFieldList = rui.dbTools.getList(keyFieldValues);
            Dictionary<string, string> errorDic = new Dictionary<string, string>();
            string rowID;
            string orderCode = rui.sessionHelper.getValue("orderCode");

            foreach (string bookCode in KeyFieldList)
            {
                rowID = ef.newGuid();
                try
                {
                    string selectSql = "SELECT COUNT(*) FROM bks_OrderInfo WHERE orderCode = @orderCode";
                    object result = ef.ExecuteScalar(selectSql, new { orderCode });
                    int count = rui.typeHelper.toInt(result);

                    int quantity = bll.ShoppingTrolley.getquantityByCode(bookCode, dc);
                    decimal price = db.bll.ShoppingTrolley.getpriceByCode(bookCode, dc);
                    decimal costPrice = db.bll.ShoppingTrolley.getcostPriceByCode(bookCode, dc);
                    decimal subTotal = (price - costPrice) * quantity; //计算总利润

                    string insertSql = "INSERT INTO bks_OrderDetail(rowID,orderCode,bookCode,quantity,price,costPrice,subTotal) VALUES (@rowID,@orderCode,@bookCode,@quantity,@price,@costPrice,@subTotal)";
                    if (ef.Execute(insertSql, new { rowID, orderCode, price, bookCode, quantity, costPrice, subTotal }) == 0)
                    {
                        rui.excptHelper.throwEx("数据未插入");
                    }

                    ef.Execute("UPDATE bks_BookStockDetail SET quantity =quantity-@quantity,sellQuantity =sellQuantity+@quantity WHERE bookCode = @bookCode", new { quantity, bookCode });
                    ef.Execute("UPDATE bks_Book SET stockSum=stockSum-@quantity,sellSum =sellSum+@quantity WHERE bookCode = @bookCode", new { quantity, bookCode });

                    ef.Execute("DELETE from dbo.bks_ShoppingTrolley where bookCode=@bookCode AND customerCode=@customerCode ", new { bookCode, customerCode });
                }
                catch (Exception ex)
                {
                    errorDic.Add(bookCode, rui.excptHelper.getExMsg(ex));
                    rui.logHelper.log(ex);
                }
            }
            return rui.dbTools.getBatchMsg("请前往订单页面付款！批量下单", KeyFieldList.Count, errorDic);
        }
        //确认付款
        public static void Pay(string rowID, db.dbEntities dc)
        {
            efHelper ef = new efHelper(ref dc);

            string sql = "update dbo.bks_OrderInfo set status='待发货' where rowID=@rowID";
            ef.Execute(sql, new { rowID });
        }
        //确认收货
        public static void Confirm(string rowID, db.dbEntities dc)
        {
            efHelper ef = new efHelper(ref dc);

            string sql = "update dbo.bks_OrderInfo set status='已签收' where rowID=@rowID";
            ef.Execute(sql, new { rowID });
        }
        //确认发货
        public static void Send(string rowID, db.dbEntities dc)
        {
            efHelper ef = new efHelper(ref dc);

            string sql = "update dbo.bks_OrderInfo set status='已发货' where rowID=@rowID";
            ef.Execute(sql, new { rowID });
        }

        public static List<SelectListItem> bindorderDdl(bool has请选择 = false, string selectedValue = "")
        {
            string customerCode = rui.sessionHelper.getValue("username");
            efHelper ef = new efHelper();
            string sql = " SELECT orderCode as code FROM dbo.bks_OrderInfo WHERE customerCode = @customerCode order by customerCode desc ";
            DataTable table = ef.ExecuteDataTable(sql, new { customerCode });
            List<SelectListItem> list = rui.listHelper.dataTableToDdlList(table, has请选择, selectedValue);
            return list;
        }
    }
}
