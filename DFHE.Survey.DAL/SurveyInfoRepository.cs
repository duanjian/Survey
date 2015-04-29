
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
    public partial class SurveyInfoRepository : BaseRepository<SurveyInfo>, ISurveyInfoRepository
    {
        public SurveyInfoRepository(DbContext dbContext)
            : base(dbContext)
        {
        }

//        public IList<SurveyResultStatisticsDTO> GetResultStatisticsDto(int surveyId)
//        {
//            string cmdTxt = @"SELECT s.[SurveyId]
//      ,[SurveyName]
//      ,[StaticUrl]      
//      ,[QuestionCount]
//      ,p.RespondentName
//      ,p.MobilePhone
//      ,p.Age
//      ,p.Gender
//      ,p.EduBackground
//      ,p.Occupation
//      ,p.Location
//      ,p.MaritalStatus
//      ,p.Referrer
//      ,p.Suggestion      
//      ,q.QuestionTitle      
//      ,sr.SelectedOptions      
//  FROM [DFHE_Survey].[dbo].[SurveyInfo] as s
//  join QuestionInfo as q on s.SurveyId = q.SurveyId
//  join SurveyResult as sr on sr.SurveyId = s.SurveyId
//  join RespondentInfo as p on sr.RespondentId = p.RespondentId";

//            StringBuilder cmdSb = new StringBuilder(cmdTxt);

//            if (surveyId > 0)
//            {
//                cmdSb.AppendFormat("Where s.SurveyId={0}", surveyId);
//            }

//            return base.ExecuteQuery<SurveyResultStatisticsDTO>(cmdSb.ToString());
//        }
    }
}
