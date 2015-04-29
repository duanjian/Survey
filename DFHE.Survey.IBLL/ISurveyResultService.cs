
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DFHE.Survey.IBLL;
using DFHE.Survey.Model;

namespace DFHE.Survey.IBLL
{
	public interface ISurveyResultService
	{
	    ResultVO GetSurveyResultStatistics(int surveyId);
	}
}
