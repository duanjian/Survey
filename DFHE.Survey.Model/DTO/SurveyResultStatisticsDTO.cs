using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DFHE.Survey.Model
{
    public class SurveyResultStatisticsDTO
    {
        public int SurveyId { get; set; }
        public string SurveyName { get; set; }
        public string StaticUrl { get; set; }
        public int QuestionCount { get; set; }
        public string RequiredInfos { get; set; }
        public int RespondentId { get; set; }
        public string RespondentName { get; set; }
        public string MobilePhone { get; set; }
        public Nullable<int> Age { get; set; }
        public string Gender { get; set; }
        public string EduBackground { get; set; }
        public string Occupation { get; set; }
        public string Location { get; set; }
        public string MaritalStatus { get; set; }
        public string Referrer { get; set; }
        public string Suggestion { get; set; }
        public int QuestionId { get; set; }
        public string SelectedOptions { get; set; }
        public DateTime CreateTime { get; set; }
    }
}
