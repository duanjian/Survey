using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DFHE.Survey.IDAL
{
    public interface IUnitOfWork
    {
        /// <summary>
        /// 提交数据
        /// </summary>
        /// <returns></returns>
        int Commit();
    }
}
