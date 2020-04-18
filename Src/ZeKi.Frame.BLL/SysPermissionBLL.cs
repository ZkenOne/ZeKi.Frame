using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dapper;
using ZeKi.Frame.Common;
using ZeKi.Frame.DB;
using ZeKi.Frame.IBLL;
using ZeKi.Frame.IDAL;
using ZeKi.Frame.Model;

namespace ZeKi.Frame.BLL
{
    public class SysPermissionBLL : BaseBLL<SysPermission>, ISysPermissionBLL
    {
        public ISysUserInfoBLL SysUserInfoBLL { get; set; }
        public ISysRoleBLL SysRoleBLL { get; set; }
        public ISysUserInfoDAL SysUserInfoDAL { get; set; }
        public ISysRoleDAL SysRoleDAL { get; set; }

        /// <summary>
        /// 展示调用
        /// </summary>
        public void Example()
        {
            #region Insert
            var insertModel = new Model.SysUserInfo()
            {
                uAddTime = DateTime.Now,
                uDepId = 2,
                uEmail = "12@qq.com",
                uGender = false,
                uId = 5,
                uIsDel = true,
                uLoginName = "zzq",
                uPwd = "jsdjfiu",
                uRemark = "ceshi1"
            };
            var res1 = SysUserInfoBLL.Insert(insertModel, true);
            insertModel.uGender = true;
            insertModel.uEmail = "222@@@@qq.com";
            insertModel.uLoginName = "lisi";
            var res2 = SysUserInfoBLL.Insert(insertModel, false);
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
            var res3 = SysUserInfoBLL.Update(updateModel);
            Console.WriteLine(res3);

            var r1 = SysUserInfoBLL.Update(new { uId = 97, renew_uRemark = "upa" });
            Console.WriteLine(r1);
            #endregion

            #region Delete
            var isOk1 = SysUserInfoBLL.Delete(new Model.SysUserInfo() { uId = 198 });
            Console.WriteLine(isOk1);
            #endregion

            #region Statistics
            var res4 = SysUserInfoDAL.Count("");
            var res5 = SysUserInfoDAL.Count("udepid=@udepid", new { udepid = 2 });
            var res6 = SysUserInfoDAL.Sum<int>("udepid", "udepid=@udepid", new { udepid = 2 });
            var res7 = SysUserInfoDAL.Count(new { udepid = 2 });
            Console.WriteLine(res4);
            Console.WriteLine(res5);
            Console.WriteLine(res6);
            Console.WriteLine(res7);
            #endregion

            #region Query

            var model1 = SysUserInfoDAL.QueryModel("where uemail=@uemail", new { uemail = "1269021626@qq.com" });
            var list1 = SysUserInfoDAL.QueryList<Model.SysUserInfo>("select uId,uLoginName from SysUserInfo where udepid=@udepid", new { udepid = 2 });

            //使用QueryMultiple--能自动释放
            var sql1 = "select * from SysUserInfo where uloginname=@uloginname;select * from SysRole_1;";
            SysUserInfoDAL.DbAction((conn) =>
            {
                var multModel = conn.QueryMultiple(sql1, new { uloginname = "zzq" });
                var userList = multModel.Read<Model.SysUserInfo>();
                var roleList = multModel.Read<Model.SysRole>();
            });

            //List<SqlParameter> pars = new List<SqlParameter>
            //{
            //    new SqlParameter("@uloginname", "zzq")
            //};
            //var dataSet = SysUserInfoDAL.ExecDataSet(sql1, pars);
            //var table = SysUserInfoDAL.ExecDataTable("select * from SysUserInfo where uloginname=@uloginname", pars.ToList());

            var pcp = new PageParameters()
            {
                PageIndex = 1,
                PageSize = 10,
                ShowField = "*",
                KeyFiled = "ro.rId DESC",
                Sql = @"ro.*,dep.depName from sysRole_1 as ro join sysDepartment as dep on dep.depId=ro.rdepId",
                Where = "where rName<>'liis'"
            };
            var dt1 = SysUserInfoDAL.PageList(pcp);
            pcp = new PageParameters()
            {
                PageIndex = 1,
                PageSize = 10,
                ShowField = "*",
                KeyFiled = "rId DESC",     // 不传 Order 则用KeyFiled
                TableName = "sysRole_1",   //与映射的表名一样可以不赋值
                Where = "where rName=@rName"
            };
            var pData = SysRoleDAL.PageList<Model.SysRole>(pcp, new { rName = "项目经理" });

            //不写sql方式
            var dictParm = new Dictionary<string, object>()
            {
                { "uloginname" , "zzq" },
                { "uRemark" , SCBuild.Like("良") },
                { "uId" , SCBuild.NotIn(new string[]{"78","333"}) },
                { "__" , SCBuild.Text("(uEmail=@uEmail1 or uEmail=@uEmail2)",new {uEmail1="12@qq.com",uEmail2="902@qq.com"}) },
                { "uDepId" , SCBuild.Combin(SCBuild.Equal(3),SCBuild.GtEqual(100)) },
                { "uAddTime" , SCBuild.Between(DateTime.Now.AddDays(-30),DateTime.Now) }
            };
            var list11 = SysUserInfoDAL.QueryList(dictParm);

            dictParm = new Dictionary<string, object>()
            {
                { "u.uloginname" , "zzq" },
                { "u.uRemark" , SCBuild.Like("良") },
                { "u.uId" , SCBuild.NotIn(new string[]{"78","333"}) },
                { "d.depName" , "公司" }
            };
            var sql_111 = "select u.*,d.depName from sysUserInfo as u left join sysDepartment as d on u.uDepId=d.depId";
            var list12 = SysUserInfoDAL.QueryList<UserInfoWithDep>(sql_111, dictParm, "u.uAddTime DESC");

            //分页____
            var pcp_n = new PageParameters()
            {
                PageIndex = 1,
                PageSize = 20,
                ShowField = "*",
                KeyFiled = "u.uAddTime DESC",
                Sql = @"u.*,d.depName from sysUserInfo as u left join sysDepartment as d on u.uDepId=d.depId",
                Where = new Dictionary<string, object>()
                {
                    { "u.uloginname" , "zz1q" },
                    //{ "u.uRemark" , SCBuild.Like("良") },
                    { "u.uId" , SCBuild.NotIn(new string[] { "7599", "333"} ) },
                    //{ "d.depName" , "公司" }
                }
            };
            var pageList = SysUserInfoDAL.PageList<UserInfoWithDep>(pcp_n);

            #endregion

            #region Dapper原生
            SysUserInfoDAL.DbAction((conn) =>
            {
                var s1 = conn.Execute("delete from sysUserInfo where uid=89");
                var sql2 = "select count(0) from sysUserInfo where uloginname=@uloginname";
                var s2 = conn.ExecuteScalar<int>(sql2, new { uloginname = "zzq" });
                Console.WriteLine(s1);
                Console.WriteLine(s2);
            });
            #endregion

            #region 事务
            SysUserInfoDAL.ExecTransaction((conn, tran) =>
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
            var s = SysRoleBLL.QueryModel<Model.SysRole>(new { });
            //SysUserInfoDAL.QueryModel("ffff=11");  //模拟出错,MiniProfiler过滤器中会记录Errored=true
            Console.WriteLine(s.rName);
            #endregion
        }
    }
}
