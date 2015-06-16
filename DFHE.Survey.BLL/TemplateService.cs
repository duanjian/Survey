using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;
using DFHE.Survey.IBLL;
using DFHE.Survey.IDAL;
using DFHE.Survey.Model;
using DFHE.Survey.Utility;

namespace DFHE.Survey.BLL
{
    public class TemplateService : ITemplateService
    {

        private ITemplateRepository _templateRepository;
        private IUnitOfWork _unitOfWork;

        public TemplateService(ITemplateRepository templateRepository, IUnitOfWork unitOfWork)
        {
            _templateRepository = templateRepository;
            _unitOfWork = unitOfWork;
        }

        public ResultVO GetTemplateList()
        {
            ResultVO result = new ResultVO() { Result = 0 };

            try
            {
                result.Data = _templateRepository.LoadEntities(t => t.Deleted == false).OrderByDescending( t => t.TmplId).ToList();
                result.Result = 1;
            }
            catch (Exception ex)
            {
                result.ErrorMsg = ex.Message;
            }

            return result;
        }

        /// <summary>
        /// 插入新模版
        /// </summary>
        /// <param name="template"></param>
        /// <returns></returns>
        public ResultVO InsertTemplate(TemplateInfo template)
        {
            ResultVO result = new ResultVO() { Result = 0 };

            try
            {
                using (TransactionScope trans = new TransactionScope())
                {

                    var insertedTmpl = _templateRepository.Insert(template);
                    _unitOfWork.Commit();


                    var arr = insertedTmpl.StoredName.Split('\\');
                    var tmplFolder = arr[arr.Length - 1];

                    CreateTempExample(tmplFolder);

                    result.Result = 1;

                    trans.Complete();

                }
            }
            catch (Exception ex)
            {
                result.ErrorMsg = ex.Message;
            }

            return result;
        }

        /// <summary>
        /// 根据主键ID删除模版
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ResultVO DeleteTemplateById(int id)
        {
            ResultVO result = new ResultVO() { Result = 0 };

            try
            {
                //_templateRepository.Delete(t => t.TmplId == id);
                var tmp = _templateRepository.LoadEntities(t => t.TmplId == id).FirstOrDefault();
                tmp.Deleted = true;
                _templateRepository.Update(tmp);
                _unitOfWork.Commit();

                result.Result = 1;
            }
            catch (Exception ex)
            {
                result.ErrorMsg = ex.Message;
            }

            return result;
        }

        /// <summary>
        /// 根据ID主键获取模版信息
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ResultVO GetTemplateById(int id)
        {
            ResultVO result = new ResultVO() { Result = 0 };

            try
            {
                var ret = _templateRepository.LoadEntities(t => t.TmplId == id).FirstOrDefault();
                result.Data = ret;
                result.Result = 1;
            }
            catch (Exception ex)
            {
                result.ErrorMsg = ex.Message;
            }

            return result;
        }

        public void CreateTempExample(string TemplateFolderName)
        {

            #region 假数据

            var surveyInfo = new SurveyInfo()
            {
                SurveyId = 1,
                SurveyName = "模版测试",
                QuestionCount = 3,
                RequiredInfos = "1,2,3"

            };

            var quesionList = new List<QuestionVO>();
            quesionList.Add(new QuestionVO()
            {
                QuesionId = 1,
                QuestionTitle = "模版测试题目1(单选)",
                QuestionOptionsCount = 3,
                RequiredOptionsCount = 1,
                IsOrientation = true,
                Options = new List<OptionInfo>()
                {
                    new OptionInfo()
                    {
                        OptionId = 1,
                        OptionKey = "1",
                        OptionValue = "模版测试选项1",
                        QuesionId = 1
                    },
                    new OptionInfo()
                    {
                        OptionId = 2,
                        OptionKey = "2",
                        OptionValue = "模版测试选项2",
                        QuesionId = 1
                    },
                    new OptionInfo()
                    {
                        OptionId = 3,
                        OptionKey = "3",
                        OptionValue = "模版测试选项3",
                        QuesionId = 1
                    }
                }

            });
            quesionList.Add(new QuestionVO()
            {
                QuesionId = 2,
                QuestionTitle = "模版测试题目2(3选2)",
                QuestionOptionsCount = 3,
                RequiredOptionsCount = 2,
                IsOrientation = true,
                Options = new List<OptionInfo>()
                {
                    new OptionInfo()
                    {
                        OptionId = 4,
                        OptionKey = "1",
                        OptionValue = "模版测试选项1",
                        QuesionId = 2
                    },
                    new OptionInfo()
                    {
                        OptionId = 5,
                        OptionKey = "2",
                        OptionValue = "模版测试选项2",
                        QuesionId = 2
                    },
                    new OptionInfo()
                    {
                        OptionId = 6,
                        OptionKey = "3",
                        OptionValue = "模版测试选项3",
                        QuesionId = 2
                    },
                }

            });

            quesionList.Add(new QuestionVO()
            {
                QuesionId = 3,
                QuestionTitle = "模版测试题目3(不定项)",
                QuestionOptionsCount = 5,
                RequiredOptionsCount = 0,
                IsOrientation = false,
                Options = new List<OptionInfo>()
                {
                    new OptionInfo()
                    {
                        OptionId = 7,
                        OptionKey = "1",
                        OptionValue = "模版测试选项1",
                        QuesionId = 3
                    },
                    new OptionInfo()
                    {
                        OptionId = 8,
                        OptionKey = "2",
                        OptionValue = "模版测试选项2",
                        QuesionId = 3
                    },
                    new OptionInfo()
                    {
                        OptionId = 9,
                        OptionKey = "3",
                        OptionValue = "模版测试选项3",
                        QuesionId = 3
                    },
                    new OptionInfo()
                    {
                        OptionId = 10,
                        OptionKey = "4",
                        OptionValue = "模版测试选项4",
                        QuesionId = 3
                    },
                    new OptionInfo()
                    {
                        OptionId = 11,
                        OptionKey = "5",
                        OptionValue = "模版测试选项5",
                        QuesionId = 3
                    },
                }
            });
            #endregion


            var surveyTmplObj = new { QuestionInfo = quesionList, SurveyInfo = surveyInfo, RequiredInfo = "1,2,3".Split(','), TemplateFolderName = TemplateFolderName, isExample = 1 };

            StaticPageHelper.GenerateExampleHtml(surveyTmplObj, "wapTmpl.html", TemplateFolderName);
        }
    }
}
