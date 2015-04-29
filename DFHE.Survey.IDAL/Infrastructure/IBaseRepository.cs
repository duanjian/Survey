using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace DFHE.Survey.IDAL
{
    public interface IBaseRepository<T> where T : class 
    {
        /// <summary>
        /// 当前数据实体集
        /// </summary>
        IQueryable<T> Entities { get; }

        /// <summary>
        /// 插入数据
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        T Insert(T entity);

        /// <summary>
        /// 插入数据-异步
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        Task<int> InsertAsync(T entity);

        /// <summary>
        /// 删除数据
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        T Delete(T entity);

        /// <summary>
        /// 删除数据
        /// </summary>
        /// <param name="where"></param>
        void Delete(Expression<Func<T, bool>> where);

        /// <summary>
        /// 修改数据
        /// </summary>
        /// <param name="entity"></param>
        void Update(T entity);

        /// <summary>
        /// 查询列表
        /// </summary>
        /// <param name="whereLambda"></param>
        /// <returns></returns>
        IQueryable<T> LoadEntities(Expression<Func<T, bool>> whereLambda);

        /// <summary>
        /// 查询SQL语句
        /// </summary>
        /// <param name="sqlParas"></param>
        /// <returns></returns>
        List<T> SearchByCondition(params System.Data.SqlClient.SqlParameter[] sqlParas);

        /// <summary>
        /// 查询SQL语句
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="pars"></param>
        /// <returns></returns>
        List<targetT> ExecuteQuery<targetT>(string sql, params System.Data.SqlClient.SqlParameter[] pars);

        /// <summary>
        /// 提交修改
        /// </summary>
        void Save();
    }
}
