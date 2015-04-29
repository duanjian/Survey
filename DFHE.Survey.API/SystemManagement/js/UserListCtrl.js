var SurveyApp = angular.module('dfhe.app');

(function (ng, app) {
    function UserListCtrl($scope, $rootScope, apiService, alert, loginInfo,AuthenticationService) {
        this.scope = $scope;
        this.rootScope = $rootScope;
        this.apiService = apiService;
        this.loginInfo = loginInfo;
        this.authService = AuthenticationService;
        this.alert = alert;
        this._init();
    }
    UserListCtrl.prototype = {
        _init: function () {
            this._setScope();
        },
        _setScope: function () {
            var _this = this;
            _this.scope.ret = {};
            _this.rootScope.EmployeeInfo = _this.loginInfo;
            _this.rootScope.logout = function () { _this.authService.logout(); };
            _this.scope.userInfo = {};
            _this._registerMethod();
            _this.scope.getUserList();
        },
        _registerMethod: function () {
            var _this = this;
            _this.scope.getUserList = function () {
                _this.apiService.get({
                    controller: 'UserInfo',
                    action: 'GetUserList'
                }, function (res) {
                    if (res.Result == 1) {
                        _this.scope.ret = res.Data;
                    }
                }, function (res) {

                });
            },
            _this.scope.deleteUser = function (index,id) {
                _this.apiService.delete({
                    controller: 'UserInfo',
                    action: 'DeleteUserInfo',
                    id: id
                }, function (res) {
                    if (res.Result == 1) {
                        _this.scope.ret.splice(index, 1);
                        _this.alert.success('删除成功');
                    } else if(res.Result == 2) {
                        _this.alert.error('系统管理帐号，无法删除');
                    }
                }, function (res) {

                });
            },
             _this.scope.$on('ui-refresh', function (event, data) {
                 if (data) {
                     _this.scope.getUserList();
                 }
             });
        }
    }
    app.controller('UserListCtrl', UserListCtrl)
    .directive("createUser", function ($modal, apiService,alert) {
        return {
            restrict: "A",
            link: function (scope, element, attr) {
                // 保存
                scope.createUser = function () {
                    apiService.post({
                        controller: 'UserInfo',
                        action: 'InsertUserInfo'
                    }, angular.toJson(scope.userInfo), function (res) {
                        if (res.Result == 1) {
                            alert.success('新增用户成功');
                            scope.ret.push(res.Data);
                            scope.modal.hide();
                        } else if (res.Result == 2) {
                            alert.error('用户已存在，请使用别的用户名');
                        };
                    }, function (res) {
                        
                    });
                };
                element.click(function () {
                    //scope.initProductInfo();
                    // 显示数据
                    scope.userInfo = {};
                    scope.modal = $modal({
                        scope: scope,
                        animation: "am-fade-and-scale",
                        template: "SystemManagement/tmpl/CreateUser.html"
                    });
                });
            }
        };
    });
})(angular, SurveyApp);