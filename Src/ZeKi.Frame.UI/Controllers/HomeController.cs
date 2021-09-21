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
        private readonly ISysUserInfoBLL _sysUserInfoBLL;
        private readonly  ISysRoleBLL _sysRoleBLL;
        private readonly  ISysPermissionBLL _sysPermissionBLL;
        private readonly  ILogger<HomeController> _logger;
        private readonly ICurrencyClient _client;

        public HomeController(ISysUserInfoBLL sysUserInfoBLL, ISysRoleBLL sysRoleBLL, ISysPermissionBLL sysPermissionBLL,
                              ILogger<HomeController> logger, ICurrencyClient client)
        {
            _sysUserInfoBLL = sysUserInfoBLL;
            _sysRoleBLL = sysRoleBLL;
            _sysPermissionBLL = sysPermissionBLL;
            _logger = logger;
            _client = client;
        }

        public IActionResult TestTran()
        {
            _sysPermissionBLL.TestTran();
            return Ok();
        }

        public IActionResult TestCache(string id_user)
        {
            var res = _sysPermissionBLL.TestCache(id_user);
            return Ok(res);
        }

        [HttpGet]
        public ActionResult<string> Index()
        {
            //_logger.LogTrace("LogTrace");
            //_logger.LogDebug("LogDebug");
            //_logger.LogWarning("LogWarning");
            //_logger.LogInformation("LogInformation");
            //_logger.LogError(new ArgumentException("LogError"), "发生错误");
            //_logger.LogCritical("LogCritical");

            _sysPermissionBLL.Example();

            return Ok("ok");
        }

        [HttpGet]
        public ActionResult<string> Index2()
        {
            var t1 = _sysUserInfoBLL.GetUserNameById(1);
            var t2 = _sysUserInfoBLL.GetTName();
            var t3 = _sysUserInfoBLL.Insert(new Model.SysUserInfo() { uId = 1, uDepId = 2, uLoginName = "f", uPwd = "1234", uAddTime = DateTime.Now });
            var t4 = _sysUserInfoBLL.Update(new Model.SysUserInfo() { uId = 22, uEmail = "2222" });
            var t5 = _sysRoleBLL.QueryModel<Model.SysRole>(new { rId = 1 });
            return Ok(new { t1, t2, t3, t4, t5 });
        }

        [HttpGet]
        public ActionResult<string> Call()
        {
            var t1 = _client.GetAsync("http://www.baidu.com?id=2").Result;
            var t2 = _client.PostAsync("http://www.4399.com").Result;
            var t3 = _sysUserInfoBLL.GetTName();
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
