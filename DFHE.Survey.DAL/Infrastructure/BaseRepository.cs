using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using DFHE.Survey.IDAL;


namespace DFHE.Survey.DAL
{
    /// <summary>
    /// 仓储基本操作的基类
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class BaseRepository<T> : IBaseRepository<T> where T : class
    {
        protected DbContext _dbContext;
        private readonly IDbSet<T> _dbSet;

        /// <summary>
        /// 仓储基类的构造函数
        /// </summary>
        /// <param name="dbContext"></param>
        protected BaseRepository(DbContext dbContext)
        {
            _dbContext = dbContext;
            _dbSet = dbContext.Set<T>();
        }

        /// <summary>
        /// 当前数据实体集
        /// </summary>
        public virtual IQueryable<T> Entities
        {
            get { return _dbSet; }
        }

        /// <summary>
        /// 插入数据-异步
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public virtual async Task<int> InsertAsync(T entity)
        {
            _dbSet.Add(entity);
            return await _dbContext.SaveChangesAsync();
        }

        /// <summary>
        /// 插入数据
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public virtual T Insert(T entity)
        {
            return _dbSet.Add(entity);
        }

        /// <summary>
        /// 删除数据
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public virtual T Delete(T entity)
        {
            return _dbSet.Remove(entity);
        }

        /// <summary>
        /// 删除数据
        /// </summary>
        /// <param name="whereLambda"></param>
        public virtual void Delete(Expression<Func<T, bool>> whereLambda)
        {
            IEnumerable<T> objects = _dbSet.Where<T>(whereLambda).AsEnumerable();
            foreach (T obj in objects)
                _dbSet.Remove(obj);
        }

        /// <summary>
        /// 修改数据
        /// </summary>
        /// <param name="entity"></param>
        public virtual void Update(T entity)
        {
            _dbContext.Entry(entity).State = EntityState.Modified;
        }

        /// <summary>
        /// 查询列表
        /// </summary>
        /// <param name="whereLambda"></param>
        /// <returns></returns>
        public IQueryable<T> LoadEntities(System.Linq.Expressions.Expression<Func<T, bool>> whereLambda)
        {
            return _dbSet.Where(whereLambda);
        }

        /// <summary>
        /// 查询SQL语句
        /// </summary>
        /// <param name="sqlParas"></param>
        /// <returns></returns>
        public List<targetT> ExecuteQuery<targetT>(string sql, params System.Data.SqlClient.SqlParameter[] pars)
        {
            return _dbContext.Database.SqlQuery<targetT>(sql, pars).ToList();
        }

        /// <summary>
        /// 查询SQL语句
        /// </summary>
        /// <param name="sqlParas"></param>
        /// <returns></returns>
        public List<T> SearchByCondition(params System.Data.SqlClient.SqlParameter[] sqlParas)
        {
            StringBuilder sbSql = new StringBuilder();
            sbSql.AppendFormat("SELECT * FROM {0} AS tb WHERE 1 = 1 ", typeof(T).Name);
            if (sqlParas != null && sqlParas.Length > 0)
            {
                #region --- 拼接条件 ---
                foreach (SqlParameter para in sqlParas)
                {
                    if (para.Value is List<int>)
                    {
                        sbSql.AppendFormat(" AND tb.{0} in ({1}) ", para.ParameterName, string.Join(",", para.Value as List<int>));
                    }
                    else
                    {
                        sbSql.AppendFormat(" AND tb.{0} = {1} ", para.ParameterName, para.Value);
                    }
                }
                #endregion
            }
            return ExecuteQuery<T>(sbSql.ToString());
        }

        /// <summary>
        /// 提交修改
        /// </summary>
        public virtual void Save()
        {
            _dbContext.SaveChanges();
        }
    }
}
