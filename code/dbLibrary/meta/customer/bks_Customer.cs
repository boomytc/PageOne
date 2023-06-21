using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace db
{
    [MetadataType(typeof(metaData))]
    public partial class bks_Customer
    {
        private class metaData
        {
            public long rowNum { get; set; }
            public string rowID { get; set; }
            [Display(Name = "用户编号")]
            public string customerCode { get; set; }
            [Display(Name = "用户名")]
            [Required]
            public string customerName { get; set; }
            [Display(Name = "密码")]
            [Required]
            public string password { get; set; }
            [Display(Name = "电话")]
            public string telephone { get; set; }
            [Display(Name = "邮箱")]
            [DataType(DataType.EmailAddress)]
            public string email { get; set; }
            [Display(Name = "评论")]
            public string remark { get; set; }
        }
    }
}
