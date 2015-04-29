var SurveyApp = angular.module('dfhe.app');
(function (ng, app) {
    function ResultStatisticsCtrl($scope, $rootScope, $routeParams, apiService, alert, dictData, loginInfo, AuthenticationService, exportTbl2csv) {
        this.scope = $scope;
        this.rootScope = $rootScope;
        this.routeParams = $routeParams;
        this.apiService = apiService;
        this.alert = alert;
        this.dictData = dictData;
        this.loginInfo = loginInfo;
        this.AuthService = AuthenticationService;
        this.exportTbl2csv = exportTbl2csv;
        this._init();
    }

    ResultStatisticsCtrl.prototype = {
        _init: function () {
            this._setScope();
        },
        _setScope: function () {
            var _this = this;
            _this.scope.SurveyTitle = '';
            _this.scope.QuestionCount = 0;
            _this.scope.QuestionHeader = [];
            _this.scope.StatisticsRet = {};
            _this.scope.RequireInfo = [];
            _this._registerMethod();
            _this.scope.getResultStatistics();
        },
        _registerMethod: function () {
            var _this = this;
            _this.scope.getResultStatistics = function () {
                _this.apiService.get({
                    controller: 'SurveyInfo',
                    action: 'GetSurveyResultStatistics',
                    surveyId: _this.routeParams.id
                }, function (res) {
                    if (res.Result == 1) {
                        var retData = res.Data;
                        if (retData.length > 0) {
                            _this.scope.SurveyTitle = retData[0].SurveyName;
                            _this.scope.QuestionCount = retData[0].QuestionCount;
                            _this.scope.StatisticsRet = retData;
                            _this.scope.RequireInfo = retData[0].RequiredInfos.split(',');

                            for (var i = 0; i < _this.scope.QuestionCount ; i++) {
                                var title = "#" + (i + 1);
                                _this.scope.QuestionHeader.push(title);
                            }
                        }
                        _this.scope.StatisticsRet = _this.transfromData(retData, _this.scope.RequireInfo, _this.scope.QuestionCount);
                    }
                }, function (res) {

                });
            },
            _this.scope.export2csv = function() {                
                _this.exportTbl2csv.save('DataTables_Table_Statistics', _this.scope.SurveyTitle);
            },
            _this.transfromData = function (data, req, qcount) {
                var targetData = [];

                var selectedOpt = [];
                var countflag = 0;
                angular.forEach(data, function (value, key) {
                    countflag++;

                    var tmpOpt = _this.formatSelectedOpt(value.SelectedOptions);
                    selectedOpt.push(tmpOpt);

                    if (countflag === qcount) {
                        var reqInfo = [];
                        angular.forEach(req, function (v, k) {
                            var propName = _this.reqNumToStr(v);
                            reqInfo.push(value[propName]);
                        });
                        var tmp = reqInfo.concat(selectedOpt);

                        var ctime = value.CreateTime.substr(0, 19).replace('T', ' ');

                        tmp = tmp.concat(ctime);
                        targetData.push(tmp);
                        selectedOpt.length = 0;
                        reqInfo.length = 0;
                        countflag = 0;
                    }
                });
                var res = targetData;
                return targetData;
            },
            _this.formatSelectedOpt = function(args) {
                var tmp = args;
                var num = tmp.split('@');
                var tmpRes = [];
                angular.forEach(num, function(v, k) {
                    var t = String.fromCharCode(64 + parseInt(v));
                    tmpRes.push(t);
                });
                return tmpRes.join('、');
            },
            _this.reqNumToStr = function (n) {
                var ret = '';
                switch (n) {
                    case '1':
                        ret = 'RespondentName';
                        break;
                    case '2':
                        ret = 'MobilePhone';
                        break;
                    case '3':
                        ret = 'Age';
                        break;
                    case '4':
                        ret = 'Gender';
                        break;
                    case '5':
                        ret = 'Occupation';
                        break;
                    case '6':
                        ret = 'EduBackground';
                        break;
                    case '7':
                        ret = 'Referrer';
                        break;
                    case '8':
                        ret = 'Location';
                        break;
                    case '9':
                        ret = 'MaritalStatus';
                        break;
                    case '10':
                        ret = 'Suggestion';
                        break;
                }
                return ret;
            }
        }
    }
    app.controller('ResultStatisticsCtrl', ResultStatisticsCtrl);
})(angular, SurveyApp)