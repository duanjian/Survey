using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DFHE.Survey.Model
{
    public class SurveyVO
    {
        public List<QuestionVO> QuestionInfo { get; set; }

        public SurveyInfo SurveyInfo { get; set; }

        public List<int> RequiredInfo { get; set; } 

        public string UserName { get; set; }

        public  int TemplateId { get; set; }
    }
}
