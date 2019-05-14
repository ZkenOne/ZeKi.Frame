using Dapper.Extensions.ZQ;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZQ.SQL.Frame.Model
{
    [Table("sysPermission")]
    public class SysPermission
    {
        public int pId { get; set; }

        public int pParent { get; set; }

        public string pName { get; set; }

        public string pAreaName { get; set; }

        public string pControllerName { get; set; }

        public string pActionName { get; set; }

        public string pFormMethod { get; set; }

        public string pFunction { get; set; }

        public string pFunName { get; set; }

        public string pPicName { get; set; }

        public int pOrder { get; set; }

        public bool pIsShow { get; set; }

        public string pRemark { get; set; }

        public bool pIsDel { get; set; }

        public DateTime pAddTime { get; set; }
    }
}
