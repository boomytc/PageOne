using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace db
{
    [MetadataType(typeof(metaData))]
    public partial class bks_BookStock
    {
        private class metaData
        {
            public long rowNum { get; set; }
            public string rowID { get; set; }
            [Display(Name = "图书进货编号")]
            public string stockCode { get; set; }
            [Display(Name = "进货时间")]
            public Nullable<System.DateTime> stockDate { get; set; }
            [Display(Name = "进货账号")]
            public string userCode { get; set; }
            [Display(Name = "供货商编号")]
            public string supplierCode { get; set; }
            [Display(Name = "状态")]
            public string status { get; set; }
            [Display(Name = "备注")]
            public string remark { get; set; }
        }
    }
}
