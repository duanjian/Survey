
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DFHE.Survey.DAL;
using DFHE.Survey.IDAL;
using DFHE.Survey.IBLL;
using DFHE.Survey.Model;

namespace DFHE.Survey.BLL
{
	public class SurveyResultService : ISurveyResultService
	{
	    private ISurveyResultRepository _surveyResultRepository;

	    private IUnitOfWork _unitOfWork;

        public SurveyResultService(ISurveyResultRepository surveyResultRepository, IUnitOfWork unitOfWork)
        {
            _surveyResultRepository = surveyResultRepository;
            _unitOfWork = unitOfWork;
        }
        /// <summary>
        /// 获取调查问卷结果统计
        /// </summary>
        /// <param name="surveyId"></param>
        /// <returns></returns>
        public ResultVO GetSurveyResultStatistics(int surveyId)
        {
            ResultVO result = new ResultVO() { Result = 0 };

            try
            {
                var tmpRet = _surveyResultRepository.GetResultStatisticsDto(surveyId);

                result.Data = tmpRet;
                result.Result = 1;
            }
            catch (Exception ex)
            {
                result.ErrorMsg = ex.Message;
            }
            return result;
        }
    }
}
