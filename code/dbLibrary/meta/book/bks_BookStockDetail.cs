using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace db
{
    [MetadataType(typeof(metaData))]
    public partial class bks_BookStockDetail
    {
        private class metaData
        {
            public long rowNum { get; set; }
            public string rowID { get; set; }
            [Display(Name = "图书进货编号")]
            public string stockCode { get; set; }
            [Display(Name = "进货详情编号")]
            public int detailNo { get; set; }
            [Display(Name = "图书编号")]
            public string bookCode { get; set; }
            [Display(Name = "进货数量")]
            public Nullable<int> quantity { get; set; }
            [Display(Name = "出售数量")]
            public Nullable<int> sellQuantity { get; set; }
            [Display(Name = "价格")]
            public Nullable<decimal> price { get; set; }
            [Display(Name = "备注")]
            public string remark { get; set; }
        }
    }
}
