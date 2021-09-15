基于Dapper为底层封装,以及面向接口的三层框架
========================================

查询
------------------------------------------------------------

示例:

```csharp
使用匿名类
var list_0 = DAL.QueryList<SysUserInfo>(new { uId = 5 });
使用指定的类进行复杂的操作拼装
var dataParms = new DataParameters();
dataParms.Add<SysUserInfo>(p => p.uLoginName, "zzq");
dataParms.Add<SysUserInfo>(p => p.uRemark, "良", ConditionOperator.Like);
var list_1 = DAL.QueryList<SysUserInfo>(dataParms);
```

存储过程
------------------------------------------------------------

示例:

```csharp
var dbParameters = new DataParameters();
dbParameters.Add("@channelId", 2, DbType.Int32);
dbParameters.Add("outstr", "未知错误", DbType.String, direction: ParameterDirection.Output);
DAL.ExecProcedure("sp_test", dbParameters);
var outstr = dbParameters.GetParamVal<string>("outstr");
```

添加
------------------------------------------------------------

示例:

```csharp
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
var res = SysUserInfoBLL.Insert(insertModel);
```

修改
------------------------------------------------------------

示例:

```csharp
var dataParam = new DataParameters();
dataParam.Add<SysUserInfo>(p => p.uId, 5);
dataParam.Add<SysUserInfo>(p => p.uLoginName, "zzq");
dataParam.AddUpdate<SysUserInfo>(p => p.uEmail, "219114@qq.com");
var res = SysUserInfoDAL.UpdatePart<SysUserInfo>(dataParam);
```

其它
------------------------------------------------------------

另外还有批量新增,标注特性使用事务等,此处就不再演示

生成Model、BLL、DAL可以使用Tool中的模板（使用CodeSimith）

使用的框架
---------------------
.Net 5.0,MiniProfiler(监控Sql)、AspectCore(实现AOP)、AutoFac、NLog、Dapper

使用的中间件
---------------------
HttpClient 过滤器 请求中间件
