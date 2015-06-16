var SurveyApp = angular.module('dfhe.app');
(function (ng, app) {
    function CreateSurveyCtrl($scope, $rootScope, apiService, alert, dictData, loginInfo, AuthenticationService,loadingModal) {
        this.scope = $scope;
        this.rootScope = $rootScope;
        this.apiService = apiService;
        this.authService = AuthenticationService;
        this.dictData = dictData;
        this.loginInfo = loginInfo;
        this.loadingModal = loadingModal;
        this.alert = alert;
        this._init();
    }

    CreateSurveyCtrl.prototype = {
        _init: function () {
            this._setScope();
            this._watch();
        },
        _setScope: function () {
            var _this = this;
            _this.rootScope.EmployeeInfo = _this.loginInfo;
            _this.rootScope.logout = function () { _this.authService.logout(); };
            _this.scope.ret = {};
            _this.scope.templates = {};
            _this.scope.selectedTmpl = {};
            _this.scope.step1 = true;
            _this.scope.step2 = false;
            _this.scope.selectedInfo = [];
            _this.scope.requireInfo = _this.dictData.requireInfo;
            _this.scope.SurveyInfo = {};
            _this.scope.questions = [];
            _this.scope.SelectedRequiredInfo = [];
            _this.scope.hasInfo = false;
            //_this.scope.question = {};
            //_this.scope.optionInfos = [];
            //_this.scope.optionInfo = {};
            _this._registerMethod();
            _this.scope.getTemplateInfo();
        },
        //监视
        _watch: function () {
            var _this = this;
            //监视下拉多选的变化
            _this.scope.$watch('SurveyInfo.QuestionCount', _this.scope.createQuestions);
            _this.scope.$watchCollection('SelectedRequiredInfo', _this.scope.closeReqInfoMsg);
        },
        _registerMethod: function () {
            var _this = this;

            _this.scope.getTemplateInfo = function() {
                _this.apiService.get({
                    controller: 'Template',
                    action: 'GetTemplateList'
                }, function (res) {
                    if (res.Result == 1) {
                        //angular.forEach(res.Data, function (v, k) {
                        //    v.StaticUrl = "http://" + location.host + '/' + v.StaticUrl;
                        //});
                        _this.scope.templates = res.Data;
                        _this.scope.selectedTmpl = _this.scope.templates[0];

                    }
                }, function (res) {

                });
            },
            _this.scope.createQuestions = function () {
                _this.scope.questions.length = 0;
                for (var i = 0; i < _this.scope.SurveyInfo.QuestionCount; i++) {
                    _this.scope.question = {
                        QuestionTitle: '',
                        IsOrientation:true,
                        QuestionOptionsCount: 0,
                        RequiredOptionsCount: 0,
                        Options: []
                    }
                    _this.scope.questions.push(_this.scope.question);
                }
            },
            _this.scope.insertOpts = function (question) {
                question.Options = {};
                var tmpOpts = [];
                for (var i = 0; i < question.QuestionOptionsCount; i++) {
                    tmpOpts.push({
                        OptionKey: i + 1,
                        OptionValue: ''
                    });
                }
                question.Options = tmpOpts;
            },
            _this.scope.createSurvey = function () {
                var survey = {
                    SurveyInfo: _this.scope.SurveyInfo,
                    QuestionInfo: _this.scope.questions,
                    RequiredInfo: _this.scope.SelectedRequiredInfo,
                    UserName: _this.rootScope.EmployeeInfo.UserName,
                    TemplateId: _this.scope.selectedTmpl.TmplId,
                }
                _this.loadingModal.show();

                _this.apiService.post({
                    controller: 'SurveyInfo',
                    action: 'InsertSurvey'
                }, angular.toJson(survey), function (res) {
                    if (res.Result == 1) {
                        _this.alert.success('创建成功');
                        _this.loadingModal.hide();
                        window.location.href = '#/index';

                    } else {
                        _this.alert.error('创建失败，请重试');
                        _this.loadingModal.hide();

                    }
                }, function (res) {
                    _this.alert.error('创建失败，请重试');
                    _this.loadingModal.hide();

                });
            },
            _this.scope.goStep1 = function () {
                _this.scope.step1 = true;
                _this.scope.step2 = false;
            },
            _this.scope.goStep2 = function () {
                if (_this.scope.SelectedRequiredInfo.length == 0) {
                    _this.scope.hasInfo = true;
                     return;
                 }
                _this.scope.step1 = false;
                _this.scope.step2 = true;
            },
            _this.scope.closeReqInfoMsg = function() {
                if (_this.scope.SelectedRequiredInfo.length > 0) {
                    _this.scope.hasInfo = false;
                }
            }
        }
    }

    app.controller('CreateSurveyCtrl', CreateSurveyCtrl);
})(angular, SurveyApp);