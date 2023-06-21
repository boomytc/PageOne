using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace db
{
    [MetadataType(typeof(metaData))]
    public partial class bks_Press
    {
        private class metaData
        {
            public long rowNum { get; set; }
            public string rowID { get; set; }
            [Display(Name = "出版社编号")]
            public string pressCode { get; set; }
            [Display(Name = "出版社")]
            public string pressName { get; set; }
            [Display(Name = "排序")]
            public Nullable<int> showOrder { get; set; }
            [Display(Name = "备注")]
            public string remark { get; set; }
        }
    }
}
