var SurveyApp = angular.module('dfhe.app');

(function (ng, app) {
    function SurveyInfoCtrl($scope, $rootScope, $routeParams, apiService, alert, dictData, loginInfo, AuthenticationService) {
        this.scope = $scope;
        this.rootScope = $rootScope;
        this.routeParams = $routeParams;
        this.authService = AuthenticationService;
        this.dictData = dictData;
        this.alert = alert;
        this.loginInfo = loginInfo;
        this.apiService = apiService;
        this._init();
    }

    SurveyInfoCtrl.prototype = {
        _init: function () {
            this._setScope();
        },
        _setScope: function () {
            var _this = this;
            _this.rootScope.EmployeeInfo = _this.loginInfo;
            _this.rootScope.logout = function () { _this.authService.logout(); };
            _this.scope.requireInfo = _this.dictData.requireInfo;
            _this.scope.ret = {};
            _this.scope.templates = {};
            _this.scope.selectedTmpl = {};
            _this._registerMethod();
            //_this.scope.loadSurveyInfo();
            _this.scope.getTemplateInfo();
        },
        _registerMethod: function () {
            var _this = this;
            _this.scope.loadSurveyInfo = function () {
                _this.apiService.get({
                    controller: 'SurveyInfo',
                    action: 'GetSurveyInfoById',
                    surveyId: _this.routeParams.id
                }, function (res) {
                    if (res.Result == 1) {
                        _this.scope.ret = res.Data;
                        var tmpSelectReq = angular.copy(res.Data.SurveyInfo.RequiredInfos);
                        res.Data.SurveyInfo.RequiredInfos = [];
                        var tmpArr = tmpSelectReq.split(',');
                        angular.forEach(tmpArr, function (v, k) {
                            _this.scope.ret.SurveyInfo.RequiredInfos.push(parseInt(v));
                        });

                        var tmplArr = _this.scope.templates;

                        angular.forEach(tmplArr, function(v, k) {
                            if (v.TmplId == res.Data.SurveyInfo.TmplId) {
                                _this.scope.selectedTmpl = v;
                            }
                        });
                    }
                }, function (res) {
                    _this.alert.error('加载失败，请刷新');
                });
            },
            _this.scope.getTemplateInfo = function () {
                _this.apiService.get({
                    controller: 'Template',
                    action: 'GetTemplateList'
                }, function (res) {
                    if (res.Result == 1) {
                        //angular.forEach(res.Data, function (v, k) {
                        //    v.StaticUrl = "http://" + location.host + '/' + v.StaticUrl;
                        //});
                        _this.scope.templates = res.Data;

                        _this.scope.loadSurveyInfo();

                        _this.scope.selectedTmpl = _this.scope.templates[0];
                    }
                }, function (res) {

                });
            },
            _this.scope.insertOpts = function (question) {
                question.Options = {};
                var tmpOpts = [];
                for (var i = 0; i < question.QuestionOptionsCount; i++) {
                    tmpOpts.push({
                        OptionKey: i,
                        OptionValue: ''
                    });
                }
                question.Options = tmpOpts;
            },
            _this.scope.updateSurveyInfo = function () {
                var postData = angular.copy(_this.scope.ret);
                postData.SurveyInfo.RequiredInfos = postData.SurveyInfo.RequiredInfos.join(',');
                postData.TemplateId = _this.scope.selectedTmpl.TmplId;

                _this.apiService.post({
                    controller: 'SurveyInfo',
                    action: 'UpdateSurveyInfo'
                }, angular.toJson(postData), function (res) {
                    if (res.Result == 1) {
                        _this.alert.success('更新成功');
                        window.location.href = '#/index';

                    } else {
                        _this.alert.error('更新失败，请重试');
                    }
                }, function (res) {
                    _this.alert.error('更新失败，请重试');
                });
            }           
        }
    }
    app.controller('SurveyInfoCtrl', SurveyInfoCtrl);
})(angular, SurveyApp);
