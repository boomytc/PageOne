using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace db
{
    [MetadataType(typeof(metaData))]
    public partial class bks_BookType
    {
        private class metaData
        {
            public long rowNum { get; set; }
            public string rowID { get; set; }
            [Display(Name = "图书类型编号")]
            public string bookTypeCode { get; set; }
            [Display(Name = "图书类型")]
            public string bookTypeName { get; set; }
            [Display(Name = "排序")]
            public Nullable<int> showOrder { get; set; }
            [Display(Name = "备注")]
            public string remark { get; set; }
        }
    }
}
