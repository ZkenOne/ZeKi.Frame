using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZeKi.Frame.IDAL;
using ZeKi.Frame.Model;

namespace ZeKi.Frame.DAL
{
    public class SysRoleDAL : BaseDAL, ISysRoleDAL
    {
        //在其中清空缓存(DAL层),外部调用使用SysRoleDAL.XXX
        //private readonly RedisHelper redisHelper = new RedisHelper();
        //public override int Insert<TModel>(TModel model, bool getId = false)
        //{
        //    var entity = model as SysRole;
        //    var res = base.Insert(entity, getId);
        //    if (res > 0)
        //    {
        //        redisHelper.HashDelete(CacheKeys.hash_tb_spfl, entity.id_masteruser);
        //    }
        //    return res;
        //}

        //public override int BatchInsert<TModel>(IEnumerable<TModel> TList, int ps = 500)
        //{
        //    var list = TList as IEnumerable<Tb_Spfl>;
        //    var res = base.BatchInsert(list, ps);
        //    if (res > 0)
        //    {
        //        foreach (var item in list.GroupBy(p => p.id_masteruser))
        //        {
        //            redisHelper.HashDelete(CacheKeys.hash_tb_spfl, item.Key);
        //        }
        //    }
        //    return res;
        //}

        //public override void BulkCopyToInsert<TModel>(IEnumerable<TModel> entitysToInsert, SqlBulkCopyOptions copyOptions = SqlBulkCopyOptions.Default, int timeOut = 600)
        //{
        //    var list = entitysToInsert as IEnumerable<Tb_Spfl>;
        //    base.BulkCopyToInsert(list, copyOptions, timeOut);
        //    foreach (var item in list.GroupBy(p => p.id_masteruser))
        //    {
        //        redisHelper.HashDelete(CacheKeys.hash_tb_spfl, item.Key);
        //    }
        //}

        //public override bool Update<TModel>(TModel model)
        //{
        //    var entity = model as Tb_Spfl;
        //    var res = base.Update(entity);
        //    if (res)
        //    {
        //        var id_masteruser = entity.id_masteruser;
        //        if (id_masteruser.IsEmpty())
        //            id_masteruser = QueryModel<Tb_Spfl>(new { entity.id }, selectFields: "id_masteruser")?.id_masteruser;
        //        redisHelper.HashDelete(CacheKeys.hash_tb_spfl, id_masteruser);
        //    }
        //    return res;
        //}

        //public override int Update<TModel>(object setAndWhere)
        //{
        //    var res = base.Update<TModel>(setAndWhere);
        //    if (res > 0)
        //    {
        //        var dict = ConvertUtil.ParamsToDictionary(setAndWhere);
        //        if (dict.ContainsKey("id_masteruser"))
        //        {
        //            redisHelper.HashDelete(CacheKeys.hash_tb_spfl, dict["id_masteruser"] as string);
        //        }
        //    }
        //    return res;
        //}

        //public override bool Delete<TModel>(TModel model)
        //{
        //    var entity = model as Tb_Spfl;
        //    var res = base.Delete(entity);
        //    if (res)
        //    {
        //        var id_masteruser = entity.id_masteruser;
        //        if (id_masteruser.IsEmpty())
        //            id_masteruser = QueryModel<Tb_Spfl>(new { entity.id }, selectFields: "id_masteruser")?.id_masteruser;
        //        redisHelper.HashDelete(CacheKeys.hash_tb_spfl, id_masteruser);
        //    }
        //    return res;
        //}

        //public override int Execute(string sql, object param = null)
        //{
        //    var res = base.Execute(sql, param);
        //    if (res > 0 && sql.TrimStart(' ').StartsWith("update"))
        //    {
        //        var dict = ConvertUtil.ParamsToDictionary(param);
        //        if (dict.ContainsKey("id_masteruser"))
        //        {
        //            redisHelper.HashDelete(CacheKeys.hash_tb_spfl, dict["id_masteruser"] as string);
        //        }
        //    }
        //    return res;
        //}
    }
}
