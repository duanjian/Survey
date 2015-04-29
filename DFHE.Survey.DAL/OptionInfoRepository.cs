
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DFHE.Survey.IDAL;
using DFHE.Survey.Model;

namespace DFHE.Survey.DAL
{
	public partial class OptionInfoRepository :BaseRepository<OptionInfo>,IOptionInfoRepository
    {
		public OptionInfoRepository(DbContext dbContext)
				: base(dbContext)
        {
        }
    }
}
