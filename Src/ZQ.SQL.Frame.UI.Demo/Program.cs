using Dapper.Extensions.ZQ;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dapper;
using System.Data;
using ZQ.SQL.Frame.Common;

namespace ZQ.SQL.Frame.UI.Demo
{
    class Program
    {
        static void Main(string[] args)
        {
            Test();
            Console.Read();
        }

        private static void Test()
        {
            var userBLL = new BLL.SysUserInfoBLL();
            var roleBLL = new BLL.SysRoleBLL();
            var permissionBLL = new BLL.SysPermissionBLL();

            #region Insert
            var insertModel = new Model.SysUserInfo()
            {
                uAddTime = DateTime.Now,
                uDepId = 2,
                uEmail = "12@qq.com",
                uGender = false,
                uId = 10,
                uIsDel = true,
                uLoginName = "zzq",
                uPwd = "jsdjfiu",
                uRemark = "ceshi1"
            };
            var res1 = userBLL.Insert(insertModel, true);
            insertModel.uGender = true;
            insertModel.uEmail = "222@@@@qq.com";
            insertModel.uLoginName = "lisi";
            var res2 = userBLL.Insert(insertModel, false);
            Console.WriteLine(res1);
            Console.WriteLine(res2);
            #endregion

            #region Update
            var updateModel = new Model.SysUserInfo()
            {
                uAddTime = DateTime.Now.AddYears(2),
                uDepId = 21,
                uEmail = "12@qq.com",
                uGender = false,
                uId = 205,
                uIsDel = true,
                uLoginName = "zzq23",
                uPwd = "123456",
                uRemark = "nihao"
            };
            var res3 = userBLL.Update(updateModel);
            Console.WriteLine(res3);
            #endregion

            #region Delete
            var isOk1 = userBLL.Delete(new Model.SysUserInfo() { uId = 198 });
            Console.WriteLine(isOk1);
            #endregion

            #region Statistics
            var res4 = userBLL.Count("");
            var res5 = userBLL.Count("udepid=@udepid", new { udepid = 2 });
            var res6 = userBLL.Sum<int>("udepid", "udepid=@udepid", new { udepid = 2 });
            Console.WriteLine(res4);
            Console.WriteLine(res5);
            Console.WriteLine(res6);
            #endregion

            #region Query

            #region 多条件查询
            var sql123 = "select * from dbo.sysUserInfo ";
            SqlBuild bulid = new SqlBuild();
            bulid.BuildParm("uGender", "1");
            bulid.BuildParm("uId", "50", SqlOpt.LessEqual);
            sql123 += bulid.ToString();
            var ulist = userBLL.QueryList(sql123, bulid.DyParm);
            #endregion

            var model1 = userBLL.QueryModel("where uemail=@uemail", new { uemail = "1269021626@qq.com" });
            var list1 = userBLL.QueryList<Model.SysUserInfo>("select uId,uLoginName from SysUserInfo where udepid=@udepid", new { udepid = 2 });

            //使用QueryMultiple--能自动释放
            var sql1 = "select * from SysUserInfo where uloginname=@uloginname;select * from SysRole_1;";
            userBLL.DbAction((conn) =>
            {
                var multModel = conn.QueryMultiple(sql1, new { uloginname = "zzq" });
                var userList = multModel.Read<Model.SysUserInfo>();
                var roleList = multModel.Read<Model.SysRole>();
            });

            List<SqlParameter> pars = new List<SqlParameter>
            {
                new SqlParameter("@uloginname", "zzq")
            };
            var dataSet = userBLL.ExecDataSet(sql1, pars);
            var table = userBLL.ExecDataTable("select * from SysUserInfo where uloginname=@uloginname", pars.ToList());

            var pcp = new PageParameters()
            {
                PageIndex = 1,
                PageSize = 10,
                ShowField = "*",
                KeyFiled = "ro.rId DESC",
                Sql = @"ro.*,dep.depName from sysRole_1 as ro join sysDepartment as dep on dep.depId=ro.rdepId",
                Where = "where rName<>'liis'"
            };
            var dt1 = userBLL.PageList(pcp);
            pcp = new PageParameters()
            {
                PageIndex = 1,
                PageSize = 10,
                ShowField = "*",
                KeyFiled = "rId DESC",     // 不传 Order 则用KeyFiled
                TableName = "sysRole_1",   //与映射的表名一样可以不赋值
                Where = "where rName=@rName"
            };
            var pData = roleBLL.PageList<Model.SysRole>(pcp, new { rName = "项目经理" });
            #endregion

            #region Dapper原生
            userBLL.DbAction((conn) =>
            {
                var s1 = conn.Execute("delete from sysUserInfo where uid=89");
                var sql2 = "select count(0) from sysUserInfo where uloginname=@uloginname";
                var s2 = conn.ExecuteScalar<int>(sql2, new { uloginname = "zzq" });
                Console.WriteLine(s1);
                Console.WriteLine(s2);
            });
            #endregion

            #region 事务
            userBLL.ExecTransaction((conn, tran) =>
            {
                var insertModel2 = new Model.SysUserInfo()
                {
                    uAddTime = DateTime.Now,
                    uDepId = 2,
                    uEmail = "1222@qq.com",
                    uGender = false,
                    uId = 10,
                    uIsDel = true,
                    uLoginName = "zz1q",
                    uPwd = "jsdjf1iu",
                    uRemark = "ces2i1"
                };
                //需要加tran
                var res12 = conn.Insert(insertModel2, true, tran);
                //var i = 0;
                //insertModel2.uDepId = 1 / i;  //模拟出错
                var res22 = conn.Insert(insertModel2, true, tran);
                var res33 = conn.Delete(new Model.SysUserInfo() { uId = (int)res12 }, tran);
                Console.WriteLine(res12);
                Console.WriteLine(res22);
                Console.WriteLine(res33);
            });
            #endregion

            #region 表别名
            var s = roleBLL.QueryModel<Model.SysRole>("where 1=1");
            Console.WriteLine(s.rName);
            #endregion
        }
    }
}
