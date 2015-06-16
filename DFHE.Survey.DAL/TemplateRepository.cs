using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DFHE.Survey.Model;
using DFHE.Survey.IDAL;

namespace DFHE.Survey.DAL
{
    public class TemplateRepository : BaseRepository<TemplateInfo>, ITemplateRepository
    {
        public TemplateRepository(DbContext dbContext)
            : base(dbContext)
        {

        }
    }
}
