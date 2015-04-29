var loginDemoApp = angular.module('dfhe.login', ['dfhe.common']);
(function (ng, app) {
    function LoginCtrl($scope, $rootScope, apiService, AuthenticationService) {
        this.scope = $scope;
        this.rootScope = $rootScope;        
        this.apiService = apiService;
        this.authService = AuthenticationService;
        this._init();
    }
    LoginCtrl.prototype = {
        //初始化
        _init: function () {
            this._setScope();
        },
        //设置域
        _setScope: function () {
            var _this = this;
            _this.scope.credentials = {};            
            _this.scope.ret = {};
            _this._registerMethod();
            //_this.authService.logout();
        },
        //注册方法
        _registerMethod: function () {
            var _this = this;
            //登录方法
            _this.scope.login = function () {
                _this.authService.login(_this.scope.credentials);
            }
        }
    }
    loginDemoApp.controller('LoginCtrl', LoginCtrl);
})(angular, loginDemoApp);

