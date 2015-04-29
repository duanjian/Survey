
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;
using System.Web.UI;
using DFHE.Survey.DAL;
using DFHE.Survey.IDAL;
using DFHE.Survey.IBLL;
using DFHE.Survey.Model;
using DFHE.Survey.Utility;

namespace DFHE.Survey.BLL
{
    public class SurveyInfoService : BaseService, ISurveyInfoService
    {
        private ISurveyInfoRepository _surveyInfoRepository;
        private IQuestionInfoRepository _quesionInfoRepository;
        private IOptionInfoRepository _optionInfoRepository;
        private IUserInfoRepository _userInfoRepository;
        private IRespondentInfoRepository _respondentInfoRepository;
        private ISurveyResultRepository _surveyResultRepository;
        private IUnitOfWork _unitOfWork;

        public SurveyInfoService(
            ISurveyInfoRepository surveyInfoRepository,
            IUserInfoRepository userInfoRepository,
            IQuestionInfoRepository quesionInfoRepository,
            IOptionInfoRepository optionInfoRepository,
            IRespondentInfoRepository respondentInfoRepository,
            ISurveyResultRepository surveyResultRepository,
            IUnitOfWork unitOfWork)
        {
            _surveyInfoRepository = surveyInfoRepository;
            _userInfoRepository = userInfoRepository;
            _quesionInfoRepository = quesionInfoRepository;
            _optionInfoRepository = optionInfoRepository;
            _respondentInfoRepository = respondentInfoRepository;
            _surveyResultRepository = surveyResultRepository;
            _unitOfWork = unitOfWork;
        }

        /// <summary>
        /// 获取调查问卷列表
        /// </summary>
        /// <returns></returns>
        public ResultVO GetSurveyList()
        {
            ResultVO result = new ResultVO() { Result = 0 };

            try
            {
                var surveyList = _surveyInfoRepository.LoadEntities(s => s.Deleted == false).Select(s => new { s.SurveyId, s.SurveyName, s.QuestionCount, s.StaticUrl, s.CreateId, s.CreateTime }).ToList();

                var retList = new List<SurveyInfoDTO>();

                surveyList.ForEach(s =>
                {
                    var tmpUser = _userInfoRepository.LoadEntities(u => u.UserId == s.CreateId).FirstOrDefault();
                    retList.Add(new SurveyInfoDTO()
                    {
                        SurveyId = s.SurveyId,
                        SurveyName = s.SurveyName,
                        QuestionCount = s.QuestionCount,
                        StaticUrl = s.StaticUrl,
                        CreateId = s.CreateId,
                        CreateTime = s.CreateTime,
                        CreateName = tmpUser != null ? tmpUser.RealName:"--"
                    });
                });

                result.Data = retList.OrderByDescending(r => r.SurveyId);
                result.Result = 1;
            }
            catch (Exception ex)
            {
                result.ErrorMsg = ex.Message;
            }

            return result;
        }

        /// <summary>
        /// 插入新调查问卷
        /// </summary>
        /// <returns></returns>
        public ResultVO InsertSurvey(SurveyVO survey)
        {
            ResultVO result = new ResultVO() { Result = 0 };

            try
            {
                using (TransactionScope trans = new TransactionScope())
                {
                    var surveyInfo = survey.SurveyInfo;
                    var questionInfo = survey.QuestionInfo;
                    var requireInfo = survey.RequiredInfo;

                    var userInfo =
                        _userInfoRepository.LoadEntities(u => u.UserName == survey.UserName && u.Deleted == false)
                            .FirstOrDefault();
                    _unitOfWork.Commit();


                    //初始化数据
                    surveyInfo.CreateTime = base.CurrentServerTime;
                    surveyInfo.Deleted = false;
                    surveyInfo.CreateId = userInfo.UserId;
                    surveyInfo.StaticUrl = String.Format("{0}/{1}", "statics", CurrentServerTime.ToString("yyyyMMddHHmmss") + ".html");
                    surveyInfo.RequiredInfos = string.Join(",", survey.RequiredInfo.ToArray());
                    var insertedSurveyInfo = _surveyInfoRepository.Insert(surveyInfo);
                    _unitOfWork.Commit();

                    var insertedQuestion = new QuestionInfo();
                    var insertedOpt = new OptionInfo();

                    questionInfo.ForEach(q =>
                    {
                        var question = new QuestionInfo()
                        {
                            SurveyId = insertedSurveyInfo.SurveyId,
                            QuestionTitle = q.QuestionTitle,
                            IsOrientation = q.IsOrientation,
                            QuestionOptionsCount = q.QuestionOptionsCount,
                            RequiredOptionsCount = q.IsOrientation == false ? 0 : q.RequiredOptionsCount,
                            Deleted = false
                        };

                        insertedQuestion = _quesionInfoRepository.Insert(question);
                        _unitOfWork.Commit();

                        q.Options.ForEach(o =>
                        {
                            o.Deleted = false;
                            o.QuesionId = insertedQuestion.QuestionId;
                            insertedOpt = _optionInfoRepository.Insert(o);
                            _unitOfWork.Commit();
                        });
                    });

                    var insertedQuestionList =
                        _quesionInfoRepository.LoadEntities(q => q.SurveyId == insertedSurveyInfo.SurveyId).ToList();

                    var quesionList = new List<QuestionVO>();

                    insertedQuestionList.ForEach(q => quesionList.Add(new QuestionVO
                    {
                        QuesionId = q.QuestionId,
                        QuestionTitle = q.QuestionTitle,
                        IsOrientation = q.IsOrientation,
                        QuestionOptionsCount = q.QuestionOptionsCount,
                        RequiredOptionsCount = q.RequiredOptionsCount,
                        Options = _optionInfoRepository.LoadEntities(o => o.QuesionId == q.QuestionId).ToList()
                    }));

                    var surveyTmplObj = new { QuestionInfo = quesionList, SurveyInfo = insertedSurveyInfo, RequiredInfo = survey.RequiredInfo };

                    GenerateHtml(surveyTmplObj, "wapTmpl.html", survey.SurveyInfo.StaticUrl.Split("/".ToCharArray())[1].Split('.')[0]);

                    trans.Complete();
                    result.Result = 1;
                }
            }
            catch (Exception ex)
            {
                result.ErrorMsg = ex.Message;
            }

            return result;
        }

        /// <summary>
        /// 根据ID获取调查问卷信息
        /// </summary>
        /// <returns></returns>
        public ResultVO GetSurveyInfoById(int surveyId)
        {
            ResultVO result = new ResultVO() { Result = 0 };

            try
            {
                var surveyInfo = _surveyInfoRepository.LoadEntities(s => s.SurveyId == surveyId).FirstOrDefault();

                var quesionInfo = _quesionInfoRepository.LoadEntities(q => q.SurveyId == surveyId).ToList();

                var optionList = new List<OptionInfo>();

                var questionList = new List<QuestionVO>();

                quesionInfo.ForEach(q => questionList.Add(new QuestionVO()
                {
                    QuesionId = q.QuestionId,
                    QuestionTitle = q.QuestionTitle,
                    IsOrientation = q.IsOrientation,
                    QuestionOptionsCount = q.QuestionOptionsCount,
                    RequiredOptionsCount = q.RequiredOptionsCount,
                    Options = _optionInfoRepository.LoadEntities(o => o.QuesionId == q.QuestionId).ToList()
                }));

                result.Data = new { SurveyInfo = surveyInfo, QuestionInfo = questionList };
                result.Result = 1;
            }
            catch (Exception ex)
            {
                result.ErrorMsg = ex.Message;
            }

            return result;
        }


        /// <summary>
        /// 更新问卷信息
        /// </summary>
        /// <param name="survey"></param>
        /// <returns></returns>
        public ResultVO UpdateSurveyInfo(SurveyVO survey)
        {
            ResultVO result = new ResultVO() { Result = 0 };

            try
            {
                using (TransactionScope tran = new TransactionScope())
                {
                    _surveyInfoRepository.Update(survey.SurveyInfo);
                    _unitOfWork.Commit();

                    survey.QuestionInfo.ForEach(q =>
                    {
                        var tmpQuestion = new QuestionInfo()
                        {
                            QuestionId = q.QuesionId,
                            QuestionOptionsCount = q.QuestionOptionsCount,
                            QuestionTitle = q.QuestionTitle,
                            IsOrientation = q.IsOrientation,
                            SurveyId = survey.SurveyInfo.SurveyId,
                            RequiredOptionsCount = q.IsOrientation == false ? 0 : q.RequiredOptionsCount
                        };

                        _quesionInfoRepository.Update(tmpQuestion);
                        _unitOfWork.Commit();

                        q.Options.ForEach(o =>
                        {
                            var tmpOpt = new OptionInfo()
                            {
                                QuesionId = q.QuesionId,
                                OptionId = o.OptionId,
                                OptionKey = o.OptionKey,
                                OptionValue = o.OptionValue
                            };

                            _optionInfoRepository.Update(tmpOpt);
                            _unitOfWork.Commit();
                        });
                    });

                    var surveyInfo =
                        _surveyInfoRepository.LoadEntities(s => s.SurveyId == survey.SurveyInfo.SurveyId)
                            .FirstOrDefault();

                    var insertedQuestionList =
                        _quesionInfoRepository.LoadEntities(q => q.SurveyId == survey.SurveyInfo.SurveyId).ToList();

                    var quesionList = new List<QuestionVO>();

                    insertedQuestionList.ForEach(q => quesionList.Add(new QuestionVO
                    {
                        QuesionId = q.QuestionId,
                        QuestionTitle = q.QuestionTitle,
                        QuestionOptionsCount = q.QuestionOptionsCount,
                        RequiredOptionsCount = q.RequiredOptionsCount,
                        Options = _optionInfoRepository.LoadEntities(o => o.QuesionId == q.QuestionId).ToList()
                    }));

                    var surveyTmplObj = new { QuestionInfo = quesionList, SurveyInfo = surveyInfo, RequiredInfo = survey.SurveyInfo.RequiredInfos.Split(',') };

                    GenerateHtml(surveyTmplObj, "wapTmpl.html", survey.SurveyInfo.StaticUrl.Split("/".ToCharArray())[1].Split('.')[0]);



                    tran.Complete();
                    result.Result = 1;
                }
            }
            catch (Exception ex)
            {
                result.ErrorMsg = ex.Message;
            }

            return result;
        }

        /// <summary>
        /// 根据ID删除问卷
        /// </summary>
        /// <param name="surveyId"></param>
        /// <returns></returns>
        public ResultVO DeleteSurveyInfoById(int surveyId)
        {
            ResultVO result = new ResultVO() { Result = 0 };

            try
            {
                using (TransactionScope trans = new TransactionScope())
                {
                    var tmpSurvey = _surveyInfoRepository.LoadEntities(s => s.SurveyId == surveyId).FirstOrDefault();
                    tmpSurvey.Deleted = true;
                    _surveyInfoRepository.Update(tmpSurvey);
                    _unitOfWork.Commit();

                    var tmpQuestions = _quesionInfoRepository.LoadEntities(q => q.SurveyId == surveyId).ToList();
                    tmpQuestions.ForEach(q =>
                    {
                        q.Deleted = true;
                        _quesionInfoRepository.Update(q);
                        _unitOfWork.Commit();

                        var tmpOpts = _optionInfoRepository.LoadEntities(o => o.QuesionId == q.QuestionId).ToList();
                        tmpOpts.ForEach(o =>
                        {
                            o.Deleted = true;
                            _optionInfoRepository.Update(o);
                            _unitOfWork.Commit();
                        });
                    });

                    var targetSurvey = _surveyInfoRepository.LoadEntities(s => s.SurveyId == surveyId).FirstOrDefault();
                    var targetPath = Path.Combine(RootPath, targetSurvey.StaticUrl);
                    if (File.Exists(targetPath))
                    {
                        File.Delete(targetPath);
                    }

                    trans.Complete();
                    result.Result = 1;
                }


            }
            catch (Exception ex)
            {
                result.ErrorMsg = ex.Message;
            }

            return result;
        }

        /// <summary>
        /// 提交问卷
        /// </summary>
        /// <param name="resultVO"></param>
        /// <returns></returns>
        public ResultVO SubmitSurvey(SurveyResultVO resultVO)
        {
            ResultVO result = new ResultVO() { Result = 0 };

            try
            {

                using (TransactionScope trans = new TransactionScope())
                {
                    RespondentInfo respondent = new RespondentInfo()
                    {
                        RespondentName = resultVO.Username,
                        MobilePhone = resultVO.Mobile,
                        Age = resultVO.Age,
                        Gender = resultVO.Gender == 0 ? null : (resultVO.Gender == 1 ? "男" : "女"),
                        EduBackground = string.IsNullOrEmpty(GetEduString(resultVO.Edu)) ? null : GetEduString(resultVO.Edu),
                        Location = resultVO.Location,
                        MaritalStatus = string.IsNullOrEmpty(GetMaritalString(resultVO.Marital)) ? null : GetMaritalString(resultVO.Marital),
                        Referrer = resultVO.Refer,
                        Suggestion = resultVO.Suggestion,
                        CreateTime = CurrentServerTime,
                        Deleted = false,
                        Occupation = string.IsNullOrEmpty(GetPosString(resultVO.Position)) ? null : GetPosString(resultVO.Position)
                    };

                    var insertedResp = _respondentInfoRepository.Insert(respondent);
                    _unitOfWork.Commit();

                    var tmpOpt = resultVO.Options.Split(',').ToList().FindAll(o => o.Contains("#"));
                    tmpOpt.ForEach(o =>
                    {
                        var questionId = Convert.ToInt32(o.Split('#')[0]);
                        var questionAns = o.Split('#')[1];

                        SurveyResult surveyResult = new SurveyResult()
                        {
                            SurveyId = resultVO.SurveyId,
                            RespondentId = insertedResp.RespondentId,
                            QuestionId = questionId,
                            SelectedOptions = questionAns,
                            CreateTime = CurrentServerTime
                        };

                        _surveyResultRepository.Insert(surveyResult);
                        _unitOfWork.Commit();

                    });

                    trans.Complete();
                    result.Result = 1;
                }
            }
            catch (Exception ex)
            {
                result.ErrorMsg = ex.Message;
            }

            return result;
        }

        /// <summary>
        /// 获取问卷你调查地址二维码
        /// </summary>
        /// <param name="surveyId"></param>
        /// <returns></returns>
        public ResultVO GetSurveyQrCode(string staticUrl)
        {
            ResultVO result = new ResultVO() { Result = 0 };

            try
            {
                var tmpBitmap = QrCodeHelper.GeneratorQrImage(staticUrl);
                var tmpBytes = ConvertHelper.BitmapToBytes(tmpBitmap, System.Drawing.Imaging.ImageFormat.Gif);
                var Base64Str = Convert.ToBase64String(tmpBytes);

                result.Data = Base64Str;
                result.Result = 1;
            }
            catch (Exception ex)
            {
                result.ErrorMsg = ex.Message;
            }
            return result;
        }

        


        #region 需填信息数字转字符串

        /// <summary>
        /// 获取学历
        /// </summary>
        /// <param name="eduNum"></param>
        /// <returns></returns>
        private string GetEduString(int eduNum)
        {
            var eduString = string.Empty;
            switch (eduNum)
            {
                case 1:
                    eduString = "本科"; break;
                case 2:
                    eduString = "大专及以下"; break;
                case 3:
                    eduString = "研究生及以上"; break;
            }

            return eduString;
        }

        /// <summary>
        /// 获取婚姻状态字符串
        /// </summary>
        /// <param name="Mar"></param>
        /// <returns></returns>
        private string GetMaritalString(int Mar)
        {
            var marString = string.Empty;

            switch (Mar)
            {
                case 1:
                    marString = "已婚"; break;
                case 2:
                    marString = "未婚"; break;
                case 3:
                    marString = "离异"; break;
                case 4:
                    marString = "保密"; break;
            }
            return marString;
        }


        /// <summary>
        /// 获取职业字符串
        /// </summary>
        /// <param name="Pos"></param>
        /// <returns></returns>
        private string GetPosString(int Pos)
        {
            var PosString = string.Empty;

            switch (Pos)
            {
                case 1:
                    PosString = "金融"; break;
                case 2:
                    PosString = "服务"; break;
                case 3:
                    PosString = "制造"; break;
                case 4:
                    PosString = "IT"; break;
                case 5:
                    PosString = "教育"; break;
                case 6:
                    PosString = "医疗"; break;
                case 7:
                    PosString = "娱乐"; break;
                case 8:
                    PosString = "政府"; break;
                case 9:
                    PosString = "其他"; break;

            }

            return PosString;
        }

        #endregion

        #region 静态页生成相关

        private void GenerateHtml(Object survey, string tmplName, string staticPageName)
        {

            try
            {
                VelocityHelper helper = new VelocityHelper("/statics/tmpl/");
                NVTools tools = new NVTools();
                helper.Put("tools", tools);
                helper.Put("Survey", survey);
                helper.GenerateHtml(tmplName, string.Format("/statics/{0}.html", staticPageName));

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private class NVTools
        {
            public string OptNumToChar(string source)
            {
                return Convert.ToChar(Convert.ToInt32(source) + 64).ToString();
            }
        }

        #endregion
    }
}
