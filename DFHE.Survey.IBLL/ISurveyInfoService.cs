
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DFHE.Survey.IBLL;
using DFHE.Survey.Model;

namespace DFHE.Survey.IBLL
{
    public interface ISurveyInfoService
    {
        ResultVO GetSurveyList();

        ResultVO InsertSurvey(SurveyVO survey);

        ResultVO GetSurveyInfoById(int surveyId);

        ResultVO UpdateSurveyInfo(SurveyVO survey);

        ResultVO DeleteSurveyInfoById(int surveyId);

        ResultVO SubmitSurvey(SurveyResultVO resultVO);

        ResultVO GetSurveyQrCode(string staticUrl);

    }
}
