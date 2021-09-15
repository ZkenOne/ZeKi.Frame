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
            var user_list = new List<SysUserInfo>
            {
                new SysUserInfo() { uId = 11, uAddTime = DateTime.Now, uDepId = 1, uLoginName = "3", uPwd = "1" },
                new SysUserInfo() { uId = 22, uAddTime = DateTime.Now, uDepId = 1, uLoginName = "1", uPwd = "2" },
                new SysUserInfo() { uId = 33, uAddTime = DateTime.Now, uDepId = 1, uLoginName = "2", uPwd = "2" }
            };
            DAL.BulkCopyToInsert(user_list);
            #endregion

            #region Update
            var updateModel = new Model.SysUserInfo()
            {
                uAddTime = DateTime.Now.AddYears(2),
                uDepId = 21,
                uEmail = "12@qq.com",
                uGender = false,
                uId = 102,
                uIsDel = true,
                uLoginName = "zzq23",
                uPwd = "123456",
                uRemark = "nihao"
            };
            var res3 = SysUserInfoBLL.Update(updateModel);
            Console.WriteLine(res3);

            var dataParams = new DataParameters();
            dataParams.Add<SysUserInfo>(p => p.uId, 3);
            dataParams.Add<SysUserInfo>(p => p.uLoginName, "cwjl");
            dataParams.Add<SysUserInfo>(p => p.uEmail, "902@qq.com");
            dataParams.AddUpdate<SysUserInfo>(p => p.uEmail, "1219114@qq.com");
            var r1 = SysUserInfoDAL.UpdatePart<SysUserInfo>(dataParams);
            Console.WriteLine(r1);
            #endregion

            #region Delete
            var isOk1 = SysUserInfoBLL.Delete(new Model.SysUserInfo() { uId = 4 });
            Console.WriteLine(isOk1);
            #endregion

            #region Statistics
            var res7 = SysUserInfoDAL.Count<SysUserInfo>(new { udepid = 2 });
            Console.WriteLine(res7);
            #endregion

            #region Query
            //匿名类查询
            var list_0 = DAL.QueryList<SysUserInfo>(new { uId = 2 });
            //dataParamseters多功能的使用
            dataParams.Clear();
            dataParams.Add<SysUserInfo>(p => p.uLoginName, "zzq");
            dataParams.Add<SysUserInfo>(p => p.uRemark, "良", ConditionOperator.Like);
            var list_1 = DAL.QueryList<SysUserInfo>(dataParams);
            dataParams.Clear();
            dataParams.Add<SysUserInfo>(p => p.uLoginName, "zzq");
            dataParams.Add<SysUserInfo>(p => p.uLoginName, "zzq", ConditionOperator.NotEqual); //可以多个同字段条件
            dataParams.Add<SysUserInfo>(p => p.uEmail, "123");
            dataParams.Add<SysUserInfo>(p => p.uRemark, "良", ConditionOperator.Like);
            dataParams.AddBetween<SysUserInfo>(p => p.uId, 1, 2000);
            var list_2 = DAL.QueryList<SysUserInfo>(dataParams, selectFields: "uloginname");
            var dict = new Dictionary<string, object>
            {
                { "uloginname", "1zzq" },
                { "uEmail", "126@qq.com" },
                { "uRemark", "nv100" }
            };
            var list_1_1 = DAL.QueryList<SysUserInfo>(dict, selectFields: "uloginname,uRemark");
            //连表使用
            var mainSql = "select u.*,dep.depName from sysUserInfo as u join sysDepartment as dep on dep.depId=u.uDepId";
            dataParams.Clear();
            dataParams.Add<SysUserInfo>(p => p.uId, 2, tablePrefix: "u");
            dataParams.Add<SysUserInfo>(p => p.uRemark, "hi", ConditionOperator.Like, tablePrefix: "u");
            dataParams.Add<SysDepartment>(p => p.depId, 6, tablePrefix: "dep");
            var list_2_1 = DAL.QueryJoinList<UserInfoWithDep>(mainSql, dataParams, "u.uId asc");

            //分页
            dataParams.Clear();
            dataParams.Add<SysUserInfo>(p => p.uId, 5, ConditionOperator.GreaterThan, tablePrefix: "u");
            //dataParams.Add<SysUserInfo>(p => p.uRemark, "hi", ConditionOperator.Like, tablePrefix: "u");
            //dataParams.Add<SysDepartment>(p => p.depId, 2, tablePrefix: "dep");
            var pcp_n = new PageParameters()
            {
                PageIndex = 1,
                PageSize = 10,
                Select = "u.*,dep.depName",
                Order = "u.uId asc",
                Table = @"sysUserInfo as u join sysDepartment as dep on dep.depId=u.uDepId",
                WhereParam = dataParams
            };
            var pageData = DAL.PageList<UserInfoWithDep>(pcp_n);
            #endregion

            #region 存储过程
            dataParams.Clear();
            dataParams.Add("@channelId", 2, DbType.Int32);
            dataParams.Add("outstr", "未知错误", DbType.String, direction: ParameterDirection.Output);
            DAL.ExecProcedure("sp_test", dataParams);
            var outstr = dataParams.GetParamVal<string>("outstr");
            Console.WriteLine(outstr);
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
