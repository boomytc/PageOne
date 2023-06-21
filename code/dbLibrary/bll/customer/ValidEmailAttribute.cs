using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace db.bll
{
    public class ValidEmailAttribute:ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            db.dbEntities db = new dbEntities();
            var emailExist = db.bks_Customer.Any(u => u.email == (string)value);
            if (!emailExist)
            {
                return new ValidationResult("邮箱地址无效");
            }
            return ValidationResult.Success;
        }
    }
}
