using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;

namespace db.bll
{
    public class studeyTest
    {
        public static void va基本使用(db.dbEntities dc)
        {
            efHelper ef = new efHelper();

            //查询多条
            {
                string sql = " select * from sbs_dept where orgCode=@orgCode";
                DataTable table = ef.ExecuteDataTable(sql, new { orgCode = "sju" });
            }
            //查询单条
            {
                string sql = " select * from sbs_dept where deptCode=@deptCode";
                DataRow row = ef.ExecuteDataRow(sql, new { deptCode = "B001" });
            }

            //新增
            {
                db.sbs_Org entry = new db.sbs_Org();
                entry.rowID = "b";
                entry.orgCode = "123";
                entry.orgName = "123";
                dc.sbs_Org.Add(entry);
            }
            //新增
            //{
            //    string sql = "INSERT INTO sbs_dept(deptCode, deptName, orgCode) VALUES (@deptCode, @deptName, @orgCode)";
            //    DataRow row = ef.ExecuteDataRow(sql, new { deptCode = "D001", deptName = "宇宙部", orgCode ="qinghua"});                
            //}

            //修改：从模型集合中查询出来，修改属性，保存
            {
                db.sbs_Org entry = dc.sbs_Org.Single(a => a.orgCode == "sju");
                entry.orgName = "三江学院xxxx";
            }
            //修改
            //{
            //    string sql = "UPDATE sbs_dept SET deptName=@deptName WHERE deptCode=@deptCode";
            //    DataRow row = ef.ExecuteDataRow(sql, new { deptCode="D001" ,deptName = "银河部" });               
            //}
        }

        public static void va事务使用(db.dbEntities dc)
        {
            efHelper ef = new efHelper(ref dc);
            try
            {
                ef.beginTran();

                //修改：从模型集合中查询出来，修改属性，保存
                {
                    string sql = "update sbs_Org get orgName=@orgName where orgCode=@orgCode";
                    ef.Execute(sql, new { orgName = "三江学院abc", orgCode = "sju" });
                }
                //查询
                {
                    string sql = " select * from sbs_dept where orgCode=@orgCode";
                    DataTable table = ef.ExecuteDataTable(sql, new { orgCode = "sju" });
                }
                //调用子方法
                va事务练习sub(dc);
                ef.commit();
            }
            catch (Exception ex)
            {
                rui.logHelper.log("出错",ex);
                ef.rollBack();
            }           
        }
        public static void va事务练习sub(db.dbEntities dc)
        {
            efHelper ef = new efHelper(ref dc);
            //新增
            {
                db.sbs_Org entry = new db.sbs_Org();
                entry.rowID = "b";
                entry.orgCode = "123";
                entry.orgName = "123";
                dc.sbs_Org.Add(entry);
                dc.SaveChanges();
            }
            
        }
    }
}
