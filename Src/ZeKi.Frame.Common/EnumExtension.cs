using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using Dapper;
using ZeKi.Frame.Model;
using System.Reflection;

namespace ZeKi.Frame.Common
{
    /// <summary>
    /// 
    /// </summary>
    public static class EnumExtension
    {
        /// <summary>
        /// 获取枚举标注特性
        /// </summary>
        /// <param name="em"></param>
        /// <returns></returns>
        public static T GetAttribute<T>(this Enum em) where T : Attribute
        {
            var type = em.GetType();
            var fd = type.GetField(em.ToString());
            if (fd == null)
                return null;
            var arritbute = fd.GetCustomAttribute<T>();
            return arritbute;
        }
    }
}
