using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using db.view;
namespace db.bll
{
    public class Sys_userService
    {
        //登录
        public db.view.bks_Customer Login(string username, string password)
        {
            db.view.bks_Customer sys_User = null;
            //编写Sql语句
            string sql = " select customerCode,cusomterName,password" +
                         " from bks_Customer" +
                         " where customerName=@cusomterName and password=@password";
            //编写参数
            SqlParameter[] pms = { new SqlParameter("customerName", username), new SqlParameter("password", password) };
            //调用SQLHelper类中的查询方法
            SqlDataReader reader = SQLHelper.ExecuteReader(sql, pms);
            if (reader.Read())
            {
                sys_User = new db.view.bks_Customer();
                sys_User.customerCode =reader["customerCode"].ToString();
                sys_User.customerName= reader["username"].ToString();
                sys_User.password = reader["password"].ToString();
            }
            reader.Close();
            return sys_User;
        }
    }

}
