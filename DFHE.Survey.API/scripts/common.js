angular.module("dfhe.common", ["ngResource", 'ngRoute', "ngCookies", 'ngSanitize', 'mgcrea.ngStrap'])
     .run(['$templateCache', function ($templateCache) {
         'use strict';
         $templateCache.put('modal/dfhe.modal.confirm.html',
             "<div class=\"modal\" tabindex=\"-1\" role=\"dialog\"><div class=\"modal-dialog\"><div class=\"modal-content\"><div class=\"modal-header\" ng-show=\"title\"><button type=\"button\" class=\"close\" ng-click=\"$hide()\">&times;</button><h4 class=\"modal-title\" ng-bind=\"title\"></h4></div><div class=\"modal-body\" ng-bind=\"content\"></div><div class=\"modal-footer\"><button type=\"button\" class=\"btn btn-default\" ng-click=\"$hide()\">取消</button><button type=\"button\" class=\"btn btn-primary\" ng-click=\"deleteByPrimaryID();$hide()\">确认</button></div></div></div></div>"
         );
     }])
    .factory("dictData", function () {
        var requireInfo = [
            { key: 1, value: '姓名' },
            { key: 2, value: '手机号码' },
            { key: 3, value: '年龄' },
            { key: 4, value: '性别' },
            { key: 5, value: '职业' },
            { key: 6, value: '教育程度' },
            { key: 7, value: '推荐人' },
            { key: 8, value: '所在地' },
            { key: 9, value: '婚姻状态' },
            { key: 10, value: '意见' }
        ];
        return {
            requireInfo: requireInfo
        };
    })
    .factory('loadingModal', function ($modal) {
        var modal = $modal({
            keyboard: false,                //backdrop 为 static 时，点击模态对话框的外部区域不会将其关闭
            backdrop: 'static',             //keyboard 为 false 时，按下 Esc 键不会关闭 Modal
            animation: "am-fade-and-scale",
            template: "/templates/_loadingModal.html",
            show: false
        });
        return {
            show: function () {
                return modal.show();
            },
            hide: function () {
                return modal.hide();
            }
        }
    })
    .factory("apiService", function ($resource, $cookieStore) {
        return $resource("api/:controller/:action/:id", { controller: "@controller", action: "@action", id: "@id" },
        {
            get: {
                method: "get",
                headers: { "Authorization": 'Basic' + ' ' + $cookieStore.get("Authorization") }
            },
            post: {
                method: "post",
                headers: { "Authorization": 'Basic' + ' ' + $cookieStore.get("Authorization") }
            },
            delete: {
                method: "delete",
                headers: { "Authorization": 'Basic' + ' ' + $cookieStore.get("Authorization") }
            },
            put: {
                method: "put",
                headers: { "Authorization": 'Basic' + ' ' + $cookieStore.get("Authorization") }
            }
        });
    })
    .factory("loginInfo", function ($cookieStore, $location, $window) {
        var loginInfo = $cookieStore.get("survey_user");
        if (!loginInfo) {
            // 没有登录信息，跳转到登录界面
            $window.location.href = "/login.html";
        }
        var result = window.Base64.decoder(loginInfo);
        return JSON.parse(result);
    })
    .factory('apiService2', function ($http, $cookieStore) {
        return {
            request: function (method, url, data, okCallback, koCallback) {
                $http({
                    method: method,
                    url: url,
                    data: data
                }).success(okCallback).error(koCallback);
            },
            authRequest: function (method, url, data, okCallback, koCallback) {
                $http({
                    method: method,
                    url: url,
                    data: data,
                    headers: { 'Authorization': $cookieStore.get("Authorization") }
                }).success(okCallback).error(koCallback);
            }
        }
    })
    .factory("SessionService", function () {
        return {
            get: function (key) {
                return sessionStorage.getItem(key);
            },
            set: function (key, val) {
                return sessionStorage.setItem(key, val);
            },
            remove: function (key) {
                return sessionStorage.removeItem(key);
            }
        }
    })
    .factory("AuthenticationService", function ($http, $location, $window, $cookieStore, apiService2, SessionService, alert) {
        var cacheSession = function () {
            $cookieStore.put("authenicated", true);
        };
        var uncacheSession = function () {
            $cookieStore.remove("authenicated");
        };
        return {
            login: function (credentials) {
                credentials.Password = window.Base64.encoder(window.Base64.encoder(credentials.Password));
                apiService2.request('post', 'api/Login/Login', JSON.stringify(credentials), function (res) {
                    if (res.Result == 1) {
                        cacheSession();
                        $cookieStore.put("Authorization", res.Data.tt);
                        // 存储当前登录用户信息
                        var empInfo = window.Base64.encoder(JSON.stringify(res.Data.UserInfo));
                        $cookieStore.put("survey_user", empInfo);
                        $window.location.href = '/index.html';
                    } else {
                        alert.error('用户名或密码错误,请重试');
                    }
                });
            },
            logout: function () {
                $cookieStore.remove("authenicated");
                $cookieStore.remove("Authorization");
                $cookieStore.remove("survey_user");

                $http({
                    method: "get",
                    url: "api/Login/Logout"
                }, function (res) {
                    if (res.result == 1) {
                        $cookieStore.put("survey_user", '');
                    }
                });
                $window.location.href = "/login.html";
            },
            isLoggedIn: function () {
                return SessionService.get('authenicated');
            }
        };
    }
    ).config([
        '$routeProvider', '$locationProvider', function ($routeProvider, $locationProvider) {
            $routeProvider
                .when('/index', {
                    templateUrl: 'SurveyManagement/SurveyList.html',
                    controller: 'SurveyListCtrl'
                })
                .when('/user', {
                    templateUrl: 'SystemManagement/UserList.html',
                    controller: 'UserListCtrl'
                })
                .when('/createsurvey', {
                    //.when('/', {
                    templateUrl: 'SurveyManagement/CreateSurvey.html',
                    controller: 'CreateSurveyCtrl'
                })
                .when('/surveyinfo/:id', {
                    templateUrl: 'SurveyManagement/SurveyInfo.html',
                    controller: 'SurveyInfoCtrl'
                })
                .when('/resultlist', {
                    templateUrl: 'SurveyResultManagement/ResultList.html',
                    controller: 'ResultListCtrl'
                })
                .when('/resultstatistics/:id', {
                    templateUrl: 'SurveyResultManagement/ResultStatistics.html',
                    controller: 'ResultStatisticsCtrl'
                })
                .otherwise({
                    redirectTo: '/createsurvey'
                    //redirectTo: '/index'
                });
            $locationProvider.html5Mode(true);
            //$locationProvider.
        }
    ])
    .factory('alert', function ($alert) {
        //$alert({ title: '操作成功', content: '新项目已建立', placement: 'top', type: 'danger', duration:3, keyboard: true, show: false });
        //提醒时间
        var duration = 3;
        var placement = 'top'; // top-right       
        return {
            success: function (content) {
                return $alert({ title: '操作成功', content: content, placement: placement, type: 'success', duration: duration, keyboard: true, show: true });
            },
            error: function (content) {
                return $alert({ title: '操作失败', content: content, placement: placement, type: 'danger', duration: duration, keyboard: true, show: true });
            }
        }
    })
    .factory('exportTbl2csv', function () {
        return {
            save: function (tableId, ReportTitle) {
                var data = $('#' + tableId).tableToJSON();
                if (data == '')
                    return;
                JSONToCSVConvertor(data, ReportTitle, true);

                function JSONToCSVConvertor(JSONData, ReportTitle, ShowLabel) {
                    //If JSONData is not an object then JSON.parse will parse the JSON string in an Object
                    var arrData = typeof JSONData != 'object' ? JSON.parse(JSONData) : JSONData;

                    var CSV = '';
                    //Set Report title in first row or line

                    //空行
                    //CSV += ReportTitle + '\r\n\n';

                    //This condition will generate the Label/Header
                    if (ShowLabel) {
                        var row = "";

                        //This loop will extract the label from 1st index of on array
                        for (var index in arrData[0]) {

                            //Now convert each value to string and comma-seprated
                            row += index + ',';
                        }

                        row = row.slice(0, -1);

                        //append Label row with line break
                        CSV += row + '\r\n';
                    }

                    //1st loop is to extract each row
                    for (var i = 0; i < arrData.length; i++) {
                        var row = "";

                        //2nd loop will extract each column and convert it in string comma-seprated
                        for (var index in arrData[i]) {
                            row += '"' + arrData[i][index] + '",';
                        }

                        row.slice(0, row.length - 1);

                        //add a line break after each row
                        CSV += row + '\r\n';
                    }

                    if (CSV == '') {
                        alert("Invalid data");
                        return;
                    }

                    //Generate a file name
                    var fileName = "Survey_Statistics";
                    //this will remove the blank-spaces from the title and replace it with an underscore
                    fileName += ReportTitle.replace(/ /g, "_");
                    var exportContent = "\uFEFF";

                    CSV = new Array(exportContent + CSV);
                    var blob = new Blob(CSV, { type: "text/csv;charset=utf-8" });
                    saveAs(blob, ReportTitle + ".csv");

                    return;

                    //Initialize file format you want csv or xls
                    // var uri = 'data:text/csv;charset=utf-8,' + escape(CSV);
                    //\ufeff BOM头，保证中文输出不乱码
                    var uri = 'data:text/csv;charset=utf-8,\ufeff' + encodeURI(CSV);

                    // Now the little tricky part.
                    // you can use either>> window.open(uri);
                    // but this will not work in some browsers
                    // or you will not get the correct filfe extension    

                    //this trick will generate a temp <a /> tag
                    var link = document.createElement("a");
                    link.href = uri;

                    //set the visibility hidden so it will not effect on your web-layout
                    link.style = "visibility:hidden";
                    link.download = fileName + ".csv";

                    //this part will append the anchor tag and remove it after automatic click
                    document.body.appendChild(link);
                    link.click();
                    document.body.removeChild(link);
                }
            }
        }
    })
    .filter('OptNumToChar', function () {
        return function (n) {
            return String.fromCharCode(64 + parseInt(n));
        }
    })
    .filter('RequireInfoFilter', function () {

        //{ key: 1, value: '姓名' },
        //{ key: 2, value: '手机号码' },
        //{ key: 3, value: '年龄' },
        //{ key: 4, value: '性别' },
        //{ key: 5, value: '职业' },
        //{ key: 6, value: '教育程度' },
        //{ key: 7, value: '推荐人' },
        //{ key: 8, value: '所在地' },
        //{ key: 9, value: '婚姻状态' },
        //{ key: 10, value: '意见' }

        return function (n) {
            var ret = '';
            switch (n) {
                case '1':
                    ret = '姓名'; break;
                case '2':
                    ret = '手机号码';
                    break;
                case '3':
                    ret = '年龄';
                    break;
                case '4':
                    ret = '性别';
                    break;
                case '5':
                    ret = '职业';
                    break;
                case '6':
                    ret = '教育程度';
                    break;
                case '7':
                    ret = '推荐人';
                    break;
                case '8':
                    ret = '所在地';
                    break;
                case '9':
                    ret = '婚姻状态';
                    break;
                case '10':
                    ret = '意见';
                    break;
            }
            return ret;
        }
    })
    //.filter('loadingFilter', function () {
    //    return function (n) {            
    //        return n || 'data:image/gif;base64,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';
    //    }
    //})
     // 系统提示功能
    .provider("sysAlter", function () {
        var defaults = {
            title: '系统提示',
            content: '',
            placement: 'top', // 默认位置
            type: 'info',// 默认样式
            duration: 3,// 提醒时间
            //container: '#alterPannel'
        };
        this.$get = ['$alert', function ($alert) {
            var sysAlterFactory = function (options, type) {
                if (angular.isString(options) || options == undefined) {
                    defaults.content = options || '正在查询数据，请稍等...';
                    defaults.type = type || 'info';
                } else {
                    defaults = angular.extend({}, defaults, options);
                }
                return $alert(defaults);
            }
            return sysAlterFactory;
        }];
    })
    // 通用确认提示框
    .directive('dfheDeleteconfirm', ['$modal', 'apiService', 'sysAlter', function ($modal, apiService, sysAlter) {
        return {
            restrict: 'AEC',
            controller: function ($scope) {
                $scope.title = '操作确认';
                $scope.content = '确认执行此操作吗？';
                $scope.deleteByPrimaryID = function () {
                    var alert = sysAlter('正在操作，请稍等...');
                    apiService.delete({
                        controller: $scope.controller,
                        action: $scope.action,
                        primaryID: $scope.primaryID
                    }, function (res) {
                        if (res) {
                            if (res.Result == 1) {
                                sysAlter("操作成功", 'success');
                                $scope.$emit('ui-refresh', true);
                            }
                            else
                                sysAlter(res.ErrorMsg, 'danger');
                        }
                    }, function (err) {
                        sysAlter(JSON.stringify(err), 'danger');
                    });
                }
            },
            link: function (scope, element, attr) {
                element.on('click', function () {
                    // 主键ID
                    scope.primaryID = attr.id;
                    // 服务地址信息
                    scope.controller = attr.controller;
                    scope.action = attr.action;
                    var mymodal = $modal({
                        scope: scope,
                        animation: 'am-fade-and-scale',
                        title: '操作确认',
                        content: '确认执行此操作吗？',
                        template: 'modal/dfhe.modal.confirm.html',
                        show: true
                    });
                });
            }
        }
    }]);