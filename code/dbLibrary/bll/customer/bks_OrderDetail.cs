using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace db.bll
{
    public class bks_OrderDetail
    {
        private static int _createDetailNo(string orderCode, db.dbEntities dc)
        {
            int value = 0;
            var result = from a in dc.bks_OrderDetail
                         where a.orderCode == orderCode
                         orderby a.rowNum descending
                         select a.rowNum;
            if (result.Count() == 0)
                value = 1;
            else
                value = rui.typeHelper.toInt(result.First()) + 1;
            return value;
        }

        private static rui.idWorker idHelper = new rui.idWorker();
        private static void _checkInput(db.bks_OrderDetail entry)
        {
            rui.dataCheck.checkNotNull(entry.orderCode, "订单编号");
        }


        public static string getCodeByRowID(string rowID, db.dbEntities dc)
        {
            efHelper ef = new efHelper(ref dc);

            string sql = "select orderCode from dbo.bks_OrderDetail where rowID=@rowID ";
            object value = ef.ExecuteScalar(sql, new { rowID });
            return rui.typeHelper.toString(value);
        }

        //通过rowID获取名称
        public static string getNameByRowID(string rowID, db.dbEntities dc)
        {
            efHelper ef = new efHelper(ref dc);

            string sql = "select rowNum from dbo.bks_OrderDetail where rowID=@rowID ";
            object value = ef.ExecuteScalar(sql, new { rowID });
            return rui.typeHelper.toString(value);
        }

        //通过编号获取名称
        public static string getNameByCode(string keyCode, db.dbEntities dc)
        {
            efHelper ef = new efHelper(ref dc);

            string sql = "select rowNum from dbo.bks_OrderDetail where orderCode=@OrderCode ";
            object value = ef.ExecuteScalar(sql, new { orderCode = keyCode });
            return rui.typeHelper.toString(value);
        }

        //获取实体
        public static db.bks_OrderDetail getEntryByRowID(string rowID, db.dbEntities dc)
        {
            efHelper ef = new efHelper(ref dc); dc = ef.dc;

            db.bks_OrderDetail entry = dc.bks_OrderDetail.Single(a => a.rowID == rowID);
            return entry;
        }

        //获取实体
        public static db.bks_OrderDetail getEntryByCode(string keyCode, db.dbEntities dc)
        {
            efHelper ef = new efHelper(ref dc);

            db.bks_OrderDetail entry = dc.bks_OrderDetail.SingleOrDefault(a => a.orderCode == keyCode);
            return entry;
        }

    }
}
