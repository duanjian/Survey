
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
	public partial class QuestionInfoRepository :BaseRepository<QuestionInfo>,IQuestionInfoRepository
    {
		public QuestionInfoRepository(DbContext dbContext)
				: base(dbContext)
        {
        }
    }
}
