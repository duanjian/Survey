using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DFHE.Survey.Model
{
    public class SurveyInfoDTO
    {
        public int SurveyId { get; set; }
        public string SurveyName { get; set; }
        public string TemplateName { get; set; }
        public int QuestionCount { get; set; }
        public string StaticUrl { get; set; }
        public int CreateId { get; set; }
        public System.DateTime CreateTime { get; set; }
        public string CreateName { get; set; }
    }
}
