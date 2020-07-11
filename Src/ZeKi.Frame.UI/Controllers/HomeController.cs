using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.DependencyInjection;
using ZeKi.Frame.Common;
using ZeKi.Frame.IBLL;
using System.Data.SqlClient;

namespace ZeKi.Frame.UI.Controllers
{
    [ApiController]
    [Route("api/[controller]/[action]")]
    public class HomeController : ControllerBase
    {
        public ISysUserInfoBLL SysUserInfoBLL { set; get; }
        public ISysRoleBLL SysRoleBLL { set; get; }
        public ISysPermissionBLL SysPermissionBLL { set; get; }
        public ILogger<HomeController> Logger { set; get; }
        public ICurrencyClient Client { set; get; }

        public IActionResult TestTran()
        {
            SysPermissionBLL.TestTran();
            return Ok();
        }

        [HttpGet]
        public ActionResult<string> Index()
        {
            //Logger.LogTrace("LogTrace");
            //Logger.LogDebug("LogDebug");
            //Logger.LogWarning("LogWarning");
            //Logger.LogInformation("LogInformation");
            //Logger.LogError(new ArgumentException("LogError"), "发生错误");
            //Logger.LogCritical("LogCritical");

            SysPermissionBLL.Example();
            return Ok();
        }

        [HttpGet]
        public ActionResult<string> Index2()
        {
            var t1 = SysUserInfoBLL.GetUserNameById(1);
            var t2 = SysUserInfoBLL.GetTName();
            var t3 = SysUserInfoBLL.Insert(new Model.SysUserInfo() { uId = 1, uDepId = 2, uLoginName = "f", uPwd = "1234", uAddTime = DateTime.Now });
            var t4 = SysUserInfoBLL.Update(new Model.SysUserInfo() { uId = 22, uEmail = "2222" });
            var t5 = SysRoleBLL.QueryModel(new { rId = 1 });
            return Ok(new { t1, t2, t3, t4, t5 });
        }

        [HttpGet]
        public ActionResult<string> Call()
        {
            var t1 = Client.GetAsync("http://www.baidu.com?id=2").Result;
            var t2 = Client.PostAsync("http://www.4399.com").Result;
            var t3 = SysUserInfoBLL.GetTName();
            return Ok(new { t1, t2, t3 });
        }

        [HttpGet]
        public ActionResult Error()
        {
            int.Parse("ff3");  //模拟错误
            return Ok();
        }
    }
}
