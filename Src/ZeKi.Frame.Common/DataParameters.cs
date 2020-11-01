using System.Collections.Generic;
using System.Data;
using System.Linq;
using Dapper;

namespace ZeKi.Frame.Common
{
    /// <summary>
    /// 存储过程或拼写参数使用
    /// </summary>
    public class DataParameters
    {
        private readonly Dictionary<string, ParamInfo> parameters = new Dictionary<string, ParamInfo>();

        /// <summary>
        /// 添加参数
        /// </summary>
        /// <param name="name"></param>
        /// <param name="value">可以是普通值,也可以是存储SCBuild.XXX方法返回值</param>
        /// <param name="dbType"></param>
        /// <param name="direction"></param>
        /// <param name="size"></param>
        /// <param name="precision"></param>
        /// <param name="scale"></param>
        public void Add(string name, object value, DbType? dbType = null, int? size = null, ParameterDirection? direction = null, byte? precision = null, byte? scale = null)
        {
            parameters[name] = new ParamInfo
            {
                Name = name,
                Value = value,
                ParameterDirection = direction ?? ParameterDirection.Input,
                DbType = dbType,
                Size = size,
                Precision = precision,
                Scale = scale
            };
        }

        public IEnumerable<string> ParameterNames => parameters.Select(p => p.Key);

        public ParamInfo Get(string name)
        {
            return parameters[name];
        }

        public void Clear()
        {
            parameters.Clear();
        }

        public Dictionary<string, ParamInfo> GetParameters()
        {
            return parameters;
        }


        public sealed class ParamInfo
        {
            public string Name { get; set; }
            /// <summary>
            /// 可以是普通值,也可以是存储SCBuild.XXX方法返回值
            /// </summary>
            public object Value { get; set; }
            public ParameterDirection ParameterDirection { get; set; }
            public DbType? DbType { get; set; }
            public int? Size { get; set; }
            public byte? Precision { get; set; }
            public byte? Scale { get; set; }
        }
    }

    /// <summary>
    /// 参数化参数以及sql条件
    /// </summary>
    public class DynamicParametersWithSql
    {
        public string SqlWhere { get; set; } = string.Empty;
        public DynamicParameters DynamicParameters { get; set; } = new DynamicParameters();
    }
}
