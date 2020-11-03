using System;
using System.Collections.Generic;
using System.Data;
using ZeKi.Frame.BLL.Interceptor;
using ZeKi.Frame.Common;
using ZeKi.Frame.IBLL;
using ZeKi.Frame.IDAL;
using ZeKi.Frame.Model;

namespace ZeKi.Frame.BLL
{
    public class SysPermissionBLL : BaseBLL, ISysPermissionBLL
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

            var r1 = SysUserInfoDAL.Update<SysUserInfo>(new { uId = 97, _new_uId = "up7a" });
            Console.WriteLine(r1);
            #endregion

            #region Delete
            var isOk1 = SysUserInfoBLL.Delete(new Model.SysUserInfo() { uId = 206 });
            Console.WriteLine(isOk1);
            #endregion

            //#region Statistics
            //var res4 = SysUserInfoDAL.Count("");
            //var res5 = SysUserInfoDAL.Count("udepid=@udepid", new { udepid = 2 });
            //var res6 = SysUserInfoDAL.Sum<int>("udepid", "udepid=@udepid", new { udepid = 2 });
            var res7 = SysUserInfoDAL.Count<SysUserInfo>(new { udepid = 2 });
            //Console.WriteLine(res4);
            //Console.WriteLine(res5);
            //Console.WriteLine(res6);
            Console.WriteLine(res7);
            //#endregion

            //#region Query

            var dataParms = new DataParameters();
            dataParms.Add("uloginname", "zzq");
            dataParms.Add("uRemark", SCBuild.Like("良"));
            dataParms.Add("uId", SCBuild.Like("良"));
            dataParms.Add("uRemark", SCBuild.NotIn(new string[] { "78", "333" }));
            var list_1 = DAL.QueryList<SysUserInfo>(dataParms, selectFields: "uloginname");
            dataParms.Clear();
            dataParms.Add("uloginname", "zzq");
            dataParms.Add("uEmail", "123", DbType.AnsiString);
            dataParms.Add("uRemark", SCBuild.Like("良"), DbType.String, 200);
            dataParms.Add("uId", SCBuild.Between(1, 2000), DbType.Int32);
            var list_2 = DAL.QueryList<SysUserInfo>(dataParms, selectFields: "uloginname");
            dataParms.Clear();
            dataParms.Add("uloginname", "zzq");
            dataParms.Add("uEmail", SCBuild.In(new List<string> { "7@8", "33@3" }), DbType.AnsiString, 40);
            dataParms.Add("uId", SCBuild.Like("良"));
            var list_3 = DAL.QueryList<SysUserInfo>(dataParms, selectFields: "uloginname");
            var dict = new Dictionary<string, object>();
            dict.Add("uloginname", "1zzq");
            dict.Add("uEmail", SCBuild.In(new List<string> { "7@8", "33@3" }));
            dict.Add("uRemark", "nv100"); //有设置uRemark属性指定数据库类型和size
            var list_1_1 = DAL.QueryList<SysUserInfo>(dict, selectFields: "uRemark");


            var user_list = new List<SysUserInfo>();
            user_list.Add(new SysUserInfo() { uId = 11, uAddTime = DateTime.Now, uDepId = 1, uLoginName = "3", uPwd = "1" });
            user_list.Add(new SysUserInfo() { uId = 22, uAddTime = DateTime.Now, uDepId = 1, uLoginName = "1", uPwd = "2" });
            user_list.Add(new SysUserInfo() { uId = 33, uAddTime = DateTime.Now, uDepId = 1, uLoginName = "2", uPwd = "2" });
            DAL.BulkCopyToInsert(user_list);
            //var model1 = SysUserInfoDAL.QueryModel("where uemail=@uemail", new { uemail = "1269021626@qq.com" });
            //var list1 = SysUserInfoDAL.QueryList<Model.SysUserInfo>("select uId,uLoginName from SysUserInfo where udepid=@udepid", new { udepid = 2 });

            ////使用QueryMultiple--能自动释放
            //var sql1 = "select * from SysUserInfo where uloginname=@uloginname;select * from SysRole_1;";
            //SysUserInfoDAL.DbAction((conn) =>
            //{
            //    var multModel = conn.QueryMultiple(sql1, new { uloginname = "zzq" });
            //    var userList = multModel.Read<Model.SysUserInfo>();
            //    var roleList = multModel.Read<Model.SysRole>();
            //});

            ////List<SqlParameter> pars = new List<SqlParameter>
            ////{
            ////    new SqlParameter("@uloginname", "zzq")
            ////};
            ////var dataSet = SysUserInfoDAL.ExecDataSet(sql1, pars);
            ////var table = SysUserInfoDAL.ExecDataTable("select * from SysUserInfo where uloginname=@uloginname", pars.ToList());

            //var pcp = new PageParameters()
            //{
            //    PageIndex = 1,
            //    PageSize = 10,
            //    ShowField = "*",
            //    KeyFiled = "ro.rId DESC",
            //    Sql = @"ro.*,dep.depName from sysRole_1 as ro join sysDepartment as dep on dep.depId=ro.rdepId",
            //    Where = "where rName<>'liis'"
            //};
            //var dt1 = SysUserInfoDAL.PageList(pcp);
            //pcp = new PageParameters()
            //{
            //    PageIndex = 1,
            //    PageSize = 10,
            //    ShowField = "*",
            //    KeyFiled = "rId DESC",     // 不传 Order 则用KeyFiled
            //    TableName = "sysRole_1",   //与映射的表名一样可以不赋值
            //    Where = "where rName=@rName"
            //};
            //var pData = SysRoleDAL.PageList<Model.SysRole>(pcp, new { rName = "项目经理" });

            ////不写sql方式
            //var dictParm = new Dictionary<string, object>()
            //{
            //    { "uloginname" , "zzq" },
            //    { "uRemark" , SCBuild.Like("良") },
            //    { "uId" , SCBuild.NotIn(new string[]{"78","333"}) },
            //    { "__" , SCBuild.Text("(uEmail=@uEmail1 or uEmail=@uEmail2)",new {uEmail1="12@qq.com",uEmail2="902@qq.com"}) },
            //    { "uAddTime" , SCBuild.Between(DateTime.Now.AddDays(-30),DateTime.Now) }
            //};
            //var list11 = SysUserInfoDAL.QueryList(dictParm);

            //dictParm = new Dictionary<string, object>()
            //{
            //    { "u.uloginname" , "zzq" },
            //    { "u.uRemark" , SCBuild.Like("良") },
            //    { "u.uId" , SCBuild.NotIn(new string[]{"78","333"}) },
            //    { "d.depName" , "公司" }
            //};
            //var sql_111 = "select u.*,d.depName from sysUserInfo as u left join sysDepartment as d on u.uDepId=d.depId";
            //var list12 = SysUserInfoDAL.QueryList<UserInfoWithDep>(sql_111, dictParm, "u.uAddTime DESC");

            ////分页____
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

            var whereDataParam = new DataParameters();
            whereDataParam.Add("u.uloginname", "zz1q");
            whereDataParam.Add("u.uEmail", SCBuild.In(new List<string> { "7@8", "33@3" }), DbType.AnsiString, 40);
            whereDataParam.Add("u.uId", SCBuild.Like("良"));
            var pcp_n_1 = new PageParameters()
            {
                PageIndex = 1,
                PageSize = 20,
                ShowField = "*",
                KeyFiled = "u.uAddTime DESC",
                Sql = @"u.*,d.depName from sysUserInfo as u left join sysDepartment as d on u.uDepId=d.depId",
                Where = whereDataParam
            };
            var pageList_1 = SysUserInfoDAL.PageList<UserInfoWithDep>(pcp_n_1);

            //#endregion

            //#region Dapper原生
            //SysUserInfoDAL.DbAction((conn) =>
            //{
            //    var s1 = conn.Execute("delete from sysUserInfo where uid=89");
            //    var sql2 = "select count(0) from sysUserInfo where uloginname=@uloginname";
            //    var s2 = conn.ExecuteScalar<int>(sql2, new { uloginname = "zzq" });
            //    Console.WriteLine(s1);
            //    Console.WriteLine(s2);
            //});
            //#endregion

            #region 存储过程
            //var dbParameters = new DataParameters();
            ////@sl_digit|@dj_digit|@je_digit|@barcode_isrepeat
            //dbParameters.Add("str", $"{digit["sl_digit"]}|{digit["dj_digit"]}|{digit["je_digit"]}|{digit["barcode_isrepeat"]}");
            //dbParameters.Add("id_bill", sp_tz_1.id);
            //dbParameters.Add("id_user", receiveModel.id_user);
            //dbParameters.Add("errorid", "-1", System.Data.DbType.String, direction: System.Data.ParameterDirection.Output);
            //dbParameters.Add("errormessage", "未知错误", System.Data.DbType.String, direction: System.Data.ParameterDirection.Output);
            //DAL.ExecProcedure("p_sp_tz_sh", dbParameters);
            //var errorid = dbParameters.GetParamVal<string>("errorid");
            //var errormessage = dbParameters.GetParamVal<string>("errormessage");
            #endregion

            #region 事务
            DAL.ExecTransaction(() =>
            {
                var insertModel2 = new Model.SysUserInfo()
                {
                    uAddTime = DateTime.Now,
                    uDepId = 2,
                    uEmail = "122276@qq.com",
                    uGender = false,
                    uIsDel = true,
                    uLoginName = "zz1q",
                    uPwd = "jsdjf1iu",
                    uRemark = "ces12i1fffdx21"
                };
                //需要加tran
                var res12 = SysUserInfoDAL.Insert(insertModel2, true);
                var res22 = SysUserInfoDAL.Insert(insertModel2, true);
                var res33 = SysUserInfoDAL.Delete(new Model.SysUserInfo() { uId = (int)res12 });

                var roleModel = new Model.SysRole()
                {
                    rDepId = 1,
                    rName = "444",
                    rAddTime = DateTime.Now
                };
                var res44 = SysRoleDAL.Insert(roleModel, true);
                //var res45 = SysRoleDAL.Delete(new Model.SysRole() { rId = res44 });
                Console.WriteLine(res12);
                Console.WriteLine(res22);
                Console.WriteLine(res33);

                //var i = 0;
                //insertModel2.uDepId = 1 / i;  //模拟出错
            });
            #endregion

            #region 事务后在添加
            insertModel = new Model.SysUserInfo()
            {
                uAddTime = DateTime.Now,
                uDepId = 2,
                uEmail = "12ui@qq.com",
                uGender = false,
                uIsDel = true,
                uLoginName = "zzq",
                uPwd = "jsdjfiu",
                uRemark = "ceshi1"
            };
            var res21 = SysUserInfoDAL.Insert(insertModel, true);
            #endregion

            #region 表别名
            var s = SysRoleBLL.QueryModel<Model.SysRole>(new { });
            //SysUserInfoDAL.QueryModel("ffff=11");  //模拟出错,MiniProfiler过滤器中会记录Errored=true
            Console.WriteLine(s.rName);
            #endregion
        }

        [Transaction(IsolationLevel.ReadUncommitted)]
        public void TestTran()
        {
            var insertModel = new Model.SysUserInfo()
            {
                uAddTime = DateTime.Now,
                uDepId = 222,
                uEmail = "@@#122276@qq6.com",
                uGender = false,
                uIsDel = true,
                uLoginName = "z1z16q",
                uPwd = "jsdjf1iu",
                uRemark = "cces12i1ff6fdx21"
            };
            //需要加tran
            var res12 = DAL.Insert(insertModel, true);
            var res22 = SysUserInfoDAL.Insert(insertModel, false);
            var res33 = SysUserInfoDAL.Delete(new Model.SysUserInfo() { uId = (int)res12 });

            var roleModel = new Model.SysRole()
            {
                rDepId = 1229,
                rName = "421d44",
                rAddTime = DateTime.Now
            };
            var res44 = SysRoleDAL.Insert(roleModel, true);
            var res45 = SysRoleDAL.Delete(new Model.SysRole() { rId = res44 });
            Console.WriteLine(res12);
            Console.WriteLine(res22);
            Console.WriteLine(res33);
            Console.WriteLine(res44);
            Console.WriteLine(res45);

            var i = 0;
            insertModel.uDepId = 1 / i;  //模拟出错

        }

        [Cache(AbsoluteExpiration = 20)]
        public string TestCache(string id_user)
        {
            return $"{DateTime.Now}__{id_user}";
        }
    }
}
