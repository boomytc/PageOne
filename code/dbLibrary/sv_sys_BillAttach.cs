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
    
    public partial class sv_sys_BillAttach
    {
        public string rowID { get; set; }
        public long rowNum { get; set; }
        public string attachCode { get; set; }
        public string relatedResourceCode { get; set; }
        public string relatedKeyCode { get; set; }
        public Nullable<int> relatedDetailNo { get; set; }
        public string attachUrl { get; set; }
        public string prieviewUrl { get; set; }
        public string attachName { get; set; }
        public Nullable<long> attachLength { get; set; }
        public string attachMIME { get; set; }
        public string attachContent { get; set; }
        public string uploadUserCode { get; set; }
        public Nullable<System.DateTime> uploadDateTime { get; set; }
        public string remark { get; set; }
        public string uploadUserName { get; set; }
    }
}
