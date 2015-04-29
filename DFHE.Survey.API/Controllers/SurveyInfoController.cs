using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using DFHE.Survey.IBLL;
using DFHE.Survey.Model;

namespace DFHE.Survey.API.Controllers
{
    public class SurveyInfoController : ApiController
    {
        private ISurveyInfoService _surveyInfoService;
        private ISurveyResultService _surveyResultService;

        public SurveyInfoController(ISurveyInfoService surveyInfoService, ISurveyResultService surveyResultService)
        {
            _surveyInfoService = surveyInfoService;
            _surveyResultService = surveyResultService;
        }

        /// <summary>
        /// 获取调查问卷列表
        /// </summary>
        /// <returns></returns>
        public ResultVO GetSurveyList()
        {
            var res = _surveyInfoService.GetSurveyList();
            return res;
        }

        /// <summary>
        /// 插入新调查问卷
        /// </summary>
        /// <param name="survey"></param>
        /// <returns></returns>
        public ResultVO InsertSurvey(SurveyVO survey)
        {
            var res = _surveyInfoService.InsertSurvey(survey);
            return res;
        }

        /// <summary>
        /// 根据ID获取问卷信息
        /// </summary>
        /// <param name="surveyId"></param>
        /// <returns></returns>
        public ResultVO GetSurveyInfoById(int surveyId)
        {
            var res = _surveyInfoService.GetSurveyInfoById(surveyId);
            return res;
        }

        /// <summary>
        /// 更新问卷
        /// </summary>
        /// <param name="survey"></param>
        /// <returns></returns>
        public ResultVO UpdateSurveyInfo(SurveyVO survey)
        {
            var res = _surveyInfoService.UpdateSurveyInfo(survey);
            return res;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="surveyId"></param>
        /// <returns></returns>
        public ResultVO DeleteSurveyInfo(int primaryID)
        {
            var res = _surveyInfoService.DeleteSurveyInfoById(primaryID);
            return res;
        }

        /// <summary>
        /// 提交问卷结果
        /// </summary>
        /// <param name="result"></param>
        /// <returns></returns>        
        [HttpPost]
        public ResultVO SubmitSurvey(SurveyResultVO result)
        {
            var res = _surveyInfoService.SubmitSurvey(result);
            return res;
        }

        /// <summary>
        /// 获取二维码
        /// </summary>
        /// <param name="staticUrl"></param>
        /// <returns></returns>
        public ResultVO GetSurveyQrCode(string staticUrl)
        {
            var res = _surveyInfoService.GetSurveyQrCode(staticUrl);
            return res;
        }


        /// <summary>
        /// 根据调查问卷主键获取问卷结果统计
        /// </summary>
        /// <param name="surveyId"></param>
        /// <returns></returns>
        public ResultVO GetSurveyResultStatistics(int surveyId)
        {
            var res = _surveyResultService.GetSurveyResultStatistics(surveyId);
            return res;
        }
    }
}
