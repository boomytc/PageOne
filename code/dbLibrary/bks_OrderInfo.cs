//------------------------------------------------------------------------------
// <auto-generated>
//     此代码已从模板生成。
//
//     手动更改此文件可能导致应用程序出现意外的行为。
//     如果重新生成代码，将覆盖对此文件的手动更改。
// </auto-generated>
//------------------------------------------------------------------------------

namespace db
{
    using System;
    using System.Collections.Generic;
    
    public partial class bks_OrderInfo
    {
        public long rowNum { get; set; }
        public string rowID { get; set; }
        public string orderCode { get; set; }
        public string customerCode { get; set; }
        public string addressCode { get; set; }
        public Nullable<System.DateTime> orderDate { get; set; }
        public Nullable<decimal> totalPrice { get; set; }
        public string status { get; set; }
        public string employeeCode { get; set; }
        public string remark { get; set; }
    }
}