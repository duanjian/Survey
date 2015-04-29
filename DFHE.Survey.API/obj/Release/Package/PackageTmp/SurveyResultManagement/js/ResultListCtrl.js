var SurveyApp = angular.module('dfhe.app');
(function (ng, app) {
    function ResultListCtrl($scope, $rootScope, apiService, loginInfo, AuthenticationService) {
        this.scope = $scope;
        this.rootScope = $rootScope;
        this.loginInfo = loginInfo;
        this.authService = AuthenticationService;
        this.apiService = apiService;
        this._init();
    }
    ResultListCtrl.prototype = {
        _init: function () {
            this._setScope();
        },
        _setScope: function () {
            var _this = this;
            _this.rootScope.EmployeeInfo = _this.loginInfo;
            _this.rootScope.logout = function () { _this.authService.logout(); };
            _this.scope.ret = {};
            _this.scope.QrCodeBase64Str = 'loading...';
            _this._registerMethod();
            _this.scope.getSurveyList();
        },
        _registerMethod: function () {
            var _this = this;
            _this.scope.getSurveyList = function () {
                _this.apiService.get({
                    controller: 'SurveyInfo',
                    action: 'GetSurveyList'
                }, function (res) {
                    if (res.Result == 1) {
                        angular.forEach(res.Data, function (v, k) {
                            v.StaticUrl = "http://" + location.host + '/' + v.StaticUrl;
                        });
                        _this.scope.ret = res.Data;
                    }
                }, function (res) {

                });
            },
            _this.scope.getQrCode = function (url) {
                _this.apiService.get({
                    controller: 'SurveyInfo',
                    action: 'GetSurveyQrCode',
                    staticUrl: url
                }, function (res) {
                    if (res.Result == 1) {
                        _this.scope.QrCodeBase64Str = '<img  alt = "loading..." src="data:image/png;base64,' + res.Data + ' " />';
                    }
                }, function (res) {

                });
            },
             _this.scope.$on('ui-refresh', function (event, data) {
                 if (data) {
                     _this.scope.getSurveyList();
                 }
             });
        }
    }
    app.controller('ResultListCtrl', ResultListCtrl);
})(angular, SurveyApp);